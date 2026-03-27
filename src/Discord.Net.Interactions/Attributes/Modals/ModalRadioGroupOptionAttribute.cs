using System;

namespace Discord.Interactions;

/// <summary>
///     Adds a radio group option to the marked field.
/// </summary>
/// <remarks>
///     To add additional metadata to enum fields, use <see cref="EnumOptionAttribute"/> instead.
/// </remarks>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalRadioGroupOptionAttribute : Attribute
{
    /// <summary>
    ///     Gets the label of the option.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the value of the option.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets or sets the description of the option.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets whether the option is selected by default.
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    ///     Create a new <see cref="ModalRadioGroupOptionAttribute"/>.
    /// </summary>
    /// <param name="label">Label of the option.</param>
    /// <param name="value">Value of the option.</param>
    /// <param name="description">Description of the option.</param>
    /// <param name="isDefault">Whether the option is selected by default.</param>
    public ModalRadioGroupOptionAttribute(string label, string value, string description = null, bool isDefault = false)
    {
        Value = value;
        Label = label;
        Description = description;
        IsDefault = isDefault;
    }
}
