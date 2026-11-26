namespace Discord;

/// <summary>
///     Represents an action item of an <see cref="OverflowMenuComponent"/>.
/// </summary>
public class MenuActionItemComponent : IInteractableComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.MenuActionItem;

    /// <inheritdoc/>
    public int? Id { get; }

    /// <inheritdoc/>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the label of this action item.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the <see cref="MenuActionItemStyle"/> of this action item.
    /// </summary>
    public MenuActionItemStyle Style { get; }

    /// <summary>
    ///     Gets the description of this action item.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>
    public string Description { get; }

    /// <summary>
    ///     Gets the URL for a <see cref="MenuActionItemStyle.Link"/> action item.
    /// </summary>
    /// <remarks>
    ///     You cannot have an action item with a <b>URL</b> and a <b>CustomId</b>.
    /// </remarks>
    public string Url { get; }

    /// <summary>
    ///     Gets the <see cref="IEmote"/> displayed with this action item.
    /// </summary>
    public IEmote Emote { get; }

    /// <summary>
    ///     Gets whether this action item is disabled or not.
    /// </summary>
    public bool IsDisabled { get; }

    /// <summary>
    ///     Converts a <see cref="MenuActionItemComponent"/> to a <see cref="MenuActionItemBuilder"/>.
    /// </summary>
    public MenuActionItemBuilder ToBuilder()
        => new(this);

    internal MenuActionItemComponent(int? id, string customId, string label, MenuActionItemStyle style,
        string description, string url, IEmote emote, bool isDisabled)
    {
        Id = id;
        CustomId = customId;
        Label = label;
        Style = style;
        Description = description;
        Url = url;
        Emote = emote;
        IsDisabled = isDisabled;
    }

    /// <inheritdoc/>
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
