using System;

namespace Discord
{
    public static class CDN
    {
        public static string GetApplicationIconUrl(ulong appId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}app-icons/{appId}/{iconId}.jpg" : null;
        public static string GetUserAvatarUrl(ulong userId, string avatarId, ushort size, ImageFormat format)
        {
            if (avatarId == null)
                return null;
            string extension = FormatToExtension(format, avatarId);
            return $"{DiscordConfig.CDNUrl}avatars/{userId}/{avatarId}.{extension}?size={size}";
        }
        public static string GetGuildIconUrl(ulong guildId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}icons/{guildId}/{iconId}.jpg" : null;
        public static string GetGuildSplashUrl(ulong guildId, string splashId)
            => splashId != null ? $"{DiscordConfig.CDNUrl}splashes/{guildId}/{splashId}.jpg" : null;
        public static string GetChannelIconUrl(ulong channelId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}channel-icons/{channelId}/{iconId}.jpg" : null;
        public static string GetEmojiUrl(ulong emojiId)
            => $"{DiscordConfig.CDNUrl}emojis/{emojiId}.png";

        private static string FormatToExtension(ImageFormat format, string imageId)
        {
            if (format == ImageFormat.Auto)
                format = imageId.StartsWith("a_") ? ImageFormat.Gif : ImageFormat.Png;
            switch (format)
            {
                case ImageFormat.Gif: return "gif";
                case ImageFormat.Jpeg: return "jpeg";
                case ImageFormat.Png: return "png";
                case ImageFormat.WebP: return "webp";
                default: throw new ArgumentException(nameof(format));
            }
        }
    }
}
