namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a user select input.
/// </summary>
public class ModalUserSelectAttribute : ModalSelectComponentAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.UserSelect;

    /// <summary>
    ///     Create a new <see cref="ModalUserSelectAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the user select component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    /// <param name="id">The optional identifier for the component.</param>
    public ModalUserSelectAttribute(string customId, int minValues = 1, int maxValues = 1, int id = 0)
        : base(customId, minValues, maxValues, id) { }
}
