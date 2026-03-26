using System;

namespace Discord.Interactions;

/// <summary>
///     Adds a checkbox group option to the marked field.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class ModalCheckboxGroupOptionAttribute : Attribute
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
    public bool DefaultState { get; set; }

    /// <summary>
    ///     Create a new <see cref="ModalRadioGroupOptionAttribute"/>.
    /// </summary>
    /// <param name="label">Label of the option.</param>
    /// <param name="value">Value of the option.</param>
    /// <param name="description">Description of the option.</param>
    /// <param name="defaultState">Whether the option is selected by default.</param>
    public ModalCheckboxGroupOptionAttribute(string value, string label, string description = null, bool defaultState = false)
    {
        Value = value;
        Label = label;
        Description = description;
        DefaultState = defaultState;
    }
}
