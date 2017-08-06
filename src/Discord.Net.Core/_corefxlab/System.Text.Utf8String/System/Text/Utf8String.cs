// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Utf16;

namespace System.Text.Utf8
{
    [DebuggerDisplay("{ToString()}u8")]
    public partial struct Utf8String
    {
        private readonly ReadOnlySpan<byte> _buffer;

        private const int StringNotFound = -1;

        static Utf8String s_empty => default;

        // TODO: Validate constructors, When should we copy? When should we just use the underlying array?
        // TODO: Should we be immutable/readonly?
        public Utf8String(ReadOnlySpan<byte> buffer)
        {
            _buffer = buffer;
        }

        public Utf8String(byte[] utf8bytes)
        {
            _buffer = new ReadOnlySpan<byte>(utf8bytes);
        }

        public Utf8String(byte[] utf8bytes, int index, int length)
        {
            _buffer = new ReadOnlySpan<byte>(utf8bytes, index, length);
        }

        public Utf8String(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("s", "String cannot be null");
            }

            if (s == string.Empty)
            {
                _buffer = ReadOnlySpan<byte>.Empty;
            }
            else
            {
                _buffer = new ReadOnlySpan<byte>(GetUtf8BytesFromString(s));
            }
        }

        /// <summary>
        /// This constructor is for use by the compiler.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Utf8String(RuntimeFieldHandle utf8Data, int length) : this(CreateArrayFromFieldHandle(utf8Data, length))
        {
        }

        public static explicit operator Utf8String(ArraySegment<byte> utf8Bytes)
        {
            return new Utf8String(utf8Bytes);
        }

        static byte[] CreateArrayFromFieldHandle(RuntimeFieldHandle utf8Data, int length)
        {
            var array = new byte[length];
            RuntimeHelpers.InitializeArray(array, utf8Data);
            return array;
        }

        public static Utf8String Empty { get { return s_empty; } }

        /// <summary>
        /// Returns length of the string in UTF-8 code units (bytes)
        /// </summary>
        public int Length
        {
            get
            {
                return _buffer.Length;
            }
        }

        public Enumerator GetEnumerator()
        {
            return new Enumerator(_buffer);
        }

        public CodePointEnumerable CodePoints
        {
            get
            {
                return new CodePointEnumerable(_buffer);
            }
        }

