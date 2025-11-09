namespace Discord.Interactions.Attributes.Modals;

public sealed class ModalSelectMenuInputAttribute : SelectInputAttribute
{
    public override ComponentType ComponentType => ComponentType.SelectMenu;

    public ModalSelectMenuInputAttribute(string customId) : base(customId)
    {

    }
}
