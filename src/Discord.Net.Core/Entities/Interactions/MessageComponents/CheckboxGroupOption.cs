namespace Discord;

/// <summary>
///     
/// </summary>
public struct CheckboxGroupOption
{
    /// <summary>
    /// 
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 
    /// </summary>
    public string Label { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>    
    public string Description { get; }

    /// <summary>
    ///     
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
