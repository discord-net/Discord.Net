using System;
using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedFooter
    {
        public string Text { get; internal set; }
        public Uri IconUrl { get; internal set; }
        public Uri ProxyUrl { get; internal set; }

        internal EmbedFooter(string text, Uri iconUrl, Uri proxyUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyUrl = proxyUrl;
        }

        private string DebuggerDisplay => $"{Text} ({IconUrl})";
        public override string ToString() => Text;
    }
}
