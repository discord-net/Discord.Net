using System;

namespace Discord
{
    /// <summary> Contains the strings related to various Content Delievery Networks (CDNs). </summary>
    public static class CDN
    {
        /// <summary> Returns the Discord developer application icon. </summary>
        public static string GetApplicationIconUrl(ulong appId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}app-icons/{appId}/{iconId}.jpg" : null;
        /// <summary> Returns the user avatar URL based on the size and <see cref="ImageFormat"/>. </summary>
        public static string GetUserAvatarUrl(ulong userId, string avatarId, ushort size, ImageFormat format)
        {
            if (avatarId == null)
                return null;
            string extension = FormatToExtension(format, avatarId);
            return $"{DiscordConfig.CDNUrl}avatars/{userId}/{avatarId}.{extension}?size={size}";
        }
        /// <summary> Returns the default user avatar URL. </summary>
        public static string GetDefaultUserAvatarUrl(ushort discriminator)
        {
            return $"{DiscordConfig.CDNUrl}embed/avatars/{discriminator % 5}.png";
        }
        public static string GetGuildIconUrl(ulong guildId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}icons/{guildId}/{iconId}.jpg" : null;
        /// <summary> Returns the guild splash URL based on the guild and icon ID. </summary>
        public static string GetGuildSplashUrl(ulong guildId, string splashId)
            => splashId != null ? $"{DiscordConfig.CDNUrl}splashes/{guildId}/{splashId}.jpg" : null;
        /// <summary> Returns the channel icon URL based on the guild and icon ID. </summary>
        public static string GetChannelIconUrl(ulong channelId, string iconId)
            => iconId != null ? $"{DiscordConfig.CDNUrl}channel-icons/{channelId}/{iconId}.jpg" : null;
        /// <summary> Returns the emoji URL based on the emoji ID. </summary>
        public static string GetEmojiUrl(ulong emojiId, bool animated)
            => $"{DiscordConfig.CDNUrl}emojis/{emojiId}.{(animated ? "gif" : "png")}";

        /// <summary> Returns the rich presence asset URL based on the asset ID and <see cref="ImageFormat"/>. </summary>
        public static string GetRichAssetUrl(ulong appId, string assetId, ushort size, ImageFormat format)
        {
            string extension = FormatToExtension(format, "");
            return $"{DiscordConfig.CDNUrl}app-assets/{appId}/{assetId}.{extension}?size={size}";
        }

        /// <summary> Returns the Spotify album URL based on the album art ID. </summary>
        public static string GetSpotifyAlbumArtUrl(string albumArtId)
            => $"https://i.scdn.co/image/{albumArtId}";

        private static string FormatToExtension(ImageFormat format, string imageId)
        {
            if (format == ImageFormat.Auto)
                format = imageId.StartsWith("a_") ? ImageFormat.Gif : ImageFormat.Png;
            switch (format)
            {
                case ImageFormat.Gif:
                    return "gif";
                case ImageFormat.Jpeg:
                    return "jpeg";
                case ImageFormat.Png:
                    return "png";
                case ImageFormat.WebP:
                    return "webp";
                default:
                    throw new ArgumentException(nameof(format));
            }
        }
    }
}
