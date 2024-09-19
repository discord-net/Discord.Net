namespace Discord;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
internal sealed class LinkHierarchicalRootAttribute : Attribute
{
    public Type[] Types { get; set; } = [];
}