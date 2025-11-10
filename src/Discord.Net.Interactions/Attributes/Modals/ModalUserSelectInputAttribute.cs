namespace Discord.Interactions.Attributes.Modals;

/// <summary>
///     Marks a <see cref="IModal"/> property as a user select input.
/// </summary>
public class ModalUserSelectInputAttribute : ModalSelectInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.UserSelect;

    /// <summary>
    ///     Create a new <see cref="ModalUserSelectInputAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the user select component.</param>
    /// <param name="minValues">Minimum number of values that can be selected.</param>
    /// <param name="maxValues">Maximum number of values that can be selected.</param>
    public ModalUserSelectInputAttribute(string customId, int minValues = 1, int maxValues = 1) : base(customId, minValues, maxValues)
    {
    }
}
