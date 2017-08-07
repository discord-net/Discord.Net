using System;
using System.Reflection;
using System.Text.Json;

namespace Discord.Serialization.Json
{
    internal class JsonPropertyMap<TModel, TType> : PropertyMap, IJsonPropertyMap<TModel>
    {
        private readonly IJsonPropertyConverter<TType> _converter;
        private readonly Func<TModel, TType> _getFunc;
        private readonly Action<TModel, TType> _setFunc;

        public JsonPropertyMap(PropertyInfo propInfo, IJsonPropertyConverter<TType> converter)
            : base(propInfo)
        {
            _converter = converter;

            _getFunc = propInfo.GetMethod.CreateDelegate(typeof(Func<TModel, TType>)) as Func<TModel, TType>;
            _setFunc = propInfo.SetMethod.CreateDelegate(typeof(Action<TModel, TType>)) as Action<TModel, TType>;
        }

        public void Write(TModel model, ref JsonWriter writer)
        {
            var value = _getFunc(model);
            if (value == null && ExcludeNull)
                return;
            _converter.Write(this, ref writer, value, true);
        }
        public void Read(TModel model, ref JsonReader reader)
        {
            var value = _converter.Read(this, ref reader, true);
            _setFunc(model, value);
        }
    }
}
