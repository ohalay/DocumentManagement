﻿using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DocumentManagement.Core.Domain;

namespace DocumentManagement.Core.Component
{
    /// <summary>
    /// Document store.
    /// </summary>
    public interface IDocumentStore
    {
        /// <summary>
        /// Upload document to document store.
        /// </summary>
        /// <param name="name">Document name.</param>
        /// <param name="stream">Document stream.</param>
        /// <returns>Operation result.</returns>
        Task<OperationResult> UploadAsync(string name, Stream stream);

        /// <summary>
        /// Delete document from document store.
        /// </summary>
        /// <param name="name">Document name.</param>
        /// <returns>Operation result.</returns>
        Task<OperationResult> DeleteAsync(string name);

        /// <summary>
        /// Get all async.
        /// </summary>
        /// <returns>Collection of document entity.</returns>
        Task<OperationResult<IReadOnlyCollection<DocumentEntity>>> GetAllAsync();

        /// <summary>
        /// Reorder async.
        /// </summary>
        /// <param name="documents">Entities.</param>
        /// <returns>Operation result.</returns>
        Task<OperationResult> ReorderAsync(params DocumentEntity[] documents);
    }
}
