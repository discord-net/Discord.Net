using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IThreadMember :
    ISnowflakeEntity,
    IThreadMemberActor,
    IRefreshable<IThreadMember, ulong, IThreadMemberModel>
{
    static IApiOutRoute<IThreadMemberModel> IRefreshable<IThreadMember, ulong, IThreadMemberModel>.RefreshRoute(
        IThreadMember self, ulong id
    ) => Routes.GetThreadMember(self.Thread.Id, id, true);

    DateTimeOffset JoinedAt { get; }
}
