using Azure;
using Microsoft.AspNetCore.Identity;

namespace AspNetCore.Identity.Stores.AzureStorageAccount.Extensions
{
    internal static class ResponseExtensions
    {
        public static IdentityResult ToIdentityResult(this Response response)
        {
            if (IsSuccess(response))
                return IdentityResult.Success;
            else
                return IdentityResult.Failed(new IdentityError { Code = response.Status.ToString(), Description = response.ReasonPhrase });
        }
        public static bool IsSuccess(this Response response) => response.Status >= 200 && response.Status <= 299;
    }
}
