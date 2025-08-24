using System.Runtime.CompilerServices;

namespace Discord.Rest;

public sealed class RestIndexableLink<TId, TActor> :
    IIndexableLink<TId, TActor>
    where TActor : RestActor<TId>
    where TId : IEquatable<TId>
{
    public TActor this[TId id] => _actors.GetOrAdd(id, _factory, _client);

    private readonly DiscordRestClient _client;
    private readonly Func<DiscordRestClient, TId, TActor> _factory;
    private readonly WeakTable<TId, TActor> _actors;
    
    
    internal RestIndexableLink(DiscordRestClient client, Func<DiscordRestClient, TId, TActor> factory)
    {
        _client = client;
        _factory = factory;
        _actors = new();
    }

}