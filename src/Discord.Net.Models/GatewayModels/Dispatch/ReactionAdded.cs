using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed class ReactionAdded : IMessageReactionAddedPayload
{
    [JsonPropertyName("user_id")]
    public ulong UserId { get; set; }

    [JsonPropertyName("channel_id")]
    public ulong ChannelId { get; set; }

    [JsonPropertyName("message_id")]
    public ulong MessageId { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("member")]
    public Optional<GuildMember> Member { get; set; }

    [JsonPropertyName("emoji")]
    public required PartialEmote Emoji { get; set; }

    [JsonPropertyName("message_author_id")]
    public Optional<ulong> MessageAuthorId { get; set; }

    [JsonPropertyName("burst")]
    public bool IsBurst { get; set; }

    [JsonPropertyName("burst_colors")]
    public Optional<string[]> BurstColors { get; set; }

    [JsonPropertyName("type")]
    public int Type { get; set; }

    IPartialEmoteModel IMessageReactionAddedPayload.Emoji => Emoji;
    ulong? IMessageReactionAddedPayload.GuildId => GuildId.ToNullable();
    IMemberModel? IMessageReactionAddedPayload.Member => ~Member;
    string[]? IMessageReactionAddedPayload.BurstColors => ~BurstColors;
    ulong? IMessageReactionAddedPayload.MessageAuthorId => MessageAuthorId.ToNullable();
}
