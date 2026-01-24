namespace Discord;

public struct RadioGroupOptionProperties
{
    /// <summary>
    ///     
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     
    /// </summary>
    public string Label { get; set; }

    /// <summary>
    ///     
    /// </summary>
    /// <remarks>
    ///     <see langword="null"/> if no description is set.
    /// </remarks>
    public string Description { get; set; }

    /// <summary>
    ///     
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
