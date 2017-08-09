using System;

namespace Discord.Serialization
{
    public enum EnumValueType
    {
        ReadWrite,
        ReadOnly,
        WriteOnly
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ModelEnumValueAttribute : Attribute
    {
        public string Key { get; }
        public EnumValueType Type { get; }

        public ModelEnumValueAttribute(string key, EnumValueType type = EnumValueType.ReadWrite)
        {
            Key = key;
            Type = type;
        }
    }
}
