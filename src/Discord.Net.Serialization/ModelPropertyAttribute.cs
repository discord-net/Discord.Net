using System;

namespace Discord.Serialization
{
    public class ModelPropertyAttribute : Attribute
    {
        public string Key { get; }
        public bool IgnoreNull { get; set; }

        public ModelPropertyAttribute(string key)
        {
            Key = key;
        }
    }
}
