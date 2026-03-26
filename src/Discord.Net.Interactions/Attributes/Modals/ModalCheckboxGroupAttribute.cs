using System;

namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a checkbox group input.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalCheckboxGroupAttribute : ModalInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.CheckboxGroup;

    /// <summary>
    ///     Gets or sets the minimum number of values that can be selected.
    /// </summary>
    public int MinValues { get; set; } = 1;

    /// <summary>
    ///     Gets or sets the maximum number of values that can be selected.
    /// </summary>
    public int MaxValues { get; set; } = 1;

    /// <summary>
    ///     Create a new <see cref="ModalCheckboxGroupAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the select menu component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalCheckboxGroupAttribute(string customId, int minValues = 1, int maxValues = 1, int id = 0)
        : base(customId, id)
    {
        MinValues = minValues;
        MaxValues = maxValues;
    }
}
