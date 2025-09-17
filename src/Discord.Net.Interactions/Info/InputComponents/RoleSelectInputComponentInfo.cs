using Discord.Interactions.Builders.Modals.Inputs;

namespace Discord.Interactions.Info.InputComponents;

public class RoleSelectInputComponentInfo : SnowflakeSelectInputComponentInfo
{
    public RoleSelectInputComponentInfo(RoleSelectInputComponentBuilder builder, ModalInfo modal) : base(builder, modal) { }
}
