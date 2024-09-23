using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

#region Casted

file sealed class CastedIdentifiable<
    TId,
    TSourceEntity,
    TSourceModel,
    TDestinationModel,
    TDestinationEntity>(
    IIdentifiable<TId, TSourceEntity, TSourceModel> source
) :
    IIdentifiable<TId, TDestinationEntity, TDestinationModel>
    where TId : IEquatable<TId>
    where TSourceEntity : class, IEntity<TId>, IEntityOf<TSourceModel>
    where TSourceModel : class, IEntityModel<TId>
    where TDestinationEntity : class, IEntity<TId>, IEntityOf<TDestinationModel>
    where TDestinationModel : class, IEntityModel<TId>
{
    public TId Id => source.Id;

    public TDestinationEntity? Entity
    {
        get
        {
            var entity = source.Entity;

            if (entity is null)
                return null;

            if (entity is not TDestinationEntity destinationEntity)
                throw new InvalidOperationException(
                    $"Expected an entity of type {typeof(TDestinationEntity).Name}, but got {entity.GetType().Name}"
                );

            return destinationEntity;
        }
    }

    public IdentityDetail Detail => source.Detail;

    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    Type IIdentifiable<TId, TDestinationEntity, TDestinationModel>.EntityType => typeof(TSourceEntity);

    Type IIdentifiable<TId, TDestinationEntity, TDestinationModel>.ModelType => typeof(TSourceModel);
}

file sealed class CastedIdentifiable<
    TId,
    TSourceEntity,
    TSourceActor,
    TSourceModel,
    TDestinationModel,
    TDestinationActor,
    TDestinationEntity>(
    IIdentifiable<TId, TSourceEntity, TSourceActor, TSourceModel> source
) :
    IIdentifiable<TId, TDestinationEntity, TDestinationActor, TDestinationModel>
    where TId : IEquatable<TId>
    where TSourceEntity : class, IEntity<TId>, IEntityOf<TSourceModel>
    where TSourceActor : class, IActor<TId, TSourceEntity>
    where TSourceModel : class, IEntityModel<TId>
    where TDestinationEntity : class, IEntity<TId>, IEntityOf<TDestinationModel>
    where TDestinationActor : class, IActor<TId, TDestinationEntity>
    where TDestinationModel : class, IEntityModel<TId>
{
    public TId Id => source.Id;

    public TDestinationActor? Actor
    {
        get
        {
            var actor = source.Actor;

            if (actor is null)
                return null;

            if (actor is not TDestinationActor destinationActor)
                throw new InvalidOperationException(
                    $"Expected an actor of type {typeof(TDestinationActor).Name}, but got {actor.GetType().Name}"
                );

            return destinationActor;
        }
    }

    public TDestinationEntity? Entity
    {
        get
        {
            var entity = source.Entity;

            if (entity is null)
                return null;

            if (entity is not TDestinationEntity destinationEntity)
                throw new InvalidOperationException(
                    $"Expected an entity of type {typeof(TDestinationEntity).Name}, but got {entity.GetType().Name}"
                );

            return destinationEntity;
        }
    }


    public IdentityDetail Detail => source.Detail;

    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    Type IIdentifiable<TId, TDestinationEntity, TDestinationModel>.EntityType => typeof(TSourceEntity);

    Type IIdentifiable<TId, TDestinationEntity, TDestinationModel>.ModelType => typeof(TSourceModel);
}

#endregion

#region Implementations

