using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public record PagePollVotersParams(
    int? PageSize = DiscordConfig.MaxPollVotersPerBatch,
    int? Total = null,
    EntityOrId<ulong, IUserActor>? After = null
) : IDirectionalPagingParams<ulong>, IPagingParams<PagePollVotersParams, PollVoters>
{
    public static int MaxPageSize => DiscordConfig.MaxPollVotersPerBatch;

    public Direction? Direction => After.Map(Discord.Direction.After);

    public Optional<ulong> From => Optional.FromNullable(After?.Id);

    public static IApiOutRoute<PollVoters>? GetRoute(
        PagePollVotersParams? self,
        IPathable path,
        PollVoters? lastRequest)
    {
        if (
            !path.TryGet<ulong, IChannel>(out var channelId) ||
            !path.TryGet<ulong, IMessage>(out var messageId) ||
            !path.TryGet<int, IPollAnswer>(out var answer)
        ) return null;

        var pageSize = IPagingParams.GetPageSize(self);

        if (lastRequest is null)
        {
            return Routes.GetPollAnswerVoters(
                channelId,
                messageId,
                answer,
                self?.After,
                pageSize
            );
        }

        return Routes.GetPollAnswerVoters(
            channelId,
            messageId,
            answer,
            lastRequest.Users.MaxBy(x => x.Id)?.Id,
            pageSize
        );
    }
}