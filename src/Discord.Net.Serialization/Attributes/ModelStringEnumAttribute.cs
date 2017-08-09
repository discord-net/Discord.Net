using System;

namespace Discord.Serialization
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ModelStringEnumAttribute : Attribute
    {
    }
}
