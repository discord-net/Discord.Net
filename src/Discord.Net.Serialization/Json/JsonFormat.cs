using Discord.Serialization.Json.Converters;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Formatting;
using System.Text.Json;

namespace Discord.Serialization.Json
{
    public class JsonFormat : SerializationFormat
    {
        public JsonFormat()
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

            AddConverter<char, Converters.CharPropertyConverter>();
            AddConverter<string, Converters.StringPropertyConverter>();

            AddConverter<DateTime, Converters.DateTimePropertyConverter>();
            AddConverter<DateTimeOffset, Converters.DateTimeOffsetPropertyConverter>();

            AddConverter<bool, Converters.BooleanPropertyConverter>();
            AddConverter<Guid, Converters.GuidPropertyConverter>();

            AddGenericConverter(typeof(List<>), typeof(Converters.ListPropertyConverter<>));
            AddGenericConverter(typeof(Nullable<>), typeof(Converters.NullablePropertyConverter<>));

            AddGenericConverter(typeof(Converters.ObjectPropertyConverter<>), (type, prop) => type.IsClass);

            //AddEnumConverter<Converters.EnumPropertyConverter>();
        }

        public void AddConverter<TValue, TConverter>()
            where TConverter : class, IJsonPropertyConverter<TValue>
            => _converters.Add<TValue, TConverter>();
        public void AddConverter<TValue, TConverter>(Func<TypeInfo, PropertyInfo, bool> condition)
            where TConverter : class, IJsonPropertyConverter<TValue>
            => _converters.Add<TValue, TConverter>(condition);

        public void AddGenericConverter(Type converter)
            => _converters.AddGeneric(converter);
        public void AddGenericConverter(Type converter, Func<TypeInfo, PropertyInfo, bool> condition)
            => _converters.AddGeneric(converter, condition);
        public void AddGenericConverter(Type value, Type converter)
            => _converters.AddGeneric(value, converter);
        public void AddGenericConverter(Type value, Type converter, Func<TypeInfo, PropertyInfo, bool> condition)
            => _converters.AddGeneric(value, converter, condition);

        protected override PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo)
        {
            var converter = (IJsonPropertyConverter<TValue>)_converters.Get<TValue>(propInfo);
            return new JsonPropertyMap<TModel, TValue>(propInfo, converter);
        }

        protected internal override TModel Read<TModel>(Serializer serializer, ReadOnlyBuffer<byte> data)
        {
            var reader = new JsonReader(data.Span, SymbolTable.InvariantUtf8);
            if (!reader.Read())
                return null;
            var converter = _converters.Get<TModel>() as IJsonPropertyConverter<TModel>;
            return converter.Read(null, reader, false);
        }

        protected internal override void Write<TModel>(Serializer serializer, ArrayFormatter stream, TModel model)
        {
            var writer = new JsonWriter(stream);
            var converter = _converters.Get<TModel>() as IJsonPropertyConverter<TModel>;
            converter.Write(null, writer, model, false);
        }
    }
}
