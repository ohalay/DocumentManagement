using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentManagement.API.Contracts;
using DocumentManagement.Core;
using DocumentManagement.Core.Component;
using DocumentManagement.Core.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DocumentManagement.API
{
    /// <summary>
    /// Document controller.
    /// </summary>
    [Route("api/[controller]")]
    [AddStatusCodeFileter]
    public class DocumentsController
    {
        private readonly IDocumentStore documentStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentsController"/> class.
        /// </summary>
        /// <param name="documentStore">Document store.</param>
        public DocumentsController(IDocumentStore documentStore)
        {
            this.documentStore = documentStore;
        }

        /// <summary>
        /// Get all documents.
        /// </summary>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpGet("")]
        public async Task<OperationResult<IReadOnlyCollection<Document>>> GetAllAsync()
        {
            var operationResult = await documentStore.GetAllAsync();

            if (operationResult.Successful)
            {
                var documents = operationResult.Result
                    .Select(Document.ToDocument)
                    .OrderBy(s => s.Order)
                    .ToArray();
                return new OperationResult<IReadOnlyCollection<Document>>(documents);
            }

            return new OperationResult<IReadOnlyCollection<Document>>(operationResult.Errors.ToArray());
        }

        /// <summary>
        /// Upload document.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("")]
        public async Task<OperationResult> UpploadAsync(IFormFile file)
        {
            var operatinoResult = DocumentEntity.Create(file.FileName, file.Length, null);

            if (!operatinoResult.Successful)
            {
                return operatinoResult;
            }

            return await documentStore.UploadAsync(operatinoResult.Result.Name, file.OpenReadStream());
        }

        /// <summary>
        /// Delete document.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpDelete("{fileName}")]
        public Task<OperationResult> DeleteAsync(string fileName)
        {
            return documentStore.DeleteAsync(fileName);
        }

        /// <summary>
        /// Reorder document.
        /// </summary>
        /// <param name="documents">New documents order.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPatch("")]
        public async Task<OperationResult> ReorderAsync([FromBody]OrderedDocument[] documents)
        {
            const long fakeSize = 1;

            var entities = documents.Select(s => DocumentEntity.Create(s.Name, fakeSize, null, s.Order))
                .ToArray();

            if (entities.Any(s => !s.Successful))
            {
                var errors = entities
                    .Aggregate(Enumerable.Empty<string>(), (ac, next) => next.Successful ? ac : ac.Union(next.Errors))
                .ToArray();

                return new OperationResult(errors);
            }

            return await documentStore.ReorderAsync(entities.Select(s => s.Result).ToArray());
        }
    }
}