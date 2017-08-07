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

            //AddEnumConverter<Converters.EnumPropertyConverter>();
        }

        public void AddConverter<TValue, TConverter>()
            where TConverter : class, IJsonPropertyConverter<TValue>
            => _converters.Add<TValue, TConverter>();
        public void AddConverter<TValue, TConverter>(Func<PropertyInfo, bool> condition)
            where TConverter : class, IJsonPropertyConverter<TValue>
            => _converters.Add<TValue, TConverter>(condition);

        public void AddGenericConverter(Type value, Type converter)
            => _converters.AddGeneric(value, converter);
        public void AddGenericConverter(Type value, Type converter, Func<PropertyInfo, bool> condition)
            => _converters.AddGeneric(value, converter);

        protected override PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo)
        {
            var converter = (IJsonPropertyConverter<TValue>)_converters.Get<TValue>(propInfo);
            return new JsonPropertyMap<TModel, TValue>(propInfo, converter);
        }

        protected internal override TModel Read<TModel>(Serializer serializer, ReadOnlyBuffer<byte> data)
        {
            var reader = new JsonReader(data.Span, SymbolTable.InvariantUtf8);
            var map = MapModel<TModel>();
            var model = new TModel();

            if (!reader.Read() || reader.TokenType != JsonTokenType.StartObject)
                throw new InvalidOperationException("Bad input, expected StartObject");
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return model;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new InvalidOperationException("Bad input, expected PropertyName");

                string key = reader.ParseString();
                if (map.PropertiesByKey.TryGetValue(key, out var property))
                    (property as IJsonPropertyMap<TModel>).Read(model, reader);
                else
                    reader.Skip(); //Unknown property, skip

                if (!reader.Read())
                    throw new InvalidOperationException("Bad input, expected Value");
            }
            throw new InvalidOperationException("Bad input, expected EndObject");
        }

        protected internal override void Write<TModel>(Serializer serializer, ArrayFormatter stream, TModel model)
        {
            var writer = new JsonWriter(stream);
            var map = MapModel<TModel>();

            writer.WriteObjectStart();
            for (int i = 0; i < map.Properties.Length; i++)
                (map.Properties[i] as IJsonPropertyMap<TModel>).Write(model, writer);
            writer.WriteObjectEnd();
        }
    }
}
