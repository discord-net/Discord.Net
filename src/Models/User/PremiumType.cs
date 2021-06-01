using System;

namespace Discord.Net.Models
{
    /// <summary>
    ///     Represents the premium type.
    /// </summary>
    public enum PremiumType
    {
        /// <summary>
        ///     User does not have Nitro.
        /// </summary>
        None = 0,

        /// <summary>
        ///     User has Nitro Classic.
        /// </summary>
        NitroClassic = 1,

        /// <summary>
        ///     User has Nitro.
        /// </summary>
        Nitro = 2,
    }
}
