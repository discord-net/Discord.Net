using Discord.Rest;

namespace Discord;

public interface IModifiable<TId, TSelf, out TParams, TApi> : IEntity<TId>, IPathable
    where TSelf : IModifiable<TId, TSelf, TParams, TApi>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApi>, new()
    where TApi : class
{
    Task ModifyAsync(Action<TParams> func, RequestOptions? options = null, CancellationToken token = default)
        => ModifyAsync(Client, this, Id, func, options, token);

    internal static Task ModifyAsync(IDiscordClient client, IPathable path, TId id, Action<TParams> func, RequestOptions? options = null, CancellationToken token = default)
    {
        var args = new TParams();
        func(args);
        return client.RestApiClient.ExecuteAsync(TSelf.ModifyRoute(path, id, args.ToApiModel()), options ?? client.DefaultRequestOptions, token);
    }

    internal static abstract ApiBodyRoute<TApi> ModifyRoute(IPathable path, TId id, TApi args);
}
