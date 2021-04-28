namespace Discord
{
    /// <summary>
    ///     Provides tags related to a discord role.
    /// </summary>
    public class RoleTags
    {
        /// <summary>
        ///     Gets the identifier of the bot that this role belongs to, if it does.
        /// </summary>
        /// <returns>
        ///     A <see langword="ulong"/> if this role belongs to a bot; otherwise
        ///     <see langword="null"/>.
        /// </returns>
        public ulong? BotId { get; }
        /// <summary>
        ///     Gets the identifier of the integration that this role belongs to, if it does.
        /// </summary>
        /// <returns>
        ///     A <see langword="ulong"/> if this role belongs to an integration; otherwise
        ///     <see langword="null"/>.
        /// </returns>
        public ulong? IntegrationId { get; }
        /// <summary>
        ///     Gets if this role is the guild's premium subscriber (booster) role.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if this role is the guild's premium subscriber role;
        ///     otherwise <see langword="false"/>.
        /// </returns>
        public bool IsPremiumSubscriberRole { get; }

        internal RoleTags(ulong? botId, ulong? integrationId, bool isPremiumSubscriber)
        {
            BotId = botId;
            IntegrationId = integrationId;
            IsPremiumSubscriberRole = isPremiumSubscriber;
        }
    }
}
