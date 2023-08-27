using System;
using System.Buffers;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.Gateway
{
    // TODO: could be useful and preformant to pool the allocs and reuse them.
    internal unsafe ref struct ETFEncoder
    {
        public static readonly byte[] NIL_BYTES =
        {
            (byte)ETF.FormatType.SMALL_ATOM_EXT,
            0x03,
            0x6e, 0x69, 0x6c
        };

        public static readonly byte[] TRUE_BYTES =
        {
            (byte)ETF.FormatType.SMALL_ATOM_EXT,
            0x04,
            0x74, 0x72, 0x75, 0x65
        };

        public static readonly byte[] FALSE_BYTES =
        {
            (byte)ETF.FormatType.SMALL_ATOM_EXT,
            0x05,
            0x66, 0x61, 0x6c, 0x73, 0x65
        };


        private readonly ArrayPool<byte> _pool;

        private Span<byte> _buffer;
        private byte[] _rented;

        private int _pos;

        public ETFEncoder(int size, ArrayPool<byte> pool)
        {
            _rented = pool.Rent(size);
            _buffer = _rented.AsSpan();
            _pool = pool;
        }

        public readonly ReadOnlyMemory<byte> GetBytes()
        {
            var buffer = new byte[_pos];

            _buffer[.._pos].CopyTo(buffer);

            return buffer;
        }

        private void Resize(int requested)
        {
            var newBuffer = _pool.Rent((_buffer.Length + requested) * 2);

            _buffer.CopyTo(newBuffer);

            _pool.Return(_rented);
            _buffer = newBuffer.AsSpan();
            _rented = newBuffer;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(ref byte bytes, int size)
        {
            if(size + _pos > _buffer.Length)
            {
                Resize(size);
            }

            Unsafe.CopyBlockUnaligned(ref _buffer[_pos], ref bytes, (uint)size);
            _pos += size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteUnmanaged<T>(scoped ref T value)
            where T : unmanaged
        {
            if(sizeof(T) + _pos > _buffer.Length)
            {
                Resize(sizeof(T));
            }

            Append(ref Unsafe.As<T, byte>(ref value), sizeof(T));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteByte(byte b)
        {
            if (_pos == _buffer.Length)
                Resize(1);

            _buffer[_pos] = b;
            _pos++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WriteType(ETF.FormatType type)
            => WriteByte((byte)type);


        public void WriteVersion()
            => WriteByte(ETF.FORMAT_VERSION);

        public void WriteNil()
            => Append(ref NIL_BYTES[0], NIL_BYTES.Length);

        public void Write(bool v)
        {
            if (v)
                Append(ref TRUE_BYTES[0], TRUE_BYTES.Length);
            else
                Append(ref FALSE_BYTES[0], FALSE_BYTES.Length);
        }

        public void Write(byte b)
        {
            WriteType(ETF.FormatType.SMALL_INTEGER_EXT);
            WriteUnmanaged(ref b);
        }

        public void Write(int i)
        {
            WriteType(ETF.FormatType.INTEGER_EXT);
            BinaryUtils.CorrectEndianness(ref i);
            WriteUnmanaged(ref i);
        }

        public void Write(long l)
        {
            WriteType(ETF.FormatType.SMALL_BIG_EXT);
            WriteByte(0x08); // byte count
            WriteByte((byte)(l < 0 ? 1 : 0)); // sign
            BinaryUtils.CorrectEndianness(ref l);
            WriteUnmanaged(ref l);
        }

        public void Write(ulong l)
        {
            WriteType(ETF.FormatType.SMALL_BIG_EXT);
            WriteByte(0x08); // byte count
            WriteByte(0); // sign
            BinaryUtils.CorrectEndianness(ref l);
            WriteUnmanaged(ref l);
        }

        public void Write(double d)
        {
            WriteType(ETF.FormatType.NEW_FLOAT_EXT);
            BinaryUtils.CorrectEndianness(ref d);
            WriteUnmanaged(ref d);
        }

        public void WriteAtom(Span<byte> bytes)
        {
            if(bytes.Length < 255)
            {
                WriteType(ETF.FormatType.SMALL_ATOM_EXT);
                WriteByte(checked((byte)bytes.Length));
                Append(ref bytes[0], bytes.Length);
                return;
            }

            if (bytes.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a buffer with a size greater than {0xFFFF} bytes");

            WriteType(ETF.FormatType.ATOM_EXT);

            var sz = (ushort)bytes.Length;

            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);
            Append(ref bytes[0], bytes.Length);
        }

        public void WriteAtom(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            if (bytes.Length < 255)
            {
                WriteType(ETF.FormatType.SMALL_ATOM_UTF8_EXT);
                WriteByte(checked((byte)bytes.Length));
                Append(ref bytes[0], bytes.Length);
                return;
            }

            if (bytes.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a buffer with a size greater than {0xFFFF} bytes");

            WriteType(ETF.FormatType.ATOM_UTF8_EXT);

            var sz = (ushort)bytes.Length;

            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);
            Append(ref bytes[0], bytes.Length);
        }

        public void Write(Span<byte> bytes)
        {
            var sz = bytes.Length;
            WriteType(ETF.FormatType.BINARY_EXT);
            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);

            Append(ref bytes[0], bytes.Length);
        }

        public void WriteBinary(string s)
        {
            Write(Encoding.ASCII.GetBytes(s).AsSpan());
        }

        public void Write(string str)
        {
            var buff = Encoding.UTF8.GetBytes(str);

            if (buff.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a string with a size greater than {0xFFFF}");


            WriteType(ETF.FormatType.STRING_EXT);
            var sz = (ushort)buff.Length;
            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);
            Append(ref buff[0], buff.Length);
    
        }

        public void WriteTupleHeader(int size)
        {
            if(size < 255)
            {
                WriteType(ETF.FormatType.SMALL_TUPLE_EXT);
                WriteByte(checked((byte)size));
                return;
            }

            WriteType(ETF.FormatType.LARGE_TUPLE_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public void WriteNilExt()
            => WriteType(ETF.FormatType.NIL_EXT);

        public void WriteListHeader(int size)
        {
            WriteType(ETF.FormatType.LIST_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public void WriteMapHeader(int size)
        {
            WriteType(ETF.FormatType.MAP_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public void Write(object? value)
        {
            switch(value)
            {
                case byte b:
                    Write(b);
                    break;
                case sbyte sb:
                    Write(sb); // as int
                    break;
                case short s:
                    Write(s);
                    break;
                case ushort us:
                    Write(us); // as int
                    break;
                case int i:
                    Write(i);
                    break;
                case uint ui:
                    Write(ui); // as long
                    break;
                case long l:
                    Write(l);
                    break;
                case ulong ul:
                    Write(ul);
                    break;
                case double d:
                    Write(d);
                    break;
                case float f:
                    Write(f); // as double
                    break;
                case bool b:
                    Write(b);
                    break;
                case string s:
                    WriteBinary(s);
                    break;
                case Array array:
                    if (array.Length == 0)
                    {
                        WriteNilExt();
                        break;
                    }

                    WriteListHeader(array.Length);
                    for(int i = 0; i != array.Length; i++)
                    {
                        Write(array.GetValue(i));
                    }
                    break;
                case IDictionary dict:
                    WriteMapHeader(dict.Count);
                    foreach (DictionaryEntry kvp in dict)
                    {
                        Write(kvp.Key);
                        Write(kvp.Value);
                    }
                    break;
                case IEnumerable enumerable:
                    List<object> items = new();
                    foreach (var item in enumerable)
                        items.Add(item);

                    if (items.Count == 0)
                    {
                        WriteNilExt();
                        return;
                    }
                    WriteListHeader(items.Count);
                    for (int i = 0; i != items.Count; i++)
                    {
                        Write(items[i]);
                    }
                    WriteNilExt();
                    break;
                case Enum e:
                    value = Convert.ChangeType(value, Enum.GetUnderlyingType(e.GetType()));
                    Write(value);
                    break;
                case object obj when obj.GetType().Assembly == typeof(ETFEncoder).Assembly:
                    var encoder = ETF.GetOrCreateEncoder(obj.GetType());
                    
                    encoder.Encode(obj, ref this);
                    break;

            }
        }

        public readonly void Dispose()
        {
            _pool.Return(_rented);
        }
    }
}

