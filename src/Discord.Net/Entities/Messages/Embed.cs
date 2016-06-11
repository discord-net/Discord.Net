using Model = Discord.API.Embed;

namespace Discord
{
    internal class Embed : IEmbed
    {
        public string Description { get; }
        public string Url { get; }
        public string Title { get; }
        public string Type { get; }
        public EmbedProvider Provider { get; }
        public EmbedThumbnail Thumbnail { get; }

        public Embed(Model model)
        {
            Url = model.Url;
            Type = model.Type;
            Title = model.Title;
            Description = model.Description;
            
            if (model.Provider.IsSpecified)
                Provider = new EmbedProvider(model.Provider.Value);
            if (model.Thumbnail.IsSpecified)
                Thumbnail = new EmbedThumbnail(model.Thumbnail.Value);
        }
    }
}
