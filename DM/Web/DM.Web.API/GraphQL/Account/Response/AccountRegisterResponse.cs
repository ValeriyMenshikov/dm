using System;

namespace DM.Web.API.GraphQL.Account.Response;

/// <summary>
/// Register response
/// </summary>
public sealed class AccountRegisterResponse
{
    /// <summary>
    /// User id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// User login
    /// </summary>
    public string Login { get; set; }
}
