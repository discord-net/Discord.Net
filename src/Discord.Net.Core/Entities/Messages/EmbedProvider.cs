using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedProvider
    {
        public string Name { get; }
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
