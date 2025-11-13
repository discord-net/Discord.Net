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
    public ModalChannelSelectAttribute(string customId, int minValues = 1, int maxValues = 1) : base(customId, minValues, maxValues) { }
}
