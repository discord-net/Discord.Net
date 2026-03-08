namespace Discord;

/// <summary>
///     Represents an option for a <see cref="ComponentType.RadioGroup"/> component.
/// </summary>
public readonly struct RadioGroupOption
{
    /// <summary>
    ///     Gets the value of the option.
    /// </summary>
    public string Value { get; }

    /// <summary>
    ///     Gets the label of the option.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the description of the option.
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>    
    public string Description { get; }

    /// <summary>
    ///     Gets a value indicating whether the option is selected by default.
    /// </summary>
    public bool IsDefault { get; }

    internal RadioGroupOption(string value, string label, string description = null, bool isDefault = false)
    {
        Value = value;
        Label = label;
        Description = description;
        IsDefault = isDefault;
    }
}
