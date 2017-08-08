using System;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Serialization
{
    public class ModelMap<TModel>
        where TModel : class, new()
    {
        public readonly PropertyMap[] Properties;
        public readonly Dictionary<ReadOnlySpan<byte>, PropertyMap> PropertiesByKey;

        public ModelMap(Dictionary<ReadOnlySpan<byte>, PropertyMap> properties)
        {
            PropertiesByKey = properties;
            Properties = PropertiesByKey.Values.ToArray();
        }
    }
}
