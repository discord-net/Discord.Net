// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Collections.Sequences;
using System.Runtime.CompilerServices;
using System.Text.Formatting;

namespace System.Text.Json
{
    public struct JsonReader
    {
        // We are using a ulong to represent our nested state, so we can only go 64 levels deep.
        private const int MaxDepth = sizeof(ulong) * 8;

        private readonly JsonEncoderState _encoderState;
        private readonly SymbolTable _symbolTable;

        private ReadOnlySpan<byte> _buffer;
        private ResizableArray<byte> _working;

        // Depth tracks the recursive depth of the nested objects / arrays within the JSON data.
        internal int _depth;

        // This mask represents a tiny stack to track the state during nested transitions.
        // The first bit represents the state of the current level (1 == object, 0 == array).
        // Each subsequent bit is the parent / containing type (object or array). Since this
        // reader does a linear scan, we only need to keep a single path as we go through the data.
        private ulong _containerMask;

        // These next 2 properties are used to check for whether we can take the fast path
        // for invariant UTF-8 or UTF-16 processing. Otherwise, we need to go through the
        // slow path which makes use of the (possibly generic) encoder.
        private bool UseFastUtf8 => _encoderState == JsonEncoderState.UseFastUtf8;
        private bool UseFastUtf16 => _encoderState == JsonEncoderState.UseFastUtf16;

        // These properties are helpers for determining the current state of the reader
        private bool IsRoot => _depth == 1;
        private bool InArray => (_containerMask & 1) == 0 && (_depth > 0);
        private bool InObject => (_containerMask & 1) == 1;

        /// <summary>
        /// Gets the token type of the last processed token in the JSON stream.
        /// </summary>
        public JsonTokenType TokenType { get; private set; }

        /// <summary>
        /// Gets the value as a ReadOnlySpan<byte> of the last processed token. The contents of this
        /// is only relevant when <see cref="TokenType" /> is <see cref="JsonTokenType.Value" /> or
        /// <see cref="JsonTokenType.PropertyName" />. Otherwise, this value should be set to
        /// <see cref="ReadOnlySpan{T}.Empty"/>.
        /// </summary>
        public ReadOnlySpan<byte> Value { get; private set; }

        /// <summary>
        /// Gets the JSON value type of the last processed token. The contents of this
        /// is only relevant when <see cref="TokenType" /> is <see cref="JsonTokenType.Value" /> or
        /// <see cref="JsonTokenType.PropertyName" />.
        /// </summary>
        public JsonValueType ValueType { get; private set; }

        /// <summary>
        /// Gets the encoder instance used when the reader was constructed.
        /// </summary>
        public SymbolTable SymbolTable => _symbolTable;

        /// <summary>
        /// Constructs a new JsonReader instance. This is a stack-only type.
        /// </summary>
        /// <param name="data">The <see cref="Span{byte}"/> value to consume. </param>
        /// <param name="encoder">An encoder used for decoding bytes from <paramref name="data"/> into characters.</param>
        public JsonReader(ReadOnlySpan<byte> data, SymbolTable symbolTable)
        {
            _buffer = data;
            _symbolTable = symbolTable;
            _depth = 0;
            _containerMask = 0;
            _working = default;

            if (_symbolTable == SymbolTable.InvariantUtf8)
                _encoderState = JsonEncoderState.UseFastUtf8;
            else if (_symbolTable == SymbolTable.InvariantUtf16)
                _encoderState = JsonEncoderState.UseFastUtf16;
            else
                _encoderState = JsonEncoderState.UseFullEncoder;

            TokenType = JsonTokenType.None;
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.Unknown;
        }

