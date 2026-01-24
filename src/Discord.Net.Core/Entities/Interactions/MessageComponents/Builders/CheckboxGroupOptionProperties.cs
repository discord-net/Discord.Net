namespace Discord;

/// <summary>
/// 
/// </summary>
public struct CheckboxGroupOptionProperties(string value, string label, string description = null, bool defaultState = false)
{
    /// <summary>
    ///     
    /// </summary>
    public string Value { get; set; } = value;

    /// <summary>
    ///     
    /// </summary>
    public string Label { get; set; } = label;

    /// <summary>
    ///     
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>
    public string Description { get; set; } = description;

    /// <summary>
    ///     
    /// </summary>
    public bool DefaultState { get; set; } = defaultState;
}
