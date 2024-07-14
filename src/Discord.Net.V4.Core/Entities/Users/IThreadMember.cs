using Discord.Models;
using Discord.Rest;

namespace Discord;

public interface IThreadMember :
    ISnowflakeEntity,
    IThreadMemberActor,
    IRefreshable<IThreadMember, ulong, IThreadMemberModel>
{
    static IApiOutRoute<IThreadMemberModel> IRefreshable<IThreadMember, ulong, IThreadMemberModel>.RefreshRoute(
        IPathable path, ulong id
    ) => Routes.GetThreadMember(path.Require<IThreadChannel>(), id, true);

    DateTimeOffset JoinedAt { get; }
}
