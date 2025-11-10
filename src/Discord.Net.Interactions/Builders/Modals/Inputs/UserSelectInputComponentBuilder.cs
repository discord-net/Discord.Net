using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating <see cref="UserSelectInputComponentInfo"/>.
/// </summary>
public class UserSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<UserSelectInputComponentInfo, UserSelectInputComponentBuilder>
{
    protected override UserSelectInputComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="UserSelectInputComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public UserSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.UserSelect) { }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="user">The user to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectInputComponentBuilder AddDefaulValue(IUser user)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(user.Id, SelectDefaultValueType.User));
        return this;
    }

    /// <summary>
    ///     Adds a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="userId">The user ID to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectInputComponentBuilder AddDefaulValue(ulong userId)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(userId, SelectDefaultValueType.User));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="users">The users to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectInputComponentBuilder AddDefaultValues(params IUser[] users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    /// <summary>
    ///     Adds default values to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="users">The users to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public UserSelectInputComponentBuilder AddDefaultValues(IEnumerable<IUser> users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    internal override UserSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
