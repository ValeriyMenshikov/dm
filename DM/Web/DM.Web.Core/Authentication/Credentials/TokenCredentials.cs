namespace DM.Web.Core.Authentication.Credentials;

/// <summary>
/// Token credentials
/// </summary>
public class TokenCredentials : AuthCredentials
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Create new token credentials with token
    /// </summary>
    /// <param name="token">Access token</param>
    /// <returns>Token credentials</returns>
    public static TokenCredentials FromToken(string token) => new() {Token = token};
}
