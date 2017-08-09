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
    public class ModelEnumAttribute : Attribute
    {
        public string Key { get; }
        public EnumValueType Type { get; }

        public ModelEnumAttribute(string key, EnumValueType type = EnumValueType.ReadWrite)
        {
            Key = key;
            Type = type;
        }
    }
}
