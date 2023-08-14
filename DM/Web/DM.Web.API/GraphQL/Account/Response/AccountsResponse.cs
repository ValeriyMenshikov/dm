using System.Collections.Generic;
using DM.Services.Core.Dto;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.GraphQL.Account.Response;

/// <summary>
/// Список пользователей
/// </summary>
public sealed class AccountsResponse
{
    /// <summary>
    /// Список пользователей
    /// </summary>
    public IEnumerable<User> Users { get; set; }

    /// <summary>
    /// Данные пагинации
    /// </summary>
    public PagingResult Paging { get; set; }
}
