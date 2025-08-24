using Discord.Models;

namespace Discord.Rest;

public sealed class RestUsersLink : IUsersLink
{
    public RestCurrentUserActor Current { get; }

    public RestUserActor this[Snowflake id] => _indexable[id];

    private readonly RestIndexableLink<Snowflake, RestUserActor> _indexable;

    private readonly DiscordRestClient _client;

    internal RestUsersLink(DiscordRestClient client)
    {
        Current = new(client, TokenUtils.GetUserIdFromToken(client.Config.Token.Value));
        _client = client;
        _indexable = new(client, static (client, id) => new RestUserActor(client, id));
    }

    ICurrentUserActor IUsersLink.Current => Current;
    IUserActor IIndexableLink<Snowflake, IUserActor>.this[Snowflake id] => this[id];
}