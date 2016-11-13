using System.Diagnostics;
using Model = Discord.API.EmbedFooter;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedFooter
    {
        public string Text { get; set; }
        public string IconUrl { get; set; }
        public string ProxyUrl { get; set; }

        private EmbedFooter(string text, string iconUrl, string proxyUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyUrl = proxyUrl;
        }
        internal static EmbedFooter Create(Model model)
        {
            return new EmbedFooter(model.Text, model.IconUrl, model.ProxyIconUrl);
        }

        private string DebuggerDisplay => $"{Text} ({IconUrl})";
        public override string ToString() => Text;
    }
}
