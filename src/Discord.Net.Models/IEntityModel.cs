namespace Discord.Models;

public interface IEntityModel<out TId> : IModel
    where TId : IEquatable<TId>
{
    TId Id { get; }
}