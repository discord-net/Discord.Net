namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent"/> Button.
/// </summary>
public class ButtonComponent : IMessageComponent
{
    /// <inheritdoc/>
    public ComponentType Type => ComponentType.Button;

    /// <summary>
    ///     Gets the <see cref="ButtonStyle"/> of this button, example buttons with each style can be found <see href="https://discord.com/assets/7bb017ce52cfd6575e21c058feb3883b.png">Here</see>.
    /// </summary>
    public ButtonStyle Style { get; }

    /// <summary>
    ///     Gets the label of the button, this is the text that is shown.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the <see cref="IEmote"/> displayed with this button.
    /// </summary>
    public IEmote Emote { get; }

    /// <inheritdoc/>
    public string CustomId { get; }

    /// <summary>
    ///     Gets the URL for a <see cref="ButtonStyle.Link"/> button.
    /// </summary>
    /// <remarks>
    ///     You cannot have a button with a <b>URL</b> and a <b>CustomId</b>.
    /// </remarks>
    public string Url { get; }

    /// <summary>
    ///     Gets whether this button is disabled or not.
    /// </summary>
    public bool IsDisabled { get; }

    /// <summary>
    ///     Gets the id of the sku associated with the current button.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the button is not of type <see cref="ButtonStyle.Premium"/>.
    /// </remarks>
    public ulong? SkuId { get; }

    /// <summary>
    ///     Turns this button into a button builder.
    /// </summary>
    /// <returns>
    ///     A newly created button builder with the same properties as this button.
    /// </returns>
    public ButtonBuilder ToBuilder()
        => new (Label, CustomId, Style, Url, Emote, IsDisabled);

    internal ButtonComponent(ButtonStyle style, string label, IEmote emote, string customId, string url, bool isDisabled, ulong? skuId)
    {
        Style = style;
        Label = label;
        Emote = emote;
        CustomId = customId;
        Url = url;
        IsDisabled = isDisabled;
        SkuId = skuId;
    }
}
