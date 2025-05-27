using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A field for an <see cref="Embed"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public record struct EmbedField
    {
        /// <summary>
        ///     Gets the name of the field.
        /// </summary>
        public string Name { get; internal set; }
        /// <summary>
        ///     Gets the value of the field.
        /// </summary>
        public string Value { get; internal set; }
        /// <summary>
        ///     Gets a value that indicates whether the field should be in-line with each other.
        /// </summary>
        public bool Inline { get; internal set; }

        internal EmbedField(string name, string value, bool inline)
        {
            Name = name;
            Value = value;
            Inline = inline;
        }

        private readonly string DebuggerDisplay => $"{Name} ({Value}";
        /// <summary>
        ///     Gets the name of the field.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="EmbedField.Name"/>.
        /// </returns>
        public override readonly string ToString() => Name;

        /// <inheritdoc />
        public override readonly int GetHashCode()
            => (Name, Value, Inline).GetHashCode();
    }
}
