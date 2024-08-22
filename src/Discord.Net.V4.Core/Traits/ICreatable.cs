using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class CreatableAttribute(string route, params Type[] fromBackLinks) : Attribute;

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class CreatableAttribute<TParams>(string route, params Type[] fromBackLinks) : Attribute
{
    public Type[]? RouteGenerics { get; set; }
}

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class ActorCreatableAttribute<TParams>(string route, string idPath, params Type[] fromBackLinks) : Attribute
{
    public Type[]? RouteGenerics { get; set; }
}

#pragma warning restore CS9113 // Parameter is unread.

public interface ICreatable<TActor, out TEntity, out TId, in TParams, TApiParams, TModel> :
    IIdentifiable<TId>,
    IEntityProvider<TEntity, TModel>,
    IPathable
    where TActor : ICreatable<TActor, TEntity, TId, TParams, TApiParams, TModel>, IEntityProvider<TEntity, TModel>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApiParams>
    where TModel : IEntityModel<TId>
    where TEntity : IEntity<TId, TModel>
{
    static abstract IApiInOutRoute<TApiParams, TModel> CreateRoute(IPathable path, TApiParams args);
}

public interface IActorCreatable<TActor, out TId, in TParams, out TApiParams, out TApi> :
    IIdentifiable<TId>,
    IPathable
    where TActor : IActorCreatable<TActor, TId, TParams, TApiParams, TApi>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApiParams>
    where TApi : class
{
    static abstract IApiInOutRoute<TApiParams, TApi> CreateRoute(IPathable path, TParams args);
}

public interface ICanonicallyCreatable<TActor, TId> :
    IClientProvider,
    IIdentifiable<TId>,
    IPathable
    where TActor : ICanonicallyCreatable<TActor, TId>
    where TId : IEquatable<TId>
{
    static abstract IApiRoute CreateRoute(IPathable path, TId id);

    Task CreateAsync(RequestOptions? options = null, CancellationToken token = default)
    {
        return Client.RestApiClient.ExecuteAsync(
            TActor.CreateRoute(this, Id),
            options ?? Client.DefaultRequestOptions,
            token
        );
    }
}