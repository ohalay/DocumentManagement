using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs.Models;
using DocumentManagement.Core;
using DocumentManagement.Core.Component;
using DocumentManagement.Core.Domain;
using Polly;

namespace DocumentManagement.DocumentStore.Blob.Decorators
{
    /// <summary>
    /// Retry blob document store decorator.
    /// </summary>
    internal class BlobDocumentStorExceptionDecorator : IDocumentStore
    {
        private readonly IDocumentStore docuemtnStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDocumentStorExceptionDecorator"/> class.
        /// </summary>
        /// <param name="docuemtnStore">Document store.</param>
        public BlobDocumentStorExceptionDecorator(IDocumentStore docuemtnStore)
        {
            this.docuemtnStore = docuemtnStore;
        }

        public Task<OperationResult> DeleteAsync(string name)
            => SafeExecuteAsync(() => docuemtnStore.DeleteAsync(name));

        public Task<OperationResult<IReadOnlyCollection<DocumentEntity>>> GetAllAsync()
            => SafeExecuteAsync(() => docuemtnStore.GetAllAsync());

        public Task<OperationResult> ReorderAsync(params DocumentEntity[] documents)
            => SafeExecuteAsync(() => docuemtnStore.ReorderAsync(documents));

        public Task<OperationResult> UploadAsync(string name, Stream stream)
             => SafeExecuteAsync(() => docuemtnStore.UploadAsync(name, stream));

        private async Task<TResponse> SafeExecuteAsync<TResponse>(Func<Task<TResponse>> func)
            where TResponse : OperationResult
        {
            try
            {
                return await func();
            }
            catch (RequestFailedException e) when (e.ErrorCode == BlobErrorCode.BlobNotFound.ToString())
            {
                return new OperationResult($"Document name is not found.") as TResponse;
            }
            catch (RequestFailedException ex)
            {
                return new OperationResult(ex.Message) as TResponse;
            }
        }
    }
}
