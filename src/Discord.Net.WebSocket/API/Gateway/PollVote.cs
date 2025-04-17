using Newtonsoft.Json;

namespace Discord.API.Gateway;

internal class PollVote
{
    [JsonProperty("user_id")]
    public ulong UserId { get; set; }

    [JsonProperty("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonProperty("message_id")]
    public ulong MessageId { get; set; }

    [JsonProperty("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonProperty("answer_id")]
    public ulong AnswerId { get; set; }
}
