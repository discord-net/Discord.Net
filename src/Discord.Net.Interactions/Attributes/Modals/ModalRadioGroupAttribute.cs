using System;

namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a radio group input.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
public class ModalRadioGroupAttribute : ModalInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.RadioGroup;

    /// <summary>
    ///     Create a new <see cref="ModalCheckboxGroupAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the select menu component.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalRadioGroupAttribute(string customId, int id = 0)
        : base(customId, id)
    {
    }
}