        public byte this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                // there is no need to check the boundaries -> Span is going to do this on it's own
                return (byte)_buffer[i];
            }
        }

        public static implicit operator ReadOnlySpan<byte>(Utf8String utf8)
        {
            return utf8.Bytes;
        }

        public static explicit operator Utf8String(string s)
        {
            return new Utf8String(s);
        }

        public static explicit operator string(Utf8String s)
        {
            return s.ToString();
        }

        public ReadOnlySpan<byte> Bytes => _buffer;

        public override string ToString()
        {
            var status = Encoders.Utf8.ToUtf16Length(this.Bytes, out int needed);
            if (status != Buffers.TransformationStatus.Done)
                return string.Empty;

            // UTF-16 is 2 bytes per char
            var chars = new char[needed >> 1];
            var utf16 = new Span<char>(chars).AsBytes();
            status = Encoders.Utf8.ToUtf16(this.Bytes, utf16, out int consumed, out int written);
            if (status != Buffers.TransformationStatus.Done)
                return string.Empty;

            return new string(chars);
        }

        public bool ReferenceEquals(Utf8String other)
        {
            return _buffer == other._buffer;
        }

        public bool Equals(Utf8String other)
        {
            return _buffer.SequenceEqual(other._buffer);
        }

        public bool Equals(string other)
        {
            CodePointEnumerator thisEnumerator = GetCodePointEnumerator();
            Utf16LittleEndianCodePointEnumerator otherEnumerator = new Utf16LittleEndianCodePointEnumerator(other);

            while (true)
            {
                bool hasNext = thisEnumerator.MoveNext();
                if (hasNext != otherEnumerator.MoveNext())
                {
                    return false;
                }

                if (!hasNext)
                {
                    return true;
                }

                if (thisEnumerator.Current != otherEnumerator.Current)
                {
                    return false;
                }
            }
        }

        public static bool operator ==(Utf8String left, Utf8String right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Utf8String left, Utf8String right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(Utf8String left, string right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Utf8String left, string right)
        {
            return !left.Equals(right);
        }

        public static bool operator ==(string left, Utf8String right)
        {
            return right.Equals(left);
        }

        public static bool operator !=(string left, Utf8String right)
        {
            return !right.Equals(left);
        }

        public int CompareTo(Utf8String other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(string other)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index">Index in UTF-8 code units (bytes)</param>
        /// <returns>Length in UTF-8 code units (bytes)</returns>
        public Utf8String Substring(int index)
        {
            return Substring(index, Length - index);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="index">Index in UTF-8 code units (bytes)</param>
        /// <returns>Length in UTF-8 code units (bytes)</returns>
        public Utf8String Substring(int index, int length)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            if (length < 0)
            {
                // TODO: Should we support that?
                throw new ArgumentOutOfRangeException("length");
            }

            if (length == 0)
            {
                return Empty;
            }

            if (length == Length)
            {
                return this;
            }

            if (index + length > Length)
            {
                // TODO: Should this be index or length?
                throw new ArgumentOutOfRangeException("index");
            }

            return new Utf8String(_buffer.Slice(index, length));
        }

        // TODO: Naive algorithm, reimplement faster
        // TODO: Should this be public?
        public int IndexOf(Utf8String value)
        {
            if (value.Length == 0)
            {
                // TODO: Is this the right answer?
                // TODO: Does this even make sense?
                return 0;
            }

            if (Length == 0)
            {
                return StringNotFound;
            }

            Utf8String restOfTheString = this;
            for (int i = 0; restOfTheString.Length <= Length; restOfTheString = Substring(++i))
            {
                int pos = restOfTheString.IndexOf(value[0]);
                if (pos == StringNotFound)
                {
                    return StringNotFound;
                }
                i += pos;
                if (IsSubstringAt(i, value))
                {
                    return i;
                }
            }

            return StringNotFound;
        }

        // TODO: Should this be public?
        public int IndexOf(byte codeUnit)
        {
            // TODO: _buffer.IndexOf(codeUnit.Value); when Span has it

            for (int i = 0; i < Length; i++)
            {
                if (codeUnit == this[i])
                {
                    return i;
                }
            }

            return StringNotFound;
        }

        // TODO: Should this be public?
        public int IndexOf(uint codePoint)
        {
            CodePointEnumerator it = GetCodePointEnumerator();
            while (it.MoveNext())
            {
                if (it.Current == codePoint)
                {
                    return it.PositionInCodeUnits;
                }
            }

            return StringNotFound;
        }

        // TODO: Re-evaluate all Substring family methods and check their parameters name
        public bool TrySubstringFrom(Utf8String value, out Utf8String result)
        {
            int idx = IndexOf(value);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(idx);
            return true;
        }

        public bool TrySubstringFrom(byte codeUnit, out Utf8String result)
        {
            int idx = IndexOf(codeUnit);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(idx);
            return true;
        }

        public bool TrySubstringFrom(uint codePoint, out Utf8String result)
        {
            int idx = IndexOf(codePoint);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(idx);
            return true;
        }

        public bool TrySubstringTo(Utf8String value, out Utf8String result)
        {
            int idx = IndexOf(value);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(0, idx);
            return true;
        }

        public bool TrySubstringTo(byte codeUnit, out Utf8String result)
        {
            int idx = IndexOf(codeUnit);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(0, idx);
            return true;
        }

        public bool TrySubstringTo(uint codePoint, out Utf8String result)
        {
            int idx = IndexOf(codePoint);

            if (idx == StringNotFound)
            {
                result = default;
                return false;
            }

            result = Substring(0, idx);
            return true;
        }

        public bool IsSubstringAt(int index, Utf8String s)
        {
            if (index < 0 || index + s.Length > Length)
            {
                return false;
            }

            return Substring(index, s.Length).Equals(s);
        }

        public void CopyTo(Span<byte> buffer)
        {
            _buffer.CopyTo(buffer);
        }

        public void CopyTo(byte[] buffer)
        {
            _buffer.CopyTo(buffer);
        }

        // TODO: write better hashing function
        // TODO: span.GetHashCode() + some constant?
        public override int GetHashCode()
        {
            unchecked
            {
                if (Length <= 4)
                {
                    int hash = Length;
                    for (int i = 0; i < Length; i++)
                    {
                        hash <<= 8;
                        hash ^= (byte)this[i];
                    }
                    return hash;
                }
                else
                {
                    int hash = Length;
                    hash ^= (byte)this[0];
                    hash <<= 8;
                    hash ^= (byte)this[1];
                    hash <<= 8;
                    hash ^= (byte)this[Length - 2];
                    hash <<= 8;
                    hash ^= (byte)this[Length - 1];
                    return hash;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Utf8String)
            {
                return Equals((Utf8String)obj);
            }
            if (obj is string)
            {
                return Equals((string)obj);
            }

            return false;
        }

        private CodePointEnumerator GetCodePointEnumerator()
        {
            return new CodePointEnumerator(_buffer);
        }

        public bool StartsWith(uint codePoint)
        {
            CodePointEnumerator e = GetCodePointEnumerator();
            if (!e.MoveNext())
            {
                return false;
            }

            return e.Current == codePoint;
        }

        public bool StartsWith(byte codeUnit)
        {
            if (Length == 0)
            {
                return false;
            }

            return this[0] == codeUnit;
        }

        public bool StartsWith(Utf8String value)
        {
            if(value.Length > this.Length)
            {
                return false;
            }

            return this.Substring(0, value.Length).Equals(value);
        }

        public bool EndsWith(byte codeUnit)
        {
            if (Length == 0)
            {
                return false;
            }

            return this[Length - 1] == codeUnit;
        }

        public bool EndsWith(Utf8String value)
        {
            if (Length < value.Length)
            {
                return false;
            }

            return this.Substring(Length - value.Length, value.Length).Equals(value);
        }

        public bool EndsWith(uint codePoint)
        {
            throw new NotImplementedException();
        }

        private static int GetUtf8LengthInBytes(IEnumerable<uint> codePoints)
        {
            int len = 0;
            foreach (var codePoint in codePoints)
            {
                len += Utf8Helper.GetNumberOfEncodedBytes(codePoint);
            }

            return len;
        }

        // TODO: This should return Utf16CodeUnits which should wrap byte[]/Span<byte>, same for other encoders
        private static byte[] GetUtf8BytesFromString(string str)
        {
            var utf16 = str.AsSpan().AsBytes();
            var status = Encoders.Utf16.ToUtf8Length(utf16, out int needed);
            if (status != Buffers.TransformationStatus.Done)
                return null;

            var utf8 = new byte[needed];
            status = Encoders.Utf16.ToUtf8(utf16, utf8, out int consumed, out int written);
            if (status != Buffers.TransformationStatus.Done)
                // This shouldn't happen...
                return null;

            return utf8;
        }

        public Utf8String TrimStart()
        {
            CodePointEnumerator it = GetCodePointEnumerator();
            while (it.MoveNext() && Utf8Helper.IsWhitespace(it.Current))
            {
            }

            return Substring(it.PositionInCodeUnits);
        }

        public Utf8String TrimStart(uint[] trimCodePoints)
        {
            throw new NotImplementedException();
        }

        public Utf8String TrimStart(byte[] trimCodeUnits)
        {
            throw new NotImplementedException();
        }

        public Utf8String TrimEnd()
        {
            CodePointReverseEnumerator it = CodePoints.GetReverseEnumerator();
            while (it.MoveNext() && Utf8Helper.IsWhitespace(it.Current))
            {
            }

            return Substring(0, it.PositionInCodeUnits);
        }

        public Utf8String TrimEnd(uint[] trimCodePoints)
        {
            throw new NotImplementedException();
        }

        public Utf8String TrimEnd(byte[] trimCodeUnits)
        {
            throw new NotImplementedException();
        }

        public Utf8String Trim()
        {
            return TrimStart().TrimEnd();
        }

        public Utf8String Trim(uint[] trimCodePoints)
        {
            throw new NotImplementedException();
        }

        public Utf8String Trim(byte[] trimCodeUnits)
        {
            throw new NotImplementedException();
        }

        // TODO: Name TBD, CopyArray? GetBytes?
        public byte[] CopyBytes()
        {
            return _buffer.ToArray();
        }

        public byte[] CopyCodeUnits()
        {
            throw new NotImplementedException();
        }

        public static bool IsWhiteSpace(byte codePoint)
        {
            return codePoint == ' ' || codePoint == '\n' || codePoint == '\r' || codePoint == '\t';
        }
    }
}
