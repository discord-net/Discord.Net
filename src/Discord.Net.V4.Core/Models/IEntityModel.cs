namespace Discord.Models;

public interface IEntityModel<T> : IEntityModel
{
    T Id { get; }
}

public interface IEntityModel
{ }
