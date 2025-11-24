using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating a <see cref="RoleSelectComponentInfo"/>.
/// </summary>
public class RoleSelectComponentBuilder : SnowflakeSelectComponentBuilder<RoleSelectComponentInfo, RoleSelectComponentBuilder>
{
    protected override RoleSelectComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="RoleSelectComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public RoleSelectComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.RoleSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="roleId">The role ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectComponentBuilder AddDefaulValue(ulong roleId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(roleId, SelectDefaultValueType.Role));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="roles">The roles to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public RoleSelectComponentBuilder AddDefaultValues(params IEnumerable<IRole> roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    internal override RoleSelectComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
