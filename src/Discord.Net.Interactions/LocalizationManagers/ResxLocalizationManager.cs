using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     The default localization provider for Resx files.
    /// </summary>
    public sealed class ResxLocalizationManager : ILocalizationManager
    {
        private const string NameIdentifier = "name";
        private const string DescriptionIdentifier = "description";

        private readonly string _baseResource;
        private readonly Assembly _assembly;
        private static readonly ConcurrentDictionary<string, ResourceManager> _localizerCache = new();
        private readonly IEnumerable<CultureInfo> _supportedLocales;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResxLocalizationManager"/> class.
        /// </summary>
        /// <param name="baseResource">Name of the base resource.</param>
        /// <param name="assembly">The main assembly for the resources.</param>
        /// <param name="supportedLocales">Cultures the <see cref="ResxLocalizationManager"/> should search for.</param>
        public ResxLocalizationManager(string baseResource, Assembly assembly, params CultureInfo[] supportedLocales)
        {
            _baseResource = baseResource;
            _assembly = assembly;
            _supportedLocales = supportedLocales;
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, DescriptionIdentifier);

        /// <inheritdoc />
        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, NameIdentifier);

        private IDictionary<string, string> GetValues(IList<string> key, string identifier)
        {
            var result = new Dictionary<string, string>();

            var resourceName = _baseResource + "." + string.Join(".", key);
            var resourceManager = _localizerCache.GetOrAdd(resourceName, new ResourceManager(resourceName, _assembly));

            foreach (var locale in _supportedLocales)
            {
                try
                {
                    var value = resourceManager.GetString(identifier, locale);
                    if (value is not null)
                        result[locale.Name] = value;
                }
                catch (MissingManifestResourceException){}
            }

            return result;
        }
    }
}
