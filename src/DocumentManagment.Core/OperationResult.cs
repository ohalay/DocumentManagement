using System;
using System.Collections.Generic;

namespace DocumentManagement.Core
{
    public class OperationResult
    {
        public OperationResult(bool successful, IReadOnlyCollection<string> errors = null)
        {
            Successful = successful;
            Errors = errors ?? Array.Empty<string>();
        }

        public static OperationResult SuccessfulResult => new OperationResult(true);

        public static OperationResult FailedResult(params string[] errors) => new OperationResult(false, errors);

        public bool Successful { get; }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
