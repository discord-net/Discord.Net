using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Utf8;

namespace Discord.Serialization
{
    internal static class EnumMap
    {
        public static EnumMap<T> For<T>() where T : struct => EnumMap<T>.Instance;
    }

    public class EnumMap<T>
        where T : struct
    {
        public static readonly EnumMap<T> Instance = new EnumMap<T>();

        private readonly Dictionary<string, T> _keyToValue;
        private readonly BufferDictionary<T> _utf8KeyToValue;
        private readonly Dictionary<long, T> _intToValue;

        private readonly Dictionary<T, string> _valueToKey;
        private readonly Dictionary<T, long> _valueToInt;

        public EnumMap()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (!typeInfo.IsEnum)
                throw new InvalidOperationException($"{typeInfo.Name} is not an Enum");

            _keyToValue = new Dictionary<string, T>();
            _utf8KeyToValue = new BufferDictionary<T>();
            _intToValue = new Dictionary<long, T>();

            _valueToKey = new Dictionary<T, string>();
            _valueToInt = new Dictionary<T, long>();

            foreach (T val in Enum.GetValues(typeof(T)).OfType<T>())
            {
                var fieldInfo = typeInfo.GetDeclaredField(Enum.GetName(typeof(T), val));
                var attr = fieldInfo.GetCustomAttribute<ModelEnumValueAttribute>();
                if (attr != null)
                {
                    string key = attr.Key;
                    var keyBuffer = new ReadOnlyBuffer<byte>(new Utf8String(attr.Key).Bytes.ToArray());
                    if (attr.Type != EnumValueType.WriteOnly)
                    {
                        _keyToValue.Add(key, val);
                        _utf8KeyToValue.Add(keyBuffer, val);
                    }
                    if (attr.Type != EnumValueType.ReadOnly)
                        _valueToKey.Add(val, key);
                }
                
                var underlyingType = Enum.GetUnderlyingType(typeof(T));
                long baseVal;
                if (underlyingType == typeof(sbyte))
                    baseVal = (sbyte)(ValueType)val;
                else if (underlyingType == typeof(short))
                    baseVal = (short)(ValueType)val;
                else if (underlyingType == typeof(int))
                    baseVal = (int)(ValueType)val;
                else if (underlyingType == typeof(long))
                    baseVal = (long)(ValueType)val;
                else if (underlyingType == typeof(byte))
                    baseVal = (byte)(ValueType)val;
                else if (underlyingType == typeof(ushort))
                    baseVal = (ushort)(ValueType)val;
                else if (underlyingType == typeof(uint))
                    baseVal = (uint)(ValueType)val;
                else if (underlyingType == typeof(ulong))
                    baseVal = (long)(ulong)(ValueType)val;
                else
                    throw new SerializationException($"Unsupported underlying enum type: {underlyingType.Name}");
                
                _intToValue.Add(baseVal, val);
                _valueToInt.Add(val, baseVal);
            }
        }
        
        public T FromKey(ReadOnlyBuffer<byte> key)
        {
            if (_utf8KeyToValue.TryGetValue(key, out var value))
                return value;
            throw new SerializationException($"Unknown enum key: {new Utf8String(key.Span).ToString()}");
        }
        public T FromKey(ReadOnlySpan<byte> key)
        {
            if (_utf8KeyToValue.TryGetValue(key, out var value))
                return value;
            throw new SerializationException($"Unknown enum key: {new Utf8String(key).ToString()}");
        }
        public string ToUtf16Key(T value)
        {
            if (_valueToKey.TryGetValue(value, out var key))
                return key;
            throw new SerializationException($"Unknown enum value: {value}");
        }

        public T FromInt64(ReadOnlyBuffer<byte> intBuffer)
            => FromInt64(intBuffer.Span);
        public T FromInt64(ReadOnlySpan<byte> intBuffer)
        {
            long intValue = intBuffer.ParseInt64();
            if (_intToValue.TryGetValue(intValue, out var value))
                return value;
            throw new SerializationException($"Unknown enum value: {intValue}");
        }
        public long ToInt64(T value)
        {
            if (_valueToInt.TryGetValue(value, out var intValue))
                return intValue;
            throw new SerializationException($"Unknown enum value: {value}");
        }

        public T FromUInt64(ReadOnlyBuffer<byte> intBuffer)
            => FromUInt64(intBuffer.Span);
        public T FromUInt64(ReadOnlySpan<byte> intBuffer)
        {
            ulong intValue = intBuffer.ParseUInt64();
            if (_intToValue.TryGetValue((long)intValue, out var value))
                return value;
            throw new SerializationException($"Unknown enum value: {intValue}");
        }
        public ulong ToUInt64(T value)
        {
            if (_valueToInt.TryGetValue(value, out var intValue))
                return (ulong)intValue;
            throw new SerializationException($"Unknown enum value: {value}");
        }
    }
}
