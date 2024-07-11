namespace Discord.Models;

public interface IEntityModel<out T> : IEntityModel
{
    T Id { get; }
}

public interface IEntityModel : IEquatable<IEntityModel>
{ }
