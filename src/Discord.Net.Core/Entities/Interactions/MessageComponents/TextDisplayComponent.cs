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

    internal TextDisplayComponent(string content, int? id = null)
    {
        Id = id;
        Content = content;
    }
}
