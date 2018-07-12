using System.Diagnostics;

namespace Discord
{
    /// <summary> A footer field for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedFooter
    {
        /// <summary>
        ///     Gets the text of the footer field.
        /// </summary>
        /// <returns>
        ///     A string containing the text of the footer field.
        /// </returns>
        public string Text { get; }
        /// <summary>
        ///     Gets the URL of the footer icon.
        /// </summary>
        /// <returns>
        ///     A string containing the URL of the footer icon.
        /// </returns>
        public string IconUrl { get; }
        /// <summary>
        ///     Gets the proxied URL of the footer icon link.
        /// </summary>
        /// <returns>
        ///     A string containing the proxied URL of the footer icon.
        /// </returns>
        public string ProxyUrl { get; }

        internal EmbedFooter(string text, string iconUrl, string proxyUrl)
        {
            Text = text;
            IconUrl = iconUrl;
            ProxyUrl = proxyUrl;
        }

        private string DebuggerDisplay => $"{Text} ({IconUrl})";
        /// <summary>
        ///     Gets the text of the footer field.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Discord.EmbedFooter.Text"/>.
        /// </returns>
        public override string ToString() => Text;
    }
}
