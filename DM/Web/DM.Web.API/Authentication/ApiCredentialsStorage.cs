using System.Linq;
using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using DM.Web.Core.Authentication.Credentials;
using Microsoft.AspNetCore.Http;

namespace DM.Web.API.Authentication;

/// <inheritdoc />
internal class ApiCredentialsStorage : ICredentialsStorage
{
    /// <summary>
    /// Authentication HTTP header key
    /// </summary>
    public const string HttpAuthTokenHeader = "X-Dm-Auth-Token";

    /// <inheritdoc />
    public Task<TokenCredentials> ExtractTokenFromRequest(HttpContext httpContext)
    {
        return ExtractTokenFromHeaders(httpContext.Request.Headers);
    }

    /// <inheritdoc />
    public Task<TokenCredentials> ExtractTokenFromResponse(HttpContext httpContext)
    {
        return ExtractTokenFromHeaders(httpContext.Response.Headers);
    }

    private static Task<TokenCredentials> ExtractTokenFromHeaders(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(HttpAuthTokenHeader, out var headerValues))
        {
            return Task.FromResult<TokenCredentials>(null);
        }
        var authToken = headerValues.FirstOrDefault();
        return string.IsNullOrEmpty(authToken)
            ? Task.FromResult<TokenCredentials>(null)
            : Task.FromResult(new TokenCredentials {Token = authToken});
    }

    /// <inheritdoc />
    public Task Load(HttpContext httpContext, IIdentity identity)
    {
        httpContext.Response.Headers[HttpAuthTokenHeader] = identity.AuthenticationToken;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task Unload(HttpContext httpContext)
    {
        httpContext.Response.Headers.Remove(HttpAuthTokenHeader);
        return Task.CompletedTask;
    }
}
