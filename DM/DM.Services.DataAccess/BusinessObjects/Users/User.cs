using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DM.Services.DataAccess.BusinessObjects.Administration;
using DM.Services.DataAccess.BusinessObjects.Common;
using DM.Services.DataAccess.BusinessObjects.DataContracts;
using DM.Services.DataAccess.BusinessObjects.Fora;
using DM.Services.DataAccess.BusinessObjects.Games;
using DM.Services.DataAccess.BusinessObjects.Games.Characters;
using DM.Services.DataAccess.BusinessObjects.Games.Links;
using DM.Services.DataAccess.BusinessObjects.Games.Posts;

namespace DM.Services.DataAccess.BusinessObjects.Users
{
    [Table("Users")]
    public class User : IPublicUser, IRemovable
    {
        [Key]
        public Guid UserId { get; set; }
        public Guid ProfileId { get; set; }

        public string Login { get; set; }
        public string Email { get; set; }

        public DateTime RegistrationDate { get; set; }
        public DateTime? LastVisitDate { get; set; }
        public string TimezoneId { get; set; }

        public UserRole Role { get; set; }
        public AccessPolicy AccessPolicy { get; set; }

        public string Salt { get; set; }
        public string PasswordHash { get; set; }

        public bool RatingDisabled { get; set; }
        public int QualityRating { get; set; }
        public int QuantityRating { get; set; }

        public bool Activated { get; set; }
        public bool CanMerge { get; set; }
        public Guid? MergeRequested { get; set; }

        public bool IsRemoved { get; set; }

        #region Profile navigations

        [ForeignKey(nameof(ProfileId))]
        public UserProfile Profile { get; set; }

        [InverseProperty(nameof(Upload.UserPicture))]
        public ICollection<Upload> ProfilePictures { get; set; }

        [InverseProperty(nameof(Token.User))]
        public ICollection<Token> Tokens { get; set; }

        #endregion

        #region Common navigations

        [InverseProperty(nameof(Comment.Author))]
        public ICollection<Comment> Comments { get; set; }

        [InverseProperty(nameof(Like.User))]
        public ICollection<Like> Likes { get; set; }

        [InverseProperty(nameof(Review.Author))]
        public ICollection<Review> Reviews { get; set; }

        [InverseProperty(nameof(Upload.User))]
        public ICollection<Upload> Uploads { get; set; }

        #endregion

        #region Forum navigations

        [InverseProperty(nameof(ForumTopic.Author))]
        public ICollection<ForumTopic> Topics { get; set; }

        [InverseProperty(nameof(ForumModerator.User))]
        public ICollection<ForumModerator> ForumModerators { get; set; }

        #endregion

        #region Game navigations

        [InverseProperty(nameof(Game.Master))]
        public ICollection<Game> GamesAsMaster { get; set; }

        [InverseProperty(nameof(Game.Assistant))]
        public ICollection<Game> GamesAsAssistant { get; set; }

        [InverseProperty(nameof(Game.Nanny))]
        public ICollection<Game> GamesAsNanny { get; set; }

        [InverseProperty(nameof(BlackListLink.User))]
        public ICollection<BlackListLink> GamesBlacklisted { get; set; }

        [InverseProperty(nameof(Reader.User))]
        public ICollection<Reader> GamesObserved { get; set; }

        [InverseProperty(nameof(Character.User))]
        public ICollection<Character> Characters { get; set; }

        [InverseProperty(nameof(Post.Author))]
        public ICollection<Post> Posts { get; set; }

        [InverseProperty(nameof(Vote.VotedUser))]
        public ICollection<Vote> VotesFrom { get; set; }

        [InverseProperty(nameof(Vote.TargetUser))]
        public ICollection<Vote> VotesFor { get; set; }

        #endregion

        #region Administration navigations

        [InverseProperty(nameof(UserNanny.Nanny))]
        public ICollection<UserNanny> Children { get; set; }

        [InverseProperty(nameof(Report.Author))]
        public ICollection<Report> ReportsGiven { get; set; }

        [InverseProperty(nameof(Report.Target))]
        public ICollection<Report> ReportsTaken { get; set; }

        [InverseProperty(nameof(Report.AnswerAuthor))]
        public ICollection<Report> ReportsAnswered { get; set; }

        [InverseProperty(nameof(Warning.User))]
        public ICollection<Warning> WarningsReceived { get; set; }

        [InverseProperty(nameof(Warning.Moderator))]
        public ICollection<Warning> WarningsGiven { get; set; }

        [InverseProperty(nameof(Ban.User))]
        public ICollection<Ban> BansReceived { get; set; }

        [InverseProperty(nameof(Ban.Moderator))]
        public ICollection<Ban> BansGiven { get; set; }

        #endregion
    }
}