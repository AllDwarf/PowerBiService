using Microsoft.PowerBI.Api;

namespace PowerBiService.Services;
public interface IAzureAdAuth
{
    Task<PowerBIClient> AuthenticateAsync();
}
