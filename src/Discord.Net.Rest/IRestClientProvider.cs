using Discord.Rest;

namespace Discord.Rest;

public interface IRestClientProvider
{
    DiscordRestClient RestClient { get; }
}
