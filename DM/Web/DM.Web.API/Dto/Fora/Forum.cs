using System.Collections.Generic;
using DM.Web.API.Dto.Users;

namespace DM.Web.API.Dto.Fora
{
    public class Forum
    {
        public string Id { get; set; }
        public int UnreadTopicsCount { get; set; }
        public IEnumerable<User> Moderators { get; set; }
    }
}