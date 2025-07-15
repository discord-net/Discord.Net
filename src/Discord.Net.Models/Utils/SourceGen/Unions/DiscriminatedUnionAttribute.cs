namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Property)]
public sealed class DiscriminatedUnionAttribute(string propertyName) : Attribute
{
    public string PropertyName { get; } = propertyName;
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public sealed class DiscriminatedUnionEntryAttribute<T>(params object[] value) : Attribute, IDiscriminatedUnionEntry
{
    public object[] Values { get; } = value;

    Type IDiscriminatedUnionEntry.Type => typeof(T);
}

internal interface IDiscriminatedUnionEntry
{
    object[] Values { get; }
    Type Type { get; }
}

#pragma warning restore CS9113 // Parameter is unread.
