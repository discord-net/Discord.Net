using Discord.Net.Converters;
using Newtonsoft.Json;

using System;

namespace Discord.Rest;

public static class EmbedBuilderUtils
{
    private static Lazy<JsonSerializerSettings> _settings = new(() =>
    {
        var serializer = new JsonSerializerSettings()
        {
            ContractResolver = new DiscordContractResolver()
        };
        return serializer;
    });
    
    /// <summary>
    ///     Parses a string into an <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <returns>An <see cref="EmbedBuilder"/> with populated values from the passed <paramref name="json"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the string passed is not valid json.</exception>
    public static EmbedBuilder Parse(string json)
    {
        try
        {
            var model = JsonConvert.DeserializeObject<API.Embed>(json, _settings.Value);

            var embed = model?.ToEntity();

            if (embed is not null)
                return embed.ToEmbedBuilder();

            return new EmbedBuilder();
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    ///     Tries to parse a string into an <see cref="EmbedBuilder"/>.
    /// </summary>
    /// <param name="json">The json string to parse.</param>
    /// <param name="builder">The <see cref="EmbedBuilder"/> with populated values. An empty instance if method returns <see langword="false"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="json"/> was successfully parsed. <see langword="false"/> if not.</returns>
    public static bool TryParse(string json, out EmbedBuilder builder)
    {
        builder = new EmbedBuilder();
        try
        {
            var model = JsonConvert.DeserializeObject<API.Embed>(json, _settings.Value);

            var embed = model?.ToEntity();

            if (embed is not null)
            {
                builder = embed.ToEmbedBuilder();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
