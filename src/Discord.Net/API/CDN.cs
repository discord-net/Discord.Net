namespace Discord.API
{
    internal static class CDN
    {
        public static string GetUserAvatarUrl(ulong userId, string avatarId)
            => avatarId != null ? $"{DiscordConfig.ClientAPIUrl}users/{userId}/avatars/{avatarId}.jpg" : null;
        public static string GetGuildIconUrl(ulong guildId, string iconId)
            => iconId != null ? $"{DiscordConfig.ClientAPIUrl}guilds/{guildId}/icons/{iconId}.jpg" : null;
        public static string GetGuildSplashUrl(ulong guildId, string splashId)
            => splashId != null ? $"{DiscordConfig.ClientAPIUrl}guilds/{guildId}/splashes/{splashId}.jpg" : null;
        public static string GetChannelIconUrl(ulong channelId, string iconId)
            => iconId != null ? $"{DiscordConfig.ClientAPIUrl}channel-icons/{channelId}/{iconId}.jpg" : null;
    }
}
