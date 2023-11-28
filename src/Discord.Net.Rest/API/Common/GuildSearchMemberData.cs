using Newtonsoft.Json;

namespace Discord.API;

internal class GuildSearchMemberData
{
    [JsonProperty("member")]
    public GuildMember Member { get; set; }

    [JsonProperty("source_invite_code")]
    public string InviteCode { get; set; }

    [JsonProperty("join_source_type")]
    public InviteSourceType JoinSourceType { get; set; }

    [JsonProperty("inviter_id")]
    public ulong? InviterId { get; set; }
}
