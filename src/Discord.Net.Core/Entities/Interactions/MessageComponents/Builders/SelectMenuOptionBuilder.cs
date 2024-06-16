using System;

namespace Discord;

/// <summary>
///     Represents a class used to build <see cref="SelectMenuOption"/>'s.
/// </summary>
public class SelectMenuOptionBuilder
{
    /// <summary>
    ///     The maximum length of a <see cref="SelectMenuOption.Label"/>.
    /// </summary>
    public const int MaxSelectLabelLength = 100;

    /// <summary>
    ///     The maximum length of a <see cref="SelectMenuOption.Description"/>.
    /// </summary>
    public const int MaxDescriptionLength = 100;

    /// <summary>
    ///     The maximum length of a <see cref="SelectMenuOption.Value"/>.
    /// </summary>
    public const int MaxSelectValueLength = 100;

    /// <summary>
    ///     Gets or sets the label of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length exceeds <see cref="MaxSelectLabelLength"/></exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Label"/> length subceeds 1.</exception>
    public string Label
    {
        get => _label;
        set => _label = value?.Length switch
        {
            > MaxSelectLabelLength => throw new ArgumentOutOfRangeException(nameof(value), $"Label length must be less or equal to {MaxSelectLabelLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Label length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the value of the current select menu.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length exceeds <see cref="MaxSelectValueLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Value"/> length subceeds 1.</exception>
    public string Value
    {
        get => _value;
        set => _value = value?.Length switch
        {
            > MaxSelectValueLength => throw new ArgumentOutOfRangeException(nameof(value), $"Value length must be less or equal to {MaxSelectValueLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Value length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets this menu options description.
    /// </summary>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length exceeds <see cref="MaxDescriptionLength"/>.</exception>
    /// <exception cref="ArgumentException" accessor="set"><see cref="Description"/> length subceeds 1.</exception>
    public string Description
    {
        get => _description;
        set => _description = value?.Length switch
        {
            > MaxDescriptionLength => throw new ArgumentOutOfRangeException(nameof(value), $"Description length must be less or equal to {MaxDescriptionLength}."),
            0 => throw new ArgumentOutOfRangeException(nameof(value), "Description length must be at least 1."),
            _ => value
        };
    }

    /// <summary>
    ///     Gets or sets the emote of this option.
    /// </summary>
    public IEmote Emote { get; set; }

    /// <summary>
    ///     Gets or sets the whether or not this option will render selected by default.
    /// </summary>
    public bool? IsDefault { get; set; }

    private string _label;
    private string _value;
    private string _description;

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/>.
    /// </summary>
    public SelectMenuOptionBuilder() { }

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/>.
    /// </summary>
    /// <param name="label">The label for this option.</param>
    /// <param name="value">The value of this option.</param>
    /// <param name="description">The description of this option.</param>
    /// <param name="emote">The emote of this option.</param>
    /// <param name="isDefault">Render this option as selected by default or not.</param>
    public SelectMenuOptionBuilder(string label, string value, string description = null, IEmote emote = null, bool? isDefault = null)
    {
        Label = label;
        Value = value;
        Description = description;
        Emote = emote;
        IsDefault = isDefault;
    }

    /// <summary>
    ///     Creates a new instance of a <see cref="SelectMenuOptionBuilder"/> from instance of a <see cref="SelectMenuOption"/>.
    /// </summary>
    public SelectMenuOptionBuilder(SelectMenuOption option)
    {
        Label = option.Label;
        Value = option.Value;
        Description = option.Description;
        Emote = option.Emote;
        IsDefault = option.IsDefault;
    }

    /// <summary>
    ///     Sets the field label.
    /// </summary>
    /// <param name="label">The value to set the field label to.</param>
    /// <inheritdoc cref="Label"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuOptionBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the field value.
    /// </summary>
    /// <param name="value">The value to set the field value to.</param>
    /// <inheritdoc cref="Value"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuOptionBuilder WithValue(string value)
    {
        Value = value;
        return this;
    }

    /// <summary>
    ///     Sets the field description.
    /// </summary>
    /// <param name="description">The value to set the field description to.</param>
    /// <inheritdoc cref="Description"/>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuOptionBuilder WithDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    ///     Sets the field emote.
    /// </summary>
    /// <param name="emote">The value to set the field emote to.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuOptionBuilder WithEmote(IEmote emote)
    {
        Emote = emote;
        return this;
    }

    /// <summary>
    ///     Sets the field default.
    /// </summary>
    /// <param name="isDefault">The value to set the field default to.</param>
    /// <returns>
    ///     The current builder.
    /// </returns>
    public SelectMenuOptionBuilder WithDefault(bool isDefault)
    {
        IsDefault = isDefault;
        return this;
    }

    /// <summary>
    ///     Builds a <see cref="SelectMenuOption"/>.
    /// </summary>
    /// <returns>The newly built <see cref="SelectMenuOption"/>.</returns>
    public SelectMenuOption Build()
    {
        if (string.IsNullOrWhiteSpace(Label))
            throw new ArgumentNullException(nameof(Label), "Option must have a label.");

        Preconditions.AtMost(Label.Length, MaxSelectLabelLength, nameof(Label), $"Label length must be less or equal to {MaxSelectLabelLength}.");

        if (string.IsNullOrWhiteSpace(Value))
            throw new ArgumentNullException(nameof(Value), "Option must have a value.");

        Preconditions.AtMost(Value.Length, MaxSelectValueLength, nameof(Value), $"Value length must be less or equal to {MaxSelectValueLength}.");

        return new SelectMenuOption(Label, Value, Description, Emote, IsDefault);
    }
}
