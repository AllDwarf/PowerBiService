using Microsoft.PowerBI.Api.Models;
using PowerBiService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Services;

public class DeploymentPipelineService : IServiceRepository
{
    private int _deploymentStageOrder;
    private Guid _pipelineId;
    private readonly IDeploymentPipelineRepository _deploymentPipelineRepository;
    public DeploymentPipelineService(string pipelineId, int deploymentStageOrder, IDeploymentPipelineRepository deploymentPipelineRepository)
    {
        _pipelineId = new Guid(pipelineId);
        _deploymentStageOrder = deploymentStageOrder;
        _deploymentPipelineRepository = deploymentPipelineRepository;
    }

    public async Task InvokeServiceAsync()
    {
        var pipeline = await _deploymentPipelineRepository.GetPipelineByIdAsync(_pipelineId);
        await _deploymentPipelineRepository.RunDeploymentPipelineForAllAsync(_deploymentStageOrder, pipeline);
    }
}
