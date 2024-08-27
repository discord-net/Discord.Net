using Discord.Models;

namespace Discord.Rest;

internal static partial class RestActors
{
    public static ILinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable.BackLink<RestGuildActor> GuildChannel<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        TModel
    >(
        Template<TActor> template,
        RestGuildActor guild,
        ApiModelProviderDelegate<IEnumerable<IGuildChannelModel>> apiProvider
    )
        where TActor : 
        class,
        IRestActor<TId, TEntity, TModel>, 
        IFactory<TActor, DiscordRestClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
        where TModel : class, IEntityModel<TId>
        
    {
        return new RestLinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable.BackLink<RestGuildActor>(
            guild,
            guild.Client,
            new RestActorProvider<TId, TActor>(
                (client, id) => TActor.Factory(client, guild.Identity, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
            ),
            apiProvider.OfType<TModel>()
        );
    }
    
    public static ILinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable.BackLink<RestGuildActor> GuildRelatedLink<
        [TransitiveFill] TActor,
        TId,
        TEntity,
        TModel
    >(
        Template<TActor> template,
        RestGuildActor guild
    )
        where TActor : 
        class,
        IRestActor<TId, TEntity, TModel>, 
        IFactory<TActor, DiscordRestClient, GuildIdentity, IIdentifiable<TId, TEntity, TActor, TModel>>
        where TId : IEquatable<TId>
        where TEntity : 
        class, 
        IEntity<TId, TModel>,
        IRestConstructable<TEntity, TActor, TModel>,
        IFetchableOfMany<TId, TModel>
        where TModel : class, IEntityModel<TId>
        
    {
        return new RestLinkType<TActor, TId, TEntity, TModel>.Enumerable.Indexable.BackLink<RestGuildActor>(
            guild,
            guild.Client,
            new RestActorProvider<TId, TActor>(
                (client, id) => TActor.Factory(client, guild.Identity, IIdentifiable<TId, TEntity, TActor, TModel>.Of(id))
            ),
            TEntity.FetchManyRoute(guild).AsRequiredProvider()
        );
    }
}