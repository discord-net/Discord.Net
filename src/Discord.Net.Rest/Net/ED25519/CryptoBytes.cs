using System;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Discord.Net.ED25519
{
    internal class CryptoBytes
    {
        /// <summary>
        /// Comparison of two arrays.
        /// 
        /// The runtime of this method does not depend on the contents of the arrays. Using constant time
        /// prevents timing attacks that allow an attacker to learn if the arrays have a common prefix.
        /// 
        /// It is important to use such a constant time comparison when verifying MACs.
        /// </summary>
        /// <param name="x">Byte array</param>
        /// <param name="y">Byte array</param>
        /// <returns>True if arrays are equal</returns>
        public static bool ConstantTimeEquals(byte[] x, byte[] y)
        {
            if (x.Length != y.Length)
                return false;
            return InternalConstantTimeEquals(x, 0, y, 0, x.Length) != 0;
        }

        /// <summary>
        /// Comparison of two array segments.
        /// 
        /// The runtime of this method does not depend on the contents of the arrays. Using constant time
        /// prevents timing attacks that allow an attacker to learn if the arrays have a common prefix.
        /// 
        /// It is important to use such a constant time comparison when verifying MACs.
        /// </summary>
        /// <param name="x">Byte array segment</param>
        /// <param name="y">Byte array segment</param>
        /// <returns>True if contents of x and y are equal</returns>
        public static bool ConstantTimeEquals(ArraySegment<byte> x, ArraySegment<byte> y)
        {
            if (x.Count != y.Count)
                return false;
            return InternalConstantTimeEquals(x.Array, x.Offset, y.Array, y.Offset, x.Count) != 0;
        }

        /// <summary>
        /// Comparison of two byte sequences.
        /// 
        /// The runtime of this method does not depend on the contents of the arrays. Using constant time
        /// prevents timing attacks that allow an attacker to learn if the arrays have a common prefix.
        /// 
        /// It is important to use such a constant time comparison when verifying MACs.
        /// </summary>
        /// <param name="x">Byte array</param>
        /// <param name="xOffset">Offset of byte sequence in the x array</param>
        /// <param name="y">Byte array</param>
        /// <param name="yOffset">Offset of byte sequence in the y array</param>
        /// <param name="length">Lengh of byte sequence</param>
        /// <returns>True if sequences are equal</returns>
        public static bool ConstantTimeEquals(byte[] x, int xOffset, byte[] y, int yOffset, int length)
        {
            return InternalConstantTimeEquals(x, xOffset, y, yOffset, length) != 0;
        }

        private static uint InternalConstantTimeEquals(byte[] x, int xOffset, byte[] y, int yOffset, int length)
        {
            int differentbits = 0;
            for (int i = 0; i < length; i++)
                differentbits |= x[xOffset + i] ^ y[yOffset + i];
            return (1 & (unchecked((uint)differentbits - 1) >> 8));
        }

        /// <summary>
        /// Overwrites the contents of the array, wiping the previous content. 
        /// </summary>
        /// <param name="data">Byte array</param>
        public static void Wipe(byte[] data)
        {
            InternalWipe(data, 0, data.Length);
        }

        /// <summary>
        /// Overwrites the contents of the array, wiping the previous content. 
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <param name="offset">Index of byte sequence</param>
        /// <param name="length">Length of byte sequence</param>
        public static void Wipe(byte[] data, int offset, int length)
        {
            InternalWipe(data, offset, length);
        }

        /// <summary>
        /// Overwrites the contents of the array segment, wiping the previous content. 
        /// </summary>
        /// <param name="data">Byte array segment</param>
        public static void Wipe(ArraySegment<byte> data)
        {
            InternalWipe(data.Array, data.Offset, data.Count);
        }

        // Secure wiping is hard
        // * the GC can move around and copy memory
        //   Perhaps this can be avoided by using unmanaged memory or by fixing the position of the array in memory
        // * Swap files and error dumps can contain secret information
        //   It seems possible to lock memory in RAM, no idea about error dumps
        // * Compiler could optimize out the wiping if it knows that data won't be read back
        //   I hope this is enough, suppressing inlining
        //   but perhaps `RtlSecureZeroMemory` is needed
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe(byte[] data, int offset, int count)
        {
            Array.Clear(data, offset, count);
        }

        // shallow wipe of structs
        [MethodImpl(MethodImplOptions.NoInlining)]
        internal static void InternalWipe<T>(ref T data)
            where T : struct
        {
            data = default(T);
        }

        /// <summary>
        /// Constant-time conversion of the bytes array to an upper-case hex string.
        /// Please see http://stackoverflow.com/a/14333437/445517 for the detailed explanation
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <returns>Hex representation of byte array</returns>
        public static string ToHexStringUpper(byte[] data)
        {
            if (data == null)
                return null;
            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(55 + b + (((b - 10) >> 31) & -7));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(55 + b + (((b - 10) >> 31) & -7));
            }
            return new string(c);
        }

        /// <summary>
        /// Constant-time conversion of the bytes array to an lower-case hex string.
        /// Please see http://stackoverflow.com/a/14333437/445517 for the detailed explanation.
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <returns>Hex representation of byte array</returns>
        public static string ToHexStringLower(byte[] data)
        {
            if (data == null)
                return null;
            char[] c = new char[data.Length * 2];
            int b;
            for (int i = 0; i < data.Length; i++)
            {
                b = data[i] >> 4;
                c[i * 2] = (char)(87 + b + (((b - 10) >> 31) & -39));
                b = data[i] & 0xF;
                c[i * 2 + 1] = (char)(87 + b + (((b - 10) >> 31) & -39));
            }
            return new string(c);
        }

        /// <summary>
        /// Converts the hex string to bytes. Case insensitive.
        /// </summary>
        /// <param name="hexString">Hex encoded byte sequence</param>
        /// <returns>Byte array</returns>
        public static byte[] FromHexString(string hexString)
        {
            if (hexString == null)
                return null;
            if (hexString.Length % 2 != 0)
                throw new FormatException("The hex string is invalid because it has an odd length");
            var result = new byte[hexString.Length / 2];
            for (int i = 0; i < result.Length; i++)
                result[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return result;
        }

        /// <summary>
        /// Encodes the bytes with the Base64 encoding. 
        /// More compact than hex, but it is case-sensitive and uses the special characters `+`, `/` and `=`.
        /// </summary>
        /// <param name="data">Byte array</param>
        /// <returns>Base 64 encoded data</returns>
        public static string ToBase64String(byte[] data)
        {
            if (data == null)
                return null;
            return Convert.ToBase64String(data);
        }

        /// <summary>
        /// Decodes a Base64 encoded string back to bytes.
        /// </summary>
        /// <param name="base64String">Base 64 encoded data</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBase64String(string base64String)
        {
            if (base64String == null)
                return null;
            return Convert.FromBase64String(base64String);
        }

        private const string strDigits = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz";

        /// <summary>
        /// Encode a byte sequence as a base58-encoded string
        /// </summary>
        /// <param name="input">Byte sequence</param>
        /// <returns>Encoding result</returns>
        public static string Base58Encode(byte[] input)
        {
            // Decode byte[] to BigInteger
            BigInteger intData = 0;
            for (int i = 0; i < input.Length; i++)
            {
                intData = intData * 256 + input[i];
            }

            // Encode BigInteger to Base58 string
            string result = "";
            while (intData > 0)
            {
                int remainder = (int)(intData % 58);
                intData /= 58;
                result = strDigits[remainder] + result;
            }

            // Append `1` for each leading 0 byte
            for (int i = 0; i < input.Length && input[i] == 0; i++)
            {
                result = '1' + result;
            }
            return result;
        }

        /// <summary>
        /// // Decode a base58-encoded string into byte array
        /// </summary>
        /// <param name="input">Base58 data string</param>
        /// <returns>Byte array</returns>
        public static byte[] Base58Decode(string input)
        {
            // Decode Base58 string to BigInteger 
            BigInteger intData = 0;
            for (int i = 0; i < input.Length; i++)
            {
                int digit = strDigits.IndexOf(input[i]); //Slow
                if (digit < 0)
                    throw new FormatException(string.Format("Invalid Base58 character `{0}` at position {1}", input[i], i));
                intData = intData * 58 + digit;
            }

            // Encode BigInteger to byte[]
            // Leading zero bytes get encoded as leading `1` characters
            int leadingZeroCount = input.TakeWhile(c => c == '1').Count();
            var leadingZeros = Enumerable.Repeat((byte)0, leadingZeroCount);
            var bytesWithoutLeadingZeros =
                intData.ToByteArray()
                .Reverse()// to big endian
                .SkipWhile(b => b == 0);//strip sign byte
            var result = leadingZeros.Concat(bytesWithoutLeadingZeros).ToArray();
            return result;
        }
    }
}
