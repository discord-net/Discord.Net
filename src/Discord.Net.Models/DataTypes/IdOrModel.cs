namespace Discord.Models;

public readonly record struct IdOrModel<TId, TModel>(TId Id)
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    public Optional<TModel> Model { get; }

    public IdOrModel(TModel model) : this(model.Id)
    {
        Model = model;
    }

    public static implicit operator TId(IdOrModel<TId, TModel> self) => self.Id;
    public static implicit operator IdOrModel<TId, TModel>(TId id) => new(id);
    public static implicit operator IdOrModel<TId, TModel>(TModel model) => new(model);
}