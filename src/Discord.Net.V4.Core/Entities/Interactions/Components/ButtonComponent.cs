using Discord.Models;
using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent" /> Button.
/// </summary>
public sealed class ButtonComponent : IMessageComponent, IConstructable<ButtonComponent, IButtonComponentModel>
{
    internal ButtonComponent(ButtonStyle style, string? label, IEmote? emote, string? customId, string? url,
        bool? isDisabled)
    {
        Style = style;
        Label = label;
        Emote = emote;
        CustomId = customId;
        Url = url;
        IsDisabled = isDisabled;
    }

    /// <summary>
    ///     Gets the <see cref="ButtonStyle" /> of this button, example buttons with each style can be found at
    ///     <see href="https://discord.com/developers/docs/interactions/message-components#button-object-button-styles" />.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    ///     Gets the label of the button, this is the text that is shown.
    /// </summary>
    public string? Label { get; }

    /// <summary>
    ///     Gets the <see cref="IEmote" /> displayed with this button.
    /// </summary>
    public IEmote? Emote { get; }

    /// <summary>
    ///     Gets the URL for a <see cref="ButtonStyle.Link" /> button.
    /// </summary>
    /// <remarks>
    ///     You cannot have a button with a <b>URL</b> and a <b>CustomId</b>.
    /// </remarks>
    public string? Url { get; }

    /// <summary>
    ///     Gets whether this button is disabled or not.
    /// </summary>
    public bool? IsDisabled { get; }

    public static ButtonComponent Construct(IDiscordClient client, IButtonComponentModel model)
        => new(
            (ButtonStyle)model.Style,
            model.Label,
            model.Emote is not null ? IEmote.Construct(client, model.Emote) : null,
            model.CustomId,
            model.Url,
            model.IsDisabled
        );

    /// <inheritdoc />
    public ComponentType Type => ComponentType.Button;

    /// <inheritdoc />
    public string? CustomId { get; }

    public MessageComponent ToApiModel(MessageComponent? existing = default) =>
        existing ?? new Models.Json.ButtonComponent
        {
            Type = (uint)Type,
            Emote = Optional.FromNullable(Emote).Map(v => (IEmoteModel)v.ToApiModel()),
            Label = Optional.FromNullable(Label),
            Style = (int)Style,
            IsDisabled = Optional.FromNullable(IsDisabled),
            CustomId = Optional.FromNullable(CustomId),
            Url = Optional.FromNullable(Url)
        };
}
