namespace Discord;

[AttributeUsage(AttributeTargets.Property)]
internal sealed class LinkMirrorAttribute : Attribute
{
    public bool OnlyBackLinks { get; set; }
}