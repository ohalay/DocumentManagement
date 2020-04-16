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
        public async Task<IReadOnlyCollection<Document>> GetAllAsync()
        {
            var entities = await documentStore.GetAllAsync();

            return entities
                .Select(Document.ToDocument)
                .ToArray();
        }

        /// <summary>
        /// Upload document.
        /// </summary>
        /// <param name="file">File.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("")]
        public async Task<OperationResult> UpploadAsync(IFormFile file)
        {
            var (operatinoResult, entity) = DocumentEntity.Create(file.FileName, file.Length, null);

            if (!operatinoResult.Successful)
            {
                return operatinoResult;
            }

            return await documentStore.UploadAsync(entity.Name, file.OpenReadStream());
        }
    }
}