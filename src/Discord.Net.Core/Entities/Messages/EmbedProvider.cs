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
    }
}
