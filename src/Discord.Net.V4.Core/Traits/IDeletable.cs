namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class DeletableAttribute(string route) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

public interface IDeletable<TId, out TSelf> : IEntity<TId>, IPathable
    where TSelf : IDeletable<TId, TSelf>, IEntity<TId>, IPathable
    where TId : IEquatable<TId>
{
    Task DeleteAsync(RequestOptions? options = null, CancellationToken token = default)
        => DeleteAsync(Client, TSelf.DeleteRoute(this, Id), options, token);

    internal static Task DeleteAsync(
        IDiscordClient client,
        IApiRoute route,
        RequestOptions? options = null,
        CancellationToken token = default
    ) => client.RestApiClient.ExecuteAsync(route, options ?? client.DefaultRequestOptions, token);

    internal static abstract IApiRoute DeleteRoute(IPathable path, TId id);
}
