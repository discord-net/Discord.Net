namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class DiscriminatedUnionTypeAttribute(string property, object? delimiter = null) : Attribute
{
    public bool WhenSpecified { get; set; }
}
#pragma warning restore CS9113 // Parameter is unread.
