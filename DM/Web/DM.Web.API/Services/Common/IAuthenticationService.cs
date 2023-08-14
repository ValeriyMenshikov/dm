using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using Microsoft.AspNetCore.Http;

namespace DM.Web.API.Services.Common;

/// <summary>
/// Authentication service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate
    /// </summary>
    Task<IIdentity> Authenticate(string accessToken, HttpContext context);
}
