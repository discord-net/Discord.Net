using System;

namespace Discord.Interactions;

/// <summary>
///     Adds a select menu option to the marked field.
/// </summary>
/// <remarks>
///     To add additional metadata to enum fields, use <see cref="SelectMenuOptionAttribute"/> instead.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalSelectMenuOptionAttribute : Attribute
{
    /// <summary>
    ///     Gets the label of the option.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets or sets the description of the option.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets the value of the option.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets or sets the emote of the option.
    /// </summary>
    public string Emote { get; set; }

    /// <summary>
    ///     Gets or sets whether the option is selected by default.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    ///     Create a new <see cref="ModalSelectComponentAttribute"/>.
    /// </summary>
    /// <param name="label">Label of the option.</param>
    /// <param name="value">Value of the option.</param>
    /// <param name="description">Description of the option.</param>
    /// <param name="emote">Emote of the option.</param>
    /// <param name="isDefault">Whether the option is selected by default</param>
    public ModalSelectMenuOptionAttribute(string label, string value, string description = null, string emote = null, bool isDefault = false)
    {
        Label = label;
        Value = value;
        Description = description;
        Emote = emote;
        IsDefault = isDefault;
    }
}
