// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Buffers;
using System.Diagnostics;

namespace System.Text.Formatting
{

    // This whole API is very speculative, i.e. I am not sure I am happy with the design
    // This API is trying to do composite formatting without boxing (or any other allocations).
    // And because not all types in the platfrom implement IBufferFormattable (in particular built-in primitives don't),
    // it needs to play some tricks with generic type parameters. But as you can see at the end of AppendUntyped, I am not sure how to tick the type system
    // not never box.
    public static class CompositeFormattingExtensions
    {
        public static void Format<TFormatter, T0>(this TFormatter formatter, string compositeFormat, T0 arg0) where TFormatter : ITextOutput
        {
            var reader = new CompositeFormatReader(compositeFormat);
            while (true)
            {
                var segment = reader.Next();
                if (segment == null) return;

                if (segment.Value.Count == 0) // insertion point
                {
                    if (segment.Value.Index == 0) formatter.AppendUntyped(arg0, segment.Value.Format);
                    else throw new Exception("invalid insertion point");
                }
                else // literal
                {
                    formatter.Append(compositeFormat, segment.Value.Index, segment.Value.Count);
                }
            }
        }

        public static void Format<TFormatter, T0, T1>(this TFormatter formatter, string compositeFormat, T0 arg0, T1 arg1) where TFormatter : ITextOutput
        {
            var reader = new CompositeFormatReader(compositeFormat);
            while (true)
            {
                var segment = reader.Next();
                if (segment == null) return;

                if (segment.Value.Count == 0) // insertion point
                {
                    if (segment.Value.Index == 0) formatter.AppendUntyped(arg0, segment.Value.Format);
                    else if (segment.Value.Index == 1) formatter.AppendUntyped(arg1, segment.Value.Format);
                    else throw new Exception("invalid insertion point");
                }
                else // literal
                {
                    formatter.Append(compositeFormat, segment.Value.Index, segment.Value.Count);
                }
            }
        }

        public static void Format<TFormatter, T0, T1, T2>(this TFormatter formatter, string compositeFormat, T0 arg0, T1 arg1, T2 arg2) where TFormatter : ITextOutput
        {
            var reader = new CompositeFormatReader(compositeFormat);
            while (true)
            {
                var segment = reader.Next();
                if (segment == null) return;

                if (segment.Value.Count == 0) // insertion point
                {
                    if (segment.Value.Index == 0) formatter.AppendUntyped(arg0, segment.Value.Format);
                    else if (segment.Value.Index == 1) formatter.AppendUntyped(arg1, segment.Value.Format);
                    else if (segment.Value.Index == 2) formatter.AppendUntyped(arg2, segment.Value.Format);
                    else throw new Exception("invalid insertion point");
                }
                else // literal
                {
                    formatter.Append(compositeFormat, segment.Value.Index, segment.Value.Count);
                }
            }
        }

        public static void Format<TFormatter, T0, T1, T2, T3>(this TFormatter formatter, string compositeFormat, T0 arg0, T1 arg1, T2 arg2, T3 arg3) where TFormatter : ITextOutput
        {
            var reader = new CompositeFormatReader(compositeFormat);
            while (true)
            {
                var segment = reader.Next();
                if (segment == null) return;

                if (segment.Value.Count == 0) // insertion point
                {
                    if (segment.Value.Index == 0) formatter.AppendUntyped(arg0, segment.Value.Format);
                    else if (segment.Value.Index == 1) formatter.AppendUntyped(arg1, segment.Value.Format);
                    else if (segment.Value.Index == 2) formatter.AppendUntyped(arg2, segment.Value.Format);
                    else if (segment.Value.Index == 3) formatter.AppendUntyped(arg3, segment.Value.Format);
                    else throw new Exception("invalid insertion point");
                }
                else // literal
                {
                    formatter.Append(compositeFormat, segment.Value.Index, segment.Value.Count);
                }
            }
        }