file sealed class Identifiable<TId, TEntity, TActor, TModel> :
    IIdentifiable<TId, TEntity, TActor, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IActor<TId, TEntity>
{
    public TId Id { get; }

    public TEntity? Entity
    {
        get
        {
            if (_entity is not null)
                return _entity;

            if (this is {_model: not null, _entityFactory: not null})
                return _entity = _entityFactory(_model);

            return null;
        }
    }

    public TActor? Actor => _actor ?? Entity switch
    {
        TActor actor => actor,
        IProxied<TActor> proxied => proxied.ProxiedValue,
        _ => null
    };

    public IdentityDetail Detail
    {
        get
        {
            if (_entity is not null)
                return IdentityDetail.Entity;

            if (_entityFactory is not null)
                return IdentityDetail.EntityFactory;

            if (_actor is not null)
                return IdentityDetail.Actor;

            return IdentityDetail.Id;
        }
    }

    private readonly Func<TModel, TEntity>? _entityFactory;
    private TEntity? _entity;
    private readonly TActor? _actor;
    private readonly TModel? _model;

    public Identifiable(TId id)
    {
        Id = id;
    }

    public Identifiable(TModel model, Func<TModel, TEntity> factory)
    {
        _model = model;
        _entityFactory = factory;
        Id = model.Id;
    }

    public Identifiable(TEntity entity)
    {
        Id = entity.Id;
        _entity = entity;
    }

    public Identifiable(TActor actor)
    {
        Id = actor.Id;
        _actor = actor;
    }

    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    public static implicit operator Identifiable<TId, TEntity, TActor, TModel>(TId id) => new(id);
    public static implicit operator Identifiable<TId, TEntity, TActor, TModel>(TEntity entity) => new(entity);

    Type IIdentifiable<TId, TEntity, TModel>.EntityType => typeof(TEntity);

    Type IIdentifiable<TId, TEntity, TModel>.ModelType => typeof(TModel);
}

file sealed class Identifiable<TId, TEntity, TModel> :
    IIdentifiable<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
{
    public TId Id { get; }

    public TEntity? Entity
    {
        get
        {
            if (_entity is not null)
                return _entity;

            if (this is {_model: not null, _entityFactory: not null})
                return _entity = _entityFactory(_model);

            return null;
        }
    }

    public IdentityDetail Detail
    {
        get
        {
            if (_entity is not null)
                return IdentityDetail.Entity;

            if (_entityFactory is not null)
                return IdentityDetail.EntityFactory;

            return IdentityDetail.Id;
        }
    }

    private readonly Func<TModel, TEntity>? _entityFactory;
    private TEntity? _entity;
    private readonly TModel? _model;

    public Identifiable(TId id)
    {
        Id = id;
    }

    public Identifiable(TModel model, Func<TModel, TEntity> factory)
    {
        _model = model;
        _entityFactory = factory;
        Id = model.Id;
    }

    public Identifiable(TEntity entity)
    {
        Id = entity.Id;
        _entity = entity;
    }

    public override int GetHashCode()
        => EqualityComparer<TId>.Default.GetHashCode(Id);

    public static implicit operator Identifiable<TId, TEntity, TModel>(TId id) => new(id);
    public static implicit operator Identifiable<TId, TEntity, TModel>(TEntity entity) => new(entity);

    Type IIdentifiable<TId, TEntity, TModel>.EntityType => typeof(TEntity);

    Type IIdentifiable<TId, TEntity, TModel>.ModelType => typeof(TModel);
}

#endregion

#region Interfaces

public enum IdentityDetail
{
    Id,
    Actor,
    EntityFactory,
    Entity,
}

public interface ISnowflakeIdentifiable : IIdentifiable<ulong>;

public interface IIdentityDetail
{
    internal IdentityDetail Detail { get; }
}

public interface IIdentifiable<out TId> : IIdentityDetail
    where TId : IEquatable<TId>
{
    TId Id { get; }

    internal new IdentityDetail Detail => IdentityDetail.Id;

    IdentityDetail IIdentityDetail.Detail => Detail;
}

public interface IIdentifiable<TId, out TEntity, out TModel> :
    IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
{
    Type EntityType { get; }
    Type ModelType { get; }

    TEntity? Entity { get; }

    public static IIdentifiable<TId, TEntity, TModel> operator |(
        IIdentifiable<TId, TEntity, TModel> left,
        IIdentifiable<TId, TEntity, TModel> right
    ) => left.MostSpecific(right);

    public static IIdentifiable<TId, TEntity, TModel> operator |(
        IIdentifiable<TId, TEntity, TModel> left,
        TEntity right
    ) => left.MostSpecific(right);

    // static IIdentifiable<TId, TEntity, TModel> FromReferenced<TConstruct, TClient>(
    //     IModel model,
    //     TId id,
    //     TClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel, TClient>
    //     where TClient : IDiscordClient
    //     => model is IModelSource source
    //         ? FromReferenced<TConstruct, TClient>(source, id, client)
    //         : Of(id);
    //
    // static IIdentifiable<TId, TEntity, TModel> FromReferenced<TConstruct, TClient>(
    //     IModelSource model,
    //     TId id,
    //     TClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel, TClient>
    //     where TClient : IDiscordClient
    //     => FromReferenced(
    //         model,
    //         id,
    //         model => TConstruct.Construct(client, model) as TEntity ?? throw new InvalidOperationException(
    //             $"{typeof(TConstruct)} is not constructable for {typeof(TEntity)}"
    //         )
    //     );
    //
    // static IIdentifiable<TId, TEntity, TModel> FromReferenced<TConstruct>(
    //     IModel model,
    //     TId id,
    //     IDiscordClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel>
    //     => model is IModelSource source
    //         ? FromReferenced<TConstruct>(source, id, client)
    //         : Of(id);
    //
    // static IIdentifiable<TId, TEntity, TModel> FromReferenced<TConstruct>(
    //     IModelSource model,
    //     TId id,
    //     IDiscordClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel>
    //     => FromReferenced(
    //         model,
    //         id,
    //         model => TConstruct.Construct(client, model) as TEntity ?? throw new InvalidOperationException(
    //             $"{typeof(TConstruct)} is not constructable for {typeof(TEntity)}"
    //         )
    //     );
    //
    // static IIdentifiable<TId, TEntity, TModel> FromReferenced(
    //     IModel model,
    //     TId id,
    //     Func<TModel, TEntity> factory)
    //     => model is IModelSource source
    //         ? FromReferenced(source, id, factory)
    //         : Of(id);
    //
    // static IIdentifiable<TId, TEntity, TModel> FromReferenced(
    //     IModelSource model,
    //     TId id,
    //     Func<TModel, TEntity> factory)
    // {
    //     var referenced = model.GetReferencedEntityModel<TId, TModel>(id);
    //
    //     return referenced is null
    //         ? Of(id)
    //         : Of(referenced, factory);
    // }

    static IIdentifiable<TId, TEntity, TModel> Of(TId id)
        => new Identifiable<TId, TEntity, TModel>(id);

    static IIdentifiable<TId, TEntity, TModel> Of(TEntity entity)
        => new Identifiable<TId, TEntity, TModel>(entity);

    static IIdentifiable<TId, TEntity, TModel> Of(TModel model, Func<TModel, TEntity> factory)
        => new Identifiable<TId, TEntity, TModel>(model, factory);

    static IIdentifiable<TId, TEntity, TModel>? OfNullable(TModel? model, Func<TModel, TEntity> factory)
        => model is not null ? Of(model, factory) : null;

    IIdentifiable<TId, TNewEntity, TNewModel> Cast<TNewEntity, TNewModel>()
        where TNewEntity : class, IEntity<TId>, IEntityOf<TNewModel>
        where TNewModel : class, IEntityModel<TId>
        => new CastedIdentifiable<TId, TEntity, TModel, TNewModel, TNewEntity>(this);
}

public interface IIdentifiable<TId, out TEntity, out TActor, out TModel> :
    IIdentifiable<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
    where TActor : class, IActor<TId, TEntity>
{
    TActor? Actor { get; }

    public static IIdentifiable<TId, TEntity, TActor, TModel> operator |(
        IIdentifiable<TId, TEntity, TActor, TModel>? left,
        TId right
    ) => left?.MostSpecific(right) ?? Of(right);

    public static IIdentifiable<TId, TEntity, TActor, TModel> operator |(
        IIdentifiable<TId, TEntity, TActor, TModel>? left,
        IIdentifiable<TId> right
    ) => left?.MostSpecific(right) ?? Of(right.Id);

    public static IIdentifiable<TId, TEntity, TActor, TModel> operator |(
        IIdentifiable<TId, TEntity, TActor, TModel>? left,
        IIdentifiable<TId, TEntity, TActor, TModel> right
    ) => left?.MostSpecific(right) ?? right;

    public static IIdentifiable<TId, TEntity, TActor, TModel> operator |(
        IIdentifiable<TId, TEntity, TActor, TModel>? left,
        TEntity right
    ) => left?.MostSpecific(right) ?? Of(right);

    public static IIdentifiable<TId, TEntity, TActor, TModel> operator |(
        IIdentifiable<TId, TEntity, TActor, TModel>? left,
        TActor right
    ) => left?.MostSpecific(right) ?? Of(right);

    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced<TConstruct, TClient>(
    //     IModel model,
    //     TId id,
    //     TClient client
    // )
    //     where TConstruct : class, IConstructable<TConstruct, TModel, TClient>
    //     where TClient : IDiscordClient
    //     => model is IModelSource source
    //         ? FromReferenced<TConstruct, TClient>(source, id, client)
    //         : Of(id);
    //
    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced<TConstruct, TClient>(
    //     IModelSource model,
    //     TId id,
    //     TClient client
    // )
    //     where TConstruct : class, IConstructable<TConstruct, TModel, TClient>
    //     where TClient : IDiscordClient
    //     => FromReferenced(
    //         model,
    //         id,
    //         model => TConstruct.Construct(client, model) as TEntity ?? throw new InvalidOperationException(
    //             $"{typeof(TConstruct)} is not constructable for {typeof(TEntity)}"
    //         )
    //     );
    //
    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced<TConstruct>(
    //     IModel model,
    //     TId id,
    //     IDiscordClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel>
    //     => model is IModelSource source
    //         ? FromReferenced<TConstruct>(source, id, client)
    //         : Of(id);
    //
    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced<TConstruct>(
    //     IModelSource model,
    //     TId id,
    //     IDiscordClient client)
    //     where TConstruct : class, IConstructable<TConstruct, TModel>
    //     => FromReferenced(
    //         model,
    //         id,
    //         model => TConstruct.Construct(client, model) as TEntity ?? throw new InvalidOperationException(
    //             $"{typeof(TConstruct)} is not constructable for {typeof(TEntity)}"
    //         )
    //     );
    //
    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced(
    //     IModel model,
    //     TId id,
    //     Func<TModel, TEntity> factory)
    //     => model is IModelSource source
    //         ? FromReferenced(source, id, factory)
    //         : Of(id);
    //
    // new static IIdentifiable<TId, TEntity, TActor, TModel> FromReferenced(
    //     IModelSource model,
    //     TId id,
    //     Func<TModel, TEntity> factory)
    // {
    //     var referenced = model.GetReferencedEntityModel<TId, TModel>(id);
    //
    //     return referenced is null
    //         ? Of(id)
    //         : Of(referenced, factory);
    // }

    new static IIdentifiable<TId, TEntity, TActor, TModel> Of(TId id)
        => new Identifiable<TId, TEntity, TActor, TModel>(id);

    new static IIdentifiable<TId, TEntity, TActor, TModel> Of(TEntity entity)
        => new Identifiable<TId, TEntity, TActor, TModel>(entity);

    static IIdentifiable<TId, TEntity, TActor, TModel> Of(TActor actor)
        => new Identifiable<TId, TEntity, TActor, TModel>(actor);

    new static IIdentifiable<TId, TEntity, TActor, TModel> Of(TModel model, Func<TModel, TEntity> factory)
        => new Identifiable<TId, TEntity, TActor, TModel>(model, factory);

    new static IIdentifiable<TId, TEntity, TActor, TModel>? OfNullable(TModel? model, Func<TModel, TEntity> factory)
        => model is not null ? Of(model, factory) : null;

    IIdentifiable<TId, TNewEntity, TNewActor, TNewModel> Cast<TNewEntity, TNewActor, TNewModel>()
        where TNewEntity : class, IEntity<TId>, IEntityOf<TNewModel>
        where TNewActor : class, IActor<TId, TNewEntity>
        where TNewModel : class, IEntityModel<TId>
        => new CastedIdentifiable<TId, TEntity, TActor, TModel, TNewModel, TNewActor, TNewEntity>(this);

    internal IIdentifiable<TId, TNewEntity, TNewActor, TNewModel> Cast
        <TNewEntity, TNewActor, TNewModel>
        (
            Template<IIdentifiable<TId, TNewEntity, TNewActor, TNewModel>> template
        )
        where TNewEntity : class, IEntity<TId>, IEntityOf<TNewModel>
        where TNewActor : class, IActor<TId, TNewEntity>
        where TNewModel : class, IEntityModel<TId>
        => new CastedIdentifiable<TId, TEntity, TActor, TModel, TNewModel, TNewActor, TNewEntity>(this);
}

#endregion

#region Extensions

public static class IIdentifiableExtensions
{
    public static IIdentifiable<TId, TEntity, TActor, TModel> MostSpecific<TId, TEntity, TActor, TModel>(
        this IIdentifiable<TId, TEntity, TActor, TModel> self,
        IIdentifiable<TId> other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (self.Detail < other.Detail)
            return IIdentifiable<TId, TEntity, TActor, TModel>.Of(other.Id);
        return self;
    }

    public static IIdentifiable<TId, TEntity, TActor, TModel> MostSpecific<TId, TEntity, TActor, TModel>(
        this IIdentifiable<TId, TEntity, TActor, TModel> self,
        IIdentifiable<TId, TEntity, TActor, TModel> other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (self.Detail < other.Detail)
            return other;
        return self;
    }

    public static IIdentifiable<TId, TEntity, TActor, TModel> MostSpecific<TId, TEntity, TActor, TModel>(
        this IIdentifiable<TId, TEntity, TActor, TModel> self,
        TActor other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (self.Detail < IdentityDetail.Actor)
            return IIdentifiable<TId, TEntity, TActor, TModel>.Of(other);
        return self;
    }

    public static IIdentifiable<TId, TEntity, TActor, TModel> MostSpecific<TId, TEntity, TActor, TModel>(
        this IIdentifiable<TId, TEntity, TActor, TModel> self,
        TEntity other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (self.Detail < IdentityDetail.Entity)
            return IIdentifiable<TId, TEntity, TActor, TModel>.Of(other);
        return self;
    }

    public static IIdentifiable<TId, TEntity, TModel> MostSpecific<TId, TEntity, TModel>(
        this IIdentifiable<TId, TEntity, TModel> self,
        IIdentifiable<TId, TEntity, TModel> other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (self.Detail < other.Detail)
            return other;
        return self;
    }

    public static IIdentifiable<TId, TEntity, TModel> MostSpecific<TId, TEntity, TModel>(
        this IIdentifiable<TId, TEntity, TModel> self,
        TEntity other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (self.Detail < IdentityDetail.Entity)
            return IIdentifiable<TId, TEntity, TModel>.Of(other);
        return self;
    }

    public static IIdentifiable<TId, TEntity, TActor, TModel> MostSpecific<TId, TEntity, TActor, TModel>(
        this IIdentifiable<TId, TEntity, TActor, TModel> self,
        TId other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
        where TActor : class, IActor<TId, TEntity>
    {
        if (self.Detail < IdentityDetail.Entity)
            return IIdentifiable<TId, TEntity, TActor, TModel>.Of(other);
        return self;
    }

    public static IIdentifiable<TId, TEntity, TModel> MostSpecific<TId, TEntity, TModel>(
        this IIdentifiable<TId, TEntity, TModel> self,
        TId other
    )
        where TId : IEquatable<TId>
        where TEntity : class, IEntity<TId>, IEntityOf<TModel>
        where TModel : class, IEntityModel<TId>
    {
        if (self.Detail < IdentityDetail.Entity)
            return IIdentifiable<TId, TEntity, TModel>.Of(other);
        return self;
    }
}

#endregion