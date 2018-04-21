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
        ///     Gets the title URL of the embed.
        /// </summary>
        string Url { get; }
        /// <summary>
        ///     Gets the title of the embed.
        /// </summary>
        string Title { get; }
        /// <summary>
        ///     Gets the description of the embed.
        /// </summary>
        string Description { get; }
        /// <summary>
        ///     Gets the type of the embed.
        /// </summary>
        EmbedType Type { get; }
        /// <summary>
        ///     Gets the timestamp of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        DateTimeOffset? Timestamp { get; }
        /// <summary>
        ///     Gets the sidebar color of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        Color? Color { get; }
        /// <summary>
        ///     Gets the image of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedImage? Image { get; }
        /// <summary>
        ///     Gets the video of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedVideo? Video { get; }
        /// <summary>
        ///     Gets the author field of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedAuthor? Author { get; }
        /// <summary>
        ///     Gets the footer field of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedFooter? Footer { get; }
        /// <summary>
        ///     Gets the provider of the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedProvider? Provider { get; }
        /// <summary>
        ///     Gets the thumbnail featured in the embed, or <see langword="null" /> if none is set.
        /// </summary>
        EmbedThumbnail? Thumbnail { get; }
        /// <summary>
        ///     Gets the fields of the embed.
        /// </summary>
        ImmutableArray<EmbedField> Fields { get; }
    }
}
