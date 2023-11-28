using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Rest;

public struct MemberSearchResult
{
    public ulong GuildId { get; }

    public IReadOnlyCollection<MemberSearchData> Members { get; }

    public int PageResultCount { get; }

    public int TotalResultCount { get; }

    public MemberSearchResult(ulong guildId, IReadOnlyCollection<MemberSearchData> members, int pageResultCount, int totalResultCount)
    {
        GuildId = guildId;
        Members = members;
        PageResultCount = pageResultCount;
        TotalResultCount = totalResultCount;
    }
}

[DebuggerDisplay(@"{DebuggerDisplay,nq}")]
public readonly struct MemberSearchData
{
    public RestGuildUser User { get; }

    public string SourceInviteCode { get; }

    public JoinSourceType JoinSourceType { get; }

    public ulong? InviterId { get; }

    public MemberSearchData(RestGuildUser user, string sourceInviteCode, JoinSourceType joinSourceType, ulong? inviterId)
    {
        User = user;
        SourceInviteCode = sourceInviteCode;
        JoinSourceType = joinSourceType;
        InviterId = inviterId;
    }

    private string DebuggerDisplay => $"{User.DisplayName} ({User.Id})";
}
