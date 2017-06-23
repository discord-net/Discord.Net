using System;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedAuthor
    {
        public string Name { get; internal set; }
        public Uri Url { get; internal set; }
        public Uri IconUrl { get; internal set; }
        public Uri ProxyIconUrl { get; internal set; }

        internal EmbedAuthor(string name, Uri url, Uri iconUrl, Uri proxyIconUrl)
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
