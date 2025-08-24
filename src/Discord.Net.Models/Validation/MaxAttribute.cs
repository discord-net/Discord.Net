namespace Discord.Models.Validation;

public sealed class MaxAttribute(long value) : Attribute
{
    public long Value { get; } = value;
}