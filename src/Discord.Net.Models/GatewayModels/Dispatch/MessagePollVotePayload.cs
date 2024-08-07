using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class MessagePollVotePayload : IMessagePollVotePayloadData
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public ulong? GuildId { get; set; }

    [JsonPropertyName("answer_id")]
    public ulong AnswerId { get; set; }
}
