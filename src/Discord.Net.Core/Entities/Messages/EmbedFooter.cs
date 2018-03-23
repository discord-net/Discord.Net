using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary> A footer field for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedFooter
    {
        /// <summary> Gets the text of the footer.</summary>
        public string Text { get; internal set; }
        /// <summary> Gets the icon URL of the footer.</summary>
        public string IconUrl { get; internal set; }
        /// <summary> Gets the proxified icon URL of the footer.</summary>
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
