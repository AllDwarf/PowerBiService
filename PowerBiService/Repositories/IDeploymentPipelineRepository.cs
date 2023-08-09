using Microsoft.PowerBI.Api.Models;

namespace PowerBiService.Repositories
{
    public interface IDeploymentPipelineRepository
    {
        Task<bool> RunDeploymentPipelineForAllAsync( int stage, Pipeline pipeline );
        Task<Pipeline> GetPipelineByIdAsync( Guid pipelineId );
    }
}
