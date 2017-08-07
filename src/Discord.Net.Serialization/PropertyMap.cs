using System.Reflection;

namespace Discord.Serialization
{
    public abstract class PropertyMap
    {
        public string Key { get; }

        public PropertyMap(PropertyInfo propInfo)
        {
            var jsonProperty = propInfo.GetCustomAttribute<ModelPropertyAttribute>();
            if (jsonProperty != null)
                Key = jsonProperty.Key;
            else
                Key = propInfo.Name;
        }
    }
}
