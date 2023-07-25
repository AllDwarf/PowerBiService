using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;

namespace PowerBiService.Services;
public class SecretManager : ISecretManager
{
    private readonly SecretClient _client;
    private readonly IDataProtector _protector;

    public SecretManager(SecretClient client, IDataProtector dataProtector)
    {
        _client = client;
        _protector = dataProtector;
    }

    // Retrieve a secret from Azure Key Vault and return the protected secret value.
    private async Task<String> GetProtectedSecretAsync(string secretName)
    {
        var secret = await _client.GetSecretAsync(secretName);
        if (secret == null)
        {
            throw new Exception($"Secret {secretName} not found.");
        }
        var val = secret.Value.Value;
        if (val == null)
        {
            throw new Exception($"Secret {secretName} has no value.");
        }
        var protectedSecret = _protector.Protect(val);
        return protectedSecret;
    }

    // Retrieve a secret from Azure Key Vault and return the protected secret value.
    public async Task<String> GetSecret(string secretName)
    {
        return await GetProtectedSecretAsync(secretName);
    }
}
