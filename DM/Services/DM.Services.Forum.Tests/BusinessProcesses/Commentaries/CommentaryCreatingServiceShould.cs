using System;
using System.Threading;
using System.Threading.Tasks;
using DM.Services.Authentication.Dto;
using DM.Services.Authentication.Implementation.UserIdentity;
using DM.Services.Common.Authorization;
using DM.Services.Common.BusinessProcesses.UnreadCounters;
using DM.Services.Core.Dto.Enums;
using DM.Services.DataAccess.BusinessObjects.Common;
using DM.Services.DataAccess.BusinessObjects.Fora;
using DM.Services.DataAccess.RelationalStorage;
using DM.Services.Forum.Authorization;
using DM.Services.Forum.BusinessProcesses.Commentaries.Creating;
using DM.Services.Forum.BusinessProcesses.Topics.Reading;
using DM.Services.Forum.Dto.Input;
using DM.Services.Forum.Dto.Output;
using DM.Services.MessageQueuing.Publish;
using DM.Tests.Core;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using Xunit;

namespace DM.Services.Forum.Tests.BusinessProcesses.Commentaries
{
    public class CommentaryCreatingServiceShould : UnitTestBase
    {
        private readonly ISetup<ITopicReadingService, Task<Topic>> topicReadingSetup;
        private readonly ISetup<IIdentity, AuthenticatedUser> currentUserSetup;
        private readonly ISetup<ICommentaryFactory, ForumComment> commentaryDalCreateSetup;
        private readonly ISetup<ICommentaryCreatingRepository, Task<Comment>> commentaryCreateSetup;
        private readonly Mock<ICommentaryCreatingRepository> commentRepository;
        private readonly Mock<IUnreadCountersRepository> countersRepository;
        private readonly CommentaryCreatingService service;
        private readonly Mock<ITopicReadingService> topicReadingService;
        private readonly Mock<IInvokedEventPublisher> invokedEventPublisher;
        private readonly Mock<IIntentionManager> intentionManager;
        private readonly Mock<IUpdateBuilder<ForumTopic>> updateBuilder;

        public CommentaryCreatingServiceShould()
        {
            var validator = Mock<IValidator<CreateComment>>();
            validator
                .Setup(v => v.ValidateAsync(It.IsAny<ValidationContext<CreateComment>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            topicReadingService = Mock<ITopicReadingService>();
            topicReadingSetup = topicReadingService.Setup(s => s.GetTopic(It.IsAny<Guid>()));

            intentionManager = Mock<IIntentionManager>();
            intentionManager
                .Setup(m => m.ThrowIfForbidden(It.IsAny<TopicIntention>(), It.IsAny<Topic>()))
                .Returns(Task.CompletedTask);

            var identityProvider = Mock<IIdentityProvider>();
            var identity = Mock<IIdentity>();
            identityProvider.Setup(p => p.Current).Returns(identity.Object);
            currentUserSetup = identity.Setup(i => i.User);

            var commentFactory = Mock<ICommentaryFactory>();
            commentaryDalCreateSetup = commentFactory
                .Setup(f => f.Create(It.IsAny<CreateComment>(), It.IsAny<Guid>()));

            commentRepository = Mock<ICommentaryCreatingRepository>();
            commentaryCreateSetup = commentRepository.Setup(r =>
                r.Create(It.IsAny<ForumComment>(), It.IsAny<IUpdateBuilder<ForumTopic>>()));

            invokedEventPublisher = Mock<IInvokedEventPublisher>();
            invokedEventPublisher
                .Setup(p => p.Publish(It.IsAny<EventType>(), It.IsAny<Guid>()))
                .Returns(Task.CompletedTask);

            countersRepository = Mock<IUnreadCountersRepository>();
            countersRepository
                .Setup(r => r.Increment(It.IsAny<Guid>(), It.IsAny<UnreadEntryType>()))
                .Returns(Task.CompletedTask);

            var updateBuilderFactory = Mock<IUpdateBuilderFactory>();
            updateBuilder = MockUpdateBuilder<ForumTopic>();
            updateBuilderFactory
                .Setup(f => f.Create<ForumTopic>(It.IsAny<Guid>()))
                .Returns(updateBuilder.Object);

            service = new CommentaryCreatingService(validator.Object, topicReadingService.Object,
                intentionManager.Object, identityProvider.Object, commentFactory.Object,
                updateBuilderFactory.Object, commentRepository.Object, countersRepository.Object,
                invokedEventPublisher.Object);
        }

        [Fact]
        public async Task AuthorizeCreateCommentaryAction()
        {
            currentUserSetup.Returns(new AuthenticatedUser());
            var topic = new Topic();
            topicReadingSetup.ReturnsAsync(topic);
            commentaryDalCreateSetup.Returns(new ForumComment());
            commentaryCreateSetup.ReturnsAsync(new Comment());

            await service.Create(new CreateComment());

            intentionManager.Verify(m => m.ThrowIfForbidden(TopicIntention.CreateComment, topic));
        }

        [Fact]
        public async Task SaveNewCommentAndUpdateDenormalizedColumn()
        {
            currentUserSetup.Returns(new AuthenticatedUser());
            var topic = new Topic();
            topicReadingSetup.ReturnsAsync(topic);
            var commentId = Guid.NewGuid();
            var forumComment = new ForumComment {ForumCommentId = commentId};
            commentaryDalCreateSetup.Returns(forumComment);
            var expected = new Comment();
            commentaryCreateSetup.ReturnsAsync(expected);

            var actual = await service.Create(new CreateComment());

            actual.Should().Be(expected);
            commentRepository.Verify(r => r.Create(forumComment, updateBuilder.Object), Times.Once);
            updateBuilder.Verify(b => b.Field(t => t.LastCommentId, commentId));
        }

        [Fact]
        public async Task IncrementUnreadCounter()
        {
            currentUserSetup.Returns(new AuthenticatedUser());
            var topicId = Guid.NewGuid();
            var topic = new Topic {Id = topicId};
            topicReadingSetup.ReturnsAsync(topic);
            commentaryDalCreateSetup.Returns(new ForumComment());
            commentaryCreateSetup.ReturnsAsync(new Comment());

            await service.Create(new CreateComment());

            countersRepository.Verify(r => r.Increment(topicId, UnreadEntryType.Message), Times.Once);
            countersRepository.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task PublishEvent()
        {
            currentUserSetup.Returns(new AuthenticatedUser());
            var topicId = Guid.NewGuid();
            var topic = new Topic {Id = topicId};
            topicReadingSetup.ReturnsAsync(topic);
            var commentId = Guid.NewGuid();
            commentaryDalCreateSetup.Returns(new ForumComment {ForumCommentId = commentId});
            commentaryCreateSetup.ReturnsAsync(new Comment());

            await service.Create(new CreateComment());

            invokedEventPublisher.Verify(p => p.Publish(EventType.NewForumComment, commentId));
            invokedEventPublisher.VerifyNoOtherCalls();
        }
    }
}