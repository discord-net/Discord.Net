namespace Discord;

/// <summary>
///     Represents a barebones lazy-loadable entity.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public interface ILoadableEntity<T>
    where T : class
{
    /// <summary>
    ///     Gets the entity if present; otherwise <see langword="null"/>.
    /// </summary>
    T? Value { get; }

    /// <summary>
    ///     Gets or loads the entity from an asynchronous source. This method
    ///     can fetch from the Discord API or a cache - depending on
    ///     configuration and implementation.
    /// </summary>
    /// <param name="options">Request options to use when making an API request.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask"/> representing the asynchronous operation of loading
    ///     the entity. The result of the <see cref="ValueTask"/> is the entity OR <see langword="null"/> if
    ///     the entity could not be loaded.
    /// </returns>
    ValueTask<T?> GetOrLoadAsync(RequestOptions? options = null, CancellationToken token = default)
        => Value is not null
            ? ValueTask.FromResult<T?>(Value)
            : LoadAsync(options, token);

    /// <summary>
    ///     Loads the entity from an asynchronous source. This method
    ///     can fetch from the Discord API or a cache - depending on
    ///     configuration and implementation.
    /// </summary>
    /// <param name="options">Request options to use when making an API request.</param>
    /// <param name="token">A cancellation token used to cancel the asynchronous operation.</param>
    /// <returns>
    ///     A <see cref="ValueTask"/> representing the asynchronous operation of loading
    ///     the entity. The result of the <see cref="ValueTask"/> is the entity OR <see langword="null"/> if
    ///     the entity could not be loaded.
    /// </returns>
    ValueTask<T?> LoadAsync(RequestOptions? options = null, CancellationToken token = default);
}
