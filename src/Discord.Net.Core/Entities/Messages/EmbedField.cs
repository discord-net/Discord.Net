using System.Diagnostics;

namespace Discord
{
    /// <summary> A field for an <see cref="Embed"/>. </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedField
    {
        /// <summary> Gets the name of the field.</summary>
        public string Name { get; internal set; }
        /// <summary> Gets the value of the field.</summary>
        public string Value { get; internal set; }
        /// <summary> Gets whether the field is inline inside an <see cref="Embed"/> or not.</summary>
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
