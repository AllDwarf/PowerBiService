using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Repositories
{
    public interface IDeploymentPipelineRepository
    {
        Task<bool> RunDeploymentPipelineForAllAsync( int stage, Pipeline pipeline );
        Task<Pipeline> GetPipelineByIdAsync( Guid pipelineId );
    }
}
