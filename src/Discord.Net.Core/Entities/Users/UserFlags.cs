using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    [Flags]
    public enum UserFlag
    {
        /// <summary>
        ///     Default value for flags, when none are given to an account.
        /// </summary>
        None = 0,
        /// <summary>
        ///     Flag given to Discord staff.
        /// </summary>
        Staff = 0b1,
        /// <summary>
        ///     Flag given to Discord partners.
        /// </summary>
        Partner = 0b10,
        HypeSquadEvents = 0b100,
        BugHunter = 0b1000,
        HypeSquadBravery = 0b100_0000,
        HypeSquadBrilliance = 0b1000_0000,
        HypeSquadBalance = 0b1_0000_0000,
        EarlySupporter = 0b10_0000_0000,
    }
}
