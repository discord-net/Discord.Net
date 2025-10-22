namespace Discord;

/// <summary>
///    Represents a layout component that wraps modal components (text input, select menu or file upload) with a label and description.
/// </summary>
public class LabelComponent : IMessageComponent
{
    /// <inheritdoc />
    public ComponentType Type => ComponentType.Label;

    /// <inheritdoc />
    public int? Id { get; }

    /// <summary>
    ///     Gets the label text.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the description text for the label.
    /// </summary>
    public string Description { get; }

    /// <summary>
    ///     Gets the component within the label.
    /// </summary>
    public IMessageComponent Component { get; }

    internal LabelComponent(int? id, string label, string description, IMessageComponent component)
    {
        Id = id;
        Label = label;
        Description = description;
        Component = component;
    }

    /// <inheritdoc />
    public IMessageComponentBuilder ToBuilder()
        => new LabelBuilder(this);
}
