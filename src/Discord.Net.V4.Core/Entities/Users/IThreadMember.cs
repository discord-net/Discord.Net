using Discord.Models;
using Discord.Rest;

namespace Discord;

[Refreshable(nameof(Routes.GetThreadMember))]
public partial interface IThreadMember :
    ISnowflakeEntity<IThreadMemberModel>,
    IThreadMemberActor
{
    DateTimeOffset JoinedAt { get; }
}