        /// <summary>
        /// Read the next token from the data buffer.
        /// </summary>
        /// <returns>True if the token was read successfully, else false.</returns>
        public bool Read()
        {
            ref byte bytes = ref _buffer.DangerousGetPinnableReference();
            int length = _buffer.Length;
            int skip = SkipWhiteSpace(ref bytes, length);

            ref byte next = ref Unsafe.Add(ref bytes, skip);
            length -= skip;

            int step = GetNextCharAscii(ref next, length, out char ch);
            if (step == 0) return false;

            switch (TokenType)
            {
                case JsonTokenType.None:
                    if (ch == JsonConstants.OpenBrace)
                        StartObject();
                    else if (ch == JsonConstants.OpenBracket)
                        StartArray();
                    else
                        throw new JsonReaderException();
                    break;

                case JsonTokenType.StartObject:
                    if (ch == JsonConstants.CloseBrace)
                        EndObject();
                    else
                        step = ConsumePropertyName(ref next, length);
                    break;

                case JsonTokenType.StartArray:
                    if (ch == JsonConstants.CloseBracket)
                        EndArray();
                    else
                        step = ConsumeValue(ch, step, ref next, length);
                    break;

                case JsonTokenType.PropertyName:
                    step = ConsumeValue(ch, step, ref next, length);
                    if (step == 0) return false;
                    break;

                case JsonTokenType.EndArray:
                case JsonTokenType.EndObject:
                case JsonTokenType.Value:
                    step = ConsumeNext(ch, step, ref next, length);
                    if (step == 0) return false;
                    break;
            }

            _buffer = _buffer.Slice(skip + step);
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartObject()
        {
            if (_depth > MaxDepth)
                throw new JsonReaderException();

            _depth++;
            _containerMask = (_containerMask << 1) | 1;
            TokenType = JsonTokenType.StartObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndObject()
        {
            if (!InObject || _depth <= 0)
                throw new JsonReaderException();

            _depth--;
            _containerMask >>= 1;
            TokenType = JsonTokenType.EndObject;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartArray()
        {
            if (_depth > MaxDepth)
                throw new JsonReaderException();

            _depth++;
            _containerMask = (_containerMask << 1);
            TokenType = JsonTokenType.StartArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EndArray()
        {
            if (!InArray || _depth <= 0)
                throw new JsonReaderException();

            _depth--;
            _containerMask >>= 1;
            TokenType = JsonTokenType.EndArray;
        }

        /// <summary>
        /// This method consumes the next token regardless of whether we are inside an object or an array.
        /// For an object, it reads the next property name token. For an array, it just reads the next value.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeNext(char marker, int markerBytes, ref byte src, int length)
        {
            int skip = markerBytes;

            switch (marker)
            {
                case (char)JsonConstants.ListSeperator:
                    {
                        skip += SkipWhiteSpace(ref Unsafe.Add(ref src, markerBytes), length - markerBytes);
                        length -= skip;
                        ref byte next = ref Unsafe.Add(ref src, skip);
                        if (InObject)
                            return skip + ConsumePropertyName(ref next, length);
                        else if (InArray)
                        {
                            int step = GetNextCharAscii(ref next, length, out char ch);
                            if (step == 0) return 0;
                            return skip + ConsumeValue(ch, step, ref next, length);
                        }
                        else
                            throw new JsonReaderException();
                    }

                case (char)JsonConstants.CloseBrace:
                    EndObject();
                    return skip;

                case (char)JsonConstants.CloseBracket:
                    EndArray();
                    return skip;

                default:
                    throw new JsonReaderException();
            }
        }

        /// <summary>
        /// This method contains the logic for processing the next value token and determining
        /// what type of data it is.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeValue(char marker, int markerBytes, ref byte src, int length)
        {
            TokenType = JsonTokenType.Value;

            switch (marker)
            {
                case (char)JsonConstants.Quote:
                    return ConsumeString(ref src, length);

                case (char)JsonConstants.OpenBrace:
                    StartObject();
                    ValueType = JsonValueType.Object;
                    return markerBytes;

                case (char)JsonConstants.OpenBracket:
                    StartArray();
                    ValueType = JsonValueType.Array;
                    return markerBytes;

                case '0':
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    return ConsumeNumber(ref src, length);

                case '-':
                    int step = GetNextCharAscii(ref src, length, out char ch);
                    if (step == 0) throw new JsonReaderException();
                    return (ch == 'I')
                        ? ConsumeInfinity(ref src, length, true)
                        : ConsumeNumber(ref src, length);

                case 'f':
                    return ConsumeFalse(ref src, length);

                case 't':
                    return ConsumeTrue(ref src, length);

                case 'n':
                    return ConsumeNull(ref src, length);

                case 'u':
                    return ConsumeUndefined(ref src, length);

                case 'N':
                    return ConsumeNaN(ref src, length);

                case 'I':
                    return ConsumeInfinity(ref src, length, false);

                case '/':
                    // TODO: Comments?
                    throw new NotImplementedException();
            }

            return 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeNumber(ref byte src, int length)
        {
            if (UseFastUtf8)
            {
                int idx = 0;
                // Scan until we find a list separator, array end, or object end.
                while (idx < length)
                {
                    ref byte b = ref Unsafe.Add(ref src, idx);
                    if (b == JsonConstants.ListSeperator || b == JsonConstants.CloseBrace || b == JsonConstants.CloseBracket)
                        break;
                    idx++;
                }

                // Calculate the real start of the number based on our current buffer location.
                int startIndex = (int)Unsafe.ByteOffset(ref _buffer.DangerousGetPinnableReference(), ref src);

                Value = _buffer.Slice(startIndex, idx);
                ValueType = JsonValueType.Number;
                return idx;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                int idx = 0;
                length >>= 1; // Each char is 2 bytes.

                // Scan until we find a list separator, array end, or object end.
                while (idx < length)
                {
                    ref char b = ref Unsafe.Add(ref chars, idx);
                    if (b == JsonConstants.ListSeperator || b == JsonConstants.CloseBrace || b == JsonConstants.CloseBracket)
                        break;
                    idx++;
                }

                // Calculate the real start of the number based on our current buffer location.
                int startIndex = (int)Unsafe.ByteOffset(ref _buffer.DangerousGetPinnableReference(), ref src);

                // consumed is in characters, but our buffer is in bytes, so we need to double it for buffer slicing.
                int bytesConsumed = idx << 1;

                Value = _buffer.Slice(startIndex, bytesConsumed);
                ValueType = JsonValueType.Number;
                return bytesConsumed;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeNaN(ref byte src, int length)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.NaN;

            if (UseFastUtf8)
            {
                if (length < 3
                    || Unsafe.Add(ref src, 0) != 'N'
                    || Unsafe.Add(ref src, 1) != 'a'
                    || Unsafe.Add(ref src, 2) != 'N')
                {
                    throw new JsonReaderException();
                }

                return 3;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 6
                    || Unsafe.Add(ref chars, 0) != 'N'
                    || Unsafe.Add(ref chars, 1) != 'a'
                    || Unsafe.Add(ref chars, 2) != 'N')
                {
                    throw new JsonReaderException();
                }

                return 6;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeNull(ref byte src, int length)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.Null;

            if (UseFastUtf8)
            {
                if (length < 4
                    || Unsafe.Add(ref src, 0) != 'n'
                    || Unsafe.Add(ref src, 1) != 'u'
                    || Unsafe.Add(ref src, 2) != 'l'
                    || Unsafe.Add(ref src, 3) != 'l')
                {
                    throw new JsonReaderException();
                }

                return 4;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 8
                    || Unsafe.Add(ref chars, 0) != 'n'
                    || Unsafe.Add(ref chars, 1) != 'u'
                    || Unsafe.Add(ref chars, 2) != 'l'
                    || Unsafe.Add(ref chars, 3) != 'l')
                {
                    throw new JsonReaderException();
                }

                return 8;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeInfinity(ref byte src, int length, bool negative)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = !negative ? JsonValueType.Infinity : JsonValueType.NegativeInfinity;

            int idx = negative ? 1 : 0;
            if (UseFastUtf8)
            {
                if (length < 8 + idx
                    || Unsafe.Add(ref src, idx++) != 'I'
                    || Unsafe.Add(ref src, idx++) != 'n'
                    || Unsafe.Add(ref src, idx++) != 'f'
                    || Unsafe.Add(ref src, idx++) != 'i'
                    || Unsafe.Add(ref src, idx++) != 'n'
                    || Unsafe.Add(ref src, idx++) != 'i'
                    || Unsafe.Add(ref src, idx++) != 't'
                    || Unsafe.Add(ref src, idx++) != 'y')
                {
                    throw new JsonReaderException();
                }

                return idx;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 16 + idx
                    || Unsafe.Add(ref chars, idx++) != 'I'
                    || Unsafe.Add(ref chars, idx++) != 'n'
                    || Unsafe.Add(ref chars, idx++) != 'f'
                    || Unsafe.Add(ref chars, idx++) != 'i'
                    || Unsafe.Add(ref chars, idx++) != 'n'
                    || Unsafe.Add(ref chars, idx++) != 'i'
                    || Unsafe.Add(ref chars, idx++) != 't'
                    || Unsafe.Add(ref chars, idx++) != 'y')
                {
                    throw new JsonReaderException();
                }

                return idx;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeUndefined(ref byte src, int length)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.Undefined;

            if (UseFastUtf8)
            {
                if (length < 9
                    || Unsafe.Add(ref src, 0) != 'u'
                    || Unsafe.Add(ref src, 1) != 'n'
                    || Unsafe.Add(ref src, 2) != 'd'
                    || Unsafe.Add(ref src, 3) != 'e'
                    || Unsafe.Add(ref src, 4) != 'f'
                    || Unsafe.Add(ref src, 5) != 'i'
                    || Unsafe.Add(ref src, 6) != 'n'
                    || Unsafe.Add(ref src, 7) != 'e'
                    || Unsafe.Add(ref src, 8) != 'd')
                {
                    throw new JsonReaderException();
                }

                return 9;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 18
                    || Unsafe.Add(ref chars, 0) != 'u'
                    || Unsafe.Add(ref chars, 1) != 'n'
                    || Unsafe.Add(ref chars, 2) != 'd'
                    || Unsafe.Add(ref chars, 3) != 'e'
                    || Unsafe.Add(ref chars, 4) != 'f'
                    || Unsafe.Add(ref chars, 5) != 'i'
                    || Unsafe.Add(ref chars, 6) != 'n'
                    || Unsafe.Add(ref chars, 7) != 'e'
                    || Unsafe.Add(ref chars, 8) != 'd')
                {
                    throw new JsonReaderException();
                }

                return 18;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeFalse(ref byte src, int length)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.False;

            if (UseFastUtf8)
            {
                if (length < 5
                    || Unsafe.Add(ref src, 0) != 'f'
                    || Unsafe.Add(ref src, 1) != 'a'
                    || Unsafe.Add(ref src, 2) != 'l'
                    || Unsafe.Add(ref src, 3) != 's'
                    || Unsafe.Add(ref src, 4) != 'e')
                {
                    throw new JsonReaderException();
                }

                return 5;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 10
                    || Unsafe.Add(ref chars, 0) != 'f'
                    || Unsafe.Add(ref chars, 1) != 'a'
                    || Unsafe.Add(ref chars, 2) != 'l'
                    || Unsafe.Add(ref chars, 3) != 's'
                    || Unsafe.Add(ref chars, 4) != 'e')
                {
                    throw new JsonReaderException();
                }

                return 10;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeTrue(ref byte src, int length)
        {
            Value = ReadOnlySpan<byte>.Empty;
            ValueType = JsonValueType.True;

            if (UseFastUtf8)
            {
                if (length < 4
                    || Unsafe.Add(ref src, 0) != 't'
                    || Unsafe.Add(ref src, 1) != 'r'
                    || Unsafe.Add(ref src, 2) != 'u'
                    || Unsafe.Add(ref src, 3) != 'e')
                {
                    throw new JsonReaderException();
                }

                return 4;
            }
            else if (UseFastUtf16)
            {
                ref char chars = ref Unsafe.As<byte, char>(ref src);
                if (length < 8
                    || Unsafe.Add(ref chars, 0) != 't'
                    || Unsafe.Add(ref chars, 1) != 'r'
                    || Unsafe.Add(ref chars, 2) != 'u'
                    || Unsafe.Add(ref chars, 3) != 'e')
                {
                    throw new JsonReaderException();
                }

                return 8;
            }
            else
                throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumePropertyName(ref byte src, int length)
        {
            if (UseFastUtf8)
                return ConsumePropertyNameUtf8(ref src, length);
            else if (UseFastUtf16)
                return ConsumePropertyNameUtf16(ref src, length);
            else
                return ConsumePropertyNameSlow(ref src, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumePropertyNameUtf8(ref byte src, int length)
        {
            int consumed = ConsumeStringUtf8(ref src, length);
            if (consumed == 0) throw new JsonReaderException();

            consumed += SkipWhiteSpaceUtf8(ref Unsafe.Add(ref src, consumed), length - consumed);
            if (consumed >= length) throw new JsonReaderException();

            // The next character must be a key / value seperator. Validate and skip.
            if (Unsafe.Add(ref src, consumed++) != JsonConstants.KeyValueSeperator)
                throw new JsonReaderException();

            TokenType = JsonTokenType.PropertyName;
            return consumed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumePropertyNameUtf16(ref byte src, int length)
        {
            int consumed = ConsumeStringUtf16(ref src, length);
            if (consumed == 0) throw new JsonReaderException();

            consumed += SkipWhiteSpaceUtf16(ref Unsafe.Add(ref src, consumed), length - consumed);
            if (consumed >= length) throw new JsonReaderException();

            // The next character must be a key / value seperator
            if (Unsafe.As<byte, char>(ref Unsafe.Add(ref src, consumed)) != JsonConstants.KeyValueSeperator)
                throw new JsonReaderException();

            consumed += 2; // Skip the key / value seperator
            TokenType = JsonTokenType.PropertyName;
            return consumed;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumePropertyNameSlow(ref byte src, int length)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeString(ref byte src, int length)
        {
            if (UseFastUtf8)
                return ConsumeStringUtf8(ref src, length);
            else if (UseFastUtf16)
                return ConsumeStringUtf16(ref src, length);
            else
                return ConsumeStringSlow(ref src, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeStringUtf8(ref byte src, int length)
        {
            // The first character MUST be a JSON string quote
            if (src != JsonConstants.Quote) throw new JsonReaderException();

            // If we are in this method, the first char is already known to be a JSON quote character.
            // Skip through the bytes until we find the closing JSON quote character.
            int idx = 1;
            int start = idx;
            bool hasEscapes = false;

            while (idx < length)
            {
                byte c = Unsafe.Add(ref src, idx++);
                if (c == JsonConstants.Quote)
                    break;
                else if (c == JsonConstants.Backslash)
                {
                    hasEscapes = true;
                    break;
                }
            }

            if (!hasEscapes) //Fast route
            {
                // If we hit the end of the source and never saw an ending quote, then fail.
                if (idx == length && Unsafe.Add(ref src, idx - 1) != JsonConstants.Quote)
                    throw new JsonReaderException();

                // Calculate the real start of the property name based on our current buffer location.
                // Also, skip the opening JSON quote character.
                int startIndex = (int)Unsafe.ByteOffset(ref _buffer.DangerousGetPinnableReference(), ref src) + 1;

                Value = _buffer.Slice(startIndex, idx - 2); // -2 to exclude the quote characters.
            }
            else //Slow route
            {
                if (_working.Items == null)
                    _working = new ResizableArray<byte>(ArrayPool<byte>.Shared.Rent(128));
                _working.Clear();

                int arrLength = idx - start;
                idx = start;
                
                bool isEscaping = false;
                bool success = false;
                while (!success && idx < length)
                {
                    byte c = Unsafe.Add(ref src, idx);
                    if (isEscaping)
                    {
                        switch (c)
                        {
                            case (byte)'/':
                            case (byte)'b':
                            case (byte)'f':
                            case (byte)'n':
                            case (byte)'r':
                            case (byte)'t':
                                {
                                    int remaining = _working.Capacity - _working.Count;
                                    if (remaining == 0)
                                    {
                                        int newSize = _working.Free.Count * 2;
                                        var newArray = ArrayPool<byte>.Shared.Rent(newSize);
                                        var oldArray = _working.Resize(newArray);
                                        ArrayPool<byte>.Shared.Return(oldArray);
                                    }

                                    var span = _working.Free.AsSpan();
                                    switch (c)
                                    {
                                        case (byte)'/': span[0] = (byte)'/'; break;
                                        case (byte)'b': span[0] = (byte)'\b'; break;
                                        case (byte)'f': span[0] = (byte)'\f'; break;
                                        case (byte)'n': span[0] = (byte)'\n'; break;
                                        case (byte)'r': span[0] = (byte)'\r'; break;
                                        case (byte)'t': span[0] = (byte)'\t'; break;
                                    }
                                    _working.Count++;
                                    start++; //Skip this char
                                }
                                break;
                            //case (byte)'u': //Not Supported
                        }
                        isEscaping = false;
                    }
                    else
                    {
                        switch (c)
                        {
                            case JsonConstants.Backslash:
                            case JsonConstants.Quote:
                                {
                                    int segmentLength = idx - start;
                                    if (segmentLength != 0)
                                    {
                                        //Ensure we have enough space in the buffer
                                        int remaining = _working.Capacity - _working.Count;
                                        if (segmentLength > remaining)
                                        {
                                            int doubleSize = _working.Free.Count * 2;
                                            int minNewSize = _working.Capacity + segmentLength;
                                            int newSize = minNewSize > doubleSize ? minNewSize : doubleSize;
                                            var newArray = ArrayPool<byte>.Shared.Rent(newSize);
                                            var oldArray = _working.Resize(newArray);
                                            ArrayPool<byte>.Shared.Return(oldArray);
                                        }

                                        //Copy all data before the backslash
                                        var span = _working.Free.AsSpan();
                                        Unsafe.CopyBlock(ref span.DangerousGetPinnableReference(), ref Unsafe.Add(ref src, start), (uint)segmentLength);
                                        _working.Count += segmentLength;
                                    }

                                    if (c == JsonConstants.Quote)
                                    {
                                        success = true;
                                        break;
                                    }
                                    else
                                    {
                                        start = idx + 1;
                                        isEscaping = true;
                                    }
                                }
                                break;
                        }
                    }
                    idx++;
                }

                if (!success)
                    throw new JsonReaderException();

                Value = _working.Full;
            }
            ValueType = JsonValueType.String;
            return idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeStringUtf16(ref byte src, int length)
        {
            ref char chars = ref Unsafe.As<byte, char>(ref src);
            length >>= 1; // sizeof(char) is 2, so we have half as many characters as count.

            // The first character MUST be a JSON string quote
            if (chars != JsonConstants.Quote) throw new JsonReaderException();

            // Skip through the bytes until we find the closing JSON quote character.
            int idx = 1;
            while (idx < length && Unsafe.Add(ref chars, idx++) != JsonConstants.Quote) ;

            // If we hit the end of the source and never saw an ending quote, then fail.
            if (idx == length && Unsafe.Add(ref chars, idx - 1) != JsonConstants.Quote)
                throw new JsonReaderException();

            // Calculate the real start of the property name based on our current buffer location.
            // Also, skip the opening JSON quote character.
            int startIndex = (int)Unsafe.ByteOffset(ref _buffer.DangerousGetPinnableReference(), ref src) + 2;

            // idx is in characters, but our buffer is in bytes, so we need to double it for buffer slicing.
            // Also, remove the quote characters as well.
            length = (idx - 2) << 1;

            Value = _buffer.Slice(startIndex, length);
            ValueType = JsonValueType.String;
            return idx * sizeof(char);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int ConsumeStringSlow(ref byte src, int length)
        {
            throw new NotImplementedException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhiteSpace(ref byte src, int length)
        {
            if (UseFastUtf8)
                return SkipWhiteSpaceUtf8(ref src, length);
            else if (UseFastUtf16)
                return SkipWhiteSpaceUtf16(ref src, length);
            else
                return SkipWhiteSpaceSlow(ref src, length);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhiteSpaceUtf8(ref byte src, int length)
        {
            int idx = 0;
            while (idx < length)
            {
                switch (Unsafe.Add(ref src, idx))
                {
                    case (byte)JsonConstants.Space:
                    case (byte)JsonConstants.CarriageReturn:
                    case (byte)JsonConstants.LineFeed:
                    case (byte)'\t':
                        idx++;
                        break;

                    default:
                        return idx;
                }
            }

            return idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhiteSpaceUtf16(ref byte src, int length)
        {
            ref char chars = ref Unsafe.As<byte, char>(ref src);
            length >>= 1;

            int idx = 0;
            while (idx < length)
            {
                switch (Unsafe.Add(ref chars, idx))
                {
                    case (char)JsonConstants.Space:
                    case (char)JsonConstants.CarriageReturn:
                    case (char)JsonConstants.LineFeed:
                    case '\t':
                        idx++;
                        break;

                    default:
                        return idx * sizeof(char);
                }
            }

            return idx * sizeof(char);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int SkipWhiteSpaceSlow(ref byte src, int length)
        {
            int idx = 0;
            while (idx < length)
            {
                int skip = GetNextCharAscii(ref Unsafe.Add(ref src, idx), length, out char ch);
                if (skip == 0)
                    break;

                switch (ch)
                {
                    case (char)JsonConstants.Space:
                    case (char)JsonConstants.CarriageReturn:
                    case (char)JsonConstants.LineFeed:
                    case '\t':
                        idx += skip;
                        break;

                    default:
                        return idx;
                }
            }

            return idx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetNextCharAscii(ref byte src, int length, out char ch)
        {
            if (UseFastUtf8)
            {
                if (length < 1)
                {
                    ch = default;
                    return 0;
                }

                ch = (char)src;
                return 1;
            }
            else if (UseFastUtf16)
            {
                if (length < 2)
                {
                    ch = default;
                    return 0;
                }

                ch = Unsafe.As<byte, char>(ref src);
                return 2;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
