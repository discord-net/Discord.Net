namespace Discord.Models.Validation;

public sealed class VariantAttribute(string propertyName, params object[] values) : Attribute
{
    public string PropertyName { get; } = propertyName;
    public object[] Values { get; } = values;
}