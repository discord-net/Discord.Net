using System;
using System.Collections.Generic;

namespace Discord.Serialization
{
    public class ModelMap
    {
        private readonly List<PropertyMap> _propList;
        private readonly BufferDictionary<PropertyMap> _propDict;

        public string Path { get; }
        public bool HasDynamics { get; }
        public IReadOnlyList<PropertyMap> Properties => _propList;

        internal ModelMap(string path)
        {
            Path = path;
            _propList = new List<PropertyMap>();
            _propDict = new BufferDictionary<PropertyMap>();
        }

        internal void AddProperty(PropertyMap propMap)
        {
            _propList.Add(propMap);
            _propDict.Add(propMap.Utf8Key, propMap);
        }

        public bool TryGetProperty(ReadOnlyBuffer<byte> key, out PropertyMap value)
            => _propDict.TryGetValue(key, out value);
        public bool TryGetProperty(ReadOnlySpan<byte> key, out PropertyMap value)
            => _propDict.TryGetValue(key, out value);
    }
}