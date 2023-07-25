using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api.Models;
using PowerBiService.Models;

namespace PowerBiService.Repositories;
public interface IDatasetRequestRepository
{
    Task<IList<Table>> GetTablesAsync(Guid workspaceId, string datasetId);
    Task<IList<Datasource>> GetDataSourcesAsync(Guid workspaceId, string datasetId);
    Task<IList<Relationship>> GetRelationshipsAsync(Guid workspaceId, string datasetId);
    Task<DatasetRequestBody> GetDatasetRequestBodyAsync(Group workspace, string datasetName, IList<Table> tables, IList<Datasource> datasources, IList<Relationship> relationships);
}
