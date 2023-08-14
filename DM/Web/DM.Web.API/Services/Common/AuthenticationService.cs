using System;
using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using DM.Web.Core.Authentication;
using DM.Web.Core.Authentication.Credentials;
using Microsoft.AspNetCore.Http;

namespace DM.Web.API.Services.Common;

/// <summary>
/// Authentication service
/// </summary>
public sealed class AuthenticationService : IAuthenticationService
{
    private readonly IWebAuthenticationService authenticationService;

    /// <summary>
    /// Authentication service
    /// </summary>
    public AuthenticationService(IWebAuthenticationService authenticationService)
    {
        this.authenticationService = authenticationService;
    }

    /// <summary>
    /// Authenticate
    /// </summary>
    public async Task<IIdentity> Authenticate(string accessToken, HttpContext context)
    {
        var credentials = TokenCredentials.FromToken(accessToken);
        var identity = await authenticationService.Authenticate(credentials, context);
        if (identity.Error is not AuthenticationError.NoError)
        {
            throw new UnauthorizedAccessException($"Not authorized: {identity.Error}");
        }

        return identity;
    }
}
