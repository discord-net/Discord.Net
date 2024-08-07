using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ModifyCurrentUserVoiceStateParams : ModifyUserVoiceStateParams
{
    [JsonPropertyName("request_to_speak_timestamp")]
    public Optional<DateTimeOffset?> RequestToSpeakTimestamp { get; set; }
}
