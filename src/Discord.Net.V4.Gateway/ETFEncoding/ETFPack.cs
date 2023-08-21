using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Discord.Gateway
{
    internal sealed class ETFPack
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

        #region Decoding

        public static T ReadObject<T>(ref ETFPackDecoder decoder)
        {
            decoder.Expect(FormatType.MAP_EXT);

            var value = _decoders.GetOrAdd(typeof(T), t => new StructureDecoder(t))
                .Decode(ref decoder);

            if (value is not T t)
                throw new InvalidCastException($"Expected type {typeof(T)} but got {value?.GetType().Name ?? "null"}");

            return t;
        }

        #endregion

        #region Encoding
        public static ReadOnlyMemory<byte> Encode<T>(in T? value)
        {
            if(value is null)
            {
                return ETFPackEncoder.NIL_BYTES;
            }

            nuint bufSize = 1024;

            if(!RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                // at max 5 extra bytes for the format type and length.
                bufSize = (nuint)Marshal.SizeOf<T>() + 5;
            }

            var encoder = new ETFPackEncoder(bufSize);

            try
            {
                Encode(in value, ref encoder);

                return encoder.GetBytes();
            }
            finally
            {
                encoder.Dispose();
            }
        }

        private static void Encode<T>(in T value, scoped ref ETFPackEncoder encoder)
        {
            switch (value)
            {
                case byte b:
                    encoder.Write(b);
                    break;
                case sbyte sb:
                    encoder.Write(sb); // as int
                    break;
                case short s:
                    encoder.Write(s);
                    break;
                case ushort us:
                    encoder.Write(us); // as int
                    break;
                case int i:
                    encoder.Write(i);
                    break;
                case uint ui:
                    encoder.Write(ui); // as long
                    break;
                case long l:
                    encoder.Write(l);
                    break;
                case ulong ul:
                    encoder.Write(ul);
                    break;
                case double d:
                    encoder.Write(d);
                    break;
                case float f:
                    encoder.Write(f); // as double
                    break;
                case bool b:
                    encoder.Write(b);
                    break;
                case string s:
                    encoder.Write(s);
                    break;
                case Array array:
                    if (array.Length == 0)
                    {
                        encoder.WriteNilExt();
                        break;
                    }

                    encoder.WriteListHeader(array.Length);
                    EncodeArray(array, ref encoder);
                    break;
                case IDictionary dict:
                    encoder.WriteMapHeader(dict.Count);
                    foreach (DictionaryEntry kvp in dict)
                    {
                        Encode(kvp.Key, ref encoder);
                        Encode(kvp.Value, ref encoder);
                    }
                    break;
                case IEnumerable enumerable:
                    List<object> items = new();
                    foreach (var item in enumerable)
                        items.Add(item);

                    if(items.Count == 0)
                    {
                        encoder.WriteNilExt();
                        return;
                    }
                    encoder.WriteListHeader(items.Count);
                    EncodeArray(items.ToArray(), ref encoder);
                    break;
                
                default:
                    if(typeof(T).IsValueType)
                        EncodeStructure<T>(in value, ref encoder); 
                    break;


            }
        }

        private static void EncodeStructure<T>(in T value, ref ETFPackEncoder encoder)
        {
            var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);
            encoder.WriteMapHeader(fields.Length);
            for(var i = 0; i != fields.Length; i++)
            {
                Encode(fields[i].GetValue(value), ref encoder);
            }
        }

        private static void EncodeArray(Array array, ref ETFPackEncoder encoder)
        {
            if (array.GetType().GetElementType()!.IsValueType)
            {
                EncodeArrayFast(array, ref encoder);
                return;
            }

            for(var i = 0; i != array.Length; i++)
            {
                Encode(array.GetValue(i), ref encoder);
            }
        }

        private static void EncodeArrayFast(Array array, ref ETFPackEncoder encoder)
        {
            ref var reference = ref MemoryMarshal.GetArrayDataReference(array);

            for(var i = 0; i != array.Length; i++)
            {
                Encode(in Unsafe.As<byte, object>(ref Unsafe.Add(ref reference, (nuint)i)), ref encoder);
            }

        }
        #endregion
    }
}

