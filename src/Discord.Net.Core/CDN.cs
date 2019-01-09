using System;

namespace Discord
{
    /// <summary>
    ///     Represents a class containing the strings related to various Content Delivery Networks (CDNs).
    /// </summary>
    public static class CDN
    {
        /// <summary>
        ///     Returns an application icon URL.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="iconId">The icon identifier.</param>
        /// <param name="format">The format to return. Mustn't be a gif.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the application's icon.
        /// </returns>
        public static string GetApplicationIconUrl(ulong appId, string iconId, ImageFormat format, ushort size)
        {
            if (string.IsNullOrWhiteSpace(iconId))
                return null;
            if (format == ImageFormat.Gif)
                throw new ArgumentException("Requested image format mustn't be a gif.");
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

            string extension = FormatToExtension(format, iconId);
            return $"{DiscordConfig.CDNUrl}app-icons/{appId}/{iconId}.{extension}?size={size}";
        }

        /// <summary>
        ///     Returns a user avatar URL.
        /// </summary>
        /// <param name="userId">The user snowflake identifier.</param>
        /// <param name="avatarId">The avatar identifier.</param>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the user's avatar in the specified size.
        /// </returns>
        public static string GetUserAvatarUrl(ulong userId, string avatarId, ImageFormat format, ushort size)
        {
            if (string.IsNullOrWhiteSpace(avatarId))
                return null;
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

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
        /// <param name="format">The format to return. Mustn't be a gif.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the guild's icon in the specified size.
        /// </returns>
        public static string GetGuildIconUrl(ulong guildId, string iconId, ImageFormat format, ushort size)
        {
            if (string.IsNullOrWhiteSpace(iconId))
                return null;
            if (format == ImageFormat.Gif)
                throw new ArgumentException("Requested image format mustn't be a gif.");
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

            string extension = FormatToExtension(format, iconId);
            return $"{DiscordConfig.CDNUrl}icons/{guildId}/{iconId}.{extension}?size={size}";
        }
        /// <summary>
        ///     Returns a guild splash URL.
        /// </summary>
        /// <param name="guildId">The guild snowflake identifier.</param>
        /// <param name="splashId">The splash icon identifier.</param>
        /// <param name="format">The format to return. Mustn't be a gif.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the guild's icon.
        /// </returns>
        public static string GetGuildSplashUrl(ulong guildId, string splashId, ImageFormat format, ushort size)
        {
            if (string.IsNullOrWhiteSpace(splashId))
                return null;
            if (format == ImageFormat.Gif)
                throw new ArgumentException("Requested image format mustn't be a gif.");
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

            string extension = FormatToExtension(format, splashId);
            return $"{DiscordConfig.CDNUrl}splashes/{guildId}/{splashId}.{ extension}?size={size}";
        }
        /// <summary>
        ///     Returns an emoji URL.
        /// </summary>
        /// <param name="emojiId">The emoji snowflake identifier.</param>
        /// <param name="animated">Whether this emoji is animated.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the custom emote.
        /// </returns>
        public static string GetEmojiUrl(ulong emojiId, bool animated, ushort size)
        {
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

            return $"{DiscordConfig.CDNUrl}emojis/{emojiId}.{(animated ? "gif" : "png")}?size={size}";
        }

        /// <summary>
        ///     Returns a Rich Presence asset URL.
        /// </summary>
        /// <param name="appId">The application identifier.</param>
        /// <param name="assetId">The asset identifier.</param>
        /// <param name="format">The format to return.</param>
        /// <param name="size">The size of the image to return in. This can be any power of two between 16 and 2048.</param>
        /// <returns>
        ///     A URL pointing to the asset image in the specified size.
        /// </returns>
        public static string GetRichAssetUrl(ulong appId, string assetId, ImageFormat format, ushort size)
        {
            if (string.IsNullOrWhiteSpace(assetId))
                return null;
            if (!(size >= 16 && size <= 2048))
                throw new ArgumentOutOfRangeException("Size must be a power of two in a range between 16 and 2048.");
            if ((size & (size - 1)) != 0)
                throw new ArgumentException("Size must be a power of two.");

            string extension = FormatToExtension(format, "");
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
                default:
                    throw new ArgumentException(nameof(format));
            }
        }
    }
}
