using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class UpdateVoiceStatePayloadData : IUpdateVoiceStatePayloadData
{
    [JsonPropertyName("guild_id")]
    public ulong GuildId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong? ChannelId { get; set; }

    [JsonPropertyName("self_mute")]
    public bool SelfMute { get; set; }

    [JsonPropertyName("self_deaf")]
    public bool SelfDeafen { get; set; }
}
