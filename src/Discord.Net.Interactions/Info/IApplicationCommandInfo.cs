namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a <see cref="InteractionService"/> command that can be registered to Discord.
    /// </summary>
    public interface IApplicationCommandInfo
    {
        /// <summary>
        ///     Gets the name of this command.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Gets the type of this command.
        /// </summary>
        ApplicationCommandType CommandType { get; }

        /// <summary>
        ///     Gets the DefaultPermission of this command.
        /// </summary>
        bool DefaultPermission { get; }
    }
}
