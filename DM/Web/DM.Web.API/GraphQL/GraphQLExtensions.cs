using DM.Web.API.GraphQL.Account;
using DM.Web.API.GraphQL.Login;
using Microsoft.Extensions.DependencyInjection;

namespace DM.Web.API.GraphQL;

/// <summary>
/// GraphQL extensions
/// </summary>
public static class GraphQLExtensions
{
    /// <summary>
    /// Add GraphQL
    /// </summary>
    public static IServiceCollection AddGraphQLService(this IServiceCollection services)
    {
        services.AddGraphQLServer()
            .AddInMemorySubscriptions()
            .AddQueryType()
            .AddMutationType()
            .AddSubscriptionType()
            .AddAccountExtensions()
            .AddLoginExtensions()
            .AddErrorFilter<ExceptionFilter>()
            .AddFiltering()
            .AddSorting()
            .AddProjections();

        return services;
    }
}
