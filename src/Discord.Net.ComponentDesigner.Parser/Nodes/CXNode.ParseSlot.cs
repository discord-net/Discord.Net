using Microsoft.CodeAnalysis.Text;
using System;

namespace Discord.CX.Parser;

partial class CXNode
{
    public readonly struct ParseSlot : IEquatable<ParseSlot>
    {
        public ICXNode Value { get; }
        public TextSpan FullSpan => Value.FullSpan;

        public readonly int Id;

        public ParseSlot(int id, ICXNode node)
        {
            Id = id;
            Value = node;
        }

        public static bool operator ==(ParseSlot slot, ICXNode node)
            => slot.Value == node;

        public static bool operator !=(ParseSlot slot, ICXNode node)
            => slot.Value != node;

        public bool Equals(ParseSlot other)
            => Equals(Value, other.Value);

        public override bool Equals(object? obj)
            => obj is ParseSlot other && Equals(other);

        public override int GetHashCode()
            => Value.GetHashCode();
    }
}
