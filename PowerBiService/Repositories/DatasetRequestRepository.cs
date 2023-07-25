using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using PowerBiService.Models;

namespace PowerBiService.Repositories;
public class DatasetRequestRepository : IDatasetRequestRepository
{
    private readonly IPowerBIClient _client;
    public DatasetRequestRepository(IPowerBIClient client)
    {
        _client = client;
    }

    public async Task<DatasetRequestBody> GetDatasetRequestBodyAsync(Group workspace, string datasetName, IList<Table> tables, IList<Datasource> datasources, IList<Relationship> relationships)
    {
        DatasetRequestBody dsRequestBody = new DatasetRequestBody
        {
            Name = datasetName,
            Tables = tables,
            Relationships = relationships,
            Datasources = datasources,
        };

        return dsRequestBody;
    }

    public async Task<IList<Datasource>> GetDataSourcesAsync(Guid workspaceId, string datasetId)
    {
        var dataSources = await _client.Datasets.GetDatasourcesAsync(workspaceId, datasetId);
        return dataSources.Value;
    }

    public async Task<IList<Relationship>> GetRelationshipsAsync(Guid workspaceId, string datasetId)
    {
        IList<Relationship> relationships = new List<Relationship>();
        return relationships;
    }

    public async Task<IList<Table>> GetTablesAsync(Guid workspaceId, string datasetId)
    {
        var tables = await _client.Datasets.GetTablesAsync(workspaceId, datasetId);
        return tables.Value;
    }

}

