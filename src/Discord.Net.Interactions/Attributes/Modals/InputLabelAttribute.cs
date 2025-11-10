using System;

namespace Discord.Interactions;

/// <summary>
///     Creates a custom label for an modal input.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class InputLabelAttribute : Attribute
{
    /// <summary>
    ///     Gets the label of the input.
    /// </summary>
    public string Label { get; }

    /// <summary>
    ///     Gets the label description of the input.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    ///     Creates a custom label for an modal input.
    /// </summary>
    /// <param name="label">The label of the input.</param>
    /// <param name="description">The label description of the input.</param>
    public InputLabelAttribute(string label, string description = null)
    {
        Label = label;
        Description = description;
    }
}
