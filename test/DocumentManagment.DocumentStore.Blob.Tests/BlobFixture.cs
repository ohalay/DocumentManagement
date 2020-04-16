using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoFixture;
using Azure.Storage.Blobs;

namespace DocumentManagment.DocumentStore.Blob.Tests
{
    public sealed class BlobFixture : IDisposable
    {
        private readonly BlobServiceClient blobClient;

        public BlobFixture()
        {
            BlobOptions = TestsConfigurationHelper.GetConfiguration();
            BlobOptions.ContainerPrefix = DateTime.UtcNow.Ticks.ToString(new NumberFormatInfo());

            blobClient = new BlobServiceClient(BlobOptions.ConnectionString);
            BlobServiceUri = blobClient.Uri;
        }

        public BlobStorageOptions BlobOptions { get; }

        public Uri BlobServiceUri { get; }

        public void Dispose()
        {
            DeleteAllContainersAsync(BlobOptions.ContainerPrefix)
                .GetAwaiter().GetResult();

            GC.SuppressFinalize(this);
        }

        private async Task DeleteAllContainersAsync(string prefix)
        {
            var pagenable = blobClient.GetBlobContainersAsync(prefix: prefix);

            await foreach (var blobItem in pagenable)
            {
                await blobClient.DeleteBlobContainerAsync(blobItem.Name);
            }
        }
    }
}
