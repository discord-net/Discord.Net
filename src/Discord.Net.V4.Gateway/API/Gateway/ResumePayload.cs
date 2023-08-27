using System;
using System.Text.Json.Serialization;

namespace Discord.API.Gateway
{
    public sealed record ResumePayload(
        [property: JsonPropertyName("token")]
        string Token,
        [property: JsonPropertyName("session_id")]
        string SessionId,
        [property: JsonPropertyName("seq")]
        int Sequence
    );
}

