using System.Threading.Tasks;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.Users.Updating;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.Services.Users;

/// <summary>
/// User profile service
/// </summary>
public sealed class UserProfileUpdateService : IUserProfileUpdateService
{
    private readonly IMapper mapper;

    private readonly IUserUpdatingService updatingService;

    /// <summary>
    /// Constructor
    /// </summary>
    public UserProfileUpdateService(IMapper mapper, IUserUpdatingService updatingService)
    {
        this.mapper = mapper;
        this.updatingService = updatingService;
    }

    /// <summary>
    /// Update user
    /// </summary>
    /// <param name="login">User login</param>
    /// <param name="userData">User data</param>
    /// <returns>User</returns>
    public async Task<Envelope<UserDetails>> UpdateUserProfileAsync(string login, UpdateUser userData)
    {
        userData.Login = login; // Cannot update user login
        var updatedUser = await updatingService.Update(userData);
        return new Envelope<UserDetails>(mapper.Map<UserDetails>(updatedUser));
    }
}
