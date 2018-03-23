using System;
using System.Collections.Immutable;

namespace Discord
{
    /// <summary> Represents a Discord embed object. </summary>
    public interface IEmbed
    {
        /// <summary> Gets the title URL of the embed. </summary>
        string Url { get; }
        /// <summary> Gets the title of the embed. </summary>
        string Title { get; }
        /// <summary> Gets the description of the embed. </summary>
        string Description { get; }
        /// <summary> Gets the type of the embed. </summary>
        EmbedType Type { get; }
        /// <summary> Gets the timestamp of the embed. </summary>
        DateTimeOffset? Timestamp { get; }
        /// <summary> Gets the sidebar color of the embed. </summary>
        Color? Color { get; }
        /// <summary> Gets the image of the embed. </summary>
        EmbedImage? Image { get; }
        /// <summary> Gets the video of the embed. </summary>
        EmbedVideo? Video { get; }
        /// <summary> Gets the author field of the embed. </summary>
        EmbedAuthor? Author { get; }
        /// <summary> Gets the footer field of the embed. </summary>
        EmbedFooter? Footer { get; }
        /// <summary> Gets the provider of the embed. </summary>
        EmbedProvider? Provider { get; }
        /// <summary> Gets the thumbnail featured in the embed. </summary>
        EmbedThumbnail? Thumbnail { get; }
        /// <summary> Gets the fields of the embed. </summary>
        ImmutableArray<EmbedField> Fields { get; }
    }
}
