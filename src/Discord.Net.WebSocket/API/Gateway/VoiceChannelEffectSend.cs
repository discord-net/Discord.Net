using Newtonsoft.Json;

namespace Discord.WebSocket;

internal class VoiceChannelEffectSend
{
    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public ulong GuildId { get; set; }

    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("emoji")]
    public Optional<API.Emoji> Emoji { get; set; }

    [JsonProperty("animation_type")]
    public Optional<AnimationType> AnimationType { get; set; }

    [JsonProperty("animation_id")]
    public Optional<int> AnimationId { get; set; }
}
