using Discord.Models;

namespace Discord.Rest;

internal static class LinkExtensions
{
    public static void AddModelSources
        <TActor, TId, TEntity, TModel>
        (
            this ILinkType<TActor, TId, TEntity, TModel>.Indexable link,
            Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
            IModelSourceOfMultiple<TModel> source
        )
        where TActor : RestActor<TActor, TId, TEntity, TModel>
        where TEntity : RestEntity<TId>, IEntity<TId, TModel>, IRestConstructable<TEntity, TActor, TModel>
        where TId : IEquatable<TId>
        where TModel : class, IEntityModel<TId>
    {
        foreach (var model in source.GetModels())
            link[model.Id].AddModelSource(model);
    }
}