namespace Discord;

public class LabelComponent : IMessageComponent
{
    /// <inheritdoc />
    public ComponentType Type => ComponentType.Label;

    /// <inheritdoc />
    public int? Id { get; private set; }

    /// <summary>
    ///     
    /// </summary>
    public string Label { get; private set; }

    /// <summary>
    ///     
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    ///     
    /// </summary>
    public IMessageComponent Component { get; private set; }

    internal LabelComponent(int? id, string label, string description, IMessageComponent component)
    {
        Id = id;
        Label = label;
        Description = description;
        Component = component;
    }

    /// <inheritdoc />
    public IMessageComponentBuilder ToBuilder()
        => new LabelBuilder(Label, Component.ToBuilder(), Description, Id);
}
