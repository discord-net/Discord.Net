using Discord.API;

namespace Discord.Tests.Framework.Mocks.Rest
{
    public static class Guilds
    {
        public static Guild DiscordApi => new Guild()
        {
            Id = 81384788765712384,
            Name = "Discord API",
            OwnerId = 53905483156684800,
            MfaLevel = 0,
            VerificationLevel = 0,
            Roles = new Role[] { Roles.Everyone(81384788765712384), Roles.DiscordEmployee, Roles.LibraryDevs },
            AFKTimeout = 3600,
            Region = "us-east",
            DefaultMessageNotifications = (DefaultMessageNotifications)1,
            EmbedChannelId = 81384788765712384,
            EmbedEnabled = true,
            Icon = "2aab26934e72b4ec300c5aa6cf67c7b3"
        };
    }
}
