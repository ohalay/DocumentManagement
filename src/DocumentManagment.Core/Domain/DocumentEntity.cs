using System;
using System.Text.RegularExpressions;

namespace DocumentManagement.Core.Domain
{
    public class DocumentEntity
    {
        private const string ValidFileNamePattern = @"^[0-9a-zA-Z \$\-_.+!*'(),]{1,1024}$";
        private const int MaxFileSizeButes = 5242880; // 5Mb
        public DocumentEntity(string name, int fileSize, Uri location, long order = 0)
        {
            Name = name;
            FileSize = fileSize;
            Location = location;
            Order = order;
        }

        public static (OperationResult, DocumentEntity) Create(string name, int size, Uri path, long order = 0)
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

            return (OperationResult.SuccessfulResult, new DocumentEntity(name, size, path));
        }

        public string Name { get; }

        public int FileSize { get; }

        public Uri Location { get; }

        public long Order { get; }
    }
}
