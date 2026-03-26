using System;

namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a checkbox input.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalCheckboxAttribute : ModalInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.Checkbox;

    /// <summary>
    ///     Create a new <see cref="ModalCheckboxGroupAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the select menu component.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalCheckboxAttribute(string customId, int id = 0)
        : base(customId, id)
    {
    }
}
