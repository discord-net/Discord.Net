using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary> A provider field for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedProvider
    {
        /// <summary>
        ///     Gets the name of the provider.
        /// </summary>
        /// <returns>
        ///     A string representing the name of the provider.
        /// </returns>
        public string Name { get; }
        /// <summary>
        ///     Gets the URL of the provider.
        /// </summary>
        /// <returns>
        ///     A string representing the link to the provider.
        /// </returns>
        public string Url { get; }

        internal EmbedProvider(string name, string url)
        {
            Name = name;
            Url = url;
        }

        private string DebuggerDisplay => $"{Name} ({Url})";
        /// <summary>
        ///     Gets the name of the provider.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="Discord.EmbedProvider.Name" />.
        /// </returns>
        public override string ToString() => Name;

        public static bool operator ==(EmbedProvider? left, EmbedProvider? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedProvider? left, EmbedProvider? right)
            => !(left == right);

        /// <summary>
        /// Determines whether the specified object is equal to the current <see cref="EmbedProvider"/>.
        /// </summary>
        /// <remarks>
        /// If the object passes is an <see cref="EmbedProvider"/>, <see cref="Equals(EmbedProvider?)"/> will be called to compare the 2 instances
        /// </remarks>
        /// <param name="obj">The object to compare with the current <see cref="EmbedProvider"/></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => obj is EmbedProvider embedProvider && Equals(embedProvider);

        /// <summary>
        /// Determines whether the specified <see cref="EmbedProvider"/> is equal to the current <see cref="EmbedProvider"/>
        /// </summary>
        /// <param name="embedProvider">The <see cref="EmbedProvider"/> to compare with the current <see cref="EmbedProvider"/></param>
        /// <returns></returns>
        public bool Equals(EmbedProvider? embedProvider)
            => GetHashCode() == embedProvider?.GetHashCode();

        /// <inheritdoc />
        public override int GetHashCode()
            => (Name, Url).GetHashCode();
    }
}
