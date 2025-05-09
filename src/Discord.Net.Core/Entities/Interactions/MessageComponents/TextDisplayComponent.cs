namespace Discord;

/// <summary>
///     Represents a text display component.
/// </summary>
public class TextDisplayComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.TextDisplay;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <summary>
    ///     Gets the content of this component.
    /// </summary>
    public string Content { get; }

    /// <summary>
    ///     Converts a <see cref="TextDisplayComponent"/> to a <see cref="TextDisplayBuilder"/>.
    /// </summary>
    public TextDisplayBuilder ToBuilder()
        => new(this);

    internal TextDisplayComponent(string content, int? id = null)
    {
        Id = id;
        Content = content;
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
