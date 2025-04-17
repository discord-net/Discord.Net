using Newtonsoft.Json;

namespace Discord.API.Gateway;

internal class Reaction
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonProperty("emoji")]
    public Emoji Emoji { get; set; }

    [JsonProperty("member")]
    public Optional<GuildMember> Member { get; set; }

    [JsonProperty("burst")]
    public bool IsBurst { get; set; }

    [JsonProperty("burst_colors")]
    public Optional<Color[]> BurstColors { get; set; }

    [JsonProperty("type")]
    public ReactionType Type { get; set; }
}
