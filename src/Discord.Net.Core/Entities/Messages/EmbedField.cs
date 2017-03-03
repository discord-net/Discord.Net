using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedField
    {
        public string Name { get; internal set; }
        public string Value { get; internal set; }
        public bool Inline { get; internal set; }

        internal EmbedField(string name, string value, bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        private string DebuggerDisplay => $"{Name} ({Value}";
        public override string ToString() => Name;
    }
}
