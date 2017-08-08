using System.Reflection;
using System.Text.Utf8;

namespace Discord.Serialization
{
    public abstract class PropertyMap
    {
        public string Utf16Key { get; }
        public Utf8String Utf8Key { get; }
        public bool ExcludeNull { get; }

        public PropertyMap(PropertyInfo propInfo)
        {
            var jsonProperty = propInfo.GetCustomAttribute<ModelPropertyAttribute>();

            Utf16Key = jsonProperty?.Key ?? propInfo.Name;
            Utf8Key = new Utf8String(Utf16Key);
            ExcludeNull = jsonProperty?.ExcludeNull ?? false;
        }
    }
}
