namespace Discord.Interactions.Attributes.Modals;

public class ModalRoleSelectInputAttribute : SelectInputAttribute
{
    public override ComponentType ComponentType => ComponentType.RoleSelect;

    public ModalRoleSelectInputAttribute(string customId) : base(customId)
    {
    }
}
