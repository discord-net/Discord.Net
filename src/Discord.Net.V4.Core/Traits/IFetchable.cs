using Discord.Models;
using Discord.Rest;
using Discord.Rest.Pipeline;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class FetchableAttribute<TRoute> : Attribute
    where TRoute : IRouteOperation<TRoute>;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class FetchableOfManyAttribute<TRoute> : Attribute
    where TRoute : IRouteOperation<TRoute>;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class PagedFetchableOfManyAttribute<TRoute, TPageParams> : Attribute
    where TPageParams : IPagingParams
    where TRoute : IRouteOperation<TRoute>;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class PagedFetchableOfManyAttribute<TRoute, TPageParams, TPagedEntity> : Attribute
    where TRoute : IRouteOperation<TRoute>
    where TPageParams : IPagingParams;

#pragma warning restore CS9113 // Parameter is unread.

public interface IFetchable<TRoute> : IPathable
    where TRoute : IRouteOperation<TRoute>;

public interface IFetchable<TRoute, TModel> : IFetchable<TRoute>
    where TRoute : IRouteOperation<TRoute>
{
    internal IRestPipeline<TModel?> CreatePipeline(RequestOptions? options)
        => TRoute.Create(this)
            .AsPipeline(options)
            .Deserialize<TModel>();
}

public interface IFetchableOfMany<TRoute, TModel> : IPathable
    where TRoute : IRouteOperation<TRoute>
{
    internal IRestPipeline<IReadOnlyCollection<TModel>?> CreatePipeline(RequestOptions? options)
        => TRoute.Create(this)
            .AsPipeline(options)
            .Deserialize<IReadOnlyCollection<TModel>>();
}

public interface IPagedFetchable<TRoute, TApiModel, TPageParams>
    where TRoute : IRouteOperation<TRoute>
    where TPageParams : IPagingParams<TRoute, TApiModel>
    where TApiModel : class;

// public interface IFetchableNew<TRoute, TModel>
//     where TRoute : IRouteOperation.Body<TModel>
// {
// }
//
// public interface IFetchable<in TId, out TModel>
//     where TId : IEquatable<TId>
//     where TModel : IEntityModel<TId>
// {
//     internal static abstract IApiOutRoute<TModel> FetchRoute(IPathable path, TId id);
// }
//
// public interface IFetchableOfMany<in TId, out TModel>
//     where TId : IEquatable<TId>
//     where TModel : IEntityModel<TId>
// {
//     internal static abstract IApiOutRoute<IEnumerable<TModel>> FetchManyRoute(IPathable path);
// }
//
// public interface IPagedFetchableOfMany<in TId, out TModel, TParams, TApi>
//     where TId : IEquatable<TId>
//     where TModel : IEntityModel<TId>
//     where TParams : class, IPagingParams<TParams, TApi>
//     where TApi : class;

