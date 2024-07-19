using Discord.Models;

namespace Discord;

file sealed class CastedIdentifiableEntityOrModel<TId, TSourceEntity, TSourceModel, TDestinationModel, TDestinationEntity>(
    IIdentifiableEntityOrModel<TId, TSourceEntity, TSourceModel> source
):
    IIdentifiableEntityOrModel<TId, TDestinationEntity, TDestinationModel>
    where TId : IEquatable<TId>
    where TDestinationEntity : class, IEntity<TId>, IEntityOf<TDestinationModel>
    where TSourceEntity : class, IEntity<TId>, IEntityOf<TSourceModel>
    where TSourceModel : class, IEntityModel<TId>
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
}

public sealed class IdentifiableEntityOrModel<TId, TEntity, TModel> :
    IIdentifiableEntityOrModel<TId, TEntity, TModel>
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

    private readonly Func<TModel, TEntity>? _entityFactory;
    private TEntity? _entity;
    private readonly TModel? _model;

    public IdentifiableEntityOrModel(TId id)
    {
        Id = id;
    }

    public IdentifiableEntityOrModel(TModel model, Func<TModel, TEntity> factory)
    {
        _model = model;
        _entityFactory = factory;
        Id = model.Id;
    }

    public IdentifiableEntityOrModel(TEntity entity)
    {
        Id = entity.Id;
        _entity = entity;
    }

    public static implicit operator IdentifiableEntityOrModel<TId, TEntity, TModel>(TId id) => new(id);
    public static implicit operator IdentifiableEntityOrModel<TId, TEntity, TModel>(TEntity entity) => new(entity);
}

public interface IIdentifiableEntityOrModel<TId, out TEntity, out TModel> : IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>, IEntityOf<TModel>
    where TModel : class, IEntityModel<TId>
{
    TEntity? Entity { get; }

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> FromReferenced<TConstruct, TClient>(
        IEntityModel model,
        TId id,
        TClient client)
        where TConstruct : class, TEntity, IConstructable<TConstruct, TModel, TClient>
        where TClient : IDiscordClient
        => FromReferenced(model, id, model => TConstruct.Construct(client, model));

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> FromReferenced<TConstruct>(
        IEntityModel model,
        TId id,
        IDiscordClient client)
        where TConstruct : class, TEntity, IConstructable<TConstruct, TModel>
        => FromReferenced(model, id, model => TConstruct.Construct(client, model));

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> FromReferenced(
        IEntityModel model,
        TId id,
        Func<TModel, TEntity> factory)
    {
        var referenced = model.GetReferencedEntityModel<TId, TModel>(id);

        return referenced is null
            ? Of(id)
            : Of(referenced, factory);
    }

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> Of(TId id)
        => new IdentifiableEntityOrModel<TId, TEntity, TModel>(id);

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> Of(TEntity entity)
        => new IdentifiableEntityOrModel<TId, TEntity, TModel>(entity);

    static IIdentifiableEntityOrModel<TId, TEntity, TModel> Of(TModel model, Func<TModel, TEntity> factory)
        => new IdentifiableEntityOrModel<TId, TEntity, TModel>(model, factory);

    static IIdentifiableEntityOrModel<TId, TEntity, TModel>? OfNullable(TModel? model, Func<TModel, TEntity> factory)
        => model is not null ? Of(model, factory) : null;

    IIdentifiableEntityOrModel<TId, TNewEntity, TNewModel> Cast<TNewEntity, TNewModel>()
        where TNewEntity : class, IEntity<TId>, IEntityOf<TNewModel>
        where TNewModel : class, IEntityModel<TId>
        => new CastedIdentifiableEntityOrModel<TId, TEntity, TModel, TNewModel, TNewEntity>(this);
}
