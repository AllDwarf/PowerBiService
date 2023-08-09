using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PowerBiService.Models;
using Microsoft.PowerBI.Api.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
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

        // Get workspace, report, dataset, tables, relationships, datasources
        // Using the repositories
        var workspaceGreen = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceGreen);
        var workspaceBlue = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceBlue);
        // Get report from green workspace
        var reportGreen = await _reportRepository.GetReportById(_reportGreenId, workspaceGreen);
        // Get dataset from green workspace
        var datasetGreen = await _datasetRepository.GetDatasetByReportAsync(workspaceGreen, reportGreen);
        // Refresh dataset
        await _datasetRepository.RefreshDatasetAsync(datasetGreen);
        // Validate and update report
        reportGreen = await _reportRepository.ValidateAndUpdateReport(reportGreen);
        // Export report to stream
        var reportStream = await _reportRepository.ExportReportStream(reportGreen);
        // Import stream to blue workspace
        var import = await _reportRepository.ImportReportStream(reportStream, workspaceBlue.Id);
        // Get report from blue workspace
        var reportBlue = await _reportRepository.GetReportById(_reportBlueId, workspaceBlue);
        // Rebind report to dataset that was imported
        await _reportRepository.RebindReportAsync(reportBlue, datasetGreen);
        // Get dataset from import
        var datasetBlue = import.Datasets[0];
        if (datasetBlue == null)
        {
            throw new Exception("Dataset is not present in imported stream!");
        }

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
