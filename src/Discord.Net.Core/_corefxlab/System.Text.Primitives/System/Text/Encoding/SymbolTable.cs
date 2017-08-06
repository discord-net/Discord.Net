// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Text
{
    public abstract partial class SymbolTable
    {
        #region Private data

        private readonly byte[][] _symbols;                     // this could be flattened into a single array
        private readonly ParsingTrie.Node[] _parsingTrie;       // prefix tree used for parsing

        #endregion Private data

        #region Constructors

        protected SymbolTable(byte[][] symbols)
        {
            _symbols = symbols;
            _parsingTrie = ParsingTrie.Create(symbols);
        }

        #endregion Constructors

        #region Static instances

        public readonly static SymbolTable InvariantUtf8 = new Utf8InvariantSymbolTable();

        public readonly static SymbolTable InvariantUtf16 = new Utf16InvariantSymbolTable();

        #endregion Static instances

        public bool TryEncode(Symbol symbol, Span<byte> destination, out int bytesWritten)
        {
            byte[] bytes = _symbols[(int)symbol];
            bytesWritten = bytes.Length;
            if (bytesWritten > destination.Length)
            {
                bytesWritten = 0;
                return false;
            }

            if (bytesWritten == 2)
            {
                destination[0] = bytes[0];
                destination[1] = bytes[1];
                return true;
            }

            if (bytesWritten == 1)
            {
                destination[0] = bytes[0];
                return true;
            }

            new Span<byte>(bytes).CopyTo(destination);
            return true;
        }

        public abstract bool TryEncode(byte utf8, Span<byte> destination, out int bytesWritten);

        public abstract bool TryEncode(ReadOnlySpan<byte> utf8, Span<byte> destination, out int bytesConsumed, out int bytesWritten);

        public bool TryParse(ReadOnlySpan<byte> source, out Symbol symbol, out int bytesConsumed)
        {
            int trieIndex = 0;
            int codeUnitIndex = 0;
            bytesConsumed = 0;
            while (true)
            {
                if (_parsingTrie[trieIndex].ValueOrNumChildren == 0)    // if numChildren == 0, we're on a leaf & we've found our value and completed the code unit
                {
                    symbol = (Symbol)_parsingTrie[trieIndex].IndexOrSymbol;  // return the parsed value
                    if (VerifySuffix(source, codeUnitIndex, symbol))
                    {
                        bytesConsumed = _symbols[(int)symbol].Length;
                        return true;
                    }
                    else
                    {
                        symbol = 0;
                        bytesConsumed = 0;
                        return false;
                    }
                }
                else
                {
                    int search = BinarySearch(trieIndex, codeUnitIndex, source[codeUnitIndex]);    // we search the _parsingTrie for the nextByte

                    if (search > 0)   // if we found a node
                    {
                        trieIndex = _parsingTrie[search].IndexOrSymbol;
                        bytesConsumed++;
                        codeUnitIndex++;
                    }
                    else
                    {
                        symbol = 0;
                        bytesConsumed = 0;
                        return false;
                    }
                }
            }
        }

        public abstract bool TryParse(ReadOnlySpan<byte> source, out byte utf8, out int bytesConsumed);

        public abstract bool TryParse(ReadOnlySpan<byte> source, Span<byte> utf8, out int bytesConsumed, out int bytesWritten);

        #region Public UTF-16 to UTF-8 helpers

        public bool TryEncode(ReadOnlySpan<char> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
        {
            ReadOnlySpan<byte> srcBytes = source.AsBytes();

            if (this == SymbolTable.InvariantUtf16)
                return TryEncodeUtf16(srcBytes, destination, out bytesConsumed, out bytesWritten);

            const int BufferSize = 256;

            int srcLength = srcBytes.Length;
            if (srcLength <= 0)
            {
                bytesConsumed = bytesWritten = 0;
                return true;
            }

            Span<byte> temp;
            unsafe
            {
                byte* pTemp = stackalloc byte[BufferSize];
                temp = new Span<byte>(pTemp, BufferSize);
            }

            bytesWritten = 0;
            bytesConsumed = 0;
            while (srcLength > bytesConsumed)
            {
                var status = Encoders.Utf16.ToUtf8(srcBytes, temp, out int consumed, out int written);
                if (status == Buffers.TransformationStatus.InvalidData)
                    goto ExitFailed;

                srcBytes = srcBytes.Slice(consumed);
                bytesConsumed += consumed;

                if (!TryEncode(temp.Slice(0, written), destination, out consumed, out written))
                    goto ExitFailed;

                destination = destination.Slice(written);
                bytesWritten += written;
            }

            return true;

        ExitFailed:
            return false;
        }

        private bool TryEncodeUtf16(ReadOnlySpan<byte> source, Span<byte> destination, out int bytesConsumed, out int bytesWritten)
        {
            // NOTE: There is no validation of this UTF-16 encoding. A caller is expected to do any validation on their own if
            //       they don't trust the data.
            bytesConsumed = source.Length;
            bytesWritten = destination.Length;
            if (bytesConsumed > bytesWritten)
            {
                source = source.Slice(0, bytesWritten);
                bytesConsumed = bytesWritten;
            }
            else
            {
                bytesWritten = bytesConsumed;
            }

            source.CopyTo(destination);
            return true;
        }

        #endregion Public UTF-16 to UTF-8 helpers

        #region Private helpers

        // This binary search implementation returns an int representing either:
        //      - the index of the item searched for (if the value is positive)
        //      - the index of the location where the item should be placed to maintain a sorted list (if the value is negative)
        private int BinarySearch(int nodeIndex, int level, byte value)
        {
            int maxMinLimits = _parsingTrie[nodeIndex].IndexOrSymbol;
            if (maxMinLimits != 0 && value > (uint)maxMinLimits >> 24 && value < (uint)(maxMinLimits << 16) >> 24)
            {
                // See the comments on the struct above for more information about this format
                return (int)(nodeIndex + ((uint)(maxMinLimits << 8) >> 24) + value - ((uint)maxMinLimits >> 24));
            }

            int leftBound = nodeIndex + 1, rightBound = nodeIndex + _parsingTrie[nodeIndex].ValueOrNumChildren;
            int midIndex = 0;
            while (true)
            {
                if (leftBound > rightBound)  // if the search failed
                {
                    // this loop is necessary because binary search takes the floor
                    // of the middle, which means it can give incorrect indices for insertion.
                    // we should never iterate up more than two indices.
                    while (midIndex < nodeIndex + _parsingTrie[nodeIndex].ValueOrNumChildren
                        && _parsingTrie[midIndex].ValueOrNumChildren < value)
                    {
                        midIndex++;
                    }
                    return -midIndex;
                }

                midIndex = (leftBound + rightBound) / 2; // find the middle value

                byte mValue = _parsingTrie[midIndex].ValueOrNumChildren;

                if (mValue < value)
                    leftBound = midIndex + 1;
                else if (mValue > value)
                    rightBound = midIndex - 1;
                else
                    return midIndex;
            }
        }

        private bool VerifySuffix(ReadOnlySpan<byte> buffer, int codeUnitIndex, Symbol symbol)
        {
            int codeUnitLength = _symbols[(int)symbol].Length;
            if (codeUnitIndex == codeUnitLength - 1)
                return true;

            for (int i = 0; i < codeUnitLength - codeUnitIndex; i++)
            {
                if (buffer[i + codeUnitIndex] != _symbols[(int)symbol][i + codeUnitIndex])
                    return false;
            }

            return true;
        }

        #endregion Private helpers
    }
}
