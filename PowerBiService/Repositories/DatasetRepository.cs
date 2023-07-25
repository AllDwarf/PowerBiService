using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using PowerBiService.Models;
using Group = Microsoft.PowerBI.Api.Models.Group;

namespace PowerBiService.Repositories;

public class DatasetRepository : IDatasetRepository
{
    // Use Azure active directory authentication to connect to the Power BI service
    private readonly IPowerBIClient _client;
    // Use logger to log errors
    public DatasetRepository(IPowerBIClient client)
    {
        _client = client;
    }

    public async Task<Dataset> GetDatasetByReportAsync(Group group, Report report)
    {
        var datasetId = report.DatasetId;
        var datasets = await _client.Datasets.GetDatasetsAsync(group.Id);

        var dataset = datasets.Value.FirstOrDefault(d => d.Id == datasetId);
        if (dataset == null)
        {
            throw new Exception($"Dataset with id {datasetId} not found");
        }
        return dataset;
    }

    public async Task<bool> RefreshDatasetAsync(Dataset dataset)
    {
        await _client.Datasets.RefreshDatasetAsync(dataset.Id);
        return true;
    }


    public async Task<bool> SwapReportDatasetsAsync(Report report, Dataset dataset1, Dataset dataset2, Guid id)
    {
        RebindReportRequest rebindReportRequest = new RebindReportRequest();
        rebindReportRequest.DatasetId = dataset2.Id;
        await _client.Reports.RebindReportAsync(report.Id, rebindReportRequest);

        report.Validate();
        // Test whether the report is rebound to the new dataset
        var reports = await _client.Reports.GetReportsInGroupAsync(id);
        var report1 = reports.Value.FirstOrDefault(r => r.Id == report.Id);
        if (report1.DatasetId != dataset2.Id)
        {
            return false;
        }
        return true;
    }
    public async Task<Dataset> GetDatasetById(string id)
    {
        var dataset = await _client.Datasets.GetDatasetAsync(id);
        if (dataset == null)
        {
            throw new Exception($"Dataset with id {id} not found");
        }
        return dataset;
    }

    public async Task<Dataset> CloneDataset(Guid workspaceId, DatasetRequestBody dsRequestBody)
    {
        CreateDatasetRequest createDatasetRequest = new();
        // Create a new dataset with the same tables as an existing dataset.
        createDatasetRequest.Tables = dsRequestBody.Tables;
        createDatasetRequest.Name = dsRequestBody.Name;
        createDatasetRequest.Datasources = dsRequestBody.Datasources;

        createDatasetRequest.Relationships = dsRequestBody.Relationships;
        createDatasetRequest.Validate();

        var dataset = await _client.Datasets.PostDatasetInGroupAsync(workspaceId, createDatasetRequest);
        return dataset;
    }
}
