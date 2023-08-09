﻿using PowerBiService.Repositories;
using PowerBiService.Services;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace PowerBiService.Services;
public class CommandLineOptions
{
    readonly Option<string> workspaceGreenOption = new(
    name: "--workspaceGreen",
    description: "An option to define Workspace Name of Green Environment",
    getDefaultValue: () => "FHL Blue Green - Green");

    readonly Option<string> workspaceBlueOption = new(
        name: "--workspaceBlue",
        description: "An option to define Workspace Name of Blue Environment",
        getDefaultValue: () => "FHL Blue Green - Blue");

    readonly Option<string> workspaceOption = new(
        name: "--workspaceName",
        description: "An option to define Workspace Name of Blue Environment",
        getDefaultValue: () => "FHL Blue Green - Green");

    readonly Option<int> stageOrderOption = new(
        name: "--stageOrder",
        description: "An option to define on which stage should be deployment performed",
        getDefaultValue: () => 0);

    readonly Option<string> pipelineOption = new(
        name: "--pipelineId",
        description: "An option to define pipeline Id for deployment",
        getDefaultValue: () => "77064aec-df24-4526-a716-81cb538e8a2b");

    readonly Option<string> reportGreenIdOption = new(
        name: "--reportGreenId",
        description: "An option to define Green ReportId",
        getDefaultValue: () => "e0ddd407-a2ce-4ead-a6cd-033ad40d6c6d");

    readonly Option<string> reportBlueIdOption = new(
        name: "--reportBlueId",
        description: "An option to define Blue ReportId",
        getDefaultValue: () => "tst");

    readonly RootCommand rootCommand = new(".Net App for PBI CICD");
    public void Execute(DatasetRepository datasetRepository, WorkspaceRepository workspaceRepository, ReportRepository reportRepository, DeploymentPipelineRepository deploymentPipelineRepository, string[] args)
    {
        CommandSetter(datasetRepository, workspaceRepository, reportRepository, deploymentPipelineRepository, out Command blueGreenCommand, out Command refreshCommand, out Command deploymentCommand);

        // Parse the command line arguments
        var parseResult = rootCommand.Parse(args);

        // Check if a subcommand was specified
        if (parseResult.CommandResult.Command != rootCommand)
        {
            // A subcommand was specified
            var subcommand = parseResult.CommandResult.Command;
            Console.WriteLine($"Subcommand '{subcommand.Name}' specified");

            // Check for options of the subcommand
            foreach (var option in subcommand.Options)
            {
                Console.WriteLine($"Option '{option.Name}' specified with value '{parseResult.CommandResult.GetValueForOption(option)}'");
                if (parseResult.CommandResult.Command.Name == "refresh" && option.Name == "workspaceName" && parseResult.CommandResult.GetValueForOption(option) != null)
                {
                    refreshCommand.Invoke(args);
                }
                else if (parseResult.CommandResult.Command.Name == "deploy" && option.Name == "pipelineId" && option.Name == "stageOrder" && parseResult.CommandResult.GetValueForOption(option) != null)
                {
                    deploymentCommand.Invoke(args);
                }
                else if (parseResult.CommandResult.Command.Name == "blueGreen" && option.Name == "workspaceGreen" && option.Name == "workspaceBlue" && parseResult.CommandResult.GetValueForOption(option) != null)
                {
                    blueGreenCommand.Invoke(args);
                }
            }
        }
        else
        {
            // No subcommand was specified
            Console.WriteLine("No subcommand specified");
        }
    }

    private void CommandSetter(DatasetRepository datasetRepository, WorkspaceRepository workspaceRepository, ReportRepository reportRepository, DeploymentPipelineRepository deploymentPipelineRepository, out Command blueGreenCommand, out Command refreshCommand, out Command deploymentCommand)
    {
        // Initiate Blue Green deployment
        blueGreenCommand = new Command("blueGreen", "Run Blue Green deployment for given workspaces.");
        rootCommand.AddCommand(blueGreenCommand);
        blueGreenCommand.AddGlobalOption(workspaceGreenOption);
        blueGreenCommand.AddGlobalOption(workspaceBlueOption);
        blueGreenCommand.SetHandler(async (reportBlueId, reportGreenId, workspaceGreen, workspaceBlue) =>
        {
            Console.WriteLine("Running Blue Green Deployment");
            var blueGreenService = new BlueGreenService(
                reportRepository, workspaceRepository, datasetRepository, 
                reportBlueId, reportGreenId, workspaceGreen, workspaceBlue);
            await blueGreenService.InvokeServiceAsync();
        }
        , reportBlueIdOption, reportGreenIdOption, workspaceGreenOption, workspaceBlueOption
        );

        // Refresh all datasets for a given workspace
        refreshCommand = new Command("refresh", "Run Refresh All Datasets for given workspace.");
        rootCommand.AddCommand(refreshCommand);
        refreshCommand.AddGlobalOption(workspaceOption);
        refreshCommand.SetHandler(async (workspaceName) =>
        {
            Console.WriteLine("Running After Deployment Refresh");
            var afterDeploymentRefresh = new RefreshService(workspaceRepository, datasetRepository, workspaceName);
            await afterDeploymentRefresh.InvokeServiceAsync();
        },
            workspaceOption);

        // Deployment of a pipeline
        deploymentCommand = new Command("deploy", "Run Refresh All Datasets for given workspace.");
        rootCommand.AddCommand(deploymentCommand);
        deploymentCommand.AddGlobalOption(pipelineOption);
        deploymentCommand.AddGlobalOption(stageOrderOption);
        deploymentCommand.SetHandler(async (pipelineId, stageOrder) =>
        {
            Console.WriteLine($"Running After Deployment of pipeline: {pipelineId}");
            var deploymentPipelineService = new DeploymentPipelineService(pipelineId, stageOrder, deploymentPipelineRepository);
            await deploymentPipelineService.InvokeServiceAsync();
        },
            pipelineOption, stageOrderOption);
    }
}