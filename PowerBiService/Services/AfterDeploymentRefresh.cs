using Microsoft.Extensions.Configuration;
using PowerBiService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerBiService.Services;
public class AfterDeploymentRefresh
{
    private readonly IWorkspaceRepository _workspaceRepository;
    private readonly IDatasetRepository _datasetRepository;
    private readonly string _workspaceName;
    public AfterDeploymentRefresh(IWorkspaceRepository workspaceRepository,
                          IDatasetRepository datasetRepository,
                          string workspaceName)
    {
        _workspaceRepository = workspaceRepository;
        _datasetRepository = datasetRepository;
        _workspaceName = workspaceName;
        RefreshAllDatasetsInWorkspace().Wait();
    }

    private async Task RefreshAllDatasetsInWorkspace()
    {
        var workspace = await _workspaceRepository.GetWorskpaceByNameAsync(_workspaceName);
        var datasets = await _datasetRepository.GetAllDatasestsByWorkspace(workspace);
        foreach ( var dataset in datasets.Value)
        {
            await _datasetRepository.RefreshDatasetAsync(dataset);
        }
    }
}
