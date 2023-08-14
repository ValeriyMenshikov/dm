using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.Users.Reading;
using DM.Services.Core.Dto;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;
using DM.Web.API.GraphQL.Account.Response;
using DM.Web.API.Services.Common;
using DM.Web.API.Services.Users;
using HotChocolate;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using UserDetails = DM.Web.API.Dto.Users.UserDetails;

namespace DM.Web.API.GraphQL.Account;

/// <summary>
/// GraphQL query
/// </summary>
[ExtendObjectType(OperationTypeNames.Query)]
public sealed class AccountQuery
{
    /// <summary>
    /// Получение текущего пользователя.
    ///
    /// Альтернатива REST метода GET /v1/account
    ///
    /// Пример вызова:
    ///
    /// query {
    ///     accountCurrent(accessToken: "...") {
    ///         resource {
    ///             login
    ///         }
    ///     }
    /// }
    ///
    /// </summary>
    public async Task<Envelope<UserDetails>> AccountCurrent(
        [Service] IAuthenticationService authenticationService,
        [Service] ILoginApiService loginService,
        [Service] IHttpContextAccessor contextAccessor,
        string accessToken)
    {
        await authenticationService.Authenticate(accessToken, contextAccessor.HttpContext);
        return await loginService.GetCurrent();
    }

    /// <summary>
    /// Метод получения списка всех активированных пользователей с пагинацией.
    ///
    /// Пример использования:
    ///
    /// query($paging: PagingQueryInput) {
    ///     accounts(withInactive: true, paging: $paging) {
    ///         users {
    ///             login
    ///         },
    ///         paging {
    ///             currentPage,
    ///             totalPagesCount,
    ///             totalEntitiesCount
    ///         }
    ///     }
    /// }
    ///
    /// Данные запроса:
    ///
    /// {
    ///     "paging": {
    ///         "size": 2,
    ///         "skip": 1
    ///     }
    /// }
    ///
    /// </summary>
    public async Task<AccountsResponse> Accounts(
        [Service] IUserReadingService userService,
        [Service] IMapper mapper,
        PagingQuery paging,
        bool withInactive)
    {
        var result = await userService.Get(paging ?? PagingQuery.Default, withInactive);

        return new AccountsResponse
        {
            Users =  mapper.Map<IEnumerable<User>>(result.users),
            Paging = result.paging
        };
    }
}
