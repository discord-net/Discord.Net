using System.Collections.Generic;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a select menu component defined at <see href="https://discord.com/developers/docs/interactions/message-components#select-menu-object"/>
/// </summary>
public class SelectMenuComponent : IInteractableComponent
{
    /// <inheritdoc/>
    public ComponentType Type { get; }

    /// <inheritdoc/>
    public int? Id { get; }

    /// <inheritdoc/>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the menus options to select from.
    /// </summary>
    public IReadOnlyCollection<SelectMenuOption> Options { get; }

    /// <summary>
    ///     Gets the custom placeholder text if nothing is selected.
    /// </summary>
    public string Placeholder { get; }

    /// <summary>
    ///     Gets the minimum number of items that must be chosen.
    /// </summary>
    public int MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of items that can be chosen.
    /// </summary>
    public int MaxValues { get; }

    /// <summary>
    ///     Gets whether this menu is disabled or not.
    /// </summary>
    public bool IsDisabled { get; }

    /// <summary>
    ///     Gets the allowed channel types for this modal
    /// </summary>
    public IReadOnlyCollection<ChannelType> ChannelTypes { get; }

    /// <summary>
    ///     Gets default values for auto-populated select menu components.
    /// </summary>
    public IReadOnlyCollection<SelectMenuDefaultValue> DefaultValues { get; }

    /// <summary>
    ///     Converts a <see cref="SelectMenuComponent"/> to a <see cref="SelectMenuBuilder"/>.
    /// </summary>
    public SelectMenuBuilder ToBuilder()
        => new(this);

    internal SelectMenuComponent(string customId, List<SelectMenuOption> options, string placeholder, int minValues, int maxValues,
        bool disabled, ComponentType type, int? id, IEnumerable<ChannelType> channelTypes = null, IEnumerable<SelectMenuDefaultValue> defaultValues = null)
    {
        CustomId = customId;
        Options = options;
        Placeholder = placeholder;
        MinValues = minValues;
        MaxValues = maxValues;
        IsDisabled = disabled;
        Type = type;
        Id = id;
        ChannelTypes = channelTypes?.ToArray() ?? [];
        DefaultValues = defaultValues?.ToArray() ?? [];
    }

    /// <inheritdoc />
    IMessageComponentBuilder IMessageComponent.ToBuilder() => ToBuilder();
}
