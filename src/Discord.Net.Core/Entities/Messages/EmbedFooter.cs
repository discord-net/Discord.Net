using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedFooter
    {
        public string Text { get; internal set; }
        public string IconUrl { get; internal set; }
        public string ProxyUrl { get; internal set; }

        internal EmbedFooter(string text, string iconUrl, string proxyUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyUrl = proxyUrl;
        }

        private string DebuggerDisplay => $"{Text} ({IconUrl})";
        public override string ToString() => Text;
    }
}
