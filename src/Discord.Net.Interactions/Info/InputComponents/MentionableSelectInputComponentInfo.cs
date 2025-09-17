using Discord.Interactions.Builders.Modals.Inputs;

namespace Discord.Interactions.Info.InputComponents;

public class MentionableSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    public MentionableSelectInputComponentInfo(MentionableSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
