using Model = Discord.API.Attachment;

namespace Discord
{
    //TODO: Rename to Attachment?
    public class RestAttachment : IAttachment
    {
        public ulong Id { get; }
        public string Filename { get; }
        public string Url { get; }
        public string ProxyUrl { get; }
        public int Size { get; }
        public int? Height { get; }
        public int? Width { get; }

        internal RestAttachment(ulong id, string filename, string url, string proxyUrl, int size, int? height, int? width)
        {
            Id = id;
            Filename = filename;
            Url = url;
            ProxyUrl = proxyUrl;
            Size = size;
            Height = height;
            Width = width;
        }
        internal static RestAttachment Create(Model model)
        {
            return new RestAttachment(model.Id, model.Filename, model.Url, model.ProxyUrl, model.Size,
                model.Height.IsSpecified ? model.Height.Value : (int?)null,
                model.Width.IsSpecified ? model.Width.Value : (int?)null);
        }
    }
}
