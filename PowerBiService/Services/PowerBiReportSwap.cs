using System.Threading.Tasks;
using PowerBiService.Repositories;
using Microsoft.Extensions.Configuration;
using PowerBiService.Models;
using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Services;
public class PowerBiReportSwap
{
    private readonly IReportRepository _reportRepository;
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly IDatasetRequestRepository _datasetRequestRepository;
    private readonly IConfigurationRoot _configuration;
    public PowerBiReportSwap(IReportRepository reportRepository,
                          IWorkspaceRepository workspaceRepository,
                          IDatasetRepository datasetRepository,
                          IConfigurationRoot config)
    {
        _reportRepository = reportRepository;
        _workspaceRepository = workspaceRepository;
        _datasetRepository = datasetRepository;
        if (config != null)
        {
            _configuration = config;
        }
        HandleBlueGreenDeploymentAsync().Wait();
    }

    public async Task HandleBlueGreenDeploymentAsync()
    {
        // Get workspaceName and reportName from appsettings.json
        var workspaceName = _configuration["WorkspaceName"];
        var reportName = _configuration["ReportName"];

        // Get workspace, report, dataset, tables, relationships, datasources
        // Using the repositories
        var workspace = await _workspaceRepository.GetWorskpaceByNameAsync(workspaceName);
        var reportGreen = await _reportRepository.GetReportyByNameAsync(reportName, workspace);
        var datasetGreen = await _datasetRepository.GetDatasetByReportAsync(workspace, reportGreen);
        var tablesGreen = await _datasetRequestRepository.GetTablesAsync(workspace.Id, datasetGreen.Id);
        var relationships = await _datasetRequestRepository.GetRelationshipsAsync(workspace.Id, datasetGreen.Id);
        var datasourcesGreen = await _datasetRequestRepository.GetDataSourcesAsync(workspace.Id, datasetGreen.Id);

        // Blue Green Deployment logic method
        await BlueGreenDeployment(workspace, reportGreen, datasetGreen, tablesGreen, relationships, datasourcesGreen);
    }

    private async Task BlueGreenDeployment(Group workspace, Report reportGreen, Dataset datasetGreen, IList<Table> tablesGreen, IList<Relationship> relationships, IList<Datasource> datasourcesGreen)
    {
        // Clone the green report and dataset to create the blue report and dataset.
        var reportBlue = await _reportRepository.CloneReportAsync(reportGreen);
        var datasetBlue = await CloneDatasetAsync(workspace, datasetGreen, tablesGreen, relationships, datasourcesGreen);
        // Swap the green and blue reports, and validate the blue report.
        await SwapAndValidateReports(reportBlue, datasetBlue);
        // Rebind the green report to the blue dataset, and validate the green report.
        await _reportRepository.RebindReportAsync(reportGreen, datasetBlue);
        await SwapAndValidateReports(reportBlue, datasetGreen);
        // Rebind the green report to the green dataset.
        await _reportRepository.RebindReportAsync(reportGreen, datasetGreen);
        // Clean up the blue report and dataset.
        await _workspaceRepository.CleanWorskpaceAsync(reportBlue, datasetBlue);
    }

    private async Task SwapAndValidateReports(Report report, Dataset dataset)
    {
        // Rebind the report to the dataset
        await _reportRepository.RebindReportAsync(report, dataset);
        // Refresh the dataset to ensure it has the latest data
        await _datasetRepository.RefreshDatasetAsync(dataset);
        // Validate the report and update the report metadata
        await _reportRepository.ValidateAndUpdateReport(report);
    }

    private async Task<Dataset> CloneDatasetAsync(Group workspace, Dataset datasetGreen, IList<Table> tablesGreen, IList<Relationship> relationships, IList<Datasource> datasourcesGreen)
    {
        var datasetRequestBody = new DatasetRequestBody();
        datasetRequestBody.Name = $"{datasetGreen.Name}_blue";
        datasetRequestBody.Tables = tablesGreen;
        datasetRequestBody.Relationships = relationships;
        datasetRequestBody.Datasources = datasourcesGreen;

        var datasetBlue = await _datasetRepository.CloneDataset(workspace.Id, datasetRequestBody);

        return datasetBlue;
    }
}
