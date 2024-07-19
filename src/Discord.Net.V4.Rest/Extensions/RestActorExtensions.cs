using Discord.Models;
using Discord.Rest.Actors;
using Discord.Rest.Channels;
using System.Diagnostics.CodeAnalysis;

namespace Discord.Rest.Extensions;

public static partial class RestActorExtensions
{
    public static async ValueTask<TEntity?> UpdateFromAsync<TId, TEntity, TModel>(
        [TransitiveFill] this TEntity? entity,
        TModel? model,
        [VariableFuncArgs] Func<TModel, TEntity> factory,
        CancellationToken token = default
    )
        where TEntity : RestEntity<TId>, IUpdatable<TModel>
        where TModel : IEntityModel<TId>
        where TId : struct, IEquatable<TId>
    {
        if (model is null)
            return null;

        if (entity is null)
            return factory(model);

        await entity.UpdateAsync(model, token);

        return entity;
    }

    [return: NotNullIfNotNull(nameof(id))]
    public static TActor? UpdateFrom<TId, TEntity, TModel, TActor>(
        [TransitiveFill] this TActor? actor,
        TId? id,
        [VariableFuncArgs] Func<IIdentifiableEntityOrModel<TId, TEntity, TModel>, TActor> factory,
        Func<TId, IIdentifiableEntityOrModel<TId, TEntity, TModel>?>? identityFactory = null
    )
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TId : struct, IEquatable<TId>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IRestActor<TId, TEntity, IIdentifiableEntityOrModel<TId, TEntity, TModel>>
    {
        if (id is null && actor is null)
            return null;

        if (actor is null && id is not null)
        {
            var identity = identityFactory?.Invoke(id.Value)
                           ?? IIdentifiableEntityOrModel<TId, TEntity, TModel>.Of(id.Value);

            return factory(identity);
        }

        if (actor is not null && id is not null && !actor.Id.Equals(id.Value))
        {
            var identity = identityFactory?.Invoke(id.Value)
                           ?? IIdentifiableEntityOrModel<TId, TEntity, TModel>.Of(id.Value);

            return factory(identity);
        }

        return actor as TActor;
    }
}
