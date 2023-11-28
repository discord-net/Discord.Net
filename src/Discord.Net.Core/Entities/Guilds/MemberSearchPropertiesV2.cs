namespace Discord;

public class MemberSearchPropertiesV2
{
    public MemberSearchPropertiesV2After After { get; set; }
}

public struct MemberSearchPropertiesV2After
{
    public ulong UserId { get; set; }

    public ulong GuildJoinedAt { get; set; }

    public MemberSearchPropertiesV2After(ulong userId, ulong guildJoinedAt)
    {
        UserId = userId;
        GuildJoinedAt = guildJoinedAt;
    }
}
