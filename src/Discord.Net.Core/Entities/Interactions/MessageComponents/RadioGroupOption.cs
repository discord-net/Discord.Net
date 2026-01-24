namespace Discord;

/// <summary>
///     
/// </summary>
public readonly struct RadioGroupOption
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
    public bool IsDefault { get; }

    internal RadioGroupOption(string value, string label, string description = null, bool isDefault = false)
    {
        Value = value;
        Label = label;
        Description = description;
        IsDefault = isDefault;
    }
}
