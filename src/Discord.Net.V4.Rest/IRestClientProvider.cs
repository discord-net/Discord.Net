namespace Discord.Rest;

public partial interface IRestClientProvider : IClientProvider
{
    [SourceOfTruth]
    new DiscordRestClient Client { get; }
}
