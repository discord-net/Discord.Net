using Model = Discord.API.Embed;

namespace Discord
{
    public struct Embed
    {
        public string Url { get; }
        public string Type { get; }
        public string Title { get; }
        public string Description { get; }
        public EmbedProvider Provider { get; }
        public EmbedThumbnail Thumbnail { get; }

        internal Embed(Model model)
        {
            Url = model.Url;
            Type = model.Type;
            Title = model.Title;
            Description = model.Description;
            
            Provider = new EmbedProvider(model.Provider);
            Thumbnail = new EmbedThumbnail(model.Thumbnail);
        }
    }
}
