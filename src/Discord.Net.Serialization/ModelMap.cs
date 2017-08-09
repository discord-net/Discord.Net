using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Serialization
{
    public class ModelMap<TModel>
        where TModel : class, new()
    {
        private BufferDictionary<PropertyMap> _dictionary;
        public PropertyMap[] Properties { get; }

        public ModelMap(List<PropertyMap> properties)
        {
            Properties = properties.ToArray();
            _dictionary = new BufferDictionary<PropertyMap>(properties.ToDictionary(x => x.Utf8Key));
        }

        public bool TryGetProperty(ReadOnlyBuffer<byte> key, out PropertyMap value)
            => _dictionary.TryGetValue(key, out value);
        public bool TryGetProperty(ReadOnlySpan<byte> key, out PropertyMap value)
            => _dictionary.TryGetValue(key, out value);
    }
}