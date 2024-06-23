using Discord.Models;
using Discord.Models.Json;
using System.Collections.Immutable;

namespace Discord;

/// <summary>
///     Represents a select menu component defined at
///     <see href="https://discord.com/developers/docs/interactions/message-components#select-menu-object" />
/// </summary>
public class SelectMenuComponent : IMessageComponent, IConstructable<SelectMenuComponent, ISelectMenuComponentModel>
{
    internal SelectMenuComponent(
        string customId, IEnumerable<SelectMenuOption>? options, string? placeholder,
        int? minValues, int? maxValues, bool? disabled, ComponentType type,
        IEnumerable<ChannelType>? channelTypes = null)
    {
        CustomId = customId;
        Options = options?.ToImmutableArray();
        Placeholder = placeholder;
        MinValues = minValues;
        MaxValues = maxValues;
        IsDisabled = disabled;
        Type = type;
        ChannelTypes = channelTypes?.ToArray() ?? [];
    }

    /// <summary>
    ///     Gets the menus options to select from.
    /// </summary>
    public IReadOnlyCollection<SelectMenuOption>? Options { get; }

    /// <summary>
    ///     Gets the custom placeholder text if nothing is selected.
    /// </summary>
    public string? Placeholder { get; }

    /// <summary>
    ///     Gets the minimum number of items that must be chosen.
    /// </summary>
    public int? MinValues { get; }

    /// <summary>
    ///     Gets the maximum number of items that can be chosen.
    /// </summary>
    public int? MaxValues { get; }

    /// <summary>
    ///     Gets whether this menu is disabled or not.
    /// </summary>
    public bool? IsDisabled { get; }

    /// <summary>
    ///     Gets the allowed channel types for this modal
    /// </summary>
    public IReadOnlyCollection<ChannelType>? ChannelTypes { get; }

    public static SelectMenuComponent Construct(IDiscordClient client, ISelectMenuComponentModel model) =>
        new(
            model.CustomId,
            model.Options.Select(x => SelectMenuOption.Construct(client, x)),
            model.Placeholder,
            model.MinValues,
            model.MaxValues,
            model.IsDisabled,
            (ComponentType)model.Type,
            model.ChannelTypes?.Select(x => (ChannelType)x)
        );

    /// <inheritdoc />
    public ComponentType Type { get; }

    /// <inheritdoc />
    public string CustomId { get; }

    public MessageComponent ToApiModel(MessageComponent? existing = default) =>
        existing ?? new Models.Json.SelectMenuComponent
        {
            Type = (uint)Type,
            CustomId = CustomId,
            IsDisabled = Optional.FromNullable(IsDisabled),
            Options = Options
                .OptionalIf(v => v?.Count > 0)
                .Map(v => v!.Select(v => v.ToApiModel()).ToArray()),
            Placeholder = Optional.FromNullable(Placeholder),
            ChannelTypes = Optional.FromNullable(ChannelTypes).Map(v => v.Select(v => (int)v).ToArray()),
            MaxValues = Optional.FromNullable(MaxValues),
            MinValues = Optional.FromNullable(MinValues)
        };
}
