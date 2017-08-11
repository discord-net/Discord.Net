// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text.Formatting;

namespace System.Text.Json
{
    public struct JsonWriter
    {
        readonly bool _prettyPrint;
        readonly ITextOutput _output;
        readonly JsonEncoderState _encoderState;

        int _indent;
        bool _firstItem;

        // These next 2 properties are used to check for whether we can take the fast path
        // for invariant UTF-8 or UTF-16 processing. Otherwise, we need to go through the
        // slow path which makes use of the (possibly generic) encoder.
        private bool UseFastUtf8 => _encoderState == JsonEncoderState.UseFastUtf8;
        private bool UseFastUtf16 => _encoderState == JsonEncoderState.UseFastUtf16;

        /// <summary>
        /// Constructs a JSON writer with a specified <paramref name="output"/>.
        /// </summary>
        /// <param name="output">An instance of <see cref="ITextOutput" /> used for writing bytes to an output channel.</param>
        /// <param name="prettyPrint">Specifies whether to add whitespace to the output text for user readability.</param>
        public JsonWriter(ITextOutput output, bool prettyPrint = false)
        {
            _output = output;
            _prettyPrint = prettyPrint;

            _indent = -1;
            _firstItem = true;

            var symbolTable = output.SymbolTable;
            if (symbolTable == SymbolTable.InvariantUtf8)
                _encoderState = JsonEncoderState.UseFastUtf8;
            else if (symbolTable == SymbolTable.InvariantUtf16)
                _encoderState = JsonEncoderState.UseFastUtf16;
            else
                _encoderState = JsonEncoderState.UseFullEncoder;
        }

        /// <summary>
        /// Write the starting tag of an object. This is used for adding an object to an
        /// array of other items. If this is used while inside a nested object, the property
        /// name will be missing and result in invalid JSON.
        /// </summary>
        public void WriteObjectStart()
        {
            WriteItemSeperator();
            WriteSpacing(false);
            WriteControl(JsonConstants.OpenBrace);

            _firstItem = true;
            _indent++;
        }

        /// <summary>
        /// Write the starting tag of an object. This is used for adding an object to a
        /// nested object. If this is used while inside a nested array, the property
        /// name will be written and result in invalid JSON.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        public void WriteObjectStart(string name)
        {
            WriteStartAttribute(name);
            WriteControl(JsonConstants.OpenBrace);

            _firstItem = true;
            _indent++;
        }

        /// <summary>
        /// Writes the end tag for an object.
        /// </summary>
        public void WriteObjectEnd()
        {
            _firstItem = false;
            _indent--;
            WriteSpacing();
            WriteControl(JsonConstants.CloseBrace);
        }

        /// <summary>
        /// Write the starting tag of an array. This is used for adding an array to a nested
        /// array of other items. If this is used while inside a nested object, the property
        /// name will be missing and result in invalid JSON.
        /// </summary>
        public void WriteArrayStart()
        {
            WriteItemSeperator();
            WriteSpacing();
            WriteControl(JsonConstants.OpenBracket);

            _firstItem = true;
            _indent++;
        }

        /// <summary>
        /// Write the starting tag of an array. This is used for adding an array to a
        /// nested object. If this is used while inside a nested array, the property
        /// name will be written and result in invalid JSON.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        public void WriteArrayStart(string name)
        {
            WriteStartAttribute(name);
            WriteControl(JsonConstants.OpenBracket);

            _firstItem = true;
            _indent++;
        }

        /// <summary>
        /// Writes the end tag for an array.
        /// </summary>
        public void WriteArrayEnd()
        {
            _firstItem = false;
            _indent--;
            WriteSpacing();
            WriteControl(JsonConstants.CloseBracket);
        }

        /// <summary>
        /// Write a quoted string value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The string value that will be quoted within the JSON data.</param>
        public void WriteAttribute(string name, string value)
        {
            WriteStartAttribute(name);
            WriteQuotedString(value);
        }

        /// <summary>
        /// Write a signed integer value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The signed integer value to be written to JSON data.</param>
        public void WriteAttribute(string name, long value)
        {
            WriteStartAttribute(name);
            WriteNumber(value);
        }

        /// <summary>
        /// Write an unsigned integer value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The unsigned integer value to be written to JSON data.</param>
        public void WriteAttribute(string name, ulong value)
        {
            WriteStartAttribute(name);
            WriteNumber(value);
        }

        /// <summary>
        /// Write a boolean value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The boolean value to be written to JSON data.</param>
        public void WriteAttribute(string name, bool value)
        {
            WriteStartAttribute(name);
            if (value)
                WriteJsonValue(JsonConstants.TrueValue);
            else
                WriteJsonValue(JsonConstants.FalseValue);
        }

        /// <summary>
        /// Write a <see cref="DateTime"/> value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The <see cref="DateTime"/> value to be written to JSON data.</param>
        public void WriteAttribute(string name, DateTime value)
        {
            WriteStartAttribute(name);
            WriteDateTime(value);
        }

