namespace Discord.Models;

public interface IEntityModel<out TId> : IModel
    where TId : IEquatable<TId>
{
    TId Id { get; }
}

public interface IModel : IEquatable<IModel>
{
    bool IEquatable<IModel>.Equals(IModel? other) => Equals(this, other);
}
