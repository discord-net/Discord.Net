using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating a <see cref="RoleSelectInputComponentInfo"/>.
/// </summary>
public class RoleSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<RoleSelectInputComponentInfo, RoleSelectInputComponentBuilder>
{
    protected override RoleSelectInputComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="RoleSelectInputComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public RoleSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.RoleSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="role">The role to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectInputComponentBuilder AddDefaulValue(IRole role)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(role.Id, SelectDefaultValueType.Role));
        return this;
    }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="roleId">The role ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectInputComponentBuilder AddDefaulValue(ulong roleId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(roleId, SelectDefaultValueType.Role));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="role">The roles to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectInputComponentBuilder AddDefaultValues(params IRole[] roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="role">The roles to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectInputComponentBuilder AddDefaultValues(IEnumerable<IRole> roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    internal override RoleSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
