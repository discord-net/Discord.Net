namespace Discord;

public class TextDisplayBuilder : IMessageComponentBuilder
{
    public ComponentType Type => ComponentType.ActionRow;

    public int? Id { get; set; }

    private string _content;
    public string Content
    {
        get => _content;
        set => _content = value;
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
        return new(_content, Id);
    }

    IMessageComponent IMessageComponentBuilder.Build() => Build();
}
