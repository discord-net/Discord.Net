using Discord.Models;
using Discord.Rest;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Gateway;

internal static partial class GatewayActorExtensions
{
    [return: NotNullIfNotNull(nameof(id))]
    public static TActor? UpdateFrom<TId, TEntity, [Shrink] TModel, TActor>(
        [TransitiveFill] this TActor? actor,
        TId? id,
        [VariableFuncArgs] Func<IIdentifiable<TId, TEntity, TActor, TModel>, TActor> factory,
        Func<TId, IIdentifiable<TId, TEntity, TActor, TModel>?>? identityFactory = null
    )
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TId : struct, IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IGatewayActor<TId, TEntity, IIdentifiable<TId, TEntity, TActor, TModel>>
    {
        if (id is null && actor is null)
            return null;

        if (actor is null && id is not null)
        {
            var identity = identityFactory?.Invoke(id.Value)
                           ?? IIdentifiable<TId, TEntity, TActor, TModel>.Of(id.Value);

            return factory(identity);
        }

        if (actor is not null && id is not null && !actor.Id.Equals(id.Value))
        {
            var identity = identityFactory?.Invoke(id.Value)
                           ?? IIdentifiable<TId, TEntity, TActor, TModel>.Of(id.Value);

            return factory(identity);
        }

        return actor as TActor;
    }
}
