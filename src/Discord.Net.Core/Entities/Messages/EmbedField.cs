using System;
using System.Diagnostics;

namespace Discord
{
    /// <summary>
    ///     A field for an <see cref="Embed"/>.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public struct EmbedField
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

        private string DebuggerDisplay => $"{Name} ({Value}";
        /// <summary>
        ///     Gets the name of the field.
        /// </summary>
        /// <returns>
        ///     A string that resolves to <see cref="EmbedField.Name"/>.
        /// </returns>
        public override string ToString() => Name;

        public static bool operator ==(EmbedField? left, EmbedField? right)
            => left is null ? right is null
                : left.Equals(right);

        public static bool operator !=(EmbedField? left, EmbedField? right)
            => !(left == right);

        public override bool Equals(object obj)
            => obj is not null && GetType() == obj.GetType() && Equals(obj as EmbedField?);

        public bool Equals(EmbedField embedField)
            => GetHashCode() == embedField.GetHashCode();

        public override int GetHashCode()
            => HashCode.Combine(Name, Value, Inline);
    }
}
