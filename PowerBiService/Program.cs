using Microsoft.Extensions.Configuration;
using PowerBiService.Models;
using PowerBiService.Services;
using PowerBiService.Repositories;
using Microsoft.Extensions.Options;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.DataProtection;

// Change SetBasePath to the path of your appsettings.json file in the root folder
var projectPath = Directory.GetCurrentDirectory();
var config = new ConfigurationBuilder()
    .SetBasePath(projectPath)
    .AddJsonFile("appsettings.development.json", optional: false)
    .AddCommandLine(args)
    .Build();

// Bind the AzureAd section of appsettings.json to the AzureAd class
var azureAd = new AzureAd();
config.Bind("AzureAd", azureAd);
IOptions<AzureAd> azureAdOption = Options.Create(azureAd);

// If config or azureAd is not set skip the rest of the code
if (config == null || azureAd == null || azureAd.KeyVaultUrl == null || azureAd.ClientSecretName == null)
{
    Console.WriteLine("Config or AzureAd is null");
    return;
}
// Get the secret from Azure Key Vault using protected values
SecretClient secretClient = new(new Uri(azureAd.KeyVaultUrl), new DefaultAzureCredential());
var dataProtector = DataProtectionProvider.Create("PowerBiServiceProtectProvider").CreateProtector("PowerBiServiceProtector");
var secretManager = new SecretManager(secretClient, dataProtector);
var secret = await secretManager.GetSecret(secretName: azureAd.ClientSecretName);

// Create an instance of the AuthenticationHandler class and pass in the azureAdOption
AzureAdAuth authenticator = new(azureAdOption, secret, dataProtector);
using var client = await authenticator.AuthenticateAsync();

// Initialize the PowerBiHandler and pass in the configuration, reportRepository, workspaceRepository, and datasetRepository
var reportRepository = new ReportRepository(client);
var workspaceRepository = new WorkspaceRepository(client);
var datasetRepository = new DatasetRepository(client);
var deploymentPipelineRepository = new DeploymentPipelineRepository(client);

// Run CommandOptions
var cmdLineOptions = new CommandLineOptions();
Console.WriteLine("Taking cmd line args");
cmdLineOptions.Execute(datasetRepository, workspaceRepository, reportRepository, deploymentPipelineRepository, args);
