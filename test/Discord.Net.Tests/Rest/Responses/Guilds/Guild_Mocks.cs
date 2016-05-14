using Discord.API;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Tests.Rest.Responses.Guilds
{
    public static class Guild_Mocks
    {
        public static Guild Guild_From_Id { get
            {
                return new Guild
                {
                    Id = 66078535390867456,
                    Name = "Discord API",
                    Icon = "c8829ee5a88cbe065519befa9093f63a",
                    Splash = null,
                    OwnerId = 66078337084162048,
                    Region = "us-east",
                    AFKChannelId = 66081976485941248,
                    AFKTimeout = 3600,
                    EmbedEnabled = true,
                    EmbedChannelId = null,
                    VerificationLevel = 0,
                    Roles = new Role[] {Roles.Constant_Role_Mocks.Mock_Everyone_Role},
                };
            } }

        public static IEnumerable<User> GuildBansList => new List<User> { Users.User_Mocks.Bot_User, Users.User_Mocks.Me_User };

        public static IEnumerable<UserGuild> UserGuildsList() => new List<UserGuild>{ User_Guild_1, User_Guild_2 };

        public static UserGuild User_Guild_1 { get
            {
                return new UserGuild
                {
                    Id = 41771983423143937,
                    Name = "Discord Developers",
                    Icon = "ae140f33228348df347067059474bb11",
                    Owner = false,
                    Permissions = 103926785
                };
            } }

        public static UserGuild User_Guild_2
        {
            get
            {
                return new UserGuild
                {
                    Id = 81384788765712384,
                    Name = "Discord API",
                    Icon = "2aab26934e72b4ec300c5aa6cf67c7b3",
                    Owner = false,
                    Permissions = 103926785
                };
            }
        }

    }
}
