// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Runtime.CompilerServices;

namespace System.Text
{
    public partial class SymbolTable
    {
        private sealed class Utf16InvariantSymbolTable : SymbolTable
        {
            private static readonly byte[][] Utf16DigitsAndSymbols = new byte[][]
            {
                new byte[] { 48, 0, }, // digit 0
                new byte[] { 49, 0, },
                new byte[] { 50, 0, },
                new byte[] { 51, 0, },
                new byte[] { 52, 0, },
                new byte[] { 53, 0, },
                new byte[] { 54, 0, },
                new byte[] { 55, 0, },
                new byte[] { 56, 0, },
                new byte[] { 57, 0, }, // digit 9
                new byte[] { 46, 0, }, // decimal separator
                new byte[] { 44, 0, }, // group separator
                new byte[] { 73, 0, 110, 0, 102, 0, 105, 0, 110, 0, 105, 0, 116, 0, 121, 0, }, // Infinity
                new byte[] { 45, 0, }, // minus sign
                new byte[] { 43, 0, }, // plus sign
                new byte[] { 78, 0, 97, 0, 78, 0, }, // NaN
                new byte[] { 69, 0, }, // E
                new byte[] { 101, 0, }, // e
            };

            public Utf16InvariantSymbolTable() : base(Utf16DigitsAndSymbols) {}

            public override bool TryEncode(byte utf8, Span<byte> destination, out int bytesWritten)
            {
                if (destination.Length < 2)
                    goto ExitFailed;

                if (utf8 > 0x7F)
                    goto ExitFailed;

                Unsafe.As<byte, char>(ref destination.DangerousGetPinnableReference()) = (char)utf8;
                bytesWritten = 2;
                return true;

            ExitFailed:
                bytesWritten = 0;
                return false;
            }

            public override bool TryEncode(ReadOnlySpan<byte> utf8, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
            {
                var status = Encoders.Utf8.ToUtf16(utf8, destination, out bytesConsumed, out bytesWritten);
                if (status != TransformationStatus.Done)
                {
                    bytesConsumed = bytesWritten = 0;
                    return false;
                }

                return true;
            }

            public override bool TryParse(ReadOnlySpan<byte> source, out byte utf8, out int bytesConsumed)
            {
                if (source.Length < 2)
                    goto ExitFailed;

                ref char value = ref Unsafe.As<byte, char>(ref source.DangerousGetPinnableReference());
                if (value > 0x7F)
                    goto ExitFailed;

                bytesConsumed = 2;
                utf8 = (byte)value;
                return true;

            ExitFailed:
                utf8 = 0;
                bytesConsumed = 0;
                return false;
            }

            public override bool TryParse(ReadOnlySpan<byte> source, Span<byte> utf8, out int bytesConsumed, out int bytesWritten)
            {
                var status = Encoders.Utf16.ToUtf8(source, utf8, out bytesConsumed, out bytesWritten);
                if (status != TransformationStatus.Done)
                {
                    bytesConsumed = bytesWritten = 0;
                    return false;
                }

                return true;
            }
        }
    }
}
