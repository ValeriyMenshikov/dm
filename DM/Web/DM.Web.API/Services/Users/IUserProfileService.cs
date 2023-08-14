using System.Threading.Tasks;
using DM.Services.Community.BusinessProcesses.Users.Updating;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.Services.Users;

/// <summary>
/// User profile
/// </summary>
public interface IUserProfileUpdateService
{
    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="login">User login</param>
    /// <param name="userData">User data</param>
    /// <returns>User</returns>
    Task<Envelope<UserDetails>> UpdateUserProfileAsync(string login, UpdateUser userData);
}
