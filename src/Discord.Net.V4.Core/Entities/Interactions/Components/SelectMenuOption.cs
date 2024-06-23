using Discord.Models;

namespace Discord;

/// <summary>
///     Represents a choice for a <see cref="SelectMenuComponent" />.
/// </summary>
public readonly struct SelectMenuOption :
    IEntityProperties<Models.Json.SelectMenuOption>,
    IConstructable<SelectMenuOption, ISelectMenuOptionModel>
{
    internal SelectMenuOption(string label, string value, string? description, IEmote? emote, bool? defaultValue)
    {
        Label = label;
        Value = value;
        Description = description;
        Emote = emote;
        IsDefault = defaultValue;
    }

    /// <summary>
    ///     Gets the user-facing name of the option.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the dev-define value of the option.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets a description of the option.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    ///     Gets the <see cref="IEmote" /> displayed with this menu option.
    /// </summary>
    public IEmote? Emote { get; }

    /// <summary>
    ///     Gets whether this option will render as selected by default.
    /// </summary>
    public bool? IsDefault { get; }

    public Models.Json.SelectMenuOption ToApiModel(Models.Json.SelectMenuOption? existing = default) =>
        existing ?? new Models.Json.SelectMenuOption
        {
            Label = Label,
            Value = Value,
            Description = Optional.FromNullable(Description),
            IsDefault = Optional.FromNullable(IsDefault),
            Emoji = Optional.FromNullable(Emote).Map(v => v.ToApiModel())
        };

    public static SelectMenuOption Construct(IDiscordClient client, ISelectMenuOptionModel model) =>
        new(
            model.Label,
            model.Value,
            model.Description,
            model.Emote is not null ? IEmote.Construct(client, model.Emote) : null,
            model.IsDefault
        );
}
