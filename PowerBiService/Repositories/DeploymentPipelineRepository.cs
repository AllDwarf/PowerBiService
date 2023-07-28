using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PowerBiService.Repositories;
public class DeploymentPipelineRepository : IDeploymentPipelineRepository
{
    // Use Azure active directory authentication to connect to the Power BI service
    private readonly IPowerBIClient _client;
    // Use logger to log errors
    public DeploymentPipelineRepository(IPowerBIClient client)
    {
        _client = client;
    }
    public async Task<Pipeline> GetPipelineByIdAsync(Guid pipelineId)
    {
        var pipeline = await _client.Pipelines.GetPipelineAsync(pipelineId);
        return pipeline;
    }

    public async Task<bool> RunDeploymentPipelineForAllAsync(int stage, Pipeline pipeline)
    {
        var deployRequest = new DeployAllRequest();
        deployRequest.SourceStageOrder = stage;
        deployRequest.Options.AllowOverwriteArtifact = true;
        deployRequest.Options.AllowCreateArtifact = true;

        var pipelineOperation = await _client.Pipelines.DeployAllAsync(pipeline.Id, deployRequest);
        var pipelineOperationResult = "";
        // Wait for the import to complete
        while (pipelineOperation.Status != "Succeeded" && pipelineOperation.Status != "Failed")
        {
            var pipelineOperationUpdated = await _client.Pipelines.GetPipelineOperationAsync(pipeline.Id, pipelineOperation.Id);
            pipelineOperationResult = pipelineOperationUpdated.Status;
            await Task.Delay(1000);
        }
        if (pipelineOperationResult != "Succeeded")
        {
            return false;
        }
        return true;
    }
}
