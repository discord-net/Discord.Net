namespace Discord;

/// <summary>
///     Represents an option for a <see cref="ComponentType.CheckboxGroup"/> component.
/// </summary>
public struct CheckboxGroupOption
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
    public bool DefaultState { get; }

    internal CheckboxGroupOption(string value, string label, string description = null, bool defaultState = false)
    {
        Value = value;
        Label = label;
        Description = description;
        DefaultState = defaultState;
    }
}
