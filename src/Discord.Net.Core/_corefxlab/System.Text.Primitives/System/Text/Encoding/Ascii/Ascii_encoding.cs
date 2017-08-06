// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;

namespace System.Text.Encoders
{
    public static partial class Ascii
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUtf16String(ReadOnlySpan<byte> bytes)
        {
            var len = bytes.Length;
            if (len == 0) {
                return string.Empty;
            }

            var result = new string('\0', len);

            unsafe
            {
                fixed (char* destination = result)
                fixed (byte* source = &bytes.DangerousGetPinnableReference()) {
                    if (!TryGetAsciiString(source, destination, len)) {
                        ThrowArgumentException();
                    }
                }
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToUtf16String(Span<byte> bytes)
        {
            var len = bytes.Length;
            if (len == 0) {
                return string.Empty;
            }

            var result = new string('\0', len);

            unsafe
            {
                fixed (char* destination = result)
                fixed (byte* source = &bytes.DangerousGetPinnableReference()) {
                    if (!TryGetAsciiString(source, destination, len)) {
                        ThrowArgumentException();
                    }
                }
            }

            return result;
        }

        static void ThrowArgumentException()
        {
            throw new ArgumentException();
        }

        static unsafe bool TryGetAsciiString(byte* input, char* output, int count)
        {
            var i = 0;

            int isValid = 0;
            while (i < count - 11) {
                isValid = isValid | *input | *(input + 1) | *(input + 2) |
                    *(input + 3) | *(input + 4) | *(input + 5) | *(input + 6) |
                    *(input + 7) | *(input + 8) | *(input + 9) | *(input + 10) |
                    *(input + 11);

                i += 12;
                *(output) = (char)*(input);
                *(output + 1) = (char)*(input + 1);
                *(output + 2) = (char)*(input + 2);
                *(output + 3) = (char)*(input + 3);
                *(output + 4) = (char)*(input + 4);
                *(output + 5) = (char)*(input + 5);
                *(output + 6) = (char)*(input + 6);
                *(output + 7) = (char)*(input + 7);
                *(output + 8) = (char)*(input + 8);
                *(output + 9) = (char)*(input + 9);
                *(output + 10) = (char)*(input + 10);
                *(output + 11) = (char)*(input + 11);
                output += 12;
                input += 12;
            }
            if (i < count - 5) {
                isValid = isValid | *input | *(input + 1) | *(input + 2) |
                    *(input + 3) | *(input + 4) | *(input + 5);

                i += 6;
                *(output) = (char)*(input);
                *(output + 1) = (char)*(input + 1);
                *(output + 2) = (char)*(input + 2);
                *(output + 3) = (char)*(input + 3);
                *(output + 4) = (char)*(input + 4);
                *(output + 5) = (char)*(input + 5);
                output += 6;
                input += 6;
            }
            if (i < count - 3) {
                isValid = isValid | *input | *(input + 1) | *(input + 2) |
                    *(input + 3);

                i += 4;
                *(output) = (char)*(input);
                *(output + 1) = (char)*(input + 1);
                *(output + 2) = (char)*(input + 2);
                *(output + 3) = (char)*(input + 3);
                output += 4;
                input += 4;
            }

            while (i < count) {
                isValid = isValid | *input;

                i++;
                *output = (char)*input;
                output++;
                input++;
            }

            return isValid <= 127;
        }     
    }
}
