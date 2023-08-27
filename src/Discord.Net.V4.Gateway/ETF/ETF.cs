using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.Gateway
{
    internal sealed class ETF
    {
        public enum FormatType : byte
        {
            NEW_FLOAT_EXT = 70,
            BIT_BINARY_EXT = 77,
            COMPRESSED = 80,
            SMALL_INTEGER_EXT = 97,
            INTEGER_EXT = 98,
            FLOAT_EXT = 99,
            ATOM_EXT = 100,
            REFERENCE_EXT = 101,
            PORT_EXT = 102,
            PID_EXT = 103,
            SMALL_TUPLE_EXT = 104,
            LARGE_TUPLE_EXT = 105,
            NIL_EXT = 106,
            STRING_EXT = 107,
            LIST_EXT = 108,
            BINARY_EXT = 109,
            SMALL_BIG_EXT = 110,
            LARGE_BIG_EXT = 111,
            NEW_FUN_EXT = 112,
            EXPORT_EXT = 113,
            NEW_REFERENCE_EXT = 114,
            SMALL_ATOM_EXT = 115,
            MAP_EXT = 116,
            FUN_EXT = 117,
            ATOM_UTF8_EXT = 118,
            SMALL_ATOM_UTF8_EXT = 119
        }

        public const byte FORMAT_VERSION = 131;

        private static readonly ConcurrentDictionary<Type, StructureDecoder> _decoders = new();
        private static readonly ConcurrentDictionary<Type, StructureEncoder> _encoders = new();

        #region Decoding
        public static StructureDecoder GetOrCreateDecoder(Type type)
            => _decoders.GetOrAdd(type, t => new StructureDecoder(t));

        public static T DecodeObject<T>(Stream stream)
        {
            var decoder = new ETFDecoder(stream);

            decoder.Expect(FormatType.MAP_EXT);

            var value = _decoders.GetOrAdd(typeof(T), t => new StructureDecoder(t))
                .Decode(ref decoder);

            if (value is not T t)
                throw new InvalidCastException($"Expected type {typeof(T)} but got {value?.GetType().Name ?? "null"}");

            return t;
        }

        #endregion

        #region Encoding
        public static StructureEncoder GetOrCreateEncoder(Type type)
            => _encoders.GetOrAdd(type, t => new StructureEncoder(t));

        public static ReadOnlyMemory<byte> EncodeObject<T>(in T? value, ArrayPool<byte> pool)
        {
            if(value is null)
            {
                return ETFEncoder.NIL_BYTES;
            }

            using var encoder = new ETFEncoder(1024, pool);

            encoder.WriteVersion();

            encoder.Write(value);

            return encoder.GetBytes();
        }

        #endregion
    }
}

