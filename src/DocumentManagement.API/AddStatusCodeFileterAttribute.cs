using System.Net;
using DocumentManagement.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DocumentManagement.API
{
    /// <summary>
    /// Add response status code filter.
    /// </summary>
    public class AddStatusCodeFileterAttribute : ResultFilterAttribute
    {
        /// <inheritdoc/>
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult
                && objectResult.Value is OperationResult operationResult
                && !operationResult.Successful)
            {
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }

            base.OnResultExecuting(context);
        }
    }
}
