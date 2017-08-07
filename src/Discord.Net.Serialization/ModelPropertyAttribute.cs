using System;

namespace Discord.Serialization
{
    public class ModelPropertyAttribute : Attribute
    {
        public string Key { get; }
        public bool ExcludeNull { get; set; }

        public ModelPropertyAttribute(string key = null)
        {
            Key = key;
        }
    }
}
