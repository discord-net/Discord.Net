using Model = Discord.API.Embed;

namespace Discord
{
    public class RestEmbed : IEmbed
    {
        public string Description { get; }
        public string Url { get; }
        public string Title { get; }
        public string Type { get; }
        public EmbedProvider? Provider { get; }
        public EmbedThumbnail? Thumbnail { get; }

        internal RestEmbed(string type, string title, string description, string url, EmbedProvider? provider, EmbedThumbnail? thumbnail)
        {
            Type = type;
            Title = title;
            Description = description;
            Url = url;
            Provider = provider;
            Thumbnail = thumbnail;
        }
        internal static RestEmbed Create(Model model)
        {
            return new RestEmbed(model.Type, model.Title, model.Description, model.Url,
                model.Provider.IsSpecified ? EmbedProvider.Create(model.Provider.Value) : (EmbedProvider?)null,
                model.Thumbnail.IsSpecified ? EmbedThumbnail.Create(model.Thumbnail.Value) : (EmbedThumbnail?)null);
        }
    }
}
