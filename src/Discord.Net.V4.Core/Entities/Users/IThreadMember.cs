using Discord.Models;
using Discord.Rest;

namespace Discord;

[FetchableOfMany(nameof(Routes.ListThreadMembers))]
[PagedFetchableOfMany<PageThreadMembersParams>(nameof(Routes.ListThreadMembersPaged))]
[Refreshable(nameof(Routes.GetThreadMember))]
public partial interface IThreadMember :
    ISnowflakeEntity<IThreadMemberModel>,
    IThreadMemberActor
{
    DateTimeOffset JoinedAt { get; }
}
