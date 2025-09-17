using Discord.Interactions.Info.InputComponents;

namespace Discord.Interactions.Builders.Modals.Inputs;

public class MentionableSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<MentionableSelectInputComponentInfo, MentionableSelectInputComponentBuilder>
{
    protected override MentionableSelectInputComponentBuilder Instance => this;

    public MentionableSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.MentionableSelect) { }

    public MentionableSelectInputComponentBuilder AddDefaultValue(ulong id, SelectDefaultValueType type)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(id, type));
        return this;
    }

    public MentionableSelectInputComponentBuilder AddDefaultValue(IUser user)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(user.Id, SelectDefaultValueType.User));
        return this;
    }

    public MentionableSelectInputComponentBuilder AddDefaultValue(IChannel channel)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channel.Id, SelectDefaultValueType.Channel));
        return this;
    }

    public MentionableSelectInputComponentBuilder AddDefaulValue(IRole role)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(role.Id, SelectDefaultValueType.Role));
        return this;
    }

    internal override MentionableSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
