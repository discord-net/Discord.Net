using Model = Discord.API.EmbedThumbnail;

namespace Discord
{
    public struct EmbedThumbnail
    {
        public string Url { get; }
        public string ProxyUrl { get; }
        public int? Height { get; }
        public int? Width { get; }

        internal EmbedThumbnail(Model model)
        {
            Url = model.Url;
            ProxyUrl = model.ProxyUrl;
            Height = model.Height;
            Width = model.Width;
        }
    }
}
