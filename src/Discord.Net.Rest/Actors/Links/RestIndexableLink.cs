using System.Runtime.CompilerServices;

namespace Discord.Rest;

public sealed class RestIndexableLink<TId, TActor> :
    IIndexableLink<TId, TActor>
    where TActor : RestActor<TId>
    where TId : IEquatable<TId>
{
    public TActor this[TId id] => _cache.Get(id);

    private readonly IKeyedCache<TId, TActor> _cache;
    
    internal RestIndexableLink(DiscordRestClient client, Func<DiscordRestClient, TId, TActor> factory)
    {
        _cache = client.Cache.CreateActorsCache(typeof(TActor), factory);
    }
}