namespace Discord;

internal sealed class LinkSchematicAttribute : Attribute
{
    public string[] Children { get; set; } = [];
}