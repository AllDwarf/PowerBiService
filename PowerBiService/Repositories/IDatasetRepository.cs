﻿using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Repositories;

public interface IDatasetRepository
{
    // create tasks to get Reports and Datasets by Id
    Task<Dataset> GetDatasetByReportAsync(Group group, Report report);
    Task<Dataset> GetDatasetById(string id);
    Task<bool> RefreshDatasetAsync(Group workspace, Dataset dataset);
    // Task<Dataset> CloneDataset(Guid workspaceId, DatasetRequestBody dsRequestBody);
    Task<Datasets> GetAllDatasestsByWorkspace(Group workspace);
    Task<bool> UpdateDataSourceConnectionDetails(Group workspace, Dataset dataset, string connectionString);
    Task<bool> TakeOwnershipOfDataset(Group workspace, Dataset dataset);
}
