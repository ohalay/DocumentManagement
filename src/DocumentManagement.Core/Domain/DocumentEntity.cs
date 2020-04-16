using System;
using System.Text.RegularExpressions;

namespace DocumentManagement.Core.Domain
{
    /// <summary>
    /// Document entity.
    /// </summary>
    public class DocumentEntity
    {
        private const string ValidFileNamePattern = @"^[0-9a-zA-Z \$\-_.+!*'(),]{1,1024}$";
        private const int MaxFileSizeButes = 5242880; // 5Mb

        private DocumentEntity(string name, int fileSize, Uri location, long order = 0)
        {
            Name = name;
            FileSize = fileSize;
            Location = location;
            Order = order;
        }

        /// <summary>
        /// Document name.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// File size.
        /// </summary>
        public int FileSize { get; }

        /// <summary>
        /// Location.
        /// </summary>
        public Uri Location { get; }

        /// <summary>
        /// Order.
        /// </summary>
        public long Order { get; }

        /// <summary>
        /// Create document entity.
        /// </summary>
        /// <param name="name">Document name.</param>
        /// <param name="size">Size.</param>
        /// <param name="location">Location.</param>
        /// <param name="order">Order.</param>
        /// <returns>Document entity.</returns>
        public static (OperationResult, DocumentEntity) Create(string name, int size, Uri location, long order = 0)
        {
            if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, ValidFileNamePattern))
            {
                var errorMessage = $"Invalid document name '{name}'. Supported name should contains only alphanumerics or special character '$-_.+!*'(),'.";
                return (OperationResult.FailedResult(errorMessage), default);
            }

            if (size > MaxFileSizeButes)
            {
                var errorMessage = $"Invalid file size '{size}'. Supported file size should be less then 5Mb.";
                return (OperationResult.FailedResult(errorMessage), default);
            }

            return (OperationResult.SuccessfulResult(), new DocumentEntity(name, size, location, order));
        }
    }
}
