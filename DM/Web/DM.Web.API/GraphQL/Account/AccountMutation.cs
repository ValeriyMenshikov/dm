using System;
using System.Threading.Tasks;
using DM.Services.Community.BusinessProcesses.Users.Updating;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;
using DM.Web.API.GraphQL.Account.Response;
using DM.Web.API.Services.Common;
using DM.Web.API.Services.Users;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;

namespace DM.Web.API.GraphQL.Account;

/// <summary>
/// GraphQL mutation
/// </summary>
[ExtendObjectType(OperationTypeNames.Mutation)]
public sealed class AccountMutation
{
    /// <summary>
    /// Регистрация пользователя.
    ///
    /// Альтернатива REST метода POST /v1/account.
    ///
    /// Пример вызова:
    ///
    /// mutation registerAccount($registration: RegistrationInput) {
    ///     registerAccount(registration: $registration) {
    ///         id,
    ///         login
    ///     }
    /// }
    ///
    /// Переменные запроса:
    ///
    /// {
    ///     "registration": {
    ///         "email": "User1@mail.ru",
    ///         "login": "User1",
    ///         "password": "..."
    ///     }
    /// }
    /// </summary>
    public async Task<AccountRegisterResponse> RegisterAccount(
        [Service] IRegistrationApiService registrationService,
        Registration registration)
    {
        var userId = await registrationService.Register(registration);

        return new AccountRegisterResponse
        {
            Id = userId,
            Login = registration.Login,
        };
    }

    /// <summary>
    /// Активация пользователя.
    ///
    /// Альтернатива REST метода PUT /v1/account/{token}
    ///
    /// Пример вызова:
    ///
    /// mutation activateAccount($activationToken: UUID!) {
    ///     activateAccount(activationToken: $activationToken) {
    ///         resource {
    ///             login,
    ///             name,
    ///             roles
    ///         }
    ///     }
    /// }
    ///
    /// Переменные запроса:
    ///
    /// {
    ///     "activationToken": "..."
    /// }
    ///
    /// </summary>
    public Task<Envelope<User>> ActivateAccount(
        [Service] IActivationApiService activationService,
        Guid activationToken)
    {
        return activationService.Activate(activationToken);
    }

    /// <summary>
    /// Сменить почту пользователя.
    ///
    /// Аналог REST метода PUT /v1/account/email
    ///
    /// Пример вызова:
    ///
    /// mutation changeAccountEmail($changeEmail: ChangeEmailInput) {
    ///     changeAccountEmail(changeEmail: $changeEmail) {
    ///         resource {
    ///             login
    ///         }
    ///     }
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    ///     "changeEmail": {
    ///         "login": "User",
    ///         "password": "...",
    ///         "email": "NewEmailUser@mail.ru"
    ///     }
    /// }
    ///
    /// </summary>
    public Task<Envelope<User>> ChangeAccountEmail(
        [Service] IEmailChangeApiService emailChangeService,
        ChangeEmail changeEmail)
    {
        return emailChangeService.Change(changeEmail);
    }

    /// <summary>
    /// Инициировать смену пароля пользователя.
    ///
    /// Альтернатива REST метода POST /v1/account/password
    ///
    /// Пример вызова:
    ///
    /// mutation resetAccountPassword($resetPassword: ResetPasswordInput) {
    ///     resetAccountPassword(resetPassword: $resetPassword) {
    ///         resource {
    ///             login
    ///         }
    ///     }
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    ///     "resetPassword": {
    ///         "login": "User",
    ///         "email": "email@mail.ru"
    ///     }
    /// }
    ///
    /// </summary>
    public Task<Envelope<User>> ResetAccountPassword(
        [Service] IPasswordResetApiService passwordService,
        ResetPassword resetPassword)
    {
        return passwordService.Reset(resetPassword);
    }

    /// <summary>
    /// Завершить смену пароля пользователя.
    ///
    /// Альтернатива REST метода PUT /v1/account/password
    ///
    /// Пример вызова:
    ///
    /// mutation changeAccountPassword($changePassword: ChangePasswordInput) {
    ///     changeAccountPassword(changePassword: $changePassword) {
    ///         resource {
    ///             login
    ///         }
    ///     }
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    ///     "changePassword": {
    ///         "login": "User",
    ///         "token": "Токен, отправленный на почту при вызове ResetAccountPassword",
    ///         "oldPassword": "...",
    ///         "newPassword": "..."
    ///     }
    /// }
    ///
    /// </summary>
    public Task<Envelope<User>> ChangeAccountPassword(
        [Service] IPasswordResetApiService passwordService,
        ChangePassword changePassword)
    {
        return passwordService.Change(changePassword);
    }

    /// <summary>
    /// Обновить данные профиля текущего пользователя.
    ///
    /// Пример вызова:
    ///
    /// mutation updateAccount($accessToken: String, $userData: UpdateUserInput) {
    ///     updateAccount(accessToken: $accessToken, userData: $userData) {
    ///         resource {
    ///             login,
    ///             name,
    ///             icq,
    ///             skype
    ///         }
    ///     }
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    ///     "accessToken": "...",
    ///     "userData": {
    ///         "name": "My user",
    ///         "icq": "1234567",
    ///         "skype": "my skype"
    ///     }
    /// }
    ///
    /// </summary>
    public async Task<Envelope<UserDetails>> UpdateAccount(
        [Service] IAuthenticationService authenticationService,
        [Service] IUserProfileUpdateService userProfileUpdateService,
        [Service] IHttpContextAccessor contextAccessor,
        string accessToken,
        UpdateUser userData)
    {
        var currentUser = await authenticationService.Authenticate(accessToken, contextAccessor.HttpContext);
        var user = await userProfileUpdateService.UpdateUserProfileAsync(currentUser.User.Login, userData);
        return user;
    }
}
