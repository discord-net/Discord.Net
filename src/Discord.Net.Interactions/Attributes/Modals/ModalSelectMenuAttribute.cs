namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a select menu input.
/// </summary>
public sealed class ModalSelectMenuAttribute : ModalSelectComponentAttribute
{
    /// <inheritdoc />
    public override ComponentType ComponentType => ComponentType.SelectMenu;

    /// <summary>
    ///     Create a new <see cref="ModalSelectMenuAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the select menu component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalSelectMenuAttribute(string customId, int minValues = 1, int maxValues = 1, int id = 0)
        : base(customId, minValues, maxValues, id) { }
}
