using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public interface ILocalizationManager
    {
        IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType);
        IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType);
    }
}
