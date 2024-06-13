using Discord.Models.Json;
using Discord.Utils;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<PollAnswerVoters[]> GetPollAnswerVoters(ulong channelId, ulong messageId, ulong answerId, ulong? afterId, int? limit)
        => new(nameof(GetPollAnswerVoters),
            RequestMethod.Get,
            $"channels/{channelId}/polls/{messageId}/answers/{answerId}{RouteUtils.GetUrlEncodedQueryParams(("after", afterId), ("limit", limit))}",
            (ScopeType.Channel, channelId));

    public static ApiRoute<Message> EndPoll(ulong channelId, ulong messageId)
        => new(nameof(EndPoll),
            RequestMethod.Post,
            $"channels/{channelId}/polls/{messageId}/expire",
            (ScopeType.Channel, channelId));
}
