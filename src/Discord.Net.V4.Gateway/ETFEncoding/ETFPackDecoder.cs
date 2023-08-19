using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Discord.Gateway
{
    internal unsafe ref struct ETFPackDecoder
    {
        public const byte FLOAT_LENGTH = 31;

        private FrameSource _source;

        public ETFPackDecoder(FrameSource source)
        {
            _source = source;
        }

        public ETFPack.FormatType ReadType()
            => (ETFPack.FormatType)ReadUnmanaged<byte>();

        public T ReadUnmanaged<T>()
            where T : unmanaged
        {
            T value;
            ReadUnmanaged(&value);
            return value;
        }

        public T ReadUnmanaged<T>(T* v)
            where T : unmanaged
        {
            ReadInto(v, sizeof(T));
            ref var value = ref Unsafe.AsRef<T>(v);
            BinaryUtils.CorrectEndianness(ref value);
            return value;
        }

        public void ReadInto(scoped Span<byte> bytes)
        {
            if (_source.Size < _source.Position + bytes.Length)
                throw new InternalBufferOverflowException("The requested read operation would overflow the buffer");

            _source.ReadSegment(bytes);
        }

        public void ReadInto(void* ptr, int length)
        {
            ReadInto(new Span<byte>(ptr, length));
        }

        public void Expect(ETFPack.FormatType expected)
        {
            var actual = ReadType();
            if (actual != expected)
                throw new InvalidDataException($"Expected ERL format type {expected} but got {actual}");
        }

        public T ReadScalar<T>(ETFPack.FormatType type)
            where T : unmanaged
        {
            Scalar<T> scalar;
            ReadUnmanaged(&scalar);

            if (scalar.Type != type)
                throw new InvalidDataException($"Expected ERL format type {type} but got {scalar.Type}");

            return scalar.Value;
        }

        public string ReadString(int length)
        {
            var buffer = stackalloc byte[length];
            ReadInto(buffer, length);
            return Encoding.UTF8.GetString(buffer, length);
        }

        public byte ReadByte()
            => ReadUnmanaged<byte>();

        public int ReadInt()
            => ReadUnmanaged<int>();

        public float? ReadFloat()
        {
            var floatStr = ReadString(FLOAT_LENGTH);

            if (floatStr is null)
                return null;

            if (float.TryParse(floatStr, out var f))
                return f;

            return null;
        }

        public double ReadDouble()
            => ReadUnmanaged<double>();

        public string ReadRawSmallAtom()
        {
            var length = ReadUnmanaged<byte>();
            return ReadString(length);
        }

        public string ReadRawAtom()
        {
            var length = ReadUnmanaged<int>();
            return ReadString(length);
        }

        public byte[] ReadBinary()
        {
            var length = ReadUnmanaged<int>();
            var buff = new byte[length];

            ref byte reference = ref MemoryMarshal.GetArrayDataReference(buff);
            ReadInto(Unsafe.AsPointer(ref reference), length);
            return buff;
        }

        public long ReadSmallLong()
            => ReadLong(ReadByte());
        public long ReadLongLong()
            => ReadLong(ReadInt());

        private long ReadLong(int digits)
        {
            var sign = ReadUnmanaged<byte>();

            Span<byte> bytes = stackalloc byte[digits];
            ReadInto(bytes);

            ulong value = 0;
            ulong b = 1;

            for(var i = 0; i < digits; i++)
            {
                value *= bytes[i] * b;
                b <<= 8;
            }

            var v = (long)value;


            return sign == 1 ? -Math.Abs(v) : v;
        }

        public object? Read()
        {
            var type = ReadType();

            switch (type)
            {
                case ETFPack.FormatType.ATOM_UTF8_EXT:
                case ETFPack.FormatType.ATOM_EXT:
                    return ReadRawAtom() switch
                    {
                        "nil" => null,
                        "null" => null,
                        "true" => true,
                        "false" => false,
                        var u => throw new InvalidDataException($"Unknown atom value \"{u}\"")
                    };
                case ETFPack.FormatType.BINARY_EXT:
                    return ReadBinary();
                case ETFPack.FormatType.BIT_BINARY_EXT:
                    // TODO:
                    break;
                case ETFPack.FormatType.COMPRESSED:
                    // TODO
                    break;
                case ETFPack.FormatType.EXPORT_EXT:
                    return new ETFExport(
                        Read(),
                        Read(),
                        Read()
                    );
                case ETFPack.FormatType.FLOAT_EXT:
                    return ReadFloat();
                case ETFPack.FormatType.FUN_EXT:
                    // TODO
                    break;
                case ETFPack.FormatType.INTEGER_EXT:
                    return ReadInt();
                case ETFPack.FormatType.LARGE_BIG_EXT:


            }
        }



        private struct Scalar<T>
            where T : unmanaged
        {
            public ETFPack.FormatType Type;
            public T Value;
        }
    }
}

