using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Discord.Models.Json;
using Discord.Models.Rest.Targets;
using Discord.Rest.Targets;

namespace Discord.Rest;

public class DiscordRestClient : IDiscordClient
{
    public RestApiClient Api { get; }
    public DiscordConfig Config { get; }

    public DiscordJsonContext JsonContext { get; }
    
    [field: MaybeNull]
    public RestUsersLink Users => field ??= new RestUsersLink(this);
    
    public IGuildsLink Guilds => throw new NotImplementedException();

    public DiscordRestClient(DiscordConfig config)
    {
        Config = config;
        Api = new(this);
        JsonContext = new();
    }
    
    IUsersLink IDiscordClient.Users => Users;
}