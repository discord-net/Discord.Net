using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A author field of an <see cref="Embed"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedAuthor
    {
        /// <summary>
        ///     Gets the name of the author field.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        ///     Gets the URL of the author field.
        /// </summary>
        public string Url { get; internal set; }
        /// <summary>
        ///     Gets the icon URL of the author field.
        /// </summary>
        public string IconUrl { get; internal set; }
        /// <summary>
        ///     Gets the proxified icon URL of the author field.
        /// </summary>
        public string ProxyIconUrl { get; internal set; }

        internal EmbedAuthor(string name, string url, string iconUrl, string proxyIconUrl)
        {
            Name = name;
            Url = url;
            IconUrl = iconUrl;
            ProxyIconUrl = proxyIconUrl;
        }

        private string DebuggerDisplay => $"{Name} ({Url})";
        /// <summary>
        ///     Gets the name of the author field.
        /// </summary>
        /// <returns>
        ///     
        /// </returns>
        public override string ToString() => Name;

        public static bool operator ==(EmbedAuthor? left, EmbedAuthor? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedAuthor? left, EmbedAuthor? right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is not null && GetType() == obj.GetType() && Equals(obj as EmbedAuthor?);

        public bool Equals(EmbedAuthor embedAuthor)
            => GetHashCode() == embedAuthor.GetHashCode();

        public override int GetHashCode()
            => HashCode.Combine(Name, Url, IconUrl);
    }
}
