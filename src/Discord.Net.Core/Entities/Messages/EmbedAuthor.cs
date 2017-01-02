using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedAuthor
    {
        public string Name { get; internal set; }
        public string Url { get; internal set; }
        public string IconUrl { get; internal set; }
        public string ProxyIconUrl { get; internal set; }

        internal EmbedAuthor(string name, string url, string iconUrl, string proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }

        private string DebuggerDisplay => $"{Name} ({Url})";
        public override string ToString() => Name;
    }
}
