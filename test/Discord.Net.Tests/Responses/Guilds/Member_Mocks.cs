using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.API;

namespace Discord.Tests.Framework.Responses.Guilds
{
    public static class Member_Mocks
    {
        public static IEnumerable<GuildMember> Guild_Members => new List<GuildMember> { Guild_Member_1, Guild_Member_2 };

        public static GuildMember Guild_Member_1
        {
            get
            {
                return new GuildMember()
                {
                    User = Users.User_Mocks.Me_User,
                    Nick = "voltana",
                    JoinedAt = new DateTime(2009, 4, 19),
                    Deaf = true,
                    Mute = false,
                    Roles = new ulong[] { 1UL }
                };
            }
        }

        public static GuildMember Guild_Member_2
        {
            get
            {
                return new GuildMember()
                {
                    User = Users.User_Mocks.Bot_User,
                    Nick = "foxbot",
                    JoinedAt = new DateTime(2016, 5, 5),
                };
            }
        }
    }
}
