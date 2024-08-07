using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class VoiceStateUpdated : IVoiceStateUpdatedPayloadData
{
    [JsonIgnore, JsonExtend]
    public required VoiceState VoiceState { get; set; }
}
