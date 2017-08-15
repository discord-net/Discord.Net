using System;
using System.Reflection;
using System.Text;
using System.Text.Formatting;
using System.Text.Json;
using System.Text.Utf8;

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

        protected override PropertyMap CreatePropertyMap<TModel, TValue>(ModelMap modelMap, PropertyInfo propInfo)
        {
            var converter = (JsonPropertyConverter<TValue>)GetConverter(typeof(TValue), propInfo);
            return new JsonPropertyMap<TModel, TValue>(this, modelMap, propInfo, converter);
        }
        
        public TModel Read<TModel>(Utf8String str)
            => Read<TModel>(str.Bytes);
        public override TModel Read<TModel>(ReadOnlySpan<byte> data)
        {
            var reader = new JsonReader(data, SymbolTable.InvariantUtf8);
            if (!reader.Read())
                return default;
            var converter = GetConverter(typeof(TModel)) as JsonPropertyConverter<TModel>;
            //Don't wrap this. We should throw an exception if we cant create the model.
            return converter.Read(this, null, null, null, ref reader, false);
        }

        public override void Write<TModel>(ArrayFormatter stream, TModel model)
        {
            var writer = new JsonWriter(stream);
            var converter = GetConverter(typeof(TModel)) as JsonPropertyConverter<TModel>;
            //Don't wrap this, always throw exceptions on Write.
            converter.Write(this, null, null, null, ref writer, model, null);
        }
    }
}
