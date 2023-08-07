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

        var reportGreen = await _reportRepository.GetReportById(_reportGreenId, workspaceGreen);
        var datasetGreen = await _datasetRepository.GetDatasetByReportAsync(workspaceGreen, reportGreen);

        await _datasetRepository.RefreshDatasetAsync(datasetGreen);
        reportGreen = await _reportRepository.ValidateAndUpdateReport(reportGreen);

        var reportStream = await _reportRepository.ExportReportStream(reportGreen);

        var import = await _reportRepository.ImportReportStream(reportStream, workspaceBlue.Id);

        var reportBlue = await _reportRepository.GetReportById(_reportBlueId, workspaceBlue);
        await _reportRepository.RebindReportAsync(reportBlue, datasetGreen);

        var datasetBlue = import.Datasets[0];
        if (datasetBlue == null)
        {
            throw new Exception("Dataset is not present in imported stream!");
        }

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
