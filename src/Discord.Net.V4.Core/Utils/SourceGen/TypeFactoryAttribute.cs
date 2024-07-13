namespace Discord;

[AttributeUsage(AttributeTargets.Constructor)]
public class TypeFactoryAttribute : Attribute
{
    public string? LastParameter { get; set; }
}
