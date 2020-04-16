using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DocumentManagement.Core;
using DocumentManagement.Core.Component;
using DocumentManagement.Core.Domain;
using Microsoft.Extensions.Options;

namespace DocumentManagement.DocumentStore.Blob
{
    /// <summary>
    /// Blob document store.
    /// </summary>
    public class BlobDocumentStore : IDocumentStore
    {
        private readonly BlobContainerClient container;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDocumentStore"/> class.
        /// </summary>
        /// <param name="options">Blob storage options.</param>
        public BlobDocumentStore(IOptions<BlobStorageOptions> options)
        {
            var containerName = string.Concat(options.Value.ContainerPrefix, options.Value.ContainerName);
            container = new BlobContainerClient(options.Value.ConnectionString, containerName);

            container.CreateIfNotExists(PublicAccessType.Blob);
        }

        /// <inheritdoc/>
        public Task<OperationResult> DeleteAsync(string name)
        {
            return SafeExecuteAsync(() => container.DeleteBlobAsync(name), name);
        }

        /// <inheritdoc/>
        public async Task<IReadOnlyCollection<DocumentEntity>> GetAllAsync()
        {
            var documents = new List<DocumentEntity>();

            await foreach (var blob in container.GetBlobsAsync(BlobTraits.Metadata))
            {
                var order = long.Parse(blob.Metadata[nameof(DocumentEntity.Order)], new NumberFormatInfo());
                var uri = $"{container.Uri.AbsoluteUri}/{blob.Name}";

                var (_, entity) = DocumentEntity.Create(blob.Name, (long)blob.Properties.ContentLength, new Uri(uri), order);
                documents.Add(entity);
            }

            return documents.ToArray();
        }

        /// <inheritdoc/>
        public async Task<OperationResult> ReorderAsync(params DocumentEntity[] documents)
        {
            var operationResult = OperationResult.SuccessfulResult();
            var metadata = new Dictionary<string, string>()
            {
                [nameof(DocumentEntity.Order)] = "0",
            };
            foreach (var document in documents)
            {
                var client = container.GetBlobClient(document.Name);
                metadata[nameof(DocumentEntity.Order)] = document.Order.ToString(new NumberFormatInfo());

                var result = await SafeExecuteAsync(
                    async () =>
                    {
                        var response = await client.SetMetadataAsync(metadata);
                        return response.GetRawResponse();
                    },
                    document.Name);

                var errors = result.Errors.Union(operationResult.Errors).ToArray();
                operationResult = new OperationResult(errors);
            }

            return operationResult;
        }

        /// <inheritdoc/>
        public async Task<OperationResult> UploadAsync(string name, Stream stream)
        {
            var metadata = new Dictionary<string, string>()
            {
                [nameof(DocumentEntity.Order)] = "0",
            };

            var client = container.GetBlobClient(name);
            var blobResponse = await client.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "application/pdf" },
                metadata);

            return blobResponse.GetRawResponse().ToOperationResult(name);
        }

        private async Task<OperationResult> SafeExecuteAsync(Func<Task<Response>> func, string name)
        {
            try
            {
                var respose = await func();
                return respose.ToOperationResult(name);
            }
            catch (RequestFailedException e) when (e.ErrorCode == BlobErrorCode.BlobNotFound.ToString())
            {
                return new OperationResult($"Document with name '{name}' is not found.");
            }
        }
    }
}
