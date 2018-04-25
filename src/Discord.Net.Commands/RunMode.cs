namespace Discord.Commands
{
    /// <summary>
    ///     Specifies the behavior of the command execution workflow.
    /// </summary>
    /// <seealso cref="CommandServiceConfig"/>
    /// <seealso cref="CommandAttribute"/>
    public enum RunMode
    {
        /// <summary>
        /// The default behaviour set in <see cref="CommandServiceConfig"/>.
        /// </summary>
        Default,
        /// <summary>
        /// Executes the command on the same thread as gateway one.
        /// </summary>
        Sync,
        /// <summary>
        /// Executes the command on a different thread from the gateway one.
        /// </summary>
        Async
    }
}
