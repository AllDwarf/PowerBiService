using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Repositories;
public class WorkspaceRepository : IWorkspaceRepository
{
    private readonly IPowerBIClient _client;
    public WorkspaceRepository(IPowerBIClient client)
    {
        _client = client;
    }
    public async Task<bool> CleanWorskpaceAsync(Report report, Dataset dataset)
    {
        await _client.Reports.DeleteReportAsync(report.Id);
        return true;
    }

    public async Task<Group> GetWorskpaceByNameAsync(string name)
    {
        var info = _client.WorkspaceInfo;
        var groups = await _client.Groups.GetGroupsAsync();
        //var groups = await _client.Groups();
        var group = groups.Value.FirstOrDefault(g => g.Name == name) ?? throw new Exception($"Group with name {name} not found");
        return group;
    }
}
