using Discord.Serialization.Converters;
using System;
using System.Reflection;
using System.Text.Json;

namespace Discord.Serialization
{
    internal static class PropertyMap
    {
        public static PropertyMap<TModel> Create<TModel>(PropertyInfo propInfo)
        {
            var type = typeof(PropertyMap<,>).MakeGenericType(typeof(TModel), propInfo.PropertyType);
            return Activator.CreateInstance(type, propInfo) as PropertyMap<TModel>;
        }
    }

    internal abstract class PropertyMap<TModel>
    {
        public string Key { get; protected set; }

        public abstract void WriteJson(TModel model, JsonWriter writer);
        public abstract void ReadJson(TModel model, JsonReader reader);
    }

    internal class PropertyMap<TModel, TProp> : PropertyMap<TModel>
    {
        private readonly IPropertyConverter<TProp> _converter;
        private readonly Func<TModel, TProp> _getFunc;
        private readonly Action<TModel, TProp> _setFunc;
        
        public PropertyMap(PropertyInfo propInfo)
        {
            var jsonProperty = propInfo.GetCustomAttribute<ModelPropertyAttribute>();
            if (jsonProperty != null)
                Key = jsonProperty.Key;
            else
                Key = propInfo.Name;
            
            _getFunc = propInfo.GetMethod.CreateDelegate(typeof(Func<TModel, TProp>)) as Func<TModel, TProp>;
            _setFunc = propInfo.SetMethod.CreateDelegate(typeof(Action<TModel, TProp>)) as Action<TModel, TProp>;

            _converter = Converter.For<TProp>();
        }

        private TProp GetValue(TModel model)
            => _getFunc(model);
        private void SetValue(TModel model, TProp prop)
            => _setFunc(model, prop);

        public override void WriteJson(TModel model, JsonWriter writer)
        {
            var value = GetValue(model);
            _converter.WriteJson(writer, value);
        }
        public override void ReadJson(TModel model, JsonReader reader)
        {
            var value = _converter.ReadJson(reader);
            SetValue(model, value);
        }
    }
}
