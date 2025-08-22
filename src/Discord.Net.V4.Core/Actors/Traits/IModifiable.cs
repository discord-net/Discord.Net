namespace Discord.Models;

public interface IModifiable<in TProperties>
{
    Task ModifyAsync(
        TProperties properties,
        RequestOptions options = default
    );
}

public interface IModifiable<in TProperties, TEntity> :
    IModifiable<TProperties>
{
    new Task<TEntity> ModifyAsync(
        TProperties properties,
        RequestOptions options = default
    );

    Task IModifiable<TProperties>.ModifyAsync(
        TProperties properties,
        RequestOptions options
    ) => ModifyAsync(properties, options);
}