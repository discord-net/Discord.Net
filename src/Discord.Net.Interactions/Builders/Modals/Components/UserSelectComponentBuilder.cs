using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="UserSelectComponentInfo"/>.
/// </summary>
public class UserSelectComponentBuilder : SnowflakeSelectComponentBuilder<UserSelectComponentInfo, UserSelectComponentBuilder>
{
    protected override UserSelectComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="UserSelectComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public UserSelectComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.UserSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="userId">The user ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectComponentBuilder AddDefaulValue(ulong userId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(userId, SelectDefaultValueType.User));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="users">The users to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectComponentBuilder AddDefaultValues(params IEnumerable<IUser> users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    internal override UserSelectComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
