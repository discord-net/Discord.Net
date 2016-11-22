using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Model = Discord.API.Embed;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Embed : IEmbed
    {
        public string Description { get; }
        public string Url { get; }
        public string Title { get; }
        public string Type { get; }
        public Color? Color { get; }
        public EmbedImage? Image { get; }
        public EmbedVideo? Video { get; }
        public EmbedAuthor? Author { get; }
        public EmbedFooter? Footer { get; }
        public EmbedProvider? Provider { get; }
        public EmbedThumbnail? Thumbnail { get; }
        public ImmutableArray<EmbedField> Fields { get; }

        internal Embed(string type, 
            string title, 
            string description, 
            string url, 
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
            Image = image;
            Video = video;
            Author = author;
            Footer = footer;
            Provider = provider;
            Thumbnail = thumbnail;
            Fields = fields;
        }
        internal static Embed Create(Model model)
        {
            return new Embed(model.Type, model.Title, model.Description, model.Url,
                model.Color.HasValue ? new Color(model.Color.Value) : (Color?)null,
                model.Image.IsSpecified ? EmbedImage.Create(model.Image.Value) : (EmbedImage?)null,
                model.Video.IsSpecified ? EmbedVideo.Create(model.Video.Value) : (EmbedVideo?)null,
                model.Author.IsSpecified ? EmbedAuthor.Create(model.Author.Value) : (EmbedAuthor?)null,
                model.Footer.IsSpecified ? EmbedFooter.Create(model.Footer.Value) : (EmbedFooter?)null,
                model.Provider.IsSpecified ? EmbedProvider.Create(model.Provider.Value) : (EmbedProvider?)null,
                model.Thumbnail.IsSpecified ? EmbedThumbnail.Create(model.Thumbnail.Value) : (EmbedThumbnail?)null,
                model.Fields.IsSpecified ? model.Fields.Value.Select(x => EmbedField.Create(x)).ToImmutableArray() : ImmutableArray.Create<EmbedField>());
        }

        public override string ToString() => Title;
        private string DebuggerDisplay => $"{Title} ({Type})";
    }
}
