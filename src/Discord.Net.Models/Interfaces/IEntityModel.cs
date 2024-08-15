namespace Discord.Models;

public interface IEntityModel<out T> : IModel
{
    T Id { get; }
}

public interface IModel : IEquatable<IModel>
{
    bool IEquatable<IModel>.Equals(IModel? other) => Equals(this, other);
}
