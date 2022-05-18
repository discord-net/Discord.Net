using Discord.Net.Converters;
using Newtonsoft.Json;
using System.Linq;
using System;

namespace Discord.Rest
{
    /// <summary>
    ///     Responsible for formatting certain entities as Json <see langword="string"/>, to reuse later on.
    /// </summary>
    public static class StringExtensions
    {
        private static Lazy<JsonSerializerSettings> _settings = new(() =>
        {
            var serializer = new JsonSerializerSettings()
            {
                ContractResolver = new DiscordContractResolver()
            };
            serializer.Converters.Add(new EmbedTypeConverter());
            return serializer;
        });

        /// <summary>
        ///     Gets a Json formatted <see langword="string"/> from an <see cref="EmbedBuilder"/>.
        /// </summary>
        /// <remarks>
        ///     See <see cref="EmbedBuilder.TryParse(string, out EmbedBuilder)"/> to parse Json back into embed.
        /// </remarks>
        /// <param name="builder">The builder to format as Json <see langword="string"/>.</param>
        /// <param name="formatting">The formatting in which the Json will be returned.</param>
        /// <returns>A Json <see langword="string"/> containing the data from the <paramref name="builder"/>.</returns>
        public static string ToJsonString(this EmbedBuilder builder, Formatting formatting = Formatting.Indented)
            => ToJsonString(builder.Build(), formatting);

        /// <summary>
        ///     Gets a Json formatted <see langword="string"/> from an <see cref="Embed"/>.
        /// </summary>
        /// <remarks>
        ///     See <see cref="EmbedBuilder.TryParse(string, out EmbedBuilder)"/> to parse Json back into embed.
        /// </remarks>
        /// <param name="embed">The embed to format as Json <see langword="string"/>.</param>
        /// <param name="formatting">The formatting in which the Json will be returned.</param>
        /// <returns>A Json <see langword="string"/> containing the data from the <paramref name="embed"/>.</returns>
        public static string ToJsonString(this Embed embed, Formatting formatting = Formatting.Indented)
            => JsonConvert.SerializeObject(embed.ToModel(), formatting, _settings.Value);
    }
}
