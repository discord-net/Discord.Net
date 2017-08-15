using System;
using System.Reflection;
using System.Text.Json;

namespace Discord.Serialization.Json
{
    internal interface IJsonPropertyMap<TModel>
    {
        string Key { get; }
        ReadOnlyBuffer<byte> Utf8Key { get; }

        void Write(Serializer serializer, TModel model, ref JsonWriter writer);
        void Read(Serializer serializer, TModel model, ref JsonReader reader);
    }

    internal class JsonPropertyMap<TModel, TValue> : PropertyMap<TModel, TValue>, IJsonPropertyMap<TModel>
    {
        private readonly ModelMap _modelMap;
        private readonly JsonPropertyConverter<TValue> _converter;
        private readonly Func<TModel, TValue> _getFunc;
        private readonly Action<TModel, TValue> _setFunc;

        public JsonPropertyMap(Serializer serializer, ModelMap modelMap, PropertyInfo propInfo, JsonPropertyConverter<TValue> converter)
            : base(serializer, modelMap, propInfo)
        {
            _modelMap = modelMap;
            _converter = converter;

            _getFunc = propInfo.GetMethod.CreateDelegate(typeof(Func<TModel, TValue>)) as Func<TModel, TValue>;
            _setFunc = propInfo.SetMethod.CreateDelegate(typeof(Action<TModel, TValue>)) as Action<TModel, TValue>;
        }

        public void Read(Serializer serializer, TModel model, ref JsonReader reader)
        {
            var value = _converter.Read(serializer, _modelMap, this, model, ref reader, true);
            _setFunc(model, value);
        }
        public void Write(Serializer serializer, TModel model, ref JsonWriter writer)
        {
            var value = _getFunc(model);
            if (value == null && ExcludeNull)
                return;
            _converter.Write(serializer, _modelMap, this, model, ref writer, value, Key);
        }
    }
}
