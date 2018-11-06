using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord
{
    /// <summary>
    ///     Represents an embed object seen in an <see cref="IUserMessage"/>.
    /// </summary>
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Embed : IEmbed
    {
        /// <inheritdoc/>
        public EmbedType Type { get; }

        /// <inheritdoc/>
        public string Description { get; internal set; }
        /// <inheritdoc/>
        public string Url { get; internal set; }
        /// <inheritdoc/>
        public string Title { get; internal set; }
        /// <inheritdoc/>
        public DateTimeOffset? Timestamp { get; internal set; }
        /// <inheritdoc/>
        public Color? Color { get; internal set; }
        /// <inheritdoc/>
        public EmbedImage? Image { get; internal set; }
        /// <inheritdoc/>
        public EmbedVideo? Video { get; internal set; }
        /// <inheritdoc/>
        public EmbedAuthor? Author { get; internal set; }
        /// <inheritdoc/>
        public EmbedFooter? Footer { get; internal set; }
        /// <inheritdoc/>
        public EmbedProvider? Provider { get; internal set; }
        /// <inheritdoc/>
        public EmbedThumbnail? Thumbnail { get; internal set; }
        /// <inheritdoc/>
        public ImmutableArray<EmbedField> Fields { get; internal set; }

        internal Embed(EmbedType type)
        {
            Type = type;
            Fields = ImmutableArray.Create<EmbedField>();
        }
        internal Embed(EmbedType type, 
            string title,
            string description,
            string url,
            DateTimeOffset? timestamp,
            Color? color,
            EmbedImage? image,
            EmbedVideo? video,
            EmbedAuthor? author, 
            EmbedFooter? footer, 
            EmbedProvider? provider, 
            EmbedThumbnail? thumbnail, 
            ImmutableArray<EmbedField> fields)
        {
            Type = type;
            Title = title;
            Description = description;
            Url = url;
            Color = color;
            Timestamp = timestamp;
            Image = image;
            Video = video;
            Author = author;
            Footer = footer;
            Provider = provider;
            Thumbnail = thumbnail;
            Fields = fields;
        }

        /// <summary>
        ///     Gets the total length of all embed properties.
        /// </summary>
        public int Length
        {
            get
            {
                int titleLength = Title?.Length ?? 0;
                int authorLength = Author?.Name?.Length ?? 0;
                int descriptionLength = Description?.Length ?? 0;
                int footerLength = Footer?.Text?.Length ?? 0;
                int fieldSum = Fields.Sum(f => f.Name?.Length + f.Value?.ToString().Length) ?? 0;
                return titleLength + authorLength + descriptionLength + footerLength + fieldSum;
            }
        }

        /// <summary>
        ///     Gets the title of the embed.
        /// </summary>
        public override string ToString() => Title;
        private string DebuggerDisplay => $"{Title} ({Type})";
    }
}
