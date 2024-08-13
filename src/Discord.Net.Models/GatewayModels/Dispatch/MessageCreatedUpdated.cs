using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class MessageCreatedUpdated : IMessageCreateUpdatePayloadData
{
    [JsonIgnore, JsonExtend]
    public required Message Message { get; set; }

    [JsonPropertyName("guild_id")]
    public Optional<ulong> GuildId { get; set; }

    [JsonPropertyName("member")]
    public Optional<PartialGuildMember> Member { get;set; }

    [JsonPropertyName("mentions")]
    public required MentionedUser[] Mentions { get; set;}

    IEnumerable<IMentionedUser> IMessageCreateUpdatePayloadData.Mentions => Mentions;
    IPartialMemberModel? IMessageCreateUpdatePayloadData.Member => ~Member;
    ulong? IMessageCreateUpdatePayloadData.GuildId => GuildId.ToNullable();
}
