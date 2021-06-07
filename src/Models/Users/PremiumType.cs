namespace Discord.Net.Models
{
    /// <summary>
    /// Declares an enum which represents the premium type for a <see cref="User"/>.
    /// </summary>
    /// <remarks>
    /// <see href="https://discord.com/developers/docs/resources/user#user-object-premium-types"/>
    /// </remarks>
    public enum PremiumType
    {
        /// <summary>
        /// User does not have Nitro.
        /// </summary>
        None = 0,

        /// <summary>
        /// User has Nitro Classic.
        /// </summary>
        NitroClassic = 1,

        /// <summary>
        /// User has Nitro.
        /// </summary>
        Nitro = 2,
    }
}
