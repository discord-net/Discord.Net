using System;

namespace Discord
{
    /// <summary>
    ///     Represents a class containing the strings related to various Content Delivery Networks (CDNs).
    /// </summary>
    public static class CDN
    {
        /// <summary>
        ///     Returns a team icon URL.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="size">The size of the image to return in horizontal pixels. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the team's icon.
        /// </returns>
        public static string GetTeamIconUrl(ulong teamId, string iconId, ushort size, ImageFormat format)
        {
            if (iconId == null)
                return null;
            string extension = FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}team-icons/{teamId}/{iconId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns an application icon URL.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the application's icon.
        /// </returns>
        public static string GetApplicationIconUrl(ulong appId, string iconId, ushort size, ImageFormat format)
        {
            if (iconId == null)
                return null;
            string extension = FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}app-icons/{appId}/{iconId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a user avatar URL.
        /// </summary>
        /// <param name="userId">The user snowflake identifier.</param>
        /// <param name="avatarId">The avatar identifier.</param>
        /// <param name="size">The size of the image to return in horizontal pixels. This can be any power of two between 16 and 2048.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the user's avatar in the specified size.
        /// </returns>
        public static string GetUserAvatarUrl(ulong userId, string avatarId, ushort size, ImageFormat format)
        {
            if (avatarId == null)
                return null;
            string extension = FormatToExtension(format, avatarId);
            return $"{DiscordConfig.CDNUrl}avatars/{userId}/{avatarId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns the default user avatar URL.
        /// </summary>
        /// <param name="discriminator">The discriminator value of a user.</param>
        /// <returns>
        ///     A URL pointing to the user's default avatar when one isn't set.
        /// </returns>
        public static string GetDefaultUserAvatarUrl(ushort discriminator)
        {
            return $"{DiscordConfig.CDNUrl}embed/avatars/{discriminator % 5}.png";
        }
        /// <summary>
        ///     Returns an icon URL.
        /// </summary>
        /// <param name="guildId">The guild snowflake identifier.</param>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="size">The size of the image to return in horizontal pixels. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the guild's icon.
        /// </returns>
        public static string GetGuildIconUrl(ulong guildId, string iconId, ushort size, ImageFormat format)
        {
            if (iconId == null)
                return null;
            string extension = FormatToExtension(format, iconId);
            return $"{DiscordConfig.CDNUrl}icons/{guildId}/{iconId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a guild splash URL.
        /// </summary>
        /// <param name="guildId">The guild snowflake identifier.</param>
        /// <param name="splashId">The splash icon identifier.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the guild's splash.
        /// </returns>
        public static string GetGuildSplashUrl(ulong guildId, string splashId, ushort size, ImageFormat format)
        {
            if (splashId == null)
                return null;
            string extension = FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}splashes/{guildId}/{splashId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a guild discovery splash URL.
        /// </summary>
        /// <param name="guildId">The guild snowflake identifier.</param>
        /// <param name="discoverySplashId">The discovery splash icon identifier.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the guild's discovery splash.
        /// </returns>
        public static string GetGuildDiscoverySplashUrl(ulong guildId, string discoverySplashId, ushort size, ImageFormat format)
        {
            if (discoverySplashId == null)
                return null;
            string extension = FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}discovery-splashes/{guildId}/{discoverySplashId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a channel icon URL.
        /// </summary>
        /// <param name="channelId">The channel snowflake identifier.</param>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the channel's icon.
        /// </returns>
        public static string GetChannelIconUrl(ulong channelId, string iconId, ushort size, ImageFormat format)
        {
            if (iconId == null)
                return null;
            string extension = FormatToExtension(format, iconId);
            return $"{DiscordConfig.CDNUrl}channel-icons/{channelId}/{iconId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a guild banner URL.
        /// </summary>
        /// <param name="guildId">The guild snowflake identifier.</param>
        /// <param name="bannerId">The banner image identifier.</param>
        /// <param name="size">The size of the image to return in horizontal pixels. This can be any power of two between 16 and 2048 inclusive.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the guild's banner image.
        /// </returns>
        public static string GetGuildBannerUrl(ulong guildId, string bannerId, ushort? size = null, ImageFormat? format = null)
        {
            if (string.IsNullOrEmpty(bannerId))
                return null;
            string extension = format.HasValue ? FormatToExtension(format.Value, string.Empty) : "png";
            return $"{DiscordConfig.CDNUrl}banners/{guildId}/{bannerId}.{extension}" + (size.HasValue ? $"?size={size}" : string.Empty);
        }
        /// <summary>
        ///     Returns an emoji URL.
        /// </summary>
        /// <param name="emojiId">The emoji snowflake identifier.</param>
        /// <param name="animated">Whether this emoji is animated.</param>
        /// <param name="size">The size of the image to return in horizontal pixels. This can be any power of two between 16 and 128.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the custom emote.
        /// </returns>
        public static string GetEmojiUrl(ulong emojiId, bool animated, ushort size, ImageFormat format)
        {
            string extension = format == ImageFormat.Auto && animated ? "gif" : FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}emojis/{emojiId}.{extension}?size={size}";
        }

        /// <summary>
        ///     Returns a Rich Presence asset URL.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="assetId">The asset identifier.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <param name="format">The format to return.</param>
        /// <returns>
        ///     A URL pointing to the asset image in the specified size.
        /// </returns>
        public static string GetRichAssetUrl(ulong appId, string assetId, ushort size, ImageFormat format)
        {
            string extension = FormatToExtension(format, string.Empty);
            return $"{DiscordConfig.CDNUrl}app-assets/{appId}/{assetId}.{extension}?size={size}";
        }

        /// <summary>
        ///     Returns a Spotify album URL.
        /// </summary>
        /// <param name="albumArtId">The identifier for the album art (e.g. 6be8f4c8614ecf4f1dd3ebba8d8692d8ce4951ac).</param>
        /// <returns>
        ///     A URL pointing to the Spotify album art.
        /// </returns>
        public static string GetSpotifyAlbumArtUrl(string albumArtId)
            => $"https://i.scdn.co/image/{albumArtId}";
        /// <summary>
        ///     Returns a Spotify direct URL for a track.
        /// </summary>
        /// <param name="trackId">The identifier for the track (e.g. 4uLU6hMCjMI75M1A2tKUQC).</param>
        /// <returns>
        ///     A URL pointing to the Spotify track.
        /// </returns>
        public static string GetSpotifyDirectUrl(string trackId)
            => $"https://open.spotify.com/track/{trackId}";

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
                case ImageFormat.Lottie:
                    return "json";
                default:
                    throw new ArgumentException(nameof(format));
            }
        }
    }
}
