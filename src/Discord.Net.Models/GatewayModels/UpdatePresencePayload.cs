using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    public sealed record UpdatePresencePayload(
        [property: JsonPropertyName("since")]
        int? Since,
        [property: JsonPropertyName("activities")]
        object[] Activities,
        [property: JsonPropertyName("status")]
        string Status,
        [property: JsonPropertyName("afk")]
        bool Afk
    );
}

