namespace Discord;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class CovariantOverrideAttribute : Attribute
{
    public bool ThrowOnInvalidVariant { get; set; } = true;
}
