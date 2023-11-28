namespace Discord.Rest;

public class MemberSearchParamsV2
{
    public MemberSearchParamsV2After After { get; set; }
}

public struct MemberSearchParamsV2After
{
    public ulong UserId { get; set; }

    public ulong GuildJoinedAt { get; set; }

    public MemberSearchParamsV2After(ulong userId, ulong guildJoinedAt)
    {
        UserId = UserId;
        GuildJoinedAt = guildJoinedAt;
    }
}
