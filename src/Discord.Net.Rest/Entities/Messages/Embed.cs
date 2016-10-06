using System.Diagnostics;
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
        public EmbedProvider? Provider { get; }
        public EmbedThumbnail? Thumbnail { get; }

        internal Embed(string type, string title, string description, string url, EmbedProvider? provider, EmbedThumbnail? thumbnail)
        {
            Type = type;
            Title = title;
            Description = description;
            Url = url;
            Provider = provider;
            Thumbnail = thumbnail;
        }
        internal static Embed Create(Model model)
        {
            return new Embed(model.Type, model.Title, model.Description, model.Url,
                model.Provider.IsSpecified ? EmbedProvider.Create(model.Provider.Value) : (EmbedProvider?)null,
                model.Thumbnail.IsSpecified ? EmbedThumbnail.Create(model.Thumbnail.Value) : (EmbedThumbnail?)null);
        }

        public override string ToString() => Title;
        private string DebuggerDisplay => $"{Title} ({Type})";
    }
}
