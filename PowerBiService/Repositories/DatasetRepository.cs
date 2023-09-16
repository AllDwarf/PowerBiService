using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
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

    public async Task<bool> RefreshDatasetAsync(Group workspace, Dataset dataset)
    {
        try
        {
            // Check for already running refreshes and if its not running then refresh the dataset
            var refreshes = await _client.Datasets.GetRefreshHistoryInGroupAsync(workspace.Id, dataset.Id);
            var refresh = refreshes.Value.FirstOrDefault(r => r.Status == "InProgress");
            if (refresh != null)
            {
                await Console.Out.WriteLineAsync($"Dataset: {dataset.Name} is already being refreshed");
                return false;
            }
            var request = await _client.Datasets.RefreshDatasetInGroupWithHttpMessagesAsync(workspace.Id, dataset.Id);
            await Console.Out.WriteLineAsync($"Request for Dataset: {dataset.Name} gets StatusCode: {request.Response.StatusCode}");
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to refresh dataset {dataset.Id} with Message: {ex.Message}");
        }
    }

    public async Task<Dataset> GetDatasetById(string id)
    {
        var dataset = await _client.Datasets.GetDatasetAsync(id) ?? throw new Exception($"Dataset with id {id} not found");
        return dataset;
    }

    public async Task<Datasets> GetAllDatasestsByWorkspace(Group workspace)
    {
        var datasets = await _client.Datasets.GetDatasetsInGroupAsync(workspace.Id) ?? throw new Exception("Datasets not found");
        return datasets;
    }

    public async Task<bool> UpdateDataSourceConnectionDetails(Group workspace, Dataset dataset, string connectionString)
    {
        try
        {
            var connectionDetails = new ConnectionDetails(connectionString);
            await _client.Datasets.SetAllDatasetConnectionsAsync(workspace.Id, dataset.Id, connectionDetails);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to set connection details for dataset {dataset.Name}. {ex.Message}");
        }
    }

    public async Task<bool> TakeOwnershipOfDataset(Group workspace, Dataset dataset)
    {
        try
        {
            var request = await _client.Datasets.TakeOverInGroupWithHttpMessagesAsync(workspace.Id, dataset.Id);
            await Console.Out.WriteLineAsync(request.Response.StatusCode.ToString());
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to take ower dataset {dataset.Name}. {ex.Message}");
        }
    }
}
