using System;
using System.Runtime.Caching;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace Damco.Common
{
    public class KeyVaultService
    {
        private static readonly InMemoryCache _cache = new InMemoryCache();
        private static readonly Lazy<SecretClient> secretClientLazy = new Lazy<SecretClient>(() =>
        {
            return new SecretClient(new Uri(ConfigurationProvider.Get(ConfigurationProvider.KeyVaultUriSettingKey)), new DefaultAzureCredential());
        });

        public string GetSecret(string secretName)
        {
            return _cache.GetOrCreate(
                 secretName,
                 () =>
                 {
                     if (EnvironmentHelper.IsAzure)
                     {
                         var secretClient = secretClientLazy.Value;
                         var secret = secretClient.GetSecret(secretName);
                         return secret.Value.Value;
                     }
                     else
                     {
                         var result = ConfigurationProvider.Get(secretName);
                         Guard.Against(result, $"Unable to retrieve secret : '{secretName}' from configuration");
                         return result;
                     }
                 },
                 new CacheItemPolicy
                 {
                     SlidingExpiration = TimeSpan.FromHours(1)
                 });
        }
    }
}