using Microsoft.PowerBI.Api.Models;
using PowerBiService.Repositories;

namespace PowerBiService.Services;
public class BlueGreenService : IServiceRepository
{
    private readonly IReportRepository _reportRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly Guid _reportBlueId;
    private readonly Guid _reportGreenId;
    private readonly string _workspaceGreen;
    private readonly string _workspaceBlue;

    public BlueGreenService(IReportRepository reportRepository,
                          IWorkspaceRepository workspaceRepository,
                          IDatasetRepository datasetRepository,
                          string reportBlueId,
                          string reportGreenId,
                          string workspaceGreen,
                          string workspaceBlue)
    {
        _reportRepository = reportRepository;
        _workspaceRepository = workspaceRepository;
        _datasetRepository = datasetRepository;
        _reportBlueId = new Guid(reportBlueId);
        _reportGreenId = new Guid(reportGreenId);
        _workspaceGreen = workspaceGreen;
        _workspaceBlue = workspaceBlue;
    }

    public async Task InvokeServiceAsync()
    {

        // Get workspaces both Blue and Green
        var workspaceGreen = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceGreen);
        var workspaceBlue = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceBlue);
        // Get report from green workspace
        var reportGreen = await _reportRepository.GetReportById(_reportGreenId, workspaceGreen);
        // Get dataset from green workspace and report
        var datasetGreen = await _datasetRepository.GetDatasetByReportAsync(workspaceGreen, reportGreen);

        // Refresh dataset
        await _datasetRepository.RefreshDatasetAsync(datasetGreen);
        // Validate and update report
        reportGreen = await _reportRepository.ValidateAndUpdateReport(reportGreen);

        // Export report and dataset to stream
        var reportStream = await _reportRepository.ExportReportStream(reportGreen);
        // Import stream to blue workspace
        var import = await _reportRepository.ImportReportStream(reportStream, workspaceBlue.Id);

        // Get import status and wait for import to complete
        var isImportStatusSuccess = import.ImportState.Equals("Succeeded");
        if (!isImportStatusSuccess)
        {
            Console.WriteLine("Import failed");
            return;
        }
        // Get report from blue workspace
        var reportBlue = await _reportRepository.GetReportById(_reportBlueId, workspaceBlue);

        // Get dataset from blue workspace and report
        var datasetBlue = await _datasetRepository.GetDatasetByReportAsync(workspaceBlue, reportBlue);

        // Rebind report to dataset that was imported
        await _reportRepository.RebindReportAsync(reportBlue, datasetGreen);

        // Swap and validate reports
        await SwapAndValidateReports(reportBlue, datasetBlue);
    }

    private async Task SwapAndValidateReports(Report report, Dataset dataset)
    {
        // Refresh the dataset to ensure it has the latest data
        await _datasetRepository.RefreshDatasetAsync(dataset);
        // Rebind the report to the dataset
        await _reportRepository.ValidateAndUpdateReport(report);
        // Rebind the report to the dataset
        await _reportRepository.RebindReportAsync(report, dataset);

    }
}
