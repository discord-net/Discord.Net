using Discord.Models.Rest;

namespace Discord.Models.Rest.Targets;

public sealed class RestUsersLink : IUsersLink
{
    public RestUserActor this[Snowflake id] => _indexable[id];

    private readonly RestIndexableLink<Snowflake, RestUserActor, RestUser> _indexable;

    private readonly DiscordRestClient _client;

    internal RestUsersLink(DiscordRestClient client)
    {
        _client = client;
        _indexable = new(client, static (id, client) => new RestUserActor(id, client));
    }
    
    IUserActor IIndexableLink<Snowflake, IUserActor>.this[Snowflake id] => this[id];
}