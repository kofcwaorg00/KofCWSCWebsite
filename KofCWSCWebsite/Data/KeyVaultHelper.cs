using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace KofCWSCWebsite.Data
{
    public class KeyVaultHelper
    {
        private readonly SecretClient _secretClient;
        private readonly IConfiguration _configuration;



        public KeyVaultHelper(IConfiguration configuration)
        {
            _configuration = configuration;
            var kvURLAZ = _configuration["KV:VAULTURL"];
            _secretClient = new SecretClient(new Uri(kvURLAZ), new DefaultAzureCredential());
        }

        public string GetSecret(string secretName)
        {
            try
            {
                KeyVaultSecret secret = _secretClient.GetSecret(secretName);
                return secret.Value; // Return connection string from Key Vault
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching secret: {ex.Message}");
                return string.Empty; // Return empty string on failure
            }
        }

    }
}
