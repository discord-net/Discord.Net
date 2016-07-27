namespace Discord.API
{
    internal static class CDN
    {
        public static string GetApplicationIconUrl(ulong appId, string iconId)
            => iconId != null ? $"{DiscordRestConfig.CDNUrl}app-icons/{appId}/{iconId}.jpg" : null;
        public static string GetUserAvatarUrl(ulong userId, string avatarId)
            => avatarId != null ? $"{DiscordRestConfig.CDNUrl}avatars/{userId}/{avatarId}.jpg" : null;
        public static string GetGuildIconUrl(ulong guildId, string iconId)
            => iconId != null ? $"{DiscordRestConfig.CDNUrl}icons/{guildId}/{iconId}.jpg" : null;
        public static string GetGuildSplashUrl(ulong guildId, string splashId)
            => splashId != null ? $"{DiscordRestConfig.CDNUrl}splashes/{guildId}/{splashId}.jpg" : null;
        public static string GetChannelIconUrl(ulong channelId, string iconId)
            => iconId != null ? $"{DiscordRestConfig.CDNUrl}channel-icons/{channelId}/{iconId}.jpg" : null;
    }
}
