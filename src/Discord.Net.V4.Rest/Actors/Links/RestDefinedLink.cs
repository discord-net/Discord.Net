using Discord.Models;

namespace Discord.Rest;

[BackLinkable]
public partial class RestDefinedLink<TActor, TId, TEntity, TModel>(
    DiscordRestClient client,
    RestIndexableLink<TActor, TId, TEntity, TModel> indexableLink,
    IReadOnlyCollection<TId> ids
) :
    IRestClientProvider,
    IDefinedLink<TActor, TId, TEntity, TModel>
    where TActor : class, IActor<TId, TEntity>, IEntityProvider<TEntity, TModel>
    where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public IReadOnlyCollection<TId> Ids { get; internal set; } = ids;

    public DiscordRestClient Client { get; } = client;

    [SourceOfTruth]
    internal TEntity CreateEntity(TModel model)
        => TEntity.Construct(Client, indexableLink[model.Id], model);
}