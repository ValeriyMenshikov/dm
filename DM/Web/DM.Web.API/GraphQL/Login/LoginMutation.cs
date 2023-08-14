using System;
using System.Threading.Tasks;
using DM.Web.API.GraphQL.Login.Events;
using DM.Web.API.GraphQL.Login.Response;
using DM.Web.API.Services.Common;
using DM.Web.API.Services.Users;
using DM.Web.Core.Authentication.Credentials;
using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace DM.Web.API.GraphQL.Login;

/// <summary>
/// GraphQL mutation
/// </summary>
[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class LoginMutation
{
    /// <summary>
    /// Авторизация пользователя.
    ///
    /// Альтернатива REST метода POST /v1/account/login
    ///
    /// Пример вызова:
    ///
    /// mutation loginAccount($login: LoginCredentialsInput) {
    ///     loginAccount(login: $login) {
    ///         user {
    ///             resource {
    ///                 login
    ///             }
    ///         },
    ///         token
    ///     }
    /// }
    ///
    /// Переменные запроса:
    ///
    /// {
    ///     "login": {
    ///         "login": "User",
    ///         "password": "...",
    ///         "rememberMe": false
    ///     }
    /// }
    ///
    /// </summary>
    public async Task<AccountLoginResponse> LoginAccount(
        [Service] ILoginApiService loginService,
        [Service] IHttpContextAccessor contextAccessor,
        [Service] ICredentialsStorage credentialsStorage,
        [Service] ITopicEventSender sender,
        LoginCredentials login)
    {
        var user = await loginService.Login(login, contextAccessor.HttpContext);
        var credentials = await credentialsStorage.ExtractTokenFromResponse(contextAccessor.HttpContext);

        await sender.SendAsync(nameof(LoginSubscription.UserLogin), new LoginEvent(
            user.Resource.Login,
            DateTimeOffset.Now));

        return new AccountLoginResponse
        {
            User = user,
            Token = credentials.Token,
        };
    }

    /// <summary>
    /// Закрытие текущей сессии пользователя.
    ///
    /// Альтернатива REST метода DELETE /v1/account/logout
    ///
    /// Пример вызова:
    ///
    /// mutation logoutAccount($accessToken: String) {
    ///     logoutAccount(accessToken: $accessToken)
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    /// "accessToken": "..."
    /// }
    ///
    /// </summary>
    public async Task<MutationResult> LogoutAccount(
        [Service] IAuthenticationService authenticationService,
        [Service] IHttpContextAccessor contextAccessor,
        [Service] ILoginApiService loginService,
        string accessToken)
    {
        await authenticationService.Authenticate(accessToken, contextAccessor.HttpContext);
        await loginService.Logout(contextAccessor.HttpContext);
        return MutationResult.Ok;
    }

    /// <summary>
    /// Закрытие всех сессий пользователя (кроме текущей).
    ///
    /// Альтернатива REST метода DELETE /v1/account/logout/all
    ///
    /// Пример вызова:
    ///
    /// mutation logoutAllAccount($accessToken: String) {
    ///     logoutAllAccount(accessToken: $accessToken)
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    /// "accessToken": "..."
    /// }
    ///
    /// </summary>
    public async Task<MutationResult> LogoutAllAccount(
        [Service] IAuthenticationService authenticationService,
        [Service] IHttpContextAccessor contextAccessor,
        [Service] ILoginApiService loginService,
        string accessToken)
    {
        await authenticationService.Authenticate(accessToken, contextAccessor.HttpContext);
        await loginService.LogoutAll(contextAccessor.HttpContext);
        return MutationResult.Ok;
    }
}
