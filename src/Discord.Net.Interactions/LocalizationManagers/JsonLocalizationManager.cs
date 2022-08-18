using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Discord.Interactions
{
    /// <summary>
    ///     The default localization provider for Json resource files.
    /// </summary>
    public sealed class JsonLocalizationManager : ILocalizationManager
    {
        private const string NameIdentifier = "name";
        private const string DescriptionIdentifier = "description";
        private const string SpaceToken = "~";

        private readonly string _basePath;
        private readonly string _fileName;
        private readonly Regex _localeParserRegex = new Regex(@"\w+.(?<locale>\w{2}(?:-\w{2})?).json", RegexOptions.Compiled | RegexOptions.Singleline);

        /// <summary>
        ///     Initializes a new instance of the <see cref="JsonLocalizationManager"/> class.
        /// </summary>
        /// <param name="basePath">Base path of the Json file.</param>
        /// <param name="fileName">Name of the Json file.</param>
        public JsonLocalizationManager(string basePath, string fileName)
        {
            _basePath = basePath;
            _fileName = fileName;
        }

        /// <inheritdoc />
        public IDictionary<string, string> GetAllDescriptions(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, DescriptionIdentifier);

        /// <inheritdoc />
        public IDictionary<string, string> GetAllNames(IList<string> key, LocalizationTarget destinationType) =>
            GetValues(key, NameIdentifier);

        private string[] GetAllFiles() =>
            Directory.GetFiles(_basePath, $"{_fileName}.*.json", SearchOption.TopDirectoryOnly);

        private IDictionary<string, string> GetValues(IList<string> key, string identifier)
        {
            var result = new Dictionary<string, string>();
            var files = GetAllFiles();

            foreach (var file in files)
            {
                var match = _localeParserRegex.Match(Path.GetFileName(file));
                if (!match.Success)
                    continue;

                var locale = match.Groups["locale"].Value;

                using var sr = new StreamReader(file);
                using var jr = new JsonTextReader(sr);
                var obj = JObject.Load(jr);
                var token = string.Join(".", key.Select(x => $"['{x}']")) + $".{identifier}";
                var value = (string)obj.SelectToken(token);
                if (value is not null)
                    result[locale] = value;
            }

            return result;
        }
    }
}
