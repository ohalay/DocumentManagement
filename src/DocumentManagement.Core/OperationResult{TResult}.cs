using System;
using System.Collections.Generic;

namespace DocumentManagement.Core
{
    /// <summary>
    /// Operation result.
    /// </summary>
    /// <typeparam name="TResult">Model type.</typeparam>
    public class OperationResult<TResult> : OperationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TResult}"/> class.
        /// </summary>
        /// <param name="result">Result.</param>
        public OperationResult(TResult result)
            : base()
        {
            Result = result;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OperationResult{TResult}"/> class.
        /// </summary>
        /// <param name="errors">Errors.</param>
        public OperationResult(params string[] errors)
            : base(errors)
        {
            Result = default;
        }

        /// <summary>
        /// Result.
        /// </summary>
        public TResult Result { get; }
    }
}
