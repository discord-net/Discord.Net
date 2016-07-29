using Model = Discord.API.Attachment;

namespace Discord
{
    internal class Attachment : IAttachment
    {
        public ulong Id { get; }
        public string Filename { get; }
        public string Url { get; }
        public string ProxyUrl { get; }
        public int Size { get; }
        public int? Height { get; }
        public int? Width { get; }

        public Attachment(Model model)
        {
            Id = model.Id;
            Filename = model.Filename;
            Size = model.Size;
            Url = model.Url;
            ProxyUrl = model.ProxyUrl;
            Height = model.Height.IsSpecified ? model.Height.Value : (int?)null;
            Width = model.Width.IsSpecified ? model.Width.Value : (int?)null;
        }
    }
}
