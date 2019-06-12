namespace Discord
{
    /// <summary>
    ///     Specifies the type of subscription a user is subscribed to.
    /// </summary>
    public enum PremiumType
    {
        /// <summary>
        ///     No subscription.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Nitro Classic subscription. Includes app perks like animated emojis and avatars, but not games.
        /// </summary>
        NitroClassic = 1,
        /// <summary>
        ///     Nitro subscription. Includes app perks as well as the games subscription service.
        /// </summary>
        Nitro = 2
    }
}
