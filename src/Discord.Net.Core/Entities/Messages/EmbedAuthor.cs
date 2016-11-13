using System.Diagnostics;
using Model = Discord.API.EmbedAuthor;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedAuthor
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string IconUrl { get; set; }
        public string ProxyIconUrl { get; set; }

        private EmbedAuthor(string name, string url, string iconUrl, string proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }
        internal static EmbedAuthor Create(Model model)
        {
            return new EmbedAuthor(model.Name, model.Url, model.IconUrl, model.ProxyIconUrl);
        }

        private string DebuggerDisplay => $"{Name} ({Url})";
        public override string ToString() => Name;
    }
}
