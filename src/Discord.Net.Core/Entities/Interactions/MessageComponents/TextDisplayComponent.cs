namespace Discord;

public class TextDisplayComponent : IMessageComponent
{
    public ComponentType Type => ComponentType.TextDisplay;

    public int? Id { get; }

    public string Content { get; }

    internal TextDisplayComponent(string content, int? id = null)
    {
        Id = id;
        Content = content;
    }
}
