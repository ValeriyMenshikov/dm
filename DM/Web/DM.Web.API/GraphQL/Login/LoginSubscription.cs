using DM.Web.API.GraphQL.Login.Events;
using HotChocolate;
using HotChocolate.Types;

namespace DM.Web.API.GraphQL.Login;

/// <summary>
/// Login subscription
/// </summary>
[ExtendObjectType(OperationTypeNames.Subscription)]
public class LoginSubscription
{
    /// <summary>
    ///
    /// Подписка на событие логина пользователя, пример (из интерфейса graphql)
    ///
    /// subscription {
    ///     userLogin {
    ///         login,
    ///         timestamp
    ///     }
    /// }
    ///
    /// </summary>
    [Subscribe]
    public LoginEvent UserLogin([EventMessage] LoginEvent loginEvent) => loginEvent;
}
