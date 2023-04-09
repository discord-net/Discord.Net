using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     Represents a localization provider for Discord Application Commands.
    /// </summary>
    public interface ILocalizationManager
    {
        /// <summary>
        ///     Get every the resource name for every available locale.
        /// </summary>
        /// <param name="key">Location of the resource.</param>
        /// <param name="destinationType">Type of the resource.</param>
        /// <returns>
        ///     A dictionary containing every available locale and the resource name.
        /// </returns>
        IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType);

        /// <summary>
        ///     Get every the resource description for every available locale.
        /// </summary>
        /// <param name="key">Location of the resource.</param>
        /// <param name="destinationType">Type of the resource.</param>
        /// <returns>
        ///     A dictionary containing every available locale and the resource name.
        /// </returns>
        IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType);
    }
}
