using System;
using System.Collections.Generic;

namespace DocumentManagement.Core
{
    /// <summary>
    /// Operation result.
    /// </summary>
    public class OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult"/> class.
        /// </summary>
        /// <param name="errors">Errors.</param>
        public OperationResult(params string[] errors)
        {
            Errors = errors ?? Array.Empty<string>();
        }

        /// <summary>
        /// Successful.
        /// </summary>
        public bool Successful
        {
            get { return Errors.Count == 0; }
        }

        /// <summary>
        /// Errors.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        /// <summary>
        /// Create Successful result.
        /// </summary>
        /// <returns>Successful result.</returns>
        public static OperationResult SuccessfulResult() => new OperationResult();
    }
}
