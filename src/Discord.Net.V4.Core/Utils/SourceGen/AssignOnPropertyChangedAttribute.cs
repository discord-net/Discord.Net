using System;

namespace Discord;

[AttributeUsage(AttributeTargets.Property)]
public class AssignOnPropertyChangedAttribute : Attribute
{
    public AssignOnPropertyChangedAttribute(string propertyName, string toSet, string value)
    { }
}
