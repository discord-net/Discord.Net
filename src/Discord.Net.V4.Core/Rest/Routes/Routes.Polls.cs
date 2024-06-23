using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<PollAnswerVoters[]> GetPollAnswerVoters(ulong channelId, ulong messageId, ulong answerId,
        ulong? afterId, int? limit) =>
        new ApiOutRoute<PollAnswerVoters[]>(nameof(GetPollAnswerVoters), RequestMethod.Get,
            $"channels/{channelId}/polls/{messageId}/answers/{answerId}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message> EndPoll(ulong channelId, ulong messageId) =>
        new ApiOutRoute<Message>(nameof(EndPoll), RequestMethod.Post, $"channels/{channelId}/polls/{messageId}/expire",
            (ScopeType.Channel, channelId));
}
