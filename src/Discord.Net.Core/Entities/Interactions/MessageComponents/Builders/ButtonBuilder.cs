using Discord.Utils;

using System;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="ButtonComponent"/>'s.
/// </summary>
public class ButtonBuilder
{
    /// <summary>
    ///     The max length of a <see cref="ButtonComponent.Label"/>.
    /// </summary>
    public const int MaxButtonLabelLength = 80;

    /// <summary>
    ///     Gets or sets the label of the current button.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxButtonLabelLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxButtonLabelLength"/>.</exception>
    public string Label
    {
        get => _label;
        set => _label = value?.Length switch
        {
            > MaxButtonLabelLength => throw new ArgumentOutOfRangeException(nameof(value), $"Label length must be less or equal to {MaxButtonLabelLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Label length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the custom id of the current button.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length exceeds <see cref="ComponentBuilder.MaxCustomIdLength"/></exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="CustomId"/> length subceeds 1.</exception>
    public string CustomId
    {
        get => _customId;
        set => _customId = value?.Length switch
        {
            > ComponentBuilder.MaxCustomIdLength => throw new ArgumentOutOfRangeException(nameof(value), $"Custom Id length must be less or equal to {ComponentBuilder.MaxCustomIdLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Custom Id length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the <see cref="ButtonStyle"/> of the current button.
    /// </summary>
    public ButtonStyle Style { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="IEmote"/> of the current button.
    /// </summary>
    public IEmote Emote { get; set; }

    /// <summary>
    ///     Gets or sets the url of the current button.
    /// </summary>
    public string Url { get; set; }

    /// <summary>
    ///     Gets or sets whether the current button is disabled.
    /// </summary>
    public bool IsDisabled { get; set; }

    /// <summary>
    ///     Gets or sets the id of the sku associated with the current button.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if the button is not of type <see cref="ButtonStyle.Premium"/>.
    /// </remarks>
    public ulong? SkuId { get; set; }

    private string _label;
    private string _customId;

    /// <summary>
    ///     Creates a new instance of a <see cref="ButtonBuilder"/>.
    /// </summary>
    public ButtonBuilder() { }

    /// <summary>
    ///     Creates a new instance of a <see cref="ButtonBuilder"/>.
    /// </summary>
    /// <param name="label">The label to use on the newly created link button.</param>
    /// <param name="url">The url of this button.</param>
    /// <param name="customId">The custom ID of this button.</param>
    /// <param name="style">The custom ID of this button.</param>
    /// <param name="emote">The emote of this button.</param>
    /// <param name="isDisabled">Disabled this button or not.</param>
    /// <param name="skuId">The sku id of this button.</param>
    public ButtonBuilder(string label = null, string customId = null, ButtonStyle style = ButtonStyle.Primary, string url = null, IEmote emote = null, bool isDisabled = false, ulong? skuId = null)
    {
        CustomId = customId;
        Style = style;
        Url = url;
        Label = label;
        IsDisabled = isDisabled;
        Emote = emote;
        SkuId = skuId;
    }

    /// <summary>
    ///     Creates a new instance of a <see cref="ButtonBuilder"/> from instance of a <see cref="ButtonComponent"/>.
    /// </summary>
    public ButtonBuilder(ButtonComponent button)
    {
        CustomId = button.CustomId;
        Style = button.Style;
        Url = button.Url;
        Label = button.Label;
        IsDisabled = button.IsDisabled;
        Emote = button.Emote;
        SkuId = button.SkuId;
    }

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Link"/> style.
    /// </summary>
    /// <param name="label">The label for this link button.</param>
    /// <param name="url">The url for this link button to go to.</param>
    /// <param name="emote">The emote for this link button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreateLinkButton(string label, string url, IEmote emote = null)
        => new (label, null, ButtonStyle.Link, url, emote: emote);

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Danger"/> style.
    /// </summary>
    /// <param name="label">The label for this danger button.</param>
    /// <param name="customId">The custom id for this danger button.</param>
    /// <param name="emote">The emote for this danger button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreateDangerButton(string label, string customId, IEmote emote = null)
        => new (label, customId, ButtonStyle.Danger, emote: emote);

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Primary"/> style.
    /// </summary>
    /// <param name="label">The label for this primary button.</param>
    /// <param name="customId">The custom id for this primary button.</param>
    /// <param name="emote">The emote for this primary button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreatePrimaryButton(string label, string customId, IEmote emote = null)
        => new (label, customId, emote: emote);

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Secondary"/> style.
    /// </summary>
    /// <param name="label">The label for this secondary button.</param>
    /// <param name="customId">The custom id for this secondary button.</param>
    /// <param name="emote">The emote for this secondary button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreateSecondaryButton(string label, string customId, IEmote emote = null)
        => new (label, customId, ButtonStyle.Secondary, emote: emote);

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Success"/> style.
    /// </summary>
    /// <param name="label">The label for this success button.</param>
    /// <param name="customId">The custom id for this success button.</param>
    /// <param name="emote">The emote for this success button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreateSuccessButton(string label, string customId, IEmote emote = null)
        => new (label, customId, ButtonStyle.Success, emote: emote);

    /// <summary>
    ///     Creates a button with the <see cref="ButtonStyle.Premium"/> style.
    /// </summary>
    /// <param name="label">The label for this premium button.</param>
    /// <param name="skuId">The sku id for this premium button.</param>
    /// <param name="emote">The emote for this premium button.</param>
    /// <returns>A builder with the newly created button.</returns>
    public static ButtonBuilder CreatePremiumButton(string label, ulong skuId, IEmote emote = null)
        => new (label, style: ButtonStyle.Success, emote: emote, skuId: skuId);

    /// <summary>
    ///     Sets the current buttons label to the specified text.
    /// </summary>
    /// <param name="label">The text for the label.</param>
    /// <inheritdoc cref="Label"/>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the current buttons style.
    /// </summary>
    /// <param name="style">The style for this builders button.</param>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithStyle(ButtonStyle style)
    {
        Style = style;
        return this;
    }

    /// <summary>
    ///     Sets the current buttons emote.
    /// </summary>
    /// <param name="emote">The emote to use for the current button.</param>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithEmote(IEmote emote)
    {
        Emote = emote;
        return this;
    }

    /// <summary>
    ///     Sets the current buttons url.
    /// </summary>
    /// <param name="url">The url to use for the current button.</param>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithUrl(string url)
    {
        Url = url;
        return this;
    }

    /// <summary>
    ///     Sets the custom id of the current button.
    /// </summary>
    /// <param name="id">The id to use for the current button.</param>
    /// <inheritdoc cref="CustomId"/>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithCustomId(string id)
    {
        CustomId = id;
        return this;
    }

    /// <summary>
    ///     Sets whether the current button is disabled.
    /// </summary>
    /// <param name="isDisabled">Whether the current button is disabled or not.</param>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithDisabled(bool isDisabled)
    {
        IsDisabled = isDisabled;
        return this;
    }

    /// <summary>
    ///     Sets the sku id of the current button.
    /// </summary>
    /// <param name="skuId">The id of the sku</param>
    /// <returns>The current builder.</returns>
    public ButtonBuilder WithSkuId(ulong? skuId)
    {
        SkuId = skuId;
        return this;
    }

    /// <summary>
    ///     Builds this builder into a <see cref="ButtonComponent"/> to be used in a <see cref="ComponentBuilder"/>.
    /// </summary>
    /// <returns>A <see cref="ButtonComponent"/> to be used in a <see cref="ComponentBuilder"/>.</returns>
    /// <exception cref="InvalidOperationException">A button must contain either a <see cref="Url"/> or a <see cref="CustomId"/>, but not both.</exception>
    /// <exception cref="InvalidOperationException">A button must have an <see cref="Emote"/> or a <see cref="Label"/>.</exception>
    /// <exception cref="InvalidOperationException">A link button must contain a URL.</exception>
    /// <exception cref="InvalidOperationException">A URL must include a protocol (http or https).</exception>
    /// <exception cref="InvalidOperationException">A non-link button must contain a custom id</exception>
    public ButtonComponent Build()
    {
        var a = 0;
        if (!string.IsNullOrWhiteSpace(Url))
            a++;
        if (!string.IsNullOrWhiteSpace(CustomId))
            a++;
        if (SkuId is not null)
            a++;

        if (a is 0 or > 1)
            throw new InvalidOperationException("A button must contain either a URL, CustomId or SkuId, but not multiple of them!");

        switch (Style)
        {
            case 0:
            {
                throw new ArgumentException("A button must have a style.", nameof(Style));
            }

            case ButtonStyle.Primary:
            case ButtonStyle.Secondary:
            case ButtonStyle.Success:
            case ButtonStyle.Danger:
            {
                if (string.IsNullOrWhiteSpace(Label) && Emote is null)
                    throw new InvalidOperationException("A button must have an Emote or a label!");
                if (string.IsNullOrWhiteSpace(CustomId))
                    throw new InvalidOperationException("Non-link and non-premium buttons must have a custom id associated with them");

            }
            break;

            case ButtonStyle.Link:
            {
                if (string.IsNullOrWhiteSpace(Label) && Emote is null)
                    throw new InvalidOperationException("A button must have an Emote or a label!");
                if (string.IsNullOrWhiteSpace(Url))
                    throw new InvalidOperationException("Link buttons must have a link associated with them");
                UrlValidation.ValidateButton(Url);
            }
            break;

            case ButtonStyle.Premium:
            {
                if (SkuId is null)
                    throw new InvalidOperationException("Premium buttons must have a sku id associated with them");
            }
            break;
        }

        return new ButtonComponent(Style, Label, Emote, CustomId, Url, IsDisabled, SkuId);
    }
}
