using System;
using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using DiscordGatewayTest.ETF.Models;

namespace Discord.Gateway
{
    internal readonly unsafe struct ETFDecoder
    {
        public const byte FLOAT_LENGTH = 31;

        private readonly Stream _stream;

        public ETFDecoder(Stream stream)
        {
            _stream = stream;
        }

        public ETF.FormatType ReadType()
            => (ETF.FormatType)ReadUnmanaged<byte>();

        public T ReadUnmanaged<T>()
            where T : unmanaged
        {
            T value;
            ReadUnmanaged(&value);
            return value;
        }

        public void ReadUnmanaged<T>(T* v)
            where T : unmanaged
        {
            ReadInto(v, sizeof(T));
            ref var value = ref Unsafe.AsRef<T>(v);
            BinaryUtils.CorrectEndianness(ref value);
        }

        public void ReadInto(scoped Span<byte> bytes)
            => _stream.Read(bytes);

        public void ReadInto(void* ptr, int length)
        {
            ReadInto(new Span<byte>(ptr, length));
        }

        public void Expect(ETF.FormatType expected)
        {
            var actual = ReadType();
            if (actual != expected)
                throw new InvalidDataException($"Expected ERL format type {expected} but got {actual}");
        }

        public T ReadScalar<T>(ETF.FormatType type)
            where T : unmanaged
        {
            Scalar<T> scalar;
            ReadUnmanaged(&scalar);

            if (scalar.Type != type)
                throw new InvalidDataException($"Expected ERL format type {type} but got {scalar.Type}");

            return scalar.Value;
        }

        public string ReadString(int length, bool utf8)
        {
            var buffer = stackalloc byte[length];
            ReadInto(buffer, length);
            return utf8
                ? Encoding.UTF8.GetString(buffer, length)
                : Encoding.ASCII.GetString(buffer, length);
        }

        public byte ReadByte()
            => ReadUnmanaged<byte>();

        public ushort ReadUShort()
            => ReadUnmanaged<ushort>();

        public int ReadInt()
            => ReadUnmanaged<int>();

        public float? ReadFloat()
        {
            var floatStr = ReadString(FLOAT_LENGTH, false);

            if (floatStr is null)
                return null;

            if (float.TryParse(floatStr, out var f))
                return f;

            return null;
        }

        public double ReadDouble()
            => ReadUnmanaged<double>();

        public string ReadRawSmallAtom(bool utf8)
        {
            var length = ReadUnmanaged<byte>();
            return ReadString(length, utf8);
        }

        public string ReadRawAtom(bool utf8)
        {
            var length = ReadUnmanaged<ushort>();
            return ReadString(length, utf8);
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

        private BigInteger ReadBigInt(int size)
        {
            Span<byte> bytes = stackalloc byte[size + 1]; // for sign byte
            ReadInto(bytes);

            // fix sign bit
            bytes[0] = bytes[0] == 1 ? (byte)0x80 : (byte)0;

            return new BigInteger(bytes);
        }

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

        private object?[] ReadArray(int length)
        {
            var array = new object?[length];

            for(var i = 0; i != length; i++)
            {
                array[i] = Read();
            }

            return array;
        }

        private object? ProcessAtom(string atom)
            => atom switch
            {
                "nil" => null,
                "null" => null,
                "true" => true,
                "false" => false,
                var u => u
            };

        public object? Read()
        {
            var type = ReadType();

            switch (type)
            {
                case ETF.FormatType.ATOM_UTF8_EXT:
                    return ProcessAtom(ReadRawAtom(true));
                case ETF.FormatType.ATOM_EXT:
                    return ProcessAtom(ReadRawAtom(false));
                case ETF.FormatType.BINARY_EXT:
                    return ReadString(ReadInt(), false);
                case ETF.FormatType.BIT_BINARY_EXT:
                    // TODO:
                    break;
                case ETF.FormatType.COMPRESSED:
                    // TODO
                    break;
                case ETF.FormatType.EXPORT_EXT:
                    return new ETFExport(
                        Read(),
                        Read(),
                        Read()
                    );
                case ETF.FormatType.FLOAT_EXT:
                    return ReadFloat();
                case ETF.FormatType.FUN_EXT:
                    // TODO
                    break;
                case ETF.FormatType.INTEGER_EXT:
                    return ReadInt();
                case ETF.FormatType.LARGE_BIG_EXT:
                    return ReadBigInt(ReadInt());
                case ETF.FormatType.LARGE_TUPLE_EXT:
                    return ReadArray(ReadInt());
                case ETF.FormatType.LIST_EXT:
                    {
                        var arr = ReadArray(ReadInt());
                        var marker = ReadType();
                        if (marker is not ETF.FormatType.NIL_EXT)
                            throw new InvalidDataException($"Expected NIL_EXT tail marker, but got {marker}");
                        return arr;
                    }
                case ETF.FormatType.MAP_EXT:
                    {
                        var length = ReadInt();
                        var map = new Dictionary<object, object?>();

                        for (var i = 0; i != length; i++)
                        {
                            var key = Read() ?? throw new NullReferenceException("Key of map cannot be null");
                            var value = Read();

                            map.Add(key, value);
                        }

                        return new ETFObject(map);
                    }
                case ETF.FormatType.NEW_FLOAT_EXT:
                    // TODO
                    break;
                case ETF.FormatType.NEW_FUN_EXT:
                    // not supported
                    break;
                case ETF.FormatType.NEW_REFERENCE_EXT:
                    {
                        var length = ReadUShort();

                        var node = Read();
                        var creation = ReadByte();

                        if (length == 0)
                            return new ETFReference(node, creation, Array.Empty<int>());

                        var ids = new int[length];

                        for(var i = 0; i != length; i++)
                        {
                            ids[i] = ReadInt();
                        }

                        return new ETFReference(node, creation, ids);
                    }
                case ETF.FormatType.NIL_EXT:
                    return Array.Empty<object>();
                case ETF.FormatType.PID_EXT:
                    return new ETFPID(
                        Read(),
                        ReadInt(),
                        ReadInt(),
                        ReadByte()
                    );
                case ETF.FormatType.PORT_EXT:
                    return new ETFPort(
                        Read(),
                        ReadInt(),
                        ReadByte()
                    );
                case ETF.FormatType.REFERENCE_EXT:
                    {
                        var node = Read();
                        var id = ReadInt();
                        var creation = ReadByte();
                        return new ETFReference(
                            node,
                            creation,
                            new int[] { id }
                        );
                    }
                case ETF.FormatType.SMALL_ATOM_EXT:
                    return ProcessAtom(ReadRawSmallAtom(false));
                case ETF.FormatType.SMALL_ATOM_UTF8_EXT:
                    return ProcessAtom(ReadRawSmallAtom(true));
                case ETF.FormatType.SMALL_BIG_EXT:
                    return ReadBigInt(ReadByte());
                case ETF.FormatType.SMALL_INTEGER_EXT:
                    return ReadByte();
                case ETF.FormatType.SMALL_TUPLE_EXT:
                    return ReadArray(ReadByte());
                case ETF.FormatType.STRING_EXT:
                    return ReadString(ReadUShort(), false);
                default:
                    break;

            }

            return null;
        }

        private struct Scalar<T>
            where T : unmanaged
        {
            public ETF.FormatType Type;
            public T Value;
        }
    }
}

