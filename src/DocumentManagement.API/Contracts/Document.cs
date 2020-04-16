using System;
using DocumentManagement.Core.Domain;

namespace DocumentManagement.API.Contracts
{
    /// <summary>
    /// Document.
    /// </summary>
    public class Document
    {
        /// <summary>
        /// Document name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// File size.
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Location.
        /// </summary>
        public Uri Location { get; set; }

        /// <summary>
        /// Order.
        /// </summary>
        public long Order { get; set; }

        internal static Document ToDocument(DocumentEntity entity)
        {
            return new Document
            {
                FileSize = entity.FileSize,
                Location = entity.Location,
                Name = entity.Name,
                Order = entity.Order,
            };
        }
    }
}
