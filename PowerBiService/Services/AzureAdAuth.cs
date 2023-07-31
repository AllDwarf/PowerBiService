using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Abstractions;
using Microsoft.Identity.Client;
using Microsoft.Identity.Web;
using Microsoft.PowerBI.Api;
using Microsoft.Rest;
using PowerBiService.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;

namespace PowerBiService.Services;
public class AzureAdAuth : IAzureAdAuth
{
    private readonly IOptions<AzureAd> azureAd;
    private readonly IDataProtector _protector;
    private string[] scopes = new string[] { "user.read" };
    private readonly string _secret;
    public AzureAdAuth(IOptions<AzureAd> azureAd, string secret, IDataProtector dataProtector)
    {
        this.azureAd = azureAd;
        _secret = secret;
        _protector = dataProtector;
    }

    public async Task<PowerBIClient> AuthenticateAsync()
    {
        AuthenticationResult? authenticationResult = null; // Initialize the result variable
        // Check for non null reference for authenticationResult
        if (authenticationResult == null || azureAd.Value.PbiPassword == null)
        {
            throw new ArgumentNullException(nameof(authenticationResult));
        }

        if (azureAd.Value.AuthenticationMode.Equals("masteruser", StringComparison.InvariantCultureIgnoreCase)) // If the authentication mode is master user...
        {
            // Create a public client to authorize the app with the AAD app
            IPublicClientApplication clientApp = PublicClientApplicationBuilder.Create(azureAd.Value.ClientId).WithAuthority(azureAd.Value.AuthorityUrl).Build();
            var userAccounts = clientApp.GetAccountsAsync().Result; // Get the user accounts

            SecureString password = new SecureString(); // Initialize a secure string to hold the password
            for (int i = 0; i < azureAd.Value.PbiPassword.Length; i++) // Loop through each character of the password
            {
                char key = azureAd.Value.PbiPassword[i];
                password.AppendChar(key); // Add the character to the secure string
            }
            // Acquire Access token from AAD app to access Power BI interactively using Master user credential
            authenticationResult = await clientApp.AcquireTokenByIntegratedWindowsAuth(azureAd.Value.ScopeBase).WithUsername(azureAd.Value.PbiUsername).ExecuteAsync();
 
        }

        // Service Principal auth is the recommended by Microsoft to achieve App Owns Data Power BI embedding
        else if (azureAd.Value.AuthenticationMode.Equals("serviceprincipal", StringComparison.InvariantCultureIgnoreCase))
        {
            // For app only authentication, we need the specific tenant id in the authority url
            var tenantSpecificUrl = azureAd.Value.AuthorityUrl.Replace("organizations", azureAd.Value.TenantId);

            // Create a confidential client to authorize the app with the AAD app
            IConfidentialClientApplication clientApp = ConfidentialClientApplicationBuilder
                                                                            .Create(azureAd.Value.ClientId)
                                                                            .WithClientSecret(_protector.Unprotect(_secret))
                                                                            .WithAuthority(tenantSpecificUrl)
                                                                            .Build();
            // Make a client call if Access token is not available in cache
            authenticationResult = clientApp.AcquireTokenForClient(azureAd.Value.ScopeBase).ExecuteAsync().Result;
        }
        var credential = new TokenCredentials(authenticationResult.AccessToken, "Bearer");
        var client = new PowerBIClient(new Uri("https://api.powerbi.com/"), credential);
        return client;
    }
}
