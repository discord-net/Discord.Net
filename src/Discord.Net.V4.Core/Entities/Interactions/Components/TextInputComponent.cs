using Discord.Models.Json;

namespace Discord;

/// <summary>
///     Represents a <see cref="IMessageComponent" /> text input.
/// </summary>
public class TextInputComponent : IMessageComponent
{
    internal TextInputComponent(string customId, string label, string? placeholder, int? minLength, int? maxLength,
        TextInputStyle style, bool? required, string? value)
    {
        CustomId = customId;
        Label = label;
        Placeholder = placeholder;
        MinLength = minLength;
        MaxLength = maxLength;
        Style = style;
        Required = required;
        Value = value;
    }

    /// <summary>
    ///     Gets the label of the component; this is the text shown above it.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the placeholder of the component.
    /// </summary>
    public string? Placeholder { get; }

    /// <summary>
    ///     Gets the minimum length of the inputted text.
    /// </summary>
    public int? MinLength { get; }

    /// <summary>
    ///     Gets the maximum length of the inputted text.
    /// </summary>
    public int? MaxLength { get; }

    /// <summary>
    ///     Gets the style of the component.
    /// </summary>
    public TextInputStyle Style { get; }

    /// <summary>
    ///     Gets whether users are required to input text.
    /// </summary>
    public bool? Required { get; }

    /// <summary>
    ///     Gets the default value of the component.
    /// </summary>
    public string? Value { get; }

    /// <inheritdoc />
    public ComponentType Type => ComponentType.TextInput;

    /// <inheritdoc />
    public string CustomId { get; }

    public MessageComponent ToApiModel(MessageComponent? existing = default)
    {
        return existing ?? new Models.Json.TextInputComponent()
        {
            Label = Label,
            Type = (uint)Type,
            CustomId = CustomId,
            Placeholder = Optional.FromNullable(Placeholder),
            Required = Optional.FromNullable(Required),
            Style = (int)Style,
            Value = Optional.FromNullable(Value),
            MaxLength = Optional.FromNullable(MaxLength),
            MinLength = Optional.FromNullable(MinLength)
        };
    }
}
