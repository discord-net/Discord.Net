namespace Discord
{
    /// <summary>
    ///     The behavior of expiring subscribers for an <see cref="IIntegration" />.
    /// </summary>
    public enum IntegrationExpireBehavior
    {
        /// <summary>
        ///     Removes a role from an expired subscriber.
        /// </summary>
        RemoveRole = 0,
        /// <summary>
        ///     Kicks an expired subscriber from the guild.
        /// </summary>
        Kick = 1
    }
}
