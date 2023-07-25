# BlueGreen Deployment of Power BI Reports and Datasets

This project demonstrates how to perform a BlueGreen deployment of Power BI reports and datasets using the [Microsoft.PowerBI.API nugget package](https://github.com/Microsoft/PowerBI-CSharp). The BlueGreen deployment strategy allows you to deploy a new version of your reports and datasets alongside the existing version, and then switch traffic to the new version once it has been fully tested and verified.

## Getting Started

To get started with this project, you will need to have the following prerequisites:

- Visual Studio 2019 or later (or VS Code with .Net extension)
- .NET 7.0 and later
- Microsoft.PowerBI.API NuGet package

Once you have installed the prerequisites, you can clone the repository and open the solution in Visual Studio. You can then build and run the solution to deploy the reports and datasets.

## Deployment

To deploy the reports and datasets, you can use the following steps:

1. Create a new workspace in Power BI for the new version of the reports and datasets.
2. Deploy the new version of the reports and datasets to the new workspace using the Microsoft.PowerBI.API NuGet package.
3. Test the new version of the reports and datasets to ensure that they are working correctly.
4. Once the new version has been fully tested and verified, switch traffic to the new version by updating the connection strings in your application or redirecting traffic to the new workspace.
5. Monitor the new version of the reports and datasets to ensure that they are working correctly.
6. If any issues are found, rollback to the previous version by redirecting traffic back to the old workspace.

## Continuous Deployment

To enable continuous deployment of the reports and datasets, you can use Azure DevOps. You can create a new pipeline that builds and deploys the reports and datasets to the new workspace whenever changes are made to the code. You can also configure the pipeline to automatically test the new version of the reports and datasets and rollback to the previous version in case of issues.

## Conclusion

This project demonstrates how to perform a BlueGreen deployment of Power BI reports and datasets using the Microsoft.PowerBI.API NuGet package. By following the steps outlined in this README.md file, you can deploy new versions of your reports and datasets with confidence, knowing that you can easily rollback to the previous version in case of issues.