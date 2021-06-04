using System.Collections.Generic;
using System.Linq;

namespace Discord.Net.SourceGenerators.Serialization
{
    public partial class SerializationSourceGenerator
    {
        private static string GenerateSerializerOptionsTemplateSourceCode()
        {
return @"
using System;
using System.Text.Json;

namespace Discord.Net.Serialization
{
    /// <summary>
    /// Defines extension methods for adding Discord.Net JSON converters to a
    /// <see cref=""JsonSerializerOptions""/> instance.
    /// </summary>
    public static partial class JsonSerializerOptionsExtensions
    {
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
        public static partial JsonSerializerOptions WithDiscordNetConverters(
            this JsonSerializerOptions options);
    }
}";
        }

        private static string GenerateSerializerOptionsSourceCode(
            List<string> converters)
        {
            var snippets = string.Join("\n",
                converters.Select(
                    x => $"options.Converters.Add(new {x}());"));

return $@"
using System;
using System.Text.Json;
using Discord.Net.Serialization.Converters;

namespace Discord.Net.Serialization
{{
    public static partial class JsonSerializerOptionsExtensions
    {{
        public static partial JsonSerializerOptions WithDiscordNetConverters(
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
