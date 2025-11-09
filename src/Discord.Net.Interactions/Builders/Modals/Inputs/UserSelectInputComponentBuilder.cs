using Discord.Interactions.Info.InputComponents;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders.Modals.Inputs;

public class UserSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<UserSelectInputComponentInfo, UserSelectInputComponentBuilder>
{
    protected override UserSelectInputComponentBuilder Instance => this;

    public UserSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.UserSelect) { }

    public UserSelectInputComponentBuilder AddDefaulValue(IUser user)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(user.Id, SelectDefaultValueType.User));
        return this;
    }

    public UserSelectInputComponentBuilder AddDefaulValue(ulong userId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(userId, SelectDefaultValueType.User));
        return this;
    }

    public UserSelectInputComponentBuilder AddDefaultValues(params IUser[] users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    public UserSelectInputComponentBuilder AddDefaultValues(IEnumerable<IUser> users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    internal override UserSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
