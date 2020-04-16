using System.Net;
using Azure;
using DocumentManagement.Core;

namespace DocumentManagment.DocumentStore.Blob
{
    internal static class OperationResultExtensions
    {
        internal static OperationResult ToOperationResult(this Response response, string fileName)
        {
            if (response.Status == (int)HttpStatusCode.OK
                || response.Status == (int)HttpStatusCode.Created
                || response.Status == (int)HttpStatusCode.Accepted)
            {
                return OperationResult.SuccessfulResult();
            }

            if (response.Status == (int)HttpStatusCode.NotFound)
            {
                return new OperationResult($"File name '{fileName}' was not found in store.");
            }

            return new OperationResult("Storage is unavailable, please try again later.");
        }
    }
}
