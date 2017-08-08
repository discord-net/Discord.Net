using System;
using System.Collections.Generic;

namespace Discord.Serialization
{
    internal class Utf8SpanComparer : IEqualityComparer<ReadOnlySpan<byte>>
    {
        public static readonly Utf8SpanComparer Instance = new Utf8SpanComparer();

        public bool Equals(ReadOnlySpan<byte> x, ReadOnlySpan<byte> y) => x.SequenceEqual(y);
        public int GetHashCode(ReadOnlySpan<byte> obj)
        {
            //From Utf8String
            //TODO: Replace when they do
            unchecked
            {
                if (obj.Length <= 4)
                {
                    int hash = obj.Length;
                    for (int i = 0; i < obj.Length; i++)
                    {
                        hash <<= 8;
                        hash ^= obj[i];
                    }
                    return hash;
                }
                else
                {
                    int hash = obj.Length;
                    hash ^= obj[0];
                    hash <<= 8;
                    hash ^= obj[1];
                    hash <<= 8;
                    hash ^= obj[obj.Length - 2];
                    hash <<= 8;
                    hash ^= obj[obj.Length - 1];
                    return hash;
                }
            }
        }
    }
}
