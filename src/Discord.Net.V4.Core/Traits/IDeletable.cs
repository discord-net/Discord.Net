namespace Discord;

public interface IDeletable : IEntity
{
    Task DeleteAsync(RequestOptions? options = null, CancellationToken token = default)
        => Client.RestApiClient.ExecuteAsync(DeleteRoute, options ?? Client.DefaultRequestOptions, token);

    internal BasicApiRoute DeleteRoute { get; }
}
