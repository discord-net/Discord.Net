namespace Discord.Models.Validation;

public sealed class MinAttribute(long value) : Attribute
{
    public long Value { get; } = value;
}