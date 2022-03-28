using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    public interface ILocalizationManager
    {
        Task<IDictionary<string, string>> GetAllNamesAsync(IList<string> key, LocalizationTarget destinationType, IServiceProvider serviceProvider);
        Task<IDictionary<string, string>> GetAllDescriptionsAsync(IList<string> key, LocalizationTarget destinationType, IServiceProvider serviceProvider);
    }
}
