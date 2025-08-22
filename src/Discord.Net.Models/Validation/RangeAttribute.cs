namespace Discord.Models.Validation;

public sealed class RangeAttribute(long lower, long upper) : Attribute
{
    public long? Lower { get; } = lower;
    public long? Upper { get; } = upper;
}