using Discord.Models.Json;
using Discord.Rest;

namespace Discord;

public sealed record PagePollVotersParams(
    int? PageSize = DiscordConfig.MaxPollVotersPerBatch,
    int? Total = null,
    IdOrEntity<ulong, IUserActor>? After = null
) : IPagingParams<Routes.GetAnswerVoters, PollVoters>
{
    public Routes.GetAnswerVoters? GetNext(
        IPathable path,
        Routes.GetAnswerVoters route, 
        PollVoters? previousApiResult)
    {
        if (previousApiResult is null)
        {
            return route with
            {
                After = (After?.Id).ToOptional(),
                Limit = IPagingParams.GetPageSize(this)
            };
        }

        if (previousApiResult.Users.Length == 0) return null;

        return route with
        {
            After = previousApiResult.Users.Max(x => x.Id),
            Limit = IPagingParams.GetPageSize(this)
        };

        // return Routes.GetPollAnswerVoters(
        //     channelId,
        //     messageId,
        //     answer,
        //     lastRequest.Users.MaxBy(x => x.Id)?.Id,
        //     pageSize
        // );
    }

    public static int MaxPageSize => DiscordConfig.MaxPollVotersPerBatch;

    // public Direction? Direction => After.Map(Discord.Direction.After);
    //
    // public Optional<ulong> From => Optional.FromNullable(After?.Id);
    //
    // public static IApiOutRoute<PollVoters>? GetRoute(
    //     PagePollVotersParams? self,
    //     IPathable path,
    //     PollVoters? lastRequest)
    // {
    //     if (
    //         !path.TryGet<ulong, IChannel>(out var channelId) ||
    //         !path.TryGet<ulong, IMessage>(out var messageId) ||
    //         !path.TryGet<int, IPollAnswer>(out var answer)
    //     ) return null;
    //
    //     var pageSize = IPagingParams.GetPageSize(self);
    //
    //     if (lastRequest is null)
    //     {
    //         return Routes.GetPollAnswerVoters(
    //             channelId,
    //             messageId,
    //             answer,
    //             self?.After,
    //             pageSize
    //         );
    //     }
    //
    //     return Routes.GetPollAnswerVoters(
    //         channelId,
    //         messageId,
    //         answer,
    //         lastRequest.Users.MaxBy(x => x.Id)?.Id,
    //         pageSize
    //     );
    // }
}