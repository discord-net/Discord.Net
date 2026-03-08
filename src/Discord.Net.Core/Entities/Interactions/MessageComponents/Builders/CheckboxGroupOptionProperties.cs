namespace Discord;

/// <summary>
///     Represents the properties of an option for a <see cref="ComponentType.CheckboxGroup"/> component, used when building a checkbox group with <see cref="RadioGroupBuilder"/>.
/// </summary>
public struct CheckboxGroupOptionProperties(string value, string label, string description = null, bool defaultState = false)
{
    /// <summary>
    ///     Gets or sets the value of the option.
    /// </summary>
    public string Value { get; set; } = value;

    /// <summary>
    ///     Gets or sets the label of the option.
    /// </summary>
    public string Label { get; set; } = label;

    /// <summary>
    ///     Gets or sets the description of the option.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>
    public string Description { get; set; } = description;

    /// <summary>
    ///     Gets or sets a value indicating whether the option is selected by default.
    /// </summary>
    public bool DefaultState { get; set; } = defaultState;
}
