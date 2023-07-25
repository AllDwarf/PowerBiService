using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Models;
public class DatasetRequestBody
{
    public string Name { get; set; }
    public IList<Table> Tables { get; set; }
    public IList<Relationship> Relationships { get; set; }
    public IList<Datasource> Datasources { get; set; }
    //public DatasetMode DatasetMode { get; set; }


}
