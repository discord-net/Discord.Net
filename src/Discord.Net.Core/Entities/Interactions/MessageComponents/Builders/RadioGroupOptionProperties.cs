namespace Discord;

/// <summary>
///     Represents the properties of an option for a <see cref="ComponentType.RadioGroup"/> component, used when building a radio group with <see cref="RadioGroupBuilder"/>.
/// </summary>
public struct RadioGroupOptionProperties
{
    /// <summary>
    ///     Gets or sets the value of the option.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Gets or sets the label of the option.
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     Gets or sets the description of the option.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the option is selected by default.
    /// </summary>
    public bool IsDefault { get; set; }

    public RadioGroupOptionProperties(string value, string label, string description = null, bool isDefault = false)
    {
        Value = value;
        Label = label;
        Description = description;
        IsDefault = isDefault;
    }
}
