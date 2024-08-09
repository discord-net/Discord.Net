using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
internal sealed class ModifiableAttribute<TParams>(string route) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

[TemplateExtension(TakesPrecedenceOver = typeof(IModifiable<,,,,,>))]
public interface IModifiable<TId, in TSelf, out TParams, TApi, in TModel> :
    IModifiable<TId, TSelf, TParams, TApi>,
    IUpdatable<TModel>
    where TSelf :
    IModifiable<TId, TSelf, TParams, TApi, TModel>,
    IEntity<TId>,
    IUpdatable<TModel>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApi>, new()
    where TApi : class
    where TModel : class, IEntityModel<TId>
{
    new sealed Task ModifyAsync(Action<TParams> func, RequestOptions? options = null, CancellationToken token = default)
        => ModifyAsync(Client, (TSelf)this, Id, func, options, token);

    internal new static async Task ModifyAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options,
        CancellationToken token)
    {
        var args = new TParams();
        func(args);

        var model = await client.RestApiClient.ExecuteRequiredAsync(
            TSelf.ModifyRoute(self, id, args.ToApiModel()
            ),
            options ?? client.DefaultRequestOptions,
            token
        );

        if (model is not TModel entityModel)
            throw new DiscordException($"Expected model type '{typeof(TModel).Name}', got '{model.GetType().Name}'");

        await self.UpdateAsync(entityModel, token);
    }

    static Task IModifiable<TId, TSelf, TParams, TApi>.ModifyAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options,
        CancellationToken token
    ) => ModifyAsync(client, self, id, func, options, token);

    Task IModifiable<TId, TSelf, TParams, TApi>.ModifyAsync(Action<TParams> func, RequestOptions? options,
        CancellationToken token)
        => ModifyAsync(func, options, token);

    internal new static abstract IApiInOutRoute<TApi, IEntityModel> ModifyRoute(IPathable path, TId id, TApi args);

    static IApiInRoute<TApi> IModifiable<TId, TSelf, TParams, TApi>.ModifyRoute(IPathable path, TId id, TApi args)
        => TSelf.ModifyRoute(path, id, args);
}

[TemplateExtension]
public interface IModifiable<TId, in TSelf, out TParams, TApi, TEntity, in TModel> :
    IModifiable<TId, TSelf, TParams, TApi>,
    IEntityProvider<TEntity, TModel>
    where TSelf :
    IModifiable<TId, TSelf, TParams, TApi, TEntity, TModel>,
    IActor<TId, TEntity>,
    IEntityProvider<TEntity, TModel>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApi>, new()
    where TApi : class
    where TEntity : IEntity<TId>
    where TModel : class, IEntityModel<TId>
{
    [return: TypeHeuristic(nameof(CreateEntity))]
    new sealed Task<TEntity> ModifyAsync(Action<TParams> func, RequestOptions? options = null,
        CancellationToken token = default)
        => ModifyAsync(Client, (TSelf)this, Id, func, options, token);

    [return: TypeHeuristic(nameof(CreateEntity))]
    internal new static async Task<TEntity> ModifyAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options,
        CancellationToken token)
    {
        return self.CreateEntity(
            await ModifyAndReturnModelAsync(client, self, id, func, options, token)
        );
    }

    internal static async Task<TModel> ModifyAndReturnModelAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options,
        CancellationToken token)
    {
        var args = new TParams();
        func(args);

        var model = await client.RestApiClient.ExecuteRequiredAsync(
            TSelf.ModifyRoute(self, id, args.ToApiModel()
            ),
            options ?? client.DefaultRequestOptions,
            token
        );

        if (model is not TModel entityModel)
            throw new DiscordException($"Expected model type '{typeof(TModel).Name}', got '{model.GetType().Name}'");

        return entityModel;
    }

    static Task IModifiable<TId, TSelf, TParams, TApi>.ModifyAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options,
        CancellationToken token
    ) => ModifyAsync(client, self, id, func, options, token);

    Task IModifiable<TId, TSelf, TParams, TApi>.ModifyAsync(Action<TParams> func, RequestOptions? options,
        CancellationToken token)
        => ModifyAsync(func, options, token);

    internal new static abstract IApiInOutRoute<TApi, IEntityModel> ModifyRoute(IPathable path, TId id, TApi args);

    static IApiInRoute<TApi> IModifiable<TId, TSelf, TParams, TApi>.ModifyRoute(IPathable path, TId id, TApi args)
        => TSelf.ModifyRoute(path, id, args);
}

[TemplateExtension, NoExposure]
public interface IModifiable<TId, in TSelf, out TParams, TApi> :
    IIdentifiable<TId>,
    IClientProvider,
    IPathable
    where TSelf : IModifiable<TId, TSelf, TParams, TApi>
    where TId : IEquatable<TId>
    where TParams : IEntityProperties<TApi>, new()
    where TApi : class
{
    Task ModifyAsync(Action<TParams> func, RequestOptions? options = null, CancellationToken token = default)
        => TSelf.ModifyAsync(Client, (TSelf)this, Id, func, options, token);

    internal static virtual Task ModifyAsync(
        IDiscordClient client,
        TSelf self,
        TId id,
        Action<TParams> func,
        RequestOptions? options = null,
        CancellationToken token = default)
    {
        var args = new TParams();
        func(args);
        return client.RestApiClient.ExecuteAsync(
            TSelf.ModifyRoute(self, id, args.ToApiModel()),
            options ?? client.DefaultRequestOptions,
            token
        );
    }

    internal static abstract IApiInRoute<TApi> ModifyRoute(IPathable path, TId id, TApi args);
}
