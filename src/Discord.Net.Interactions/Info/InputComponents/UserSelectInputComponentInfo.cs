using Discord.Interactions.Builders.Modals.Inputs;

namespace Discord.Interactions.Info.InputComponents;

public class UserSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    public UserSelectInputComponentInfo(UserSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