        /// <summary>
        /// Write a <see cref="DateTimeOffset"/> value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The <see cref="DateTimeOffset"/> value to be written to JSON data.</param>
        public void WriteAttribute(string name, DateTimeOffset value)
        {
            WriteStartAttribute(name);
            WriteDateTimeOffset(value);
        }

        /// <summary>
        /// Write a <see cref="Guid"/> value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        /// <param name="value">The <see cref="Guid"/> value to be written to JSON data.</param>
        public void WriteAttribute(string name, Guid value)
        {
            WriteStartAttribute(name);
            WriteGuid(value);
        }

        /// <summary>
        /// Write a null value along with a property name into the current object.
        /// </summary>
        /// <param name="name">The name of the property (i.e. key) within the containing object.</param>
        public void WriteAttributeNull(string name)
        {
            WriteStartAttribute(name);
            WriteJsonValue(JsonConstants.NullValue);
        }

        /// <summary>
        /// Writes a quoted string value into the current array.
        /// </summary>
        /// <param name="value">The string value that will be quoted within the JSON data.</param>
        public void WriteValue(string value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteQuotedString(value);
        }

        /// <summary>
        /// Write a signed integer value into the current array.
        /// </summary>
        /// <param name="value">The signed integer value to be written to JSON data.</param>
        public void WriteValue(long value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteNumber(value);
        }

        /// <summary>
        /// Write a unsigned integer value into the current array.
        /// </summary>
        /// <param name="value">The unsigned integer value to be written to JSON data.</param>
        public void WriteValue(ulong value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteNumber(value);
        }

        /// <summary>
        /// Write a boolean value into the current array.
        /// </summary>
        /// <param name="value">The boolean value to be written to JSON data.</param>
        public void WriteValue(bool value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            if (value)
                WriteJsonValue(JsonConstants.TrueValue);
            else
                WriteJsonValue(JsonConstants.FalseValue);
        }

        /// <summary>
        /// Write a <see cref="DateTime"/> value into the current array.
        /// </summary>
        /// <param name="value">The <see cref="DateTime"/> value to be written to JSON data.</param>
        public void WriteValue(DateTime value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteDateTime(value);
        }

        /// <summary>
        /// Write a <see cref="DateTimeOffset"/> value into the current array.
        /// </summary>
        /// <param name="value">The <see cref="DateTimeOffset"/> value to be written to JSON data.</param>
        public void WriteValue(DateTimeOffset value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteDateTimeOffset(value);
        }

        /// <summary>
        /// Write a <see cref="Guid"/> value into the current array.
        /// </summary>
        /// <param name="value">The <see cref="Guid"/> value to be written to JSON data.</param>
        public void WriteValue(Guid value)
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteGuid(value);
        }

