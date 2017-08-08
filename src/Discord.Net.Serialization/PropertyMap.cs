using System;
using System.Reflection;
using System.Text.Utf8;

namespace Discord.Serialization
{
    public abstract class PropertyMap
    {
        public string Key { get; }
        public ReadOnlyBuffer<byte> Utf8Key { get; }
        public bool ExcludeNull { get; }

        public PropertyMap(PropertyInfo propInfo)
        {
            var jsonProperty = propInfo.GetCustomAttribute<ModelPropertyAttribute>();

            Key = jsonProperty?.Key ?? propInfo.Name;
            Utf8Key = new ReadOnlyBuffer<byte>(new Utf8String(Key).Bytes.ToArray());
            ExcludeNull = jsonProperty?.ExcludeNull ?? false;
        }
    }
}
