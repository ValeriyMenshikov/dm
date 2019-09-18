using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using DM.Services.DataAccess;
using DM.Services.DataAccess.BusinessObjects.Fora;
using DM.Services.DataAccess.RelationalStorage;
using DM.Services.Forum.Dto.Output;
using Microsoft.EntityFrameworkCore;

namespace DM.Services.Forum.BusinessProcesses.Commentaries.Creating
{
    /// <inheritdoc />
    public class CommentaryCreatingRepository : ICommentaryCreatingRepository
    {
        private readonly DmDbContext dbContext;
        private readonly IMapper mapper;

        /// <inheritdoc />
        public CommentaryCreatingRepository(
            DmDbContext dbContext,
            IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<Comment> Create(ForumComment comment, IUpdateBuilder<ForumTopic> topicUpdate)
        {
            dbContext.Comments.Add(comment);
            topicUpdate.AttachTo(dbContext);
            await dbContext.SaveChangesAsync();
            return await dbContext.Comments
                .Where(c => c.ForumCommentId == comment.ForumCommentId)
                .ProjectTo<Comment>(mapper.ConfigurationProvider)
                .FirstAsync();
        }
    }
}