namespace Discord
{
    /// <summary>
    ///     Provides the account information for an <see cref="IIntegration" />.
    /// </summary>
    public interface IIntegrationAccount
    {
        /// <summary>
        ///     Gets the ID of the account.
        /// </summary>
        /// <returns>
        ///     A <see cref="string"/> unique identifier of this integration account.
        /// </returns>
        string Id { get; }
        /// <summary>
        ///     Gets the name of the account.
        /// </summary>
        /// <returns>
        ///     A string containing the name of this integration account.
        /// </returns>
        string Name { get; }
    }
}
