using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Embed : IEmbed
    {
        public EmbedType Type { get; }

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

        public int Length => Title?.Length + Author?.Name?.Length + Description?.Length + Footer?.Text?.Length + Fields.Sum(f => f.Name.Length + f.Value.ToString().Length) ?? 0;

        public override string ToString() => Title;
        private string DebuggerDisplay => $"{Title} ({Type})";
    }
}
