namespace Discord.Interactions.Attributes.Modals;

public class ModalChannelSelectInputAttribute : SelectInputAttribute
{
    public override ComponentType ComponentType => ComponentType.ChannelSelect;

    public ModalChannelSelectInputAttribute(string customId) : base(customId)
    {
    }
}
