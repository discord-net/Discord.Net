using System.Collections.Generic;
using System.Linq;

namespace Discord.Net.SourceGenerators.Serialization
{
    public partial class SerializationSourceGenerator
    {
        private static string GenerateSerializerOptionsSourceCode(
            string @namespace,
            IEnumerable<SerializedType> converters)
        {
            var snippets = string.Join("\n",
                converters.Select(
                    x => $"            options.Converters.Add(new {@namespace}.Internal.Converters.{x.ConverterTypeName}());"));

return $@"using System;
using System.Text.Json;
using Discord.Net.Serialization.Converters;

namespace {@namespace}
{{
    /// <summary>
    /// Defines extension methods for adding Discord.Net JSON converters to a
    /// <see cref=""JsonSerializerOptions""/> instance.
    /// </summary>
    public static class JsonSerializerOptionsExtensions
    {{
        /// <summary>
        /// Adds Discord.Net JSON converters to the passed
        /// <see cref=""JsonSerializerOptions""/>.
        /// </summary>
        /// <param name=""options"">
        /// The serializer options to add Discord.Net converters to.
        /// </param>
        /// <returns>
        /// The modified <see cref=""JsonSerializerOptions""/>, so this method
        /// can be chained.
        /// </returns>
        public static JsonSerializerOptions WithDiscordNetConverters(
            this JsonSerializerOptions options)
        {{
            options.Converters.Add(new OptionalConverterFactory());
{snippets}

            return options;
        }}
    }}
}}";
        }
    }
}
