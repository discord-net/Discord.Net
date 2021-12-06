namespace Discord.Interactions
{
    /// <summary>
    ///     Specifies the behavior of the command execution workflow.
    /// </summary>
    /// <seealso cref="InteractionServiceConfig"/>
    public enum RunMode
    {
        /// <summary>
        ///     Executes the command on the same thread as gateway one.
        /// </summary>
        Sync,
        /// <summary>
        ///     Executes the command on a different thread from the gateway one.
        /// </summary>
        Async,
        /// <summary>
        ///     The default behaviour set in <see cref="InteractionServiceConfig"/>.
        /// </summary>
        Default
    }
}
