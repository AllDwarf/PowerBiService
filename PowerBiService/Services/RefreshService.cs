using PowerBiService.Repositories;

namespace PowerBiService.Services;
public class RefreshService : IServiceRepository
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly string _workspaceName;
    public RefreshService(IWorkspaceRepository workspaceRepository,
                          IDatasetRepository datasetRepository,
                          string workspaceName)
    {
        _workspaceRepository = workspaceRepository;
        _datasetRepository = datasetRepository;
        _workspaceName = workspaceName;
    }

    public async Task InvokeServiceAsync()
    {
        var workspace = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceName);
        var datasets = await _datasetRepository.GetAllDatasestsByWorkspace(workspace);
        foreach (var dataset in datasets.Value)
        {
            //await _datasetRepository.TakeOwnershipOfDataset(workspace, dataset);
            await _datasetRepository.RefreshDatasetAsync(workspace, dataset);
        }
    }
}
