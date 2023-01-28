using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     An enum representing preset filter types.
    /// </summary>
    public enum KeywordPresetTypes
    {
        /// <summary>
        ///     Words that may be considered forms of swearing or cursing.
        /// </summary>
        Profanity = 1,

        /// <summary>
        ///     Words that refer to sexually explicit behavior or activity.
        /// </summary>
        SexualContent = 2,

        /// <summary>
        ///     Personal insults or words that may be considered hate speech.
        /// </summary>
        Slurs = 3,
    }
}
