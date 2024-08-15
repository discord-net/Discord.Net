using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class MentionedUser : IMentionedUser
{
    [JsonIgnore, JsonExtend] public User User { get; set; } = null!;

    [JsonPropertyName("member")]
    public required PartialGuildMember Member { get; set; }

    IPartialMemberModel IMentionedUser.Member => Member;
}
