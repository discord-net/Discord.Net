// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace System.Text
{
    public partial class SymbolTable
    {
        private sealed class Utf8InvariantSymbolTable : SymbolTable
        {
            private static readonly byte[][] Utf8DigitsAndSymbols = new byte[][]
            {
                new byte[] { 48, },
                new byte[] { 49, },
                new byte[] { 50, },
                new byte[] { 51, },
                new byte[] { 52, },
                new byte[] { 53, },
                new byte[] { 54, },
                new byte[] { 55, },
                new byte[] { 56, },
                new byte[] { 57, }, // digit 9
                new byte[] { 46, }, // decimal separator
                new byte[] { 44, }, // group separator
                new byte[] { 73, 110, 102, 105, 110, 105, 116, 121, },
                new byte[] { 45, }, // minus sign
                new byte[] { 43, }, // plus sign
                new byte[] { 78, 97, 78, }, // NaN
                new byte[] { 69, }, // E
                new byte[] { 101, }, // e
            };

            public Utf8InvariantSymbolTable() : base(Utf8DigitsAndSymbols) {}

            public override bool TryEncode(byte utf8, Span<byte> destination, out int bytesWritten)
            {
                if (destination.Length < 1)
                    goto ExitFailed;

                if (utf8 > 0x7F)
                    goto ExitFailed;

                destination[0] = utf8;
                bytesWritten = 1;
                return true;

            ExitFailed:
                bytesWritten = 0;
                return false;
            }

            public override bool TryEncode(ReadOnlySpan<byte> utf8, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
            {
                // TODO: We might want to validate that the stream we are moving from utf8 to destination (also UTF-8) is valid.
                //       For now, we are just doing a copy.
                if (utf8.TryCopyTo(destination))
                {
                    bytesConsumed = bytesWritten = utf8.Length;
                    return true;
                }

                bytesConsumed = bytesWritten = 0;
                return false;
            }

            public override bool TryParse(ReadOnlySpan<byte> source, out byte utf8, out int bytesConsumed)
            {
                if (source.Length < 1)
                    goto ExitFailed;

                utf8 = source[0];
                if (utf8 > 0x7F)
                    goto ExitFailed;

                bytesConsumed = 1;
                return true;

            ExitFailed:
                utf8 = 0;
                bytesConsumed = 0;
                return false;
            }

            public override bool TryParse(ReadOnlySpan<byte> source, Span<byte> utf8, out int bytesConsumed, out int bytesWritten)
            {
                // TODO: We might want to validate that the stream we are moving from utf8 to destination (also UTF-8) is valid.
                //       For now, we are just doing a copy.
                if (source.TryCopyTo(utf8))
                {
                    bytesConsumed = bytesWritten = source.Length;
                    return true;
                }

                bytesConsumed = bytesWritten = 0;
                return false;
            }
        }
    }
}
