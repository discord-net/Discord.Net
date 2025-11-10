namespace Discord.Interactions.Builders;

/// <summary>
///     Represents a builder for creating a <see cref="MentionableSelectInputComponentInfo"/>.
/// </summary>
public class MentionableSelectInputComponentBuilder : SnowflakeSelectInputComponentBuilder<MentionableSelectInputComponentInfo, MentionableSelectInputComponentBuilder>
{
    protected override MentionableSelectInputComponentBuilder Instance => this;

    /// <summary>
    ///     Initialize a new <see cref="MentionableSelectInputComponentBuilder"/>.
    /// </summary>
    /// <param name="modal">Parent modal of this input component.</param>
    public MentionableSelectInputComponentBuilder(ModalBuilder modal) : base(modal, ComponentType.MentionableSelect) { }

    /// <summary>
    ///     Adds a snowflake ID as a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="id">The ID to add as a default value.</param>
    /// <param name="type">Enitity type of the snowflake ID.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectInputComponentBuilder AddDefaultValue(ulong id, SelectDefaultValueType type)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(id, type));
        return this;
    }

    /// <summary>
    ///     Adds a user as a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="user">The user to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectInputComponentBuilder AddDefaultValue(IUser user)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(user.Id, SelectDefaultValueType.User));
        return this;
    }

    /// <summary>
    ///     Adds a channel as a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="channel">The channel to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectInputComponentBuilder AddDefaultValue(IChannel channel)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(channel.Id, SelectDefaultValueType.Channel));
        return this;
    }

    /// <summary>
    ///     Adds a role as a default value to <see cref="SnowflakeSelectInputComponentBuilder{TInfo, TBuilder}.DefaultValues"/>.
    /// </summary>
    /// <param name="role">The role to add as a default value.</param>
    /// <returns>
    ///     The builder instance.
    /// </returns>
    public MentionableSelectInputComponentBuilder AddDefaulValue(IRole role)
    {
        _defaultValues.Add(new SelectMenuDefaultValue(role.Id, SelectDefaultValueType.Role));
        return this;
    }

    internal override MentionableSelectInputComponentInfo Build(ModalInfo modal)
        => new(this, modal);
}
