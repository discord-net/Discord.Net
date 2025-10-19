using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Discord.Models.Json;

namespace Discord.Rest;

public class DiscordRestClient : IDiscordClient
{
    public RestApiClient Api { get; }
    public DiscordConfig Config { get; }

    public DiscordJsonContext JsonContext { get; }
    
    [field: MaybeNull]
    public RestUsersLink Users => field ??= new RestUsersLink(this);
    
    public IGuildsLink Guilds => throw new NotImplementedException();

    internal RestCache Cache { get; }
    
    public DiscordRestClient(DiscordConfig config)
    {
        Config = config;
        Cache = new(this);
        Api = new(this);
        JsonContext = new(null);
    }
    
    IUsersLink IDiscordClient.Users => Users;
}