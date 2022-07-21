using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
        private const string SpaceToken = "~";

        private readonly string _baseResource;
        private readonly Assembly _assembly;
        private static readonly ConcurrentDictionary<string, ResourceManager> _localizerCache = new();
        private readonly IEnumerable<CultureInfo> _supportedLocales;
        private readonly IEnumerable<string> _resourceNames;

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
            _resourceNames = assembly.GetManifestResourceNames();
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, DescriptionIdentifier);

        /// <inheritdoc />
        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, NameIdentifier);

        private IDictionary<string, string> GetValues(IList<string> key, string identifier)
        {
            var resourceName = (_baseResource + "." + string.Join(".", key)).Replace(" ", SpaceToken);

            if (!_resourceNames.Any(x => string.Equals(resourceName + ".resources", x, StringComparison.OrdinalIgnoreCase)))
                return ImmutableDictionary<string, string>.Empty;

            var result = new Dictionary<string, string>();
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
