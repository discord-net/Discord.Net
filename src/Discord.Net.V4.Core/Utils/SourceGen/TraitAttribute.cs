namespace Discord;

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class TraitAttribute : Attribute
{
    public bool Overrides { get; set; }
}
