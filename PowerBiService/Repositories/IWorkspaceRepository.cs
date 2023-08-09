using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Repositories;
public interface IWorkspaceRepository
{
    Task<bool> CleanWorskpaceAsync(Report report, Dataset dataset);
    Task<Group> GetWorskpaceByNameAsync(string name);
}
