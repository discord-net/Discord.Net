using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Discord.Serialization
{
    public class ModelMap<TModel>
        where TModel : class, new()
    {
        private BufferDictionary<PropertyMap> _propDict;

        public bool HasDynamics { get; }
        public PropertyMap[] Properties { get; }

        public ModelMap(Serializer serializer, TypeInfo type, List<PropertyMap> properties)
        {
            Properties = properties.ToArray();
            _propDict = new BufferDictionary<PropertyMap>(properties.ToDictionary(x => x.Utf8Key));
        }

        public bool TryGetProperty(ReadOnlyBuffer<byte> key, out PropertyMap value)
            => _propDict.TryGetValue(key, out value);
        public bool TryGetProperty(ReadOnlySpan<byte> key, out PropertyMap value)
            => _propDict.TryGetValue(key, out value);
    }
}