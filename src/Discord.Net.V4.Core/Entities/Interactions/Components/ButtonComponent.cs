using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent"/> Button.
/// </summary>
public sealed class ButtonComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Button;

    /// <summary>
    ///     Gets the <see cref="ButtonStyle"/> of this button, example buttons with each style can be found at
    ///     <see href="https://discord.com/developers/docs/interactions/message-components#button-object-button-styles"/>.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    ///     Gets the label of the button, this is the text that is shown.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the <see cref="IEmote"/> displayed with this button.
    /// </summary>
    public IEmote? Emote { get; }

    /// <inheritdoc/>
    public string? CustomId { get; }

    /// <summary>
    ///     Gets the URL for a <see cref="ButtonStyle.Link"/> button.
    /// </summary>
    /// <remarks>
    ///     You cannot have a button with a <b>URL</b> and a <b>CustomId</b>.
    /// </remarks>
    public string? Url { get; }

    /// <summary>
    ///     Gets whether this button is disabled or not.
    /// </summary>
    public bool IsDisabled { get; }

    internal ButtonComponent(ButtonStyle style, string label, IEmote? emote, string? customId, string? url, bool isDisabled)
    {
        Style = style;
        Label = label;
        Emote = emote;
        CustomId = customId;
        Url = url;
        IsDisabled = isDisabled;
    }
}
