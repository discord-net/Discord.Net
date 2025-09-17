using Discord.Interactions.Info.InputComponents;
using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders.Modals.Inputs;

public class RoleSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<RoleSelectInputComponentInfo, RoleSelectInputComponentBuilder>
{
    protected override RoleSelectInputComponentBuilder Instance => this;

    public RoleSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.RoleSelect) { }

    public RoleSelectInputComponentBuilder AddDefaulValue(IRole role)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(role.Id, SelectDefaultValueType.Role));
        return this;
    }

    public RoleSelectInputComponentBuilder AddDefaulValue(ulong roleId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(roleId, SelectDefaultValueType.Role));
        return this;
    }

    public RoleSelectInputComponentBuilder AddDefaultValues(params IRole[] roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    public RoleSelectInputComponentBuilder AddDefaultValues(IEnumerable<IRole> roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    internal override RoleSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
