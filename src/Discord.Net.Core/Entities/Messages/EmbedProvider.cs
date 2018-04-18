using System.Diagnostics;

namespace Discord
{
    /// <summary> A provider field for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedProvider
    {
        /// <summary> Gets the name of the provider.</summary>
        public string Name { get; }
        /// <summary> Gets the URL of the provider.</summary>
        public string Url { get; }

        internal EmbedProvider(string name, string url)
        {
            Name = name;
            Url = url;
        }

        private string DebuggerDisplay => $"{Name} ({Url})";
        public override string ToString() => Name;
    }
}
