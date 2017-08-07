using System.Collections.Generic;
using System.Linq;

namespace Discord.Serialization
{
    public class ModelMap<TModel>
        where TModel : class, new()
    {
        public readonly PropertyMap[] Properties;
        public readonly Dictionary<string, PropertyMap> PropertiesByKey;

        public ModelMap(Dictionary<string, PropertyMap> properties)
        {
            PropertiesByKey = properties;
            Properties = PropertiesByKey.Values.ToArray();
        }
    }
}
