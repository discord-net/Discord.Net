using System;
using System.Collections.Immutable;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Embed : IEmbed
    {
        public string Type { get; }

        public string Description { get; internal set; }
        public string Url { get; internal set; }
        public string Title { get; internal set; }
        public DateTimeOffset? Timestamp { get; internal set; }
        public Color? Color { get; internal set; }
        public EmbedImage? Image { get; internal set; }
        public EmbedVideo? Video { get; internal set; }
        public EmbedAuthor? Author { get; internal set; }
        public EmbedFooter? Footer { get; internal set; }
        public EmbedProvider? Provider { get; internal set; }
        public EmbedThumbnail? Thumbnail { get; internal set; }
        public ImmutableArray<EmbedField> Fields { get; internal set; }

        internal Embed(string type)
        {
            Type = type;
            Fields = ImmutableArray.Create<EmbedField>();
        }
        internal Embed(string type, 
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

        public override string ToString() => Title;
        private string DebuggerDisplay => $"{Title} ({Type})";
    }
}
