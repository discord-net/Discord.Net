using System;
using System.Linq;

namespace Discord;

/// <summary>
///     Represents a builder for creating a <see cref="TextInputComponent"/>.
/// </summary>
public class TextInputBuilder
{
    /// <summary>
    ///     The max length of a <see cref="TextInputComponent.Placeholder"/>.
    /// </summary>
    public const int MaxPlaceholderLength = 100;
    public const int LargestMaxLength = 4000;

    /// <summary>
    ///     Gets or sets the custom id of the current text input.
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
    ///     Gets or sets the style of the current text input.
    /// </summary>
    public TextInputStyle Style { get; set; } = TextInputStyle.Short;

    /// <summary>
    ///     Gets or sets the label of the current text input.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     Gets or sets the placeholder of the current text input.
    /// </summary>
    /// <exception cref="ArgumentException"><see cref="Placeholder"/> is longer than <see cref="MaxPlaceholderLength"/> characters</exception>
    public string Placeholder
    {
        get => _placeholder;
        set => _placeholder = (value?.Length ?? 0) <= MaxPlaceholderLength
            ? value
            : throw new ArgumentException($"Placeholder cannot have more than {MaxPlaceholderLength} characters.");
    }

    /// <summary>
    ///     Gets or sets the minimum length of the current text input.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is greater than <see cref="LargestMaxLength"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MinLength"/> is greater than <see cref="MaxLength"/>.</exception>
    public int? MinLength
    {
        get => _minLength;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must not be less than 0");
            if (value > LargestMaxLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must not be greater than {LargestMaxLength}");
            if (value > (MaxLength ?? LargestMaxLength))
                throw new ArgumentOutOfRangeException(nameof(value), $"MinLength must be less than MaxLength");
            _minLength = value;
        }
    }

    /// <summary>
    ///     Gets or sets the maximum length of the current text input.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is greater than <see cref="LargestMaxLength"/>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="MaxLength"/> is less than <see cref="MinLength"/>.</exception>
    public int? MaxLength
    {
        get => _maxLength;
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must not be less than 0");
            if (value > LargestMaxLength)
                throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength most not be greater than {LargestMaxLength}");
            if (value < (MinLength ?? -1))
                throw new ArgumentOutOfRangeException(nameof(value), $"MaxLength must be greater than MinLength ({MinLength})");
            _maxLength = value;
        }
    }

    /// <summary>
    ///     Gets or sets whether the user is required to input text.
    /// </summary>
    public bool? Required { get; set; }

    /// <summary>
    ///     Gets or sets the default value of the text input.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="Value"/>.Length is less than 0.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     <see cref="Value"/>.Length is greater than <see cref="LargestMaxLength"/> or <see cref="MaxLength"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    ///     <see cref="Style"/> is <see cref="TextInputStyle.Short"/> and <see cref="Value"/> contains a new line character.
    /// </exception>
    public string Value
    {
        get => _value;
        set
        {
            if (value?.Length > (MaxLength ?? LargestMaxLength))
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must not be longer than {MaxLength ?? LargestMaxLength}.");
            if (value?.Length < (MinLength ?? 0))
                throw new ArgumentOutOfRangeException(nameof(value), $"Value must not be shorter than {MinLength}");

            _value = value;
        }
    }

    private string _customId;
    private int? _maxLength;
    private int? _minLength;
    private string _placeholder;
    private string _value;

    /// <summary>
    ///     Creates a new instance of a <see cref="TextInputBuilder"/>.
    /// </summary>
    /// <param name="label">The text input's label.</param>
    /// <param name="style">The text input's style.</param>
    /// <param name="customId">The text input's custom id.</param>
    /// <param name="placeholder">The text input's placeholder.</param>
    /// <param name="minLength">The text input's minimum length.</param>
    /// <param name="maxLength">The text input's maximum length.</param>
    /// <param name="required">The text input's required value.</param>
    public TextInputBuilder(string label, string customId, TextInputStyle style = TextInputStyle.Short, string placeholder = null,
        int? minLength = null, int? maxLength = null, bool? required = null, string value = null)
    {
        Label = label;
        Style = style;
        CustomId = customId;
        Placeholder = placeholder;
        MinLength = minLength;
        MaxLength = maxLength;
        Required = required;
        Value = value;
    }

    /// <summary>
    ///     Creates a new instance of a <see cref="TextInputBuilder"/>.
    /// </summary>
    public TextInputBuilder()
    {

    }

    /// <summary>
    ///     Sets the label of the current builder.
    /// </summary>
    /// <param name="label">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithLabel(string label)
    {
        Label = label;
        return this;
    }

    /// <summary>
    ///     Sets the style of the current builder.
    /// </summary>
    /// <param name="style">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithStyle(TextInputStyle style)
    {
        Style = style;
        return this;
    }

    /// <summary>
    ///     Sets the custom id of the current builder.
    /// </summary>
    /// <param name="customId">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithCustomId(string customId)
    {
        CustomId = customId;
        return this;
    }

    /// <summary>
    ///     Sets the placeholder of the current builder.
    /// </summary>
    /// <param name="placeholder">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithPlaceholder(string placeholder)
    {
        Placeholder = placeholder;
        return this;
    }

    /// <summary>
    ///     Sets the value of the current builder.
    /// </summary>
    /// <param name="value">The value to set</param>
    /// <returns>The current builder.</returns>
    public TextInputBuilder WithValue(string value)
    {
        Value = value;
        return this;
    }

    /// <summary>
    ///     Sets the minimum length of the current builder.
    /// </summary>
    /// <param name="minLength">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithMinLength(int minLength)
    {
        MinLength = minLength;
        return this;
    }

    /// <summary>
    ///     Sets the maximum length of the current builder.
    /// </summary>
    /// <param name="maxLength">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithMaxLength(int maxLength)
    {
        MaxLength = maxLength;
        return this;
    }

    /// <summary>
    ///     Sets the required value of the current builder.
    /// </summary>
    /// <param name="required">The value to set.</param>
    /// <returns>The current builder. </returns>
    public TextInputBuilder WithRequired(bool required)
    {
        Required = required;
        return this;
    }

    public TextInputComponent Build()
    {
        if (string.IsNullOrEmpty(CustomId))
            throw new ArgumentException("TextInputComponents must have a custom id.", nameof(CustomId));
        if (string.IsNullOrWhiteSpace(Label))
            throw new ArgumentException("TextInputComponents must have a label.", nameof(Label));
        if (Style is TextInputStyle.Short && Value?.Any(x => x == '\n') is true)
            throw new ArgumentException($"Value must not contain new line characters when style is {TextInputStyle.Short}.", nameof(Value));

        return new TextInputComponent(CustomId, Label, Placeholder, MinLength, MaxLength, Style, Required, Value);
    }
}
