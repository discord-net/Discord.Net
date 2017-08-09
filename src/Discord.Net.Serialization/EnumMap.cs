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

        private readonly BufferDictionary<T> _keyToValue;
        private readonly Dictionary<T, string> _valueToKey;
        private readonly Dictionary<T, ReadOnlyBuffer<byte>> _valueToUtf8Key;

        public EnumMap()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            if (!typeInfo.IsEnum)
                throw new InvalidOperationException($"{typeInfo.Name} is not an Enum");

            _keyToValue = new BufferDictionary<T>();
            _valueToKey = new Dictionary<T, string>();
            _valueToUtf8Key = new Dictionary<T, ReadOnlyBuffer<byte>>();

            foreach (T val in Enum.GetValues(typeof(T)).OfType<T>())
            {
                var fieldInfo = typeInfo.GetDeclaredField(Enum.GetName(typeof(T), val));
                var attr = fieldInfo.GetCustomAttribute<ModelEnumAttribute>();
                if (attr != null)
                {
                    var key = new ReadOnlyBuffer<byte>(new Utf8String(attr.Key).Bytes.ToArray());
                    if (attr.Type != EnumValueType.WriteOnly)
                        _keyToValue.Add(key, val);
                    if (attr.Type != EnumValueType.ReadOnly)
                    {
                        _valueToUtf8Key.Add(val, key);
                        _valueToKey.Add(val, attr.Key);
                    }
                }
            }
        }

        public T GetValue(ReadOnlyBuffer<byte> key)
        {
            if (_keyToValue.TryGetValue(key, out var value))
                return value;
            throw new SerializationException($"Unknown enum key: {new Utf8String(key.Span).ToString()}");
        }
        public T GetValue(ReadOnlySpan<byte> key)
        {
            if (_keyToValue.TryGetValue(key, out var value))
                return value;
            throw new SerializationException($"Unknown enum key: {new Utf8String(key).ToString()}");
        }
        public string GetKey(T value)
        {
            if (_valueToKey.TryGetValue(value, out var key))
                return key;
            throw new SerializationException($"Unknown enum value: {value}");
        }
        public ReadOnlyBuffer<byte> GetUtf8Key(T value)
        {
            if (_valueToUtf8Key.TryGetValue(value, out var key))
                return key;
            throw new SerializationException($"Unknown enum value: {value}");
        }
    }
}
