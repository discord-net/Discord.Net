using System;

namespace Discord.Serialization
{
    internal class ModelPropertyAttribute : Attribute
    {
        public string Key { get; }
        public bool IgnoreNull { get; set; }

        public ModelPropertyAttribute(string key)
        {
            Key = key;
        }
    }
}
