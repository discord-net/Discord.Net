using Model = Discord.API.EmbedThumbnail;

namespace Discord
{
    public struct EmbedThumbnail
    {
        public string Url { get; }
        public string ProxyUrl { get; }
        public int? Height { get; }
        public int? Width { get; }

        public EmbedThumbnail(string url, string proxyUrl, int? height, int? width)
        {
            Url = url;
            ProxyUrl = proxyUrl;
            Height = height;
            Width = width;
        }

        internal EmbedThumbnail(Model model)
            : this(
                  model.Url, 
                  model.ProxyUrl,
                  model.Height.IsSpecified ? model.Height.Value : (int?)null,
                  model.Width.IsSpecified ? model.Width.Value : (int?)null)
        {
        }
    }
}
