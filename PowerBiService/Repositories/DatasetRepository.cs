using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
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

        var dataset = datasets.Value.FirstOrDefault(d => d.Id == datasetId) ?? throw new Exception($"Dataset with id {datasetId} not found");
        return dataset;
    }

    public async Task<bool> RefreshDatasetAsync(Dataset dataset)
    {
        await _client.Datasets.RefreshDatasetAsync(dataset.Id);
        return true;
    }

    public async Task<Dataset> GetDatasetById(string id)
    {
        var dataset = await _client.Datasets.GetDatasetAsync(id) ?? throw new Exception($"Dataset with id {id} not found");
        return dataset;
    }


    // public async Task<Dataset> CloneDataset(Guid workspaceId, DatasetRequestBody dsRequestBody)
    // {
    //     CreateDatasetRequest createDatasetRequest = new();
    //     // Create a new dataset with the same tables as an existing dataset.
    //     createDatasetRequest.Tables = dsRequestBody.Tables;
    //     createDatasetRequest.Name = dsRequestBody.Name;
    //     createDatasetRequest.Datasources = dsRequestBody.Datasources;

    //     createDatasetRequest.Relationships = dsRequestBody.Relationships;
    //     createDatasetRequest.Validate();

    //     var dataset = await _client.Datasets.PostDatasetInGroupAsync(workspaceId, createDatasetRequest);
    //     return dataset;
    // }

    public async Task<Datasets> GetAllDatasestsByWorkspace(Group workspace)
    {
        var datasets = await _client.Datasets.GetDatasetsInGroupAsync(workspace.Id);
        return datasets;
    }
}
