namespace Discord.Models;

public interface IEntityModel<T>
{
    T Id { get; }
}
