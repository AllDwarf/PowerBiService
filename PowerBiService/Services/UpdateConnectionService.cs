// Create class UpdateConnectionStringRequest use the same way as I did in BlueGreenService.cs
//
using Microsoft.PowerBI.Api.Models;
using PowerBiService.Repositories;

namespace PowerBiService.Services;
public class UpdateConnectionStringService : IServiceRepository
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly string _connectionString;
    private readonly string _workspaceName;
    private readonly string _datasetName;
    public UpdateConnectionStringService(IWorkspaceRepository workspaceRepository,
                          IDatasetRepository datasetRepository,
                          string connectionString,
                          string workspaceName,
                          string datasetName)
    {
        _workspaceRepository = workspaceRepository;
        _datasetRepository = datasetRepository;
        _connectionString = connectionString;
        _workspaceName = workspaceName;
        _datasetName = datasetName;
    }

    public async Task InvokeServiceAsync()
    {
        try
        {
            var workspace = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceName);
            var datasets = await _datasetRepository.GetAllDatasestsByWorkspace(workspace);
            if(datasets.Value.Count > 0)
            {
                foreach (var dataset in datasets.Value)
                {
                    if(dataset.Name == _datasetName)
                    {
                        await _datasetRepository.UpdateDataSourceConnectionDetails(workspace, dataset, _connectionString);
                    }
                }
            }
            else
            {
                throw new Exception($"No datasets found in the workspace with name {_workspaceName}");
            }
        }
        catch(Exception)
        {
            throw new Exception($"Error while updating the connection string for the workspace with name {_workspaceName}");
        }
    }
}