        /// <summary>
        /// Write a null value into the current array.
        /// </summary>
        public void WriteNull()
        {
            WriteItemSeperator();
            _firstItem = false;
            WriteSpacing();
            WriteJsonValue(JsonConstants.NullValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteStartAttribute(string name)
        {
            WriteItemSeperator();
            _firstItem = false;

            WriteSpacing();
            WriteQuotedString(name);
            WriteControl(JsonConstants.KeyValueSeperator);

            if (_prettyPrint)
                WriteControl(JsonConstants.Space);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteControl(byte value)
        {
            if (UseFastUtf8)
            {
                EnsureBuffer(1).DangerousGetPinnableReference() = value;
                _output.Advance(1);
            }
            else if (UseFastUtf16)
            {
                var buffer = EnsureBuffer(2);
                Unsafe.As<byte, char>(ref buffer.DangerousGetPinnableReference()) = (char)value;
                _output.Advance(2);
            }
            else
            {
                var buffer = _output.Buffer;
                int written;
                while (!_output.SymbolTable.TryEncode(value, buffer, out written))
                    buffer = EnsureBuffer();

                _output.Advance(written);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteQuotedString(string value)
        {
            WriteControl(JsonConstants.Quote);
            // TODO: We need to handle escaping.
            Write(value.AsSpan());
            WriteControl(JsonConstants.Quote);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNumber(long value)
        {
            var buffer = _output.Buffer;
            int written;
            while (!value.TryFormat(buffer, out written, JsonConstants.NumberFormat, _output.SymbolTable))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteNumber(ulong value)
        {
            var buffer = _output.Buffer;
            int written;
            while (!value.TryFormat(buffer, out written, JsonConstants.NumberFormat, _output.SymbolTable))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteDateTime(DateTime value)
        {
            var buffer = _output.Buffer;
            int written;
            while (!value.TryFormat(buffer, out written, JsonConstants.DateTimeFormat, _output.SymbolTable))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteDateTimeOffset(DateTimeOffset value)
        {
            var buffer = _output.Buffer;
            int written;
            while (!value.TryFormat(buffer, out written, JsonConstants.DateTimeFormat, _output.SymbolTable))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteGuid(Guid value)
        {
            var buffer = _output.Buffer;
            int written;
            while (!value.TryFormat(buffer, out written, JsonConstants.GuidFormat, _output.SymbolTable))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Write(ReadOnlySpan<char> value)
        {
            ReadOnlySpan<byte> source = value.AsBytes();

            if (UseFastUtf8)
            {
                Span<byte> destination = _output.Buffer;

                while (true)
                {
                    var status = Encoders.Utf16.ToUtf8(source, destination, out int consumed, out int written);
                    if (status == Buffers.TransformationStatus.Done)
                    {
                        _output.Advance(written);
                        return;
                    }

                    if (status == Buffers.TransformationStatus.DestinationTooSmall)
                    {
                        destination = EnsureBuffer();
                        continue;
                    }

                    // This is a failure due to bad input. This shouldn't happen under normal circumstances.
                    throw new FormatException();
                }
            }
            else if (UseFastUtf16)
            {
                Span<byte> destination = EnsureBuffer(source.Length);
                source.CopyTo(destination);
                _output.Advance(source.Length);
            }
            else
            {
                Span<byte> destination = _output.Buffer;
                if (!_output.SymbolTable.TryEncode(source, destination, out int consumed, out int written))
                    destination = EnsureBuffer();

                _output.Advance(written);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteJsonValue(ReadOnlySpan<byte> values)
        {
            var buffer = _output.Buffer;
            int written;
            while (!_output.SymbolTable.TryEncode(values, buffer, out int consumed, out written))
                buffer = EnsureBuffer();

            _output.Advance(written);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteItemSeperator()
        {
            if (_firstItem) return;

            WriteControl(JsonConstants.ListSeperator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSpacing(bool newLine = true)
        {
            if (!_prettyPrint) return;

            if (UseFastUtf8)
                WriteSpacingUtf8(newLine);
            else if (UseFastUtf16)
                WriteSpacingUtf16(newLine);
            else
                WriteSpacingSlow(newLine);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSpacingSlow(bool newLine)
        {
            if (newLine)
            {
                WriteControl(JsonConstants.CarriageReturn);
                WriteControl(JsonConstants.LineFeed);
            }

            int indent = _indent;
            while (indent-- >= 0)
            {
                WriteControl(JsonConstants.Space);
                WriteControl(JsonConstants.Space);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSpacingUtf8(bool newline)
        {
            var indent = _indent;
            var bytesNeeded = newline ? 2 : 0;
            bytesNeeded += (indent + 1) * 2;

            var buffer = EnsureBuffer(bytesNeeded);
            ref byte utf8Bytes = ref buffer.DangerousGetPinnableReference();
            int idx = 0;

            if (newline)
            {
                Unsafe.Add(ref utf8Bytes, idx++) = JsonConstants.CarriageReturn;
                Unsafe.Add(ref utf8Bytes, idx++) = JsonConstants.LineFeed;
            }

            while (indent-- >= 0)
            {
                Unsafe.Add(ref utf8Bytes, idx++) = JsonConstants.Space;
                Unsafe.Add(ref utf8Bytes, idx++) = JsonConstants.Space;
            }

            _output.Advance(bytesNeeded);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteSpacingUtf16(bool newline)
        {
            var indent = _indent;
            var bytesNeeded = newline ? 2 : 0;
            bytesNeeded += (indent + 1) * 2;
            bytesNeeded *= sizeof(char);

            var buffer = EnsureBuffer(bytesNeeded);
            var span = buffer.NonPortableCast<byte, char>();
            ref char utf16Bytes = ref span.DangerousGetPinnableReference();
            int idx = 0;

            if (newline)
            {
                Unsafe.Add(ref utf16Bytes, idx++) = (char)JsonConstants.CarriageReturn;
                Unsafe.Add(ref utf16Bytes, idx++) = (char)JsonConstants.LineFeed;
            }

            while (indent-- >= 0)
            {
                Unsafe.Add(ref utf16Bytes, idx++) = (char)JsonConstants.Space;
                Unsafe.Add(ref utf16Bytes, idx++) = (char)JsonConstants.Space;
            }

            _output.Advance(bytesNeeded);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Span<byte> EnsureBuffer(int needed = 256)
        {
            // Anytime we need to request an enlarge for the buffer, we may as well ask for something
            // larger than we are likely to need.
            const int BufferEnlargeCount = 1024;

            var buffer = _output.Buffer;
            var currentSize = buffer.Length;
            if (currentSize >= needed)
                return buffer;

            _output.Enlarge(BufferEnlargeCount);
            buffer = _output.Buffer;

            int newSize = buffer.Length;
            if (newSize < needed || newSize <= currentSize)
                throw new OutOfMemoryException();

            return buffer;
        }
    }
}
