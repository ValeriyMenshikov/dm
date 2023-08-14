using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DM.Web.API.GraphQL.Account;

/// <summary>
/// Account extensions
/// </summary>
public static class AccountExtensions
{
    /// <summary>
    /// Add account extensions
    /// </summary>
    public static IRequestExecutorBuilder AddAccountExtensions(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<AccountQuery>()
            .AddTypeExtension<AccountMutation>();
    }
}