        public static void Format<TFormatter, T0, T1, T2, T3, T4>(this TFormatter formatter, string compositeFormat, T0 arg0, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TFormatter : ITextOutput
        {
            var reader = new CompositeFormatReader(compositeFormat);
            while (true)
            {
                var segment = reader.Next();
                if (segment == null) return;

                if (segment.Value.Count == 0) // insertion point
                {
                    if (segment.Value.Index == 0) formatter.AppendUntyped(arg0, segment.Value.Format);
                    else if (segment.Value.Index == 1) formatter.AppendUntyped(arg1, segment.Value.Format);
                    else if (segment.Value.Index == 2) formatter.AppendUntyped(arg2, segment.Value.Format);
                    else if (segment.Value.Index == 3) formatter.AppendUntyped(arg3, segment.Value.Format);
                    else if (segment.Value.Index == 4) formatter.AppendUntyped(arg4, segment.Value.Format);
                    else throw new Exception("invalid insertion point");
                }
                else // literal
                {
                    formatter.Append(compositeFormat, segment.Value.Index, segment.Value.Count);
                }
            }
        }

        // TODO: this should be removed and an ability to append substrings should be added
        static void Append<TFormatter>(this TFormatter formatter, string whole, int index, int count) where TFormatter : ITextOutput
        {
            var buffer = formatter.Buffer;
            var maxBytes = count << 4; // this is the worst case, i.e. 4 bytes per char
            while(buffer.Length < maxBytes)
            {
                formatter.Enlarge(maxBytes);
                buffer = formatter.Buffer;
            }

            // this should be optimized using fixed pointer to substring, but I will wait with this till we design proper substring

            var characters = whole.Slice(index, count);
            if (!formatter.TryAppend(characters, formatter.SymbolTable))
            {
                Debug.Assert(false, "this should never happen"); // because I pre-resized the buffer to 4 bytes per char at the top of this method.
            }
        }

        static void AppendUntyped<TFormatter, T>(this TFormatter formatter, T value, ParsedFormat format) where TFormatter : ITextOutput
        {
            #region Built in types
            var i32 = value as int?;
            if (i32 != null)
            {
                formatter.Append(i32.Value, format);
                return;
            }
            var i64 = value as long?;
            if (i64 != null)
            {
                formatter.Append(i64.Value, format);
                return;
            }
            var i16 = value as short?;
            if (i16 != null)
            {
                formatter.Append(i16.Value, format);
                return;
            }
            var b = value as byte?;
            if (b != null)
            {
                formatter.Append(b.Value, format);
                return;
            }
            var c = value as char?;
            if (c != null)
            {
                formatter.Append(c.Value);
                return;
            }
            var u32 = value as uint?;
            if (u32 != null)
            {
                formatter.Append(u32.Value, format);
                return;
            }
            var u64 = value as ulong?;
            if (u64 != null)
            {
                formatter.Append(u64.Value, format);
                return;
            }
            var u16 = value as ushort?;
            if (u16 != null)
            {
                formatter.Append(u16.Value, format);
                return;
            }
            var sb = value as sbyte?;
            if (sb != null)
            {
                formatter.Append(sb.Value, format);
                return;
            }
            var str = value as string;
            if (str != null)
            {
                formatter.Append(str);
                return;
            }
            var dt = value as DateTime?;
            if (dt != null)
            {
                formatter.Append(dt.Value, format);
                return;
            }
            var dto = value as DateTimeOffset?;
            if (dto != null) {
                formatter.Append(dto.Value, format);
                return;
            }
            var ts = value as TimeSpan?;
            if (ts != null)
            {
                formatter.Append(ts.Value, format);
                return;
            }
            var guid = value as Guid?;
            if (guid != null) {
                formatter.Append(guid.Value, format);
                return;
            }
            #endregion

            if (value is IBufferFormattable)
            {
                formatter.Append((IBufferFormattable)value, format); // this is boxing. not sure how to avoid it.
                return;
            }

            throw new NotSupportedException("value is not formattable.");
        }

        // this is just a state machine walking the composite format and instructing CompositeFormattingExtensions.Format overloads on what to do.
        // this whole type is not just a hacky prototype.
        // I will clean it up later if I decide that I like this whole composite format model.
        struct CompositeFormatReader
        {
            string _compositeFormatString;
            int _currentIndex;
            int _spanStart;
            State _state;

            public CompositeFormatReader(string format)
            {
                _compositeFormatString = format;
                _currentIndex = 0;
                _spanStart = 0;
                _state = State.New;
            }

            public CompositeSegment? Next()
            {
                while (_currentIndex < _compositeFormatString.Length)
                {
                    char c = _compositeFormatString[_currentIndex];
                    if (c == '{')
                    {
                        if (_state == State.Literal)
                        {
                            _state = State.New;
                            return CompositeSegment.Literal(_spanStart, _currentIndex);
                        }
                        if ((_currentIndex + 1 < _compositeFormatString.Length) && (_compositeFormatString[_currentIndex + 1] == c))
                        {
                            _state = State.Literal;
                            _currentIndex++;
                            _spanStart = _currentIndex;
                        }
                        else
                        {
                            _currentIndex++;
                            return ParseInsertionPoint();
                        }
                    }
                    else if (c == '}')
                    {
                        if ((_currentIndex + 1 < _compositeFormatString.Length) && (_compositeFormatString[_currentIndex + 1] == c))
                        {
                            if (_state == State.Literal)
                            {
                                _state = State.New;
                                return CompositeSegment.Literal(_spanStart, _currentIndex);
                            }
                            _state = State.Literal;
                            _currentIndex++;
                            _spanStart = _currentIndex;
                        }
                        else
                        {
                            throw new Exception("missing start bracket");
                        }
                    }
                    else
                    {
                        if (_state != State.Literal)
                        {
                            _state = State.Literal;
                            _spanStart = _currentIndex;
                        }
                    }
                    _currentIndex++;
                }
                if (_state == State.Literal)
                {
                    _state = State.New;
                    return CompositeSegment.Literal(_spanStart, _currentIndex);
                }
                return null;
            }

            // this should be replaced with InvariantFormatter.Parse
            static bool TryParse(string compositeFormat, int start, int count, out uint value, out int consumed)
            {
                consumed = 0;
                value = 0;
                for (int i = start; i < start + count; i++)
                {
                    var digit = (byte)(compositeFormat[i] - '0');
                    if (digit >= 0 && digit <= 9)
                    {
                        value *= 10;
                        value += digit;
                        consumed++;
                    }
                    else
                    {
                        if (i == start) return false;
                        else return true;
                    }
                }
                return true;
            }

            CompositeSegment ParseInsertionPoint()
            {
                uint arg;
                int consumed;
                char? formatSpecifier = null;

                if (!TryParse(_compositeFormatString, _currentIndex, 5, out arg, out consumed))
                {
                    throw new Exception("invalid insertion point");
                }
                _currentIndex += consumed;
                if (_currentIndex >= _compositeFormatString.Length)
                {
                    throw new Exception("missing end bracket");
                }

                if(_compositeFormatString[_currentIndex] == ':')
                {
                    _currentIndex++;
                    formatSpecifier = _compositeFormatString[_currentIndex];
                    _currentIndex++;
                }

                if (_compositeFormatString[_currentIndex] != '}')
                {
                    throw new Exception("missing end bracket");
                }

                _currentIndex++;
                var parsedFormat = (formatSpecifier.HasValue && formatSpecifier.Value != 0) ? new ParsedFormat(formatSpecifier.Value) : default;
                return CompositeSegment.InsertionPoint(arg, parsedFormat);
            }

            public enum State : byte
            {
                New,
                Literal,
                InsertionPoint
            }

            public struct CompositeSegment
            {
                public ParsedFormat Format { get; private set; }
                public int Index { get; private set; }
                public int Count { get; private set; }

                public static CompositeSegment InsertionPoint(uint argIndex, ParsedFormat format)
                {
                    return new CompositeSegment() { Index = (int)argIndex, Format = format };
                }

                public static CompositeSegment Literal(int startIndex, int endIndex)
                {
                    return new CompositeSegment() { Index = startIndex, Count = endIndex - startIndex };
                }
            }
        }
    }
}
