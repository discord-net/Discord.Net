using Discord.Models;
using Discord.Rest;
using Discord.Rest.Pipeline;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class LoadableAttribute<TRoute> : Attribute
    where TRoute : IRouteOperation<TRoute>;

#pragma warning restore CS9113 // Parameter is unread.

public static class LoadableExt
{
    public static void FetchAsyncTest<TRoute>(
        this IFetchable<TRoute> loadable
    )
        where TRoute : class, IRouteOperation<TRoute>
    {
        IGuildChannelActor x = null!;

        x.FetchAsyncTest();
    }
}

public interface ILoadable<TRoute, TEntity, TModel> :
    IFetchable<TRoute, TModel>,
    IEntityProvider<TEntity, TModel>
    where TRoute : IRouteOperation<TRoute>
    where TEntity : IEntityOf<TModel>
    where TModel : class, IModel
{
    ValueTask<TEntity?> GetAsync(RequestOptions? options = null, CancellationToken token = default);
    
    ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
        => CreatePipeline(options)
            .IfNotNull()
            .Transform(CreateEntityAsync)
            .AsNullable()
            .RunAsync(Client, token);

    async ValueTask<TEntity?> GetOrFetchAsync(RequestOptions? options = null, CancellationToken token = default)
        => await GetAsync(options, token) ?? await FetchAsync(options, token);
}

// public interface ILoadable<TSelf, TId, TEntity, TModel> :
//     IFetchable<TId, TModel>,
//     IEntityProvider<TEntity, TModel>,
//     IIdentifiable<TId>,
//     IPathable
//     where TSelf : ILoadable<TSelf, TId, TEntity, TModel>
//     where TEntity : class, IEntity<TId>
//     where TId : IEquatable<TId>
//     where TModel : class, IEntityModel<TId>
// {
//     async ValueTask<TEntity?> GetOrFetchAsync(RequestOptions? options = null, CancellationToken token = default)
//     {
//         if ((options ?? Client.DefaultRequestOptions).AllowCached)
//         {
//             var result = await GetAsync(token);
//
//             if (result is not null)
//                 return result;
//         }
//
//         return await FetchAsync(options, token);
//     }
//
//     ValueTask<TEntity?> GetAsync(CancellationToken token = default) => default;
//
//     ValueTask<TEntity?> FetchAsync(RequestOptions? options = null, CancellationToken token = default)
//         => FetchInternalAsync(Client, (TSelf)this, TSelf.FetchRoute(this, Id), options, token);
//
//     internal static async ValueTask<TEntity?> FetchInternalAsync(
//         IDiscordClient client,
//         TSelf entityProvider,
//         IApiOutRoute<TModel> route,
//         RequestOptions? options = null,
//         CancellationToken token = default)
//     {
//         var model = await client.RestApiClient.ExecuteAsync(
//             route,
//             options ?? client.DefaultRequestOptions,
//             token
//         );
//
//         return model is null ? null : await entityProvider.CreateEntityAsync(model, token);
//     }
// }