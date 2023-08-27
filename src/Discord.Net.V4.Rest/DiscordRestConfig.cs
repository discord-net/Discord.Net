namespace Discord.Rest;

public class DiscordRestConfig : DiscordConfig
{
    public IRestApiProvider? ApiClient { get; init; }

    public DiscordRestConfig(DiscordToken token)
        : base(token)
    {
    }
}
