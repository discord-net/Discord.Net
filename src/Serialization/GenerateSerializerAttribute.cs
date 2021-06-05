using System;

namespace Discord.Net.Serialization
{
    /// <summary>
    /// Defines an attribute which informs the serializer generator to generate
    /// a serializer for this type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
        AllowMultiple = false,
        Inherited = false)]
    public class GenerateSerializerAttribute : Attribute
    {

    }
}
