namespace Discord;

public interface IDeletable : IEntity
{
    Task DeleteAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(Route, options ?? Client.DefaultRequestOptions, token);

    internal BasicApiRoute Route { get; }
}
