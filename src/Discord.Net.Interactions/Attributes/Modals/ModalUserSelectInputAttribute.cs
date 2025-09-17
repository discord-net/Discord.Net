namespace Discord.Interactions.Attributes.Modals;

public class ModalUserSelectInputAttribute : SelectInputAttribute
{
    public override ComponentType ComponentType => ComponentType.UserSelect;

    public ModalUserSelectInputAttribute(string customId) : base(customId)
    {
    }
}
