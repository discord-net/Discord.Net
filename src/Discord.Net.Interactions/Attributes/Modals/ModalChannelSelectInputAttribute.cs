namespace Discord.Interactions.Attributes.Modals;

/// <summary>
///     Marks a <see cref="IModal"/> property as a channel select.
/// </summary>
public class ModalChannelSelectInputAttribute : ModalSelectInputAttribute
{
    /// <inheritdoc/>
    public override ComponentType ComponentType => ComponentType.ChannelSelect;

    /// <summary>
    ///     Create a new <see cref="ModalChannelSelectInputAttribute"/>.
    /// </summary>
    /// <param name="customId">Custom ID of the channel select component.</param>
    public ModalChannelSelectInputAttribute(string customId) : base(customId)
    {
    }
}
