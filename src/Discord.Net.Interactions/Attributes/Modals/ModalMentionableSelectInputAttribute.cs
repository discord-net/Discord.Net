namespace Discord.Interactions.Attributes.Modals;

public class ModalMentionableSelectInputAttribute : SelectInputAttribute
{
    public override ComponentType ComponentType => ComponentType.MentionableSelect;

    public ModalMentionableSelectInputAttribute(string customId) : base(customId)
    {
    }
}
