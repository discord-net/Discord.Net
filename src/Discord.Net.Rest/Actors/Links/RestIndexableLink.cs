using System.Runtime.CompilerServices;
using Discord.Rest.Actors;

namespace Discord.Rest;

public sealed class RestIndexableLink<TId, TActor, TEntity> :
    IIndexableLink<TId, TActor>
    where TActor : RestActor<TId, TEntity>
    where TEntity : RestEntity<TId>
    where TId : IEquatable<TId>
{
    public TActor this[TId id] => _actors.GetOrAdd(id, _factory, _client);

    private readonly DiscordRestClient _client;
    private readonly Func<TId, DiscordRestClient, TActor> _factory;
    private readonly WeakTable<TId, TActor> _actors;
    
    
    internal RestIndexableLink(DiscordRestClient client, Func<TId, DiscordRestClient, TActor> factory)
    {
        _client = client;
        _factory = factory;
        _actors = new();
    }

}