using Discord.Models;
using Discord.Rest;

namespace Discord;

public sealed record PageUserBasicReactionsParams(
    int? PageSize = DiscordConfig.MaxUserReactionsPerBatch,
    int? Total = null,
    EntityOrId<ulong, IMember>? After = null
) : PageUserReactionsParams(PageSize, Total, ReactionType.Normal, After);

public sealed record PageUserSuperReactionsParams(
    int? PageSize = DiscordConfig.MaxUserReactionsPerBatch,
    int? Total = null,
    EntityOrId<ulong, IMember>? After = null
) : PageUserReactionsParams(PageSize, Total, ReactionType.Burst, After);

public record PageUserReactionsParams(
    int? PageSize = DiscordConfig.MaxUserReactionsPerBatch,
    int? Total = null,
    ReactionType? Type = null,
    EntityOrId<ulong, IMember>? After = null
) :
    IDirectionalPagingParams<ulong>,
    IPagingParams<PageUserReactionsParams, IEnumerable<IUserModel>>
{
    public static int MaxPageSize => DiscordConfig.MaxUserReactionsPerBatch;

    public Direction? Direction => After.Map(Discord.Direction.After);

    public Optional<ulong> From => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<IEnumerable<IUserModel>>? GetRoute(
        PageUserReactionsParams? self,
        IPathable path,
        IEnumerable<IUserModel>? lastRequest)
    {
        if (
            !path.TryGet<ulong, IChannel>(out var channelId) ||
            !path.TryGet<ulong, IMessage>(out var messageId) ||
            !path.TryGet<DiscordEmojiId, IReaction>(out var reactionId)
        ) return null;

        var pageSize = IPagingParams.GetPageSize(self);

        if (lastRequest is null)
        {
            return Routes.GetReactions(
                channelId,
                messageId,
                reactionId,
                afterId: self?.After,
                limit: pageSize,
                type: self?.Type
            );
        }

        return Routes.GetReactions(
            channelId,
            messageId,
            reactionId,
            afterId: lastRequest.MaxBy(x => x.Id)?.Id,
            limit: pageSize,
            type: self?.Type
        );
    }
}