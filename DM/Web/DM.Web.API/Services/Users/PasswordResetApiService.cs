using System.Threading.Tasks;
using AutoMapper;
using DM.Services.Community.BusinessProcesses.PasswordReset;
using DM.Web.API.Dto.Contracts;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.Services.Users
{
    /// <inheritdoc />
    public class PasswordResetApiService : IPasswordResetApiService
    {
        private readonly IPasswordResetService passwordResetService;
        private readonly IMapper mapper;

        /// <inheritdoc />
        public PasswordResetApiService(
            IPasswordResetService passwordResetService,
            IMapper mapper)
        {
            this.passwordResetService = passwordResetService;
            this.mapper = mapper;
        }
        
        /// <inheritdoc />
        public async Task<Envelope<User>> Reset(ResetPassword resetPassword)
        {
            var user = await passwordResetService.Reset(resetPassword.Login, resetPassword.Email);
            return new Envelope<User>(mapper.Map<User>(user));
        }
    }
}