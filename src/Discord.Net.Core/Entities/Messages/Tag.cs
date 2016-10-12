using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{DebuggerDisplay,nq}")]
    public class Tag<T> : ITag
    {
        public TagType Type { get; }
        public int Index { get; }
        public int Length { get; }
        public ulong Key { get; }
        public T Value { get; }

        internal Tag(TagType type, int index, int length, ulong key, T value)
        {
            Type = type;
            Index = index;
            Length = length;
            Key = key;
            Value = value;
        }

        private string DebuggerDisplay => $"{Value?.ToString() ?? "null"} ({Type})";
        public override string ToString() => $"{Value?.ToString() ?? "null"} ({Type})";

        object ITag.Value => Value;
    }
}
