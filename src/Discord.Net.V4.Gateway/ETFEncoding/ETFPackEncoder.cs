using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.Gateway
{
    // TODO: could be useful and preformant to pool the allocs and reuse them.
    internal unsafe ref struct ETFPackEncoder
    {
        public static readonly byte[] NIL_BYTES =
        {
            (byte)ETFPack.FormatType.SMALL_ATOM_EXT,
            0x03,
            0x6e, 0x69, 0x6c
        };

        public static readonly byte[] TRUE_BYTES =
        {
            (byte)ETFPack.FormatType.SMALL_ATOM_EXT,
            0x04,
            0x74, 0x72, 0x75, 0x65
        };

        public static readonly byte[] FALSE_BYTES =
        {
            (byte)ETFPack.FormatType.SMALL_ATOM_EXT,
            0x05,
            0x66, 0x61, 0x6c, 0x73, 0x65
        };


        private Span<byte> _buffer;
        private byte* _ptr;
        private int _pos;

        public ETFPackEncoder(nuint sz)
        {
            _ptr = (byte*)NativeMemory.Alloc(sz);
            _buffer = new Span<byte>(_ptr, checked((int)sz));
        }

        public readonly ReadOnlyMemory<byte> GetBytes()
        {
            var buff = new byte[_pos];
            Unsafe.CopyBlockUnaligned(ref buff[0], ref Unsafe.AsRef<byte>(_ptr), (uint)_pos);
            return buff;
        }

        private void Resize(int t)
        {
            var newSize = (_buffer.Length + t) * 2;

            var newBuff = (byte*)NativeMemory.Alloc((nuint)newSize);

            Unsafe.CopyBlockUnaligned(ref Unsafe.AsRef<byte>(newBuff), ref _buffer[0], (uint)_buffer.Length);

            _buffer = new(newBuff, newSize);

            NativeMemory.Free(_ptr);

            _ptr = newBuff;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(ref byte b, int size)
            => Append(Unsafe.AsPointer(ref b), size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Append(void* ptr, int size)
        {
            if(size + _pos > _buffer.Length)
            {
                Resize(size);
            }

            Unsafe.CopyBlockUnaligned(_ptr + _pos, ptr, (uint)size);
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

            Append(Unsafe.AsPointer(ref value), sizeof(T));
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
        private void WriteType(ETFPack.FormatType type)
            => WriteByte((byte)type);


        public void WriteVersion()
            => WriteByte(ETFPack.FORMAT_VERSION);

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
            WriteType(ETFPack.FormatType.SMALL_INTEGER_EXT);
            WriteUnmanaged(ref b);
        }

        public void Write(int i)
        {
            WriteType(ETFPack.FormatType.INTEGER_EXT);
            BinaryUtils.CorrectEndianness(ref i);
            WriteUnmanaged(ref i);
        }

        public void Write(long l)
        {
            WriteType(ETFPack.FormatType.SMALL_BIG_EXT);
            WriteByte(0x08); // byte count
            WriteByte((byte)(l < 0 ? 1 : 0)); // sign
            BinaryUtils.CorrectEndianness(ref l);
            WriteUnmanaged(ref l);
        }

        public void Write(ulong l)
        {
            WriteType(ETFPack.FormatType.SMALL_BIG_EXT);
            WriteByte(0x08); // byte count
            WriteByte(0); // sign
            BinaryUtils.CorrectEndianness(ref l);
            WriteUnmanaged(ref l);
        }

        public void Write(double d)
        {
            WriteType(ETFPack.FormatType.NEW_FLOAT_EXT);
            BinaryUtils.CorrectEndianness(ref d);
            WriteUnmanaged(ref d);
        }

        public void WriteAtom(Span<byte> bytes)
        {
            if(bytes.Length < 255)
            {
                WriteType(ETFPack.FormatType.SMALL_ATOM_EXT);
                WriteByte(checked((byte)bytes.Length));
                Append(ref bytes[0], bytes.Length);
                return;
            }

            if (bytes.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a buffer with a size greater than {0xFFFF} bytes");

            WriteType(ETFPack.FormatType.ATOM_EXT);

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
                WriteType(ETFPack.FormatType.SMALL_ATOM_UTF8_EXT);
                WriteByte(checked((byte)bytes.Length));
                Append(ref bytes[0], bytes.Length);
                return;
            }

            if (bytes.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a buffer with a size greater than {0xFFFF} bytes");

            WriteType(ETFPack.FormatType.ATOM_UTF8_EXT);

            var sz = (ushort)bytes.Length;

            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);
            Append(ref bytes[0], bytes.Length);
        }

        public void Write(Span<byte> bytes)
        {
            var sz = bytes.Length;
            WriteType(ETFPack.FormatType.BINARY_EXT);
            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);

            Append(ref bytes[0], bytes.Length);
        }

        public void Write(string str)
        {
            var buff = Encoding.UTF8.GetBytes(str);

            if (buff.Length > 0xFFFF)
                throw new ArgumentOutOfRangeException($"Cannot write a string with a size greater than {0xFFFF}");


            WriteType(ETFPack.FormatType.STRING_EXT);
            var sz = (ushort)buff.Length;
            BinaryUtils.CorrectEndianness(ref sz);
            WriteUnmanaged(ref sz);
            Append(ref buff[0], buff.Length);
    
        }

        public void WriteTupleHeader(int size)
        {
            if(size < 255)
            {
                WriteType(ETFPack.FormatType.SMALL_TUPLE_EXT);
                WriteByte(checked((byte)size));
                return;
            }

            WriteType(ETFPack.FormatType.LARGE_TUPLE_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public void WriteNilExt()
            => WriteType(ETFPack.FormatType.NIL_EXT);

        public void WriteListHeader(int size)
        {
            WriteType(ETFPack.FormatType.LIST_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public void WriteMapHeader(int size)
        {
            WriteType(ETFPack.FormatType.MAP_EXT);
            BinaryUtils.CorrectEndianness(ref size);
            WriteUnmanaged(ref size);
        }

        public readonly void Dispose()
        {
            NativeMemory.Free(_ptr);
        }
    }
}

