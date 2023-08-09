namespace PowerBiService.Services;
public interface ISecretManager
{
    Task<String> GetSecret(string secretName);
}
