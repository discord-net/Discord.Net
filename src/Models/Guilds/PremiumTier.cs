namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the premium tier (server boost tier) for a <see cref="Guild"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/guild#guild-object-premium-tier"/>
    /// </remarks>
    public enum PremiumTier
    {
        /// <summary>
        /// <see cref="Guild"/> has not unlocked any Server Boost perks.
        /// </summary>
        None = 0,

        /// <summary>
        /// <see cref="Guild"/> has unlocked Server Boost level 1 perks.
        /// </summary>
        Tier1 = 1,

        /// <summary>
        /// <see cref="Guild"/> has unlocked Server Boost level 2 perks.
        /// </summary>
        Tier2 = 2,

        /// <summary>
        /// <see cref="Guild"/> has unlocked Server Boost level 3 perks.
        /// </summary>
        Tier3 = 3,
    }
}
