namespace Discord.Models;

public interface IEntityModel : IModel;

public interface IEntityModel<out TId> : IEntityModel
    where TId : IEquatable<TId>
{
    TId Id { get; }
}