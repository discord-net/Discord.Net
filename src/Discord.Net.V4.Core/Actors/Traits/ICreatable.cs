namespace Discord.Models;

public interface ICreatable<in TProperties, TResult> :
    ICreatable<TProperties>
{
    new Task<TResult> CreateAsync(
        TProperties properties,
        RequestOptions options = default
    );

    Task ICreatable<TProperties>.CreateAsync(TProperties properties, RequestOptions options)
        => CreateAsync(properties, options);
}

public interface ICreatable<in TProperties>
{
    Task CreateAsync(
        TProperties properties,
        RequestOptions options = default
    );
}