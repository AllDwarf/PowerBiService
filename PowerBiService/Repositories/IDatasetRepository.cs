using Microsoft.PowerBI.Api.Models;
using PowerBiService.Models;

namespace PowerBiService.Repositories;

public interface IDatasetRepository
{
    // create tasks to get Reports and Datasets by Id
    Task<Dataset> GetDatasetByReportAsync(Group group, Report report);
    Task<Dataset> GetDatasetById(string id);
    Task<bool> RefreshDatasetAsync(Dataset dataset);
    Task<Dataset> CloneDataset(Guid workspaceId, DatasetRequestBody dsRequestBody);
}
