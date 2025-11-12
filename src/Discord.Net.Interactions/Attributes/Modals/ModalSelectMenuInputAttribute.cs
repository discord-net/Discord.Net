namespace Discord.Interactions.Attributes.Modals;

/// <summary>
///     Marks a <see cref="IModal"/> property as a select menu input.
/// </summary>
public sealed class ModalSelectMenuInputAttribute : ModalSelectInputAttribute
{
    /// <inheritdoc />
    public override ComponentType ComponentType => ComponentType.SelectMenu;

    /// <summary>
    ///     Create a new <see cref="ModalSelectMenuInputAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the select menu component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    public ModalSelectMenuInputAttribute(string customId, int minValues = 1, int maxValues = 1) : base(customId, minValues, maxValues)
    {
    }
}
