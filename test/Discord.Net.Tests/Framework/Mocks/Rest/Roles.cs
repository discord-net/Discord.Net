using Discord.API;

namespace Discord.Tests.Framework.Mocks.Rest
{
    public static class Roles
    {
        // TODO: These mocks need to include 'mentionable' when the library implements it.

        public static Role Everyone(ulong guildId) => new Role()
        {
            Id = guildId,
            Name = "@everyone",
            Color = 0,
            Hoist = false,
            Permissions = 104324097,
            Position = 0,
            Managed = false
        };

        public static Role LibraryDevs => new Role()
        {
            Id = 81793792671232000,
            Name = "Library Devs",
            Color = 42607,
            Hoist = true,
            Permissions = 268435456,
            Position = 17,
            Managed = false
        };

        public static Role DiscordEmployee => new Role()
        {
            Id = 103548914652696576,
            Name = "Discord Employee",
            Color = 10181046,
            Hoist = false,
            Permissions = 29368358,
            Position = 20,
            Managed = false
        };

    }
}
