# BlueGreen Deployment of Power BI Reports and Datasets
[![.NET](https://github.com/AllDwarf/PowerBiService/actions/workflows/dotnet.yml/badge.svg?event=pull_request)](https://github.com/AllDwarf/PowerBiService/actions/workflows/dotnet.yml)

This project demonstrates how to perform a BlueGreen deployment of Power BI reports and datasets using the [Microsoft.PowerBI.API nugget package](https://github.com/Microsoft/PowerBI-CSharp). The BlueGreen deployment strategy allows you to deploy a new version of your reports and datasets alongside the existing version, and then switch traffic to the new version once it has been fully tested and verified.

## Getting Started

To get started with this project, you will need to have the following prerequisites:

- Visual Studio 2019 or later (or VS Code with .Net extension)
- .NET 7.0 and later
- Microsoft.PowerBI.API NuGet package
- Micorsoft.Identity

Once you have installed the prerequisites, you can clone the repository and open the solution in Visual Studio. You can then build and run the solution to deploy the reports and datasets.

## Usage
You can run dot net cli to perform certain commands using some arguments:
### Commands
- ```blueGreen```: Run Blue Green deployment for given workspaces.
    - ```dotnet run blueGreen --reportBlueId "123" --reportGreenId "456" --workspaceGreen "YOUR GREEN WORKSPACE NAME" --workspaceBlue "YOUR BLUE WORKSPACE NAME"```
- ```refresh```: Run refresh for all objects for given workspace.
    - ```dotnet run refresh --workspaceName "YOUR WORKSPACE NAME"```
- ```deploy```: Run deployment pipeline for given workspace.
    - ```dotnet run deploy --pipelineId "789" --stageOrder 0```
### Options
- ```blueGreen```
    - ```--reportBlueId```: The ID of the blue report.
    - ```--reportGreenId```: The ID of the green report.
    - ```--workspaceGreen```: The name of the green workspace.
    - ```--workspaceBlue```: The name of the blue workspace.
 - ```refresh```   
    - ```--workspaceName```: The name of the workspace.
- ```deploy```
    - ```--pipelineId```: The ID of the deployment pipeline.
    - ```--stageOrder```: The order of the stage in the deployment pipeline.
## Continuous Deployment

To enable continuous deployment of the reports and datasets, you can use Azure DevOps. You can create a new pipeline that builds and deploys the reports and datasets to the new workspace whenever changes are made to the code. You can also configure the pipeline to automatically test the new version of the reports and datasets and rollback to the previous version in case of issues.

## Conclusion

This project demonstrates how to perform a BlueGreen deployment of Power BI reports and datasets using the Microsoft.PowerBI.API NuGet package. By following the steps outlined in this README.md file, you can deploy new versions of your reports and datasets with confidence, knowing that you can easily rollback to the previous version in case of issues.