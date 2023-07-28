using PowerBiService.Repositories;
using PowerBiService.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Services;
public class CommandLineOptions
{
    Option<string> workspaceGreenOption = new Option<string>(
    name: "--workspaceGreen",
    description: "An option to define Workspace Name of Green Environment",
    getDefaultValue: () => "FHL Blue Green - Green");

    Option<string> workspaceBlueOption = new Option<string>(
        name: "--workspaceBlue",
        description: "An option to define Workspace Name of Blue Environment",
        getDefaultValue: () => "FHL Blue Green - Blue");

    Option<string> workspaceOption = new Option<string>(
        name: "--workspaceName",
        description: "An option to define Workspace Name of Blue Environment",
        getDefaultValue: () => "FHL Blue Green - Green");

    Option<int> stageOrderOption = new Option<int>(
        name: "--stageOrder",
        description: "An option to define on which stage should be deployment performed",
        getDefaultValue: () => 0);

    Option<string> pipelineOption = new Option<string>(
        name: "--pipelineId",
        description: "An option to define pipeline Id for deployment",
        getDefaultValue: () => "77064aec-df24-4526-a716-81cb538e8a2b");

    Option<string> reportGreenIdOption = new Option<string>(
        name: "--reportGreenId",
        description: "An option to define Green ReportId",
        getDefaultValue: () => "e0ddd407-a2ce-4ead-a6cd-033ad40d6c6d");

    Option<string> reportBlueIdOption = new Option<string>(
        name: "--reportBlueId",
        description: "An option to define Blue ReportId",
        getDefaultValue: () => "tst");

    RootCommand rootCommand = new RootCommand(".Net App for PBI CICD");
    public void Execute(DatasetRepository datasetRepository, WorkspaceRepository workspaceRepository, ReportRepository reportRepository, DeploymentPipelineRepository deploymentPipelineRepository)
    {
        rootCommand.AddGlobalOption(workspaceGreenOption);
        rootCommand.AddGlobalOption(workspaceBlueOption);

        var blueGreenCommand = new Command("blueGreen", "Run Blue Green deployment for given workspaces.");
        rootCommand.AddCommand(blueGreenCommand);
        blueGreenCommand.SetHandler((reportBlueId, reportGreenId, workspaceGreen, workspaceBlue) =>
        {
            new DeploymentBlueGreen(reportRepository, workspaceRepository, datasetRepository, reportBlueId, reportGreenId, workspaceGreen, workspaceBlue);
        }
        , reportBlueIdOption, reportGreenIdOption, workspaceGreenOption, workspaceBlueOption
        );

        var refreshCommand = new Command("refresh", "Run Refresh All Datasets for given workspace.");
        rootCommand.AddCommand(refreshCommand);
        refreshCommand.AddOption(workspaceOption);
        refreshCommand.SetHandler((workspaceName) =>
        {
            new AfterDeploymentRefresh(workspaceRepository, datasetRepository, workspaceName);
        },
            workspaceOption);

        var deploymentCommand = new Command("deploy", "Run Refresh All Datasets for given workspace.");
        rootCommand.AddCommand(deploymentCommand);
        deploymentCommand.AddOption(pipelineOption);
        deploymentCommand.AddOption(stageOrderOption);
        deploymentCommand.SetHandler((pipelineId, stageOrder) =>
        {
            new DeploymentPipeline(pipelineId, stageOrder, deploymentPipelineRepository);
        },
            pipelineOption, stageOrderOption);
    }
}