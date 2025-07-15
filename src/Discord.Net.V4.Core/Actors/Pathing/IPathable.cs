using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Discord.Models;

namespace Discord;

file sealed class EmptyPath : IPathable
{
    public static readonly EmptyPath Instance = new();
    
    public Optional<TId> Get<TId, TEntity>()
        where TId : IEquatable<TId>
        where TEntity : IIdentifiable<TId>
        => default;
}

public interface IPathEntry
{
    PathEntryType Type { get; }
}

public interface IPathEntry<out T> : IPathEntry
{
    T Value { get; }
}

public enum PathEntryType
{
    Variable,
    Id
}

public interface IPathable : 
    IEnumerable<IPathEntry>
{
    static IPathable Empty => EmptyPath.Instance;

    Optional<TId> Get<TId, TEntity>() 
        where TEntity : IIdentifiable<TId>
        where TId : IEquatable<TId>;

    // internal U Variable<T, U>()
    //     where T : IPathIdProvider<U>
    //     where U : IEquatable<U>
    // {
    //     if (this is not T provider)
    //         throw new InvalidOperationException(
    //             $"Expected {GetType()} to implement {typeof(T)}"
    //         );
    //     
    //     return provider.PathId;
    // }
    //
    // internal IIdentifiable<TId, TEntity, TActor, TModel> RequireIdentity<TActor, TId, TEntity, TModel>(
    //     Template<IIdentifiable<TId, TEntity, TActor, TModel>> template
    // )
    //     where TId : IEquatable<TId>
    //     where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    //     where TModel : class, IEntityModel<TId>
    //     where TActor : class, IActor<TId, TEntity>
    // {
    //     return this switch
    //     {
    //         TEntity entity => IIdentifiable<TId, TEntity, TActor, TModel>.Of(entity),
    //         TActor actor => IIdentifiable<TId, TEntity, TActor, TModel>.Of(actor),
    //         IRelationship<TActor, TId, TEntity> relationship => IIdentifiable<TId, TEntity, TActor, TModel>.Of(relationship.RelationshipActor),
    //         IRelation<TId, TEntity> relationship => IIdentifiable<TId, TEntity, TActor, TModel>.Of(relationship.RelationshipId),
    //         _ => throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}")
    //     };
    // }
    //
    // internal bool TryGetIdentity<TActor, TId, TEntity, TModel>(
    //     Template<IIdentifiable<TId, TEntity, TActor, TModel>> template,
    //     [MaybeNullWhen(false)]
    //     out IIdentifiable<TId, TEntity, TActor, TModel> identity
    // )
    //     where TId : IEquatable<TId>
    //     where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    //     where TModel : class, IEntityModel<TId>
    //     where TActor : class, IActor<TId, TEntity>
    // {
    //     switch (this)
    //     {
    //         case TEntity entity:
    //             identity = IIdentifiable<TId, TEntity, TActor, TModel>.Of(entity);
    //             return true;
    //         case TActor actor:
    //             identity = IIdentifiable<TId, TEntity, TActor, TModel>.Of(actor);
    //             return true;
    //         case IRelationship<TActor, TId, TEntity> relationship:
    //             identity = IIdentifiable<TId, TEntity, TActor, TModel>.Of(relationship.RelationshipActor);
    //             return true;
    //         case IRelation<TId, TEntity> relation:
    //             identity = IIdentifiable<TId, TEntity, TActor, TModel>.Of(relation.RelationshipId);
    //             return true;
    //     }
    //
    //     identity = null;
    //     return false;
    // }
    //
    // ulong Require<TEntity>()
    //     where TEntity : class, IEntity<ulong>
    //     => Require<ulong, TEntity>();
    //
    // ulong? Optionally<TEntity>()
    //     where TEntity : class, IEntity<ulong>
    //     => TryGet<ulong, TEntity>(out var id) ? id : null;
    //
    // TId? Optionally<TId, TEntity>()
    //     where TEntity : class, IEntity<TId>
    //     where TId : struct, IEquatable<TId>
    //     => TryGet<TId, TEntity>(out var id) ? id : null;
    //
    // TId Require<TId, TEntity>()
    //     where TEntity : class, IEntity<TId>
    //     where TId : IEquatable<TId>
    // {
    //     return this switch
    //     {
    //         TEntity entity => entity.Id,
    //         IActor<TId, TEntity> actor => actor.Id,
    //         IRelation<TId, TEntity> relationship => relationship.RelationshipId,
    //         _ => throw new KeyNotFoundException($"Cannot find path from {GetType().Name} to {typeof(TEntity).Name}")
    //     };
    // }
    //
    // bool TryGet<TId, TEntity>([MaybeNullWhen(false)] out TId id)
    //     where TEntity : class, IEntity<TId>
    //     where TId : IEquatable<TId>
    // {
    //     switch (this)
    //     {
    //         case TEntity entity:
    //             id = entity.Id;
    //             return true;
    //         case IActor<TId, TEntity> actor:
    //             id = actor.Id;
    //             return true;
    //         case IRelation<TId, TEntity> relationship:
    //             id = relationship.RelationshipId;
    //             return true;
    //         default:
    //             id = default;
    //             return false;
    //     }
    // }
}