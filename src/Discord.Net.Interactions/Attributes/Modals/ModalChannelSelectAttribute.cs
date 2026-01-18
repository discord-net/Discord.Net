namespace Discord.Interactions;

/// <summary>
///     Marks a <see cref="IModal"/> property as a channel select.
/// </summary>
public class ModalChannelSelectAttribute : ModalSelectComponentAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.ChannelSelect;

    /// <summary>
    ///     Create a new <see cref="ModalChannelSelectAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the channel select component.</param>
    /// <param name="minValues">The minimum number of values that can be selected.</param>
    /// <param name="maxValues">The maximum number of values that can be selected.</param>
    /// <param name="id">Optional identifier for the component.</param>
    public ModalChannelSelectAttribute(string customId, int minValues = 1, int maxValues = 1, int? id = null)
        : base(customId, minValues, maxValues, id) { }
}
