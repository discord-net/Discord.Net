using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<PollVoters> GetPollAnswerVoters([IdHeuristic<IChannel>] ulong channelId,
        [IdHeuristic<IMessage>] ulong messageId, int answerId,
        ulong? afterId, int? limit) =>
        new ApiOutRoute<PollVoters>(nameof(GetPollAnswerVoters), RequestMethod.Get,
            $"channels/{channelId}/polls/{messageId}/answers/{answerId}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static IApiOutRoute<Message> EndPoll([IdHeuristic<IChannel>] ulong channelId, [IdHeuristic<IMessage>] ulong messageId) =>
        new ApiOutRoute<Message>(nameof(EndPoll), RequestMethod.Post, $"channels/{channelId}/polls/{messageId}/expire",
            (ScopeType.Channel, channelId));
}
