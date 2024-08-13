using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class MentionedUser : IMentionedUser
{
    [JsonIgnore, JsonExtend]
    public required User User { get; set; }

    [JsonPropertyName("member")]
    public required PartialGuildMember Member { get; set; }

    IPartialMemberModel IMentionedUser.Member => Member;
}
