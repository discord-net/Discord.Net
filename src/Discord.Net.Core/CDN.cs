namespace Discord
{
    public static class CDN
    {
        public static string GetApplicationIconUrl(ulong appId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}app-icons/{appId}/{iconId}.jpg" : null;
        public static string GetUserAvatarUrl(ulong userId, string avatarId, ushort size, AvatarFormat format)
        {
            if (avatarId == null)
                return null;
            var baseUrl = $"{DiscordConfig.CDNUrl}avatars/{userId}/{avatarId}";
            if (format == AvatarFormat.Auto)
                return baseUrl + (avatarId.StartsWith("a_") ? "gif" : "png") + $"?size={size}";
            else
                return baseUrl + format.ToString().ToLower() + $"?size={size}";
        }
        public static string GetGuildIconUrl(ulong guildId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}icons/{guildId}/{iconId}.jpg" : null;
        public static string GetGuildSplashUrl(ulong guildId, string splashId)
            => splashId != null ? $"{DiscordConfig.CDNUrl}splashes/{guildId}/{splashId}.jpg" : null;
        public static string GetChannelIconUrl(ulong channelId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}channel-icons/{channelId}/{iconId}.jpg" : null;
        public static string GetEmojiUrl(ulong emojiId)
            => $"{DiscordConfig.CDNUrl}emojis/{emojiId}.png";
    }
}
