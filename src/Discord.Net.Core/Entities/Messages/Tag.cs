using System.Diagnostics;

namespace Discord
{
    [DebuggerDisplay(@"{" + nameof(DebuggerDisplay) + @",nq}")]
    public class Tag<T> : ITag
    {
        internal Tag(TagType type, int index, int length, ulong key, T value)
        {
            Type = type;
            Index = index;
            Length = length;
            Key = key;
            Value = value;
        }

        public T Value { get; }

        private string DebuggerDisplay => $"{Value?.ToString() ?? "null"} ({Type})";
        public TagType Type { get; }
        public int Index { get; }
        public int Length { get; }
        public ulong Key { get; }

        object ITag.Value => Value;
        public override string ToString() => $"{Value?.ToString() ?? "null"} ({Type})";
    }
}
