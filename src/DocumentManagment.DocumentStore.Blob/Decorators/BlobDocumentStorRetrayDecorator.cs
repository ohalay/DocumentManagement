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
    internal class BlobDocumentStorRetrayDecorator : IDocumentStore
    {
        private readonly IAsyncPolicy policy = Policy.Handle<RequestFailedException>(ex =>
            ex.ErrorCode == BlobErrorCode.OperationTimedOut.ToString()
            || ex.ErrorCode == BlobErrorCode.InternalError.ToString())
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        private readonly IDocumentStore docuemtnStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="BlobDocumentStorRetrayDecorator"/> class.
        /// </summary>
        /// <param name="docuemtnStore">Document store.</param>
        public BlobDocumentStorRetrayDecorator(IDocumentStore docuemtnStore)
        {
            this.docuemtnStore = docuemtnStore;
        }

        public Task<OperationResult> DeleteAsync(string name)
            => policy.ExecuteAsync(() => docuemtnStore.DeleteAsync(name));

        public Task<OperationResult<IReadOnlyCollection<DocumentEntity>>> GetAllAsync()
            => policy.ExecuteAsync(() => docuemtnStore.GetAllAsync());

        public Task<OperationResult> ReorderAsync(params DocumentEntity[] documents)
            => policy.ExecuteAsync(() => docuemtnStore.ReorderAsync(documents));

        public Task<OperationResult> UploadAsync(string name, Stream stream)
             => policy.ExecuteAsync(() => docuemtnStore.UploadAsync(name, stream));
    }
}
