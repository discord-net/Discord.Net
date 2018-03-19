namespace Discord.Commands
{
    public enum RunMode
    {
        /// <summary>
        /// Default <see cref="RunMode"/> behaviour set in <see cref="CommandServiceConfig"/>.
        /// </summary>
        Default,
        /// <summary>
        /// Executes the command on the same thread as gateway.
        /// </summary>
        Sync,
        /// <summary>
        /// Executes the command on a different thread from the gateway one.
        /// </summary>
        Async
    }
}
