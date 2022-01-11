using System;

namespace Discord.Net.ED25519
{
    // Loops? Arrays? Never heard of that stuff
    // Library avoids unnecessary heap allocations and unsafe code
    // so this ugly code becomes necessary :(
    internal static class ByteIntegerConverter
    {
        public static ulong LoadBigEndian64(byte[] buf, int offset)
        {
            return
                (ulong)(buf[offset + 7])
                | (((ulong)(buf[offset + 6])) << 8)
                | (((ulong)(buf[offset + 5])) << 16)
                | (((ulong)(buf[offset + 4])) << 24)
                | (((ulong)(buf[offset + 3])) << 32)
                | (((ulong)(buf[offset + 2])) << 40)
                | (((ulong)(buf[offset + 1])) << 48)
                | (((ulong)(buf[offset + 0])) << 56);
        }

        public static void StoreBigEndian64(byte[] buf, int offset, ulong value)
        {
            buf[offset + 7] = unchecked((byte)value);
            buf[offset + 6] = unchecked((byte)(value >> 8));
            buf[offset + 5] = unchecked((byte)(value >> 16));
            buf[offset + 4] = unchecked((byte)(value >> 24));
            buf[offset + 3] = unchecked((byte)(value >> 32));
            buf[offset + 2] = unchecked((byte)(value >> 40));
            buf[offset + 1] = unchecked((byte)(value >> 48));
            buf[offset + 0] = unchecked((byte)(value >> 56));
        }

        public static void Array16LoadBigEndian64(out Array16<ulong> output, byte[] input, int inputOffset)
        {
            output.x0 = LoadBigEndian64(input, inputOffset + 0);
            output.x1 = LoadBigEndian64(input, inputOffset + 8);
            output.x2 = LoadBigEndian64(input, inputOffset + 16);
            output.x3 = LoadBigEndian64(input, inputOffset + 24);
            output.x4 = LoadBigEndian64(input, inputOffset + 32);
            output.x5 = LoadBigEndian64(input, inputOffset + 40);
            output.x6 = LoadBigEndian64(input, inputOffset + 48);
            output.x7 = LoadBigEndian64(input, inputOffset + 56);
            output.x8 = LoadBigEndian64(input, inputOffset + 64);
            output.x9 = LoadBigEndian64(input, inputOffset + 72);
            output.x10 = LoadBigEndian64(input, inputOffset + 80);
            output.x11 = LoadBigEndian64(input, inputOffset + 88);
            output.x12 = LoadBigEndian64(input, inputOffset + 96);
            output.x13 = LoadBigEndian64(input, inputOffset + 104);
            output.x14 = LoadBigEndian64(input, inputOffset + 112);
            output.x15 = LoadBigEndian64(input, inputOffset + 120);
        }
    }
}
