using System;
using System.Collections.Immutable;

namespace Discord
{
    /// <summary>
    ///     Represents a Discord embed object.
    /// </summary>
    public interface IEmbed
    {
        /// <summary>
        ///     Gets the title URL of this embed.
        /// </summary>
        /// <returns>
        ///     A string containing the URL set in a title of the embed.
        /// </returns>
        string Url { get; }
        /// <summary>
        ///     Gets the title of this embed.
        /// </summary>
        /// <returns>
        ///     The title of the embed.
        /// </returns>
        string Title { get; }
        /// <summary>
        ///     Gets the description of this embed.
        /// </summary>
        /// <returns>
        ///     The description field of the embed.
        /// </returns>
        string Description { get; }
        /// <summary>
        ///     Gets the type of this embed.
        /// </summary>
        /// <returns>
        ///     The type of the embed.
        /// </returns>
        EmbedType Type { get; }
        /// <summary>
        ///     Gets the timestamp of this embed.
        /// </summary>
        /// <returns>
        ///     A <see cref="DateTimeOffset"/> based on the timestamp present at the bottom left of the embed, or 
        ///     <c>null</c> if none is set.
        /// </returns>
        DateTimeOffset? Timestamp { get; }
        /// <summary>
        ///     Gets the color of this embed.
        /// </summary>
        /// <returns>
        ///     The color of the embed present on the side of the embed, or <c>null</c> if none is set.
        /// </returns>
        Color? Color { get; }
        /// <summary>
        ///     Gets the image of this embed.
        /// </summary>
        /// <returns>
        ///     The image of the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedImage? Image { get; }
        /// <summary>
        ///     Gets the video of this embed.
        /// </summary>
        /// <returns>
        ///     The video of the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedVideo? Video { get; }
        /// <summary>
        ///     Gets the author field of this embed.
        /// </summary>
        /// <returns>
        ///     The author field of the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedAuthor? Author { get; }
        /// <summary>
        ///     Gets the footer field of this embed.
        /// </summary>
        /// <returns>
        ///     The author field of the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedFooter? Footer { get; }
        /// <summary>
        ///     Gets the provider of this embed.
        /// </summary>
        /// <returns>
        ///     The source of the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedProvider? Provider { get; }
        /// <summary>
        ///     Gets the thumbnail featured in this embed.
        /// </summary>
        /// <returns>
        ///     The thumbnail featured in the embed, or <c>null</c> if none is set.
        /// </returns>
        EmbedThumbnail? Thumbnail { get; }
        /// <summary>
        ///     Gets the fields of the embed.
        /// </summary>
        /// <returns>
        ///     An array of the fields of the embed.
        /// </returns>
        ImmutableArray<EmbedField> Fields { get; }
    }
}
