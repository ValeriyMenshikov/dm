using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.GraphQL.Login.Response;

/// <summary>
/// Результат авторизации пользователя
/// </summary>
public sealed class AccountLoginResponse
{
    /// <summary>
    /// Токен доступа
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Данные пользователя
    /// </summary>
    public Envelope<User> User { get; set; }
}
