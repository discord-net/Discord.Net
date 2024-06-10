using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    public sealed record IdentityPayload(
        [property: JsonPropertyName("token")]
        string Token,
        [property: JsonPropertyName("properties")]
        IdentityConnectionProperties Properties,
        [property: JsonPropertyName("intents")]
        int Intents,
        [property: JsonPropertyName("compress")]
        Optional<bool> Compress = default,
        [property: JsonPropertyName("large_threshold")]
        Optional<int> LargeThreashold = default,
        [property: JsonPropertyName("shard")]
        Optional<int[]> Shard = default,
        [property: JsonPropertyName("presence")]
        Optional<UpdatePresencePayload> Presence = default
    );

    public sealed record IdentityConnectionProperties(
        [property: JsonPropertyName("os")]
        string OS,
        [property: JsonPropertyName("browser")]
        string Browser,
        [property: JsonPropertyName("device")]
        string Device
    );

}

