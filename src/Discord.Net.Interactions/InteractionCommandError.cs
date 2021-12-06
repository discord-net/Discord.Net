namespace Discord.Interactions
{
    /// <summary>
    ///     Defines the type of error a command can throw.
    /// </summary>
    public enum InteractionCommandError
    {
        /// <summary>
        ///     Thrown when the command is unknown.
        /// </summary>
        UnknownCommand,

        /// <summary>
        ///     Thrown when the Slash Command parameter fails to be converted by a TypeReader.
        /// </summary>
        ConvertFailed,

        /// <summary>
        ///     Thrown when the input text has too few or too many arguments.
        /// </summary>
        BadArgs,

        /// <summary>
        ///     Thrown when an exception occurs mid-command execution.
        /// </summary>
        Exception,

        /// <summary>
        ///     Thrown when the command is not successfully executed on runtime.
        /// </summary>
        Unsuccessful,

        /// <summary>
        ///     Thrown when the command fails to meet a <see cref="PreconditionAttribute"/>'s conditions.
        /// </summary>
        UnmetPrecondition,

        /// <summary>
        ///     Thrown when the command context cannot be parsed by the <see cref="ICommandInfo"/>.
        /// </summary>
        ParseFailed
    }
}
