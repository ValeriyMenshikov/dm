using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DM.Web.API.GraphQL.Login;

/// <summary>
/// Login extensions
/// </summary>
public static class LoginExtensions
{
    /// <summary>
    /// Add login extensions
    /// </summary>
    public static IRequestExecutorBuilder AddLoginExtensions(this IRequestExecutorBuilder builder)
    {
        return builder
            .AddTypeExtension<LoginMutation>()
            .AddTypeExtension<LoginSubscription>();
    }
}
