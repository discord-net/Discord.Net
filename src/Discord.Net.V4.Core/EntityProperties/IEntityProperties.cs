namespace Discord;

public interface IEntityProperties<T>
{
    T ToApiModel(T? existing = default);
}
