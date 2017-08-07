using System.Reflection;

namespace Discord.Serialization
{
    public abstract class PropertyMap
    {
        public string Key { get; }
        public bool ExcludeNull { get; }

        public PropertyMap(PropertyInfo propInfo)
        {
            var jsonProperty = propInfo.GetCustomAttribute<ModelPropertyAttribute>();

            Key = jsonProperty?.Key ?? propInfo.Name;
            ExcludeNull = jsonProperty?.ExcludeNull ?? false;
        }
    }
}
