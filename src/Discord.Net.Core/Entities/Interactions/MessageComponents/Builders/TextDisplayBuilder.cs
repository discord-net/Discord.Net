namespace Discord;

public class TextDisplayBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.ActionRow;

    public int? Id { get; set; }

    public string Content { get; set; }

    public TextDisplayBuilder() { }

    public TextDisplayBuilder(string content, int? id = null)
    {
        Content = content;
        Id = id;
    }

    public TextDisplayBuilder WithContent(string content)
    {
        Content = content;
        return this;
    }

    public TextDisplayBuilder WithId(int? id)
    {
        Id = id;
        return this;
    }

    public TextDisplayComponent Build()
    {
        return new(Content, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
