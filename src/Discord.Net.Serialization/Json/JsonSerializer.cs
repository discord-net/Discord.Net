using System;
using System.Reflection;
using System.Text;
using System.Text.Formatting;
using System.Text.Json;

namespace Discord.Serialization.Json
{
    public abstract class JsonSerializer : Serializer
    {
        protected JsonSerializer(JsonSerializer parent) : base(parent) { }

        public void AddConverter<TValue, TConverter>()
            where TConverter : JsonPropertyConverter<TValue>
            => AddConverter(typeof(TValue), typeof(TConverter));
        public void AddConverter<TValue, TConverter>(Func<TypeInfo, PropertyInfo, bool> condition)
            where TConverter : JsonPropertyConverter<TValue>
            => AddConverter(typeof(TValue), typeof(TConverter), condition);

        protected override PropertyMap CreatePropertyMap<TModel, TValue>(PropertyInfo propInfo)
        {
            var converter = (JsonPropertyConverter<TValue>)GetConverter(typeof(TValue), propInfo);
            return new JsonPropertyMap<TModel, TValue>(this, propInfo, converter);
        }

        public override TModel Read<TModel>(ReadOnlyBuffer<byte> data)
        {
            var reader = new JsonReader(data.Span, SymbolTable.InvariantUtf8);
            if (!reader.Read())
                return default;
            var converter = GetConverter(typeof(TModel)) as JsonPropertyConverter<TModel>;
            return converter.Read(null, null, ref reader, false);
        }
        public override void Write<TModel>(ArrayFormatter stream, TModel model)
        {
            var writer = new JsonWriter(stream);
            var converter = GetConverter(typeof(TModel)) as JsonPropertyConverter<TModel>;
            converter.Write(null, null, ref writer, model, null);
        }
    }
}
