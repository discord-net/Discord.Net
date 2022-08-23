using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Discord.Interactions
{
    /// <summary>
    ///     The default localization provider for Resx files.
    /// </summary>
    public sealed class ResxLocalizationManager : ILocalizationManager
    {
        private const string NameIdentifier = "name";
        private const string DescriptionIdentifier = "description";

        private readonly ResourceManager _resourceManager;
        private readonly IEnumerable<CultureInfo> _supportedLocales;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResxLocalizationManager"/> class.
        /// </summary>
        /// <param name="baseResource">Name of the base resource.</param>
        /// <param name="assembly">The main assembly for the resources.</param>
        /// <param name="supportedLocales">Cultures the <see cref="ResxLocalizationManager"/> should search for.</param>
        public ResxLocalizationManager(string baseResource, Assembly assembly, params CultureInfo[] supportedLocales)
        {
            _supportedLocales = supportedLocales;
            _resourceManager = new ResourceManager(baseResource, assembly);
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, DescriptionIdentifier);

        /// <inheritdoc />
        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, NameIdentifier);

        private IDictionary<string, string> GetValues(IList<string> key, string identifier)
        {
            var entryKey = (string.Join(".", key) + "." + identifier);

            var result = new Dictionary<string, string>();

            foreach (var locale in _supportedLocales)
            {
                var value = _resourceManager.GetString(entryKey, locale);
                if (value is not null)
                    result[locale.Name] = value;
            }

            return result;
        }
    }
}
