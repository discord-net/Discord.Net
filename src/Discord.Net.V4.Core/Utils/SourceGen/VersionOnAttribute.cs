namespace Discord;

[AttributeUsage(AttributeTargets.Property)]
public sealed class VersionOnAttribute : Attribute
{
    public VersionOnAttribute(string property, string? init = null)
    {
    }
}
