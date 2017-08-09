using System;

namespace Discord.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
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
