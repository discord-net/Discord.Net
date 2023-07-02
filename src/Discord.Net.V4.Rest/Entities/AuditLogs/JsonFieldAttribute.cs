using System;

namespace Discord.Rest;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
internal class JsonFieldAttribute : Attribute
{
    public string FieldName { get; }

    public JsonFieldAttribute(string fieldName)
    {
        FieldName = fieldName;
    }
}
