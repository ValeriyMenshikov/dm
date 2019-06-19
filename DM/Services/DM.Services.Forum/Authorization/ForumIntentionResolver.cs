using System.Linq;
using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using DM.Services.Common.Authorization;
using DM.Services.Core.Dto.Enums;
using DM.Services.Forum.BusinessProcesses.Common;

namespace DM.Services.Forum.Authorization
{
    /// <inheritdoc />
    public class ForumIntentionResolver : IIntentionResolver<ForumIntention, Dto.Output.Forum>
    {
        private readonly IAccessPolicyConverter accessPolicyConverter;

        /// <inheritdoc />
        public ForumIntentionResolver(
            IAccessPolicyConverter accessPolicyConverter)
        {
            this.accessPolicyConverter = accessPolicyConverter;
        }

        /// <inheritdoc />
        public Task<bool> IsAllowed(AuthenticatedUser user, ForumIntention intention, Dto.Output.Forum target)
        {
            switch (intention)
            {
                case ForumIntention.CreateTopic when user.IsAuthenticated:
                    var userPolicy = accessPolicyConverter.Convert(user.Role);
                    return Task.FromResult((target.CreateTopicPolicy & userPolicy) != ForumAccessPolicy.NoOne);
                case ForumIntention.AdministrateTopics when user.IsAuthenticated:
                    return Task.FromResult(user.Role.HasFlag(UserRole.Administrator) ||
                                           target.ModeratorIds.Contains(user.UserId));
                default:
                    return Task.FromResult(false);
            }
        }
    }
}