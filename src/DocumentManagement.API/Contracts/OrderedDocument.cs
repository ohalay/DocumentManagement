using System;
using System.ComponentModel.DataAnnotations;
using DocumentManagement.Core.Domain;

namespace DocumentManagement.API.Contracts
{
    /// <summary>
    /// Ordered Document.
    /// </summary>
    public class OrderedDocument
    {
        /// <summary>
        /// Document name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Order.
        /// </summary>
        public long Order { get; set; } = 0;
    }
}
