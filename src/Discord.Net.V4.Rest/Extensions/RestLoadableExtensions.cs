using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest.Extensions;

public static partial class RestLoadableExtensions
{
    [return: NotNullIfNotNull(nameof(id))]
    public static TResult? UpdateFrom<TId, TEntity, TCoreEntity, TModel, TResult, TIdentity>(
        this IRestLoadableActor<TId, TEntity, TCoreEntity, TModel>? loadableActor,
        TId? id,
        [VariableFuncArgs] Func<TIdentity, TResult> factory,
        Func<TId, TIdentity> identityFactory
    )
        where TCoreEntity : class, IEntity<TId>
        where TEntity     : class, IEntity<TId>, IEntityOf<TModel>, TCoreEntity
        where TId         : struct, IEquatable<TId>
        where TModel      : class, IEntityModel<TId>
        where TResult     : class, IRestLoadableActor<TId, TEntity, TCoreEntity, TModel>
        where TIdentity   : class, IIdentifiableEntityOrModel<TId, TEntity, TModel>
    {
        if (id is null && loadableActor is not null)
            return null;

        if (loadableActor is null && id is not null)
            return factory(identityFactory(id.Value));

        if (loadableActor is not null && id is not null && !id.Value.Equals(loadableActor.Loadable.Id))
        {
            loadableActor.Loadable.Identity = identityFactory(id.Value);
        }

        return loadableActor as TResult;
    }
}
