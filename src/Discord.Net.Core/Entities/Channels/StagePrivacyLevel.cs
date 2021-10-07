using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     Represents the privacy level of a stage.
    /// </summary>
    public enum StagePrivacyLevel
    {
        /// <summary>
        ///     The stage is a public stage.
        /// </summary>
        Public = 1,

        /// <summary>
        ///     The stage is non public and is only accessable from the guild.
        /// </summary>
        GuildOnly = 2,
    }
}
