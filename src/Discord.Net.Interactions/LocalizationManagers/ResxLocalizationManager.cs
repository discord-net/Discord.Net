using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    internal sealed class ResxLocalizationManager : ILocalizationManager
    {
        private const string NameIdentifier = "name";
        private const string DescriptionIdentifier = "description";

        private readonly string _baseResource;
        private readonly Assembly _assembly;
        private readonly ConcurrentDictionary<string, ResourceManager> _localizerCache = new();
        private readonly IEnumerable<CultureInfo> _supportedLocales;

        public ResxLocalizationManager(string baseResource, Assembly assembly, params CultureInfo[] supportedLocales)
        {
            _baseResource = baseResource;
            _assembly = assembly;
            _supportedLocales = supportedLocales;
        }

        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, DescriptionIdentifier);

        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, NameIdentifier);

        private IDictionary<string, string> GetValues(IList<string> key, string identifier)
        {
            var result = new Dictionary<string, string>();

            var resourceName = _baseResource + "." + string.Join(".", key);
            var resourceManager = _localizerCache.GetOrAdd(resourceName, new ResourceManager(resourceName, _assembly));

            foreach (var locale in _supportedLocales)
            {
                var value = resourceManager.GetString(identifier, locale);
                if (value is not null)
                    result[locale.Name] = value;
            }

            return result;
        }
    }
}
