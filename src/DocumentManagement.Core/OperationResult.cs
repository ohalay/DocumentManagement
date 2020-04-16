using System;
using System.Collections.Generic;

namespace DocumentManagement.Core
{
    /// <summary>
    /// Operation result.
    /// </summary>
    public class OperationResult
    {
        private OperationResult(bool successful, IReadOnlyCollection<string> errors = null)
        {
            Successful = successful;
            Errors = errors ?? Array.Empty<string>();
        }

        /// <summary>
        /// Successful.
        /// </summary>
        public bool Successful { get; }

        /// <summary>
        /// Errors.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Create Successful result.
        /// </summary>
        /// <returns>Successful result.</returns>
        public static OperationResult SuccessfulResult() => new OperationResult(true);

        /// <summary>
        /// Create failed result.
        /// </summary>
        /// <param name="errors">Errors.</param>
        /// <returns>Failed result.</returns>
        public static OperationResult FailedResult(params string[] errors) => new OperationResult(false, errors);
    }
}
