using System;
using System.IO;

namespace Discord
{
    internal static class StreamExtensions
    {
#if MSTRYBUFFER
        public static byte[] GetBuffer(this MemoryStream stream)
        {
            if (stream.TryGetBuffer(out var streamBuffer))
                return streamBuffer.Array;
            else
                return stream.ToArray();
        }
#elif !MSBUFFER
        public static byte[] GetBuffer(this MemoryStream stream) => stream.ToArray();
#endif

        public static ReadOnlyBuffer<byte> ToReadOnlyBuffer(this MemoryStream stream)
            => new ReadOnlyBuffer<byte>(stream.GetBuffer(), 0, (int)stream.Length);
        public static ReadOnlySpan<byte> ToSpan(this MemoryStream stream)
            => new ReadOnlySpan<byte>(stream.GetBuffer(), 0, (int)stream.Length);
    }
}
