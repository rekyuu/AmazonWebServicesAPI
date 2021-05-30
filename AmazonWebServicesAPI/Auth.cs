using Microsoft.AspNetCore.Http;
using System;

namespace AmazonWebServicesAPI
{
    public static class Auth
    {
        public static bool IsAuthorized(HttpRequest request)
        {
            if (!request.Headers.ContainsKey("X-ACCESS-TOKEN")) return false;
            if (request.Headers["X-ACCESS-TOKEN"] != Environment.GetEnvironmentVariable("ACCESS_TOKEN")) return false;

            return true;
        }
    }
}
