using System.IO;
using Microsoft.Extensions.Configuration;

namespace DocumentManagment.DocumentStore.Blob.Tests
{
    public static class TestsConfigurationHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddUserSecrets(typeof(TestsConfigurationHelper).Assembly)
                .AddEnvironmentVariables()
                .Build();
        }

        public static BlobStorageOptions GetConfiguration()
        {
            return GetIConfigurationRoot()
                .GetSection(nameof(BlobStorageOptions))
                .Get<BlobStorageOptions>();
        }
    }
}