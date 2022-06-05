using System;
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

        public static bool operator ==(EmbedFooter? left, EmbedFooter? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedFooter? left, EmbedFooter? right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is not null && GetType() == obj.GetType() && Equals(obj as EmbedFooter?);

        public bool Equals(EmbedFooter embedFooter)
            => GetHashCode() == embedFooter.GetHashCode();

        public override int GetHashCode()
            => (Text, IconUrl, ProxyUrl).GetHashCode();
    }
}
