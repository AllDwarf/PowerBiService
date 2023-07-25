using System.Net;
using Microsoft.Identity.Client;
using Moq;
using PowerBiService.Models;
using PowerBiService.Services;
using Xunit;

namespace PowerBiService.Tests.Services;
public class AzureAdAuthTests
{
    [Fact]
    public async Task AzureAdAuth_WithValidCredentials_ReturnsAuthenticationResult()
    {
        // Arrange
        var clientId = "your-client-id";
        var clientSecret = "your-client-secret";
        var authorityUri = "https://login.microsoftonline.com/your-tenant-id";
        var resourceUri = "https://analysis.windows.net/powerbi/api";
        string[] scopeBase = { "https://analysis.windows.net/powerbi/api/.default" };
        var expectedToken = "your-expected-token";

        //// Set up the mock public client application to return the expected token
        //var mockApp = new Mock<IConfidentialClientApplication>();
        //await mockApp.Setup(a => a.AcquireTokenForClient(scopeBase).ExecuteAsync())
        //    .ReturnsAsync(new AuthenticationResult(expectedToken));

        //// Create a new AzureAdAuth object with the mock public client application
        //var auth = new AzureAdAuth(mockApp.Object);

        //// Act
        //var result = await auth.Authenticate(clientId, clientSecret, authorityUri, resourceUri, username, password);

        // Assert
        //Assert.Equal(expectedToken, result.AccessToken);
    }
}
