using Discord.Models;

namespace Discord;

public sealed class IdentifiableEntityOrModel<TId, TEntity, TModel> : IIdentifiableEntityOrModel<TId, TEntity, TModel>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>
    where TModel : IEntityModel<TId>
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

public interface IIdentifiableEntityOrModel<out TId, out TEntity, out TModel> : IIdentifiable<TId>
    where TId : IEquatable<TId>
    where TEntity : class, IEntity<TId>
    where TModel : IEntityModel<TId>
{
    TEntity? Entity { get; }
}
