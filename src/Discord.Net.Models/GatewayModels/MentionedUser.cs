using System.Text.Json.Serialization;

namespace Discord.Models.Json;

public sealed partial class MentionedUser : IMentionedUser
{
    [JsonIgnore, JsonExtend]
    public required User User { get; set; }

    [JsonPropertyName("member")]
    public required GuildMember Member { get; set; }

    IMemberModel IMentionedUser.Member => Member;
}
