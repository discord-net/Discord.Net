using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Serialization.Json
{
    public class DefaultJsonSerializer : JsonSerializer
    {
        private static readonly Lazy<DefaultJsonSerializer> _singleton = new Lazy<DefaultJsonSerializer>();
        public static DefaultJsonSerializer Global => _singleton.Value;

        public DefaultJsonSerializer()
            : this((JsonSerializer)null) { }
        public DefaultJsonSerializer(JsonSerializer parent)
            : base(parent)
        {
            AddConverter<sbyte, Converters.Int8PropertyConverter>();
            AddConverter<short, Converters.Int16PropertyConverter>();
            AddConverter<int, Converters.Int32PropertyConverter>();
            AddConverter<long, Converters.Int64PropertyConverter>();

            AddConverter<byte, Converters.UInt8PropertyConverter>();
            AddConverter<ushort, Converters.UInt16PropertyConverter>();
            AddConverter<uint, Converters.UInt32PropertyConverter>();
            AddConverter<ulong, Converters.UInt64PropertyConverter>();

            AddConverter<float, Converters.SinglePropertyConverter>();
            AddConverter<double, Converters.DoublePropertyConverter>();
            AddConverter<decimal, Converters.DecimalPropertyConverter>();

            //AddConverter<char, Converters.CharPropertyConverter>(); //TODO: char.Parse does not support Json.Net's serialization
            AddConverter<string, Converters.StringPropertyConverter>();

            AddConverter<DateTime, Converters.DateTimePropertyConverter>();
            AddConverter<DateTimeOffset, Converters.DateTimeOffsetPropertyConverter>();

            AddConverter<bool, Converters.BooleanPropertyConverter>();
            AddConverter<Guid, Converters.GuidPropertyConverter>();

            AddConverter<object, Converters.DynamicPropertyConverter>(
                (type, prop) => prop.GetCustomAttributes<ModelSelectorAttribute>().Any());

            AddGenericConverter(typeof(List<>), typeof(Converters.ListPropertyConverter<>));
            //AddGenericConverter(typeof(IReadOnlyList<>), typeof(Converters.ListPropertyConverter<>));
            //AddGenericConverter(typeof(IReadOnlyCollection<>), typeof(Converters.ListPropertyConverter<>));
            //AddGenericConverter(typeof(IEnumerable<>), typeof(Converters.ListPropertyConverter<>));
            AddGenericConverter(typeof(Dictionary<,>), typeof(Converters.DictionaryPropertyConverter<>));
            //AddGenericConverter(typeof(IReadOnlyDictionary<,>), typeof(Converters.DictionaryPropertyConverter<>));
            AddGenericConverter(typeof(Nullable<>), typeof(Converters.NullablePropertyConverter<>));

            AddGenericConverter(typeof(Converters.ArrayPropertyConverter<>), //Arrays
                (type, prop) => type.IsArray, innerType => innerType.GetElementType());
            AddGenericConverter(typeof(Converters.StringEnumPropertyConverter<>), //Enums : string
                (type, prop) => type.IsEnum && prop.GetCustomAttribute<ModelStringEnumAttribute>() != null);
            AddGenericConverter(typeof(Converters.Int64EnumPropertyConverter<>), //Enums : sbyte/short/int/long
                (type, prop) => type.IsEnum && IsSignedEnum(Enum.GetUnderlyingType(type.AsType())));
            AddGenericConverter(typeof(Converters.UInt64EnumPropertyConverter<>), //Enums: byte/ushort/uint/ulong
                (type, prop) => type.IsEnum && IsUnsignedEnum(Enum.GetUnderlyingType(type.AsType())));
            AddGenericConverter(typeof(Converters.ObjectPropertyConverter<>), //Classes
                (type, prop) => type.IsClass && type.DeclaredConstructors.Any(x => x.GetParameters().Length == 0));

            //TODO: Structs?
        }

        private DefaultJsonSerializer(DefaultJsonSerializer parent)
            : base(parent) { }
        public DefaultJsonSerializer CreateScope() => new DefaultJsonSerializer(this);

        private static bool IsSignedEnum(Type underlyingType)
            => underlyingType == typeof(sbyte) ||
                underlyingType == typeof(short) ||
                underlyingType == typeof(int) ||
                underlyingType == typeof(long);
        private static bool IsUnsignedEnum(Type underlyingType)
            => underlyingType == typeof(byte) ||
                underlyingType == typeof(ushort) ||
                underlyingType == typeof(uint) ||
                underlyingType == typeof(ulong);
    }
}
