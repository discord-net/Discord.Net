using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord
{
    /// <summary>
    ///     An enum representing the type of content which can trigger the rule.
    /// </summary>
    public enum AutoModTriggerType
    {
        /// <summary>
        ///     Check if content contains words from a user defined list of keywords.
        /// </summary>
        Keyword = 1,

        /// <summary>
        ///     Check if content contains any harmful links.
        /// </summary>
        HarmfulLink = 2,

        /// <summary>
        ///     Check if content represents generic spam.
        /// </summary>
        Spam = 3,

        /// <summary>
        ///     Check if content contains words from internal pre-defined wordsets.
        /// </summary>
        KeywordPreset = 4,

        /// <summary>
        ///     Check if content contains more unique mentions than allowed.
        /// </summary>
        MentionSpam = 5,
    }
}
