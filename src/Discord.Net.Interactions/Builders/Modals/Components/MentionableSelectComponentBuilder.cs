using System.Collections.Generic;
using System.Linq;

namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating a <see cref="MentionableSelectComponentInfo"/>.
/// </summary>
public class MentionableSelectComponentBuilder : SnowflakeSelectComponentBuilder<MentionableSelectComponentInfo, MentionableSelectComponentBuilder>
{
    protected override MentionableSelectComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="MentionableSelectComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public MentionableSelectComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.MentionableSelect) { }

    /// <summary>
    ///     Adds a snowflake ID as a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="id">The ID to add as a default value.</param>
    /// <param name="type">Enitity type of the snowflake ID.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectComponentBuilder AddDefaultValue(ulong id, SelectDefaultValueType type)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(id, type));
        return this;
    }

    /// <summary>
    ///     Add users as a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="users">The users to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectComponentBuilder AddDefaultValue(params IEnumerable<IUser> users)
    {
        _defaultValues.AddRange(users.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.User)));
        return this;
    }

    /// <summary>
    ///     Adds channels as a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channels">The channel to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectComponentBuilder AddDefaultValue(params IEnumerable<IChannel> channels)
    {
        _defaultValues.AddRange(channels.Select(x =>new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Channel)));
        return this;
    }

    /// <summary>
    ///     Adds roles as a default value to <see cref="SnowflakeSelectComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="roles">The role to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectComponentBuilder AddDefaulValue(params IEnumerable<IRole> roles)
    {
        _defaultValues.AddRange(roles.Select(x => new SelectMenuDefaultValue(x.Id, SelectDefaultValueType.Role)));
        return this;
    }

    internal override MentionableSelectComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
