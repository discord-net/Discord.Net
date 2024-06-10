using Discord.Rest;

namespace Discord;

public interface IModifiable<out TParams, TApi> : IEntity
    where TParams : IEntityProperties<TApi>, new()
    where TApi : class
{
    delegate ApiBodyRoute<TApi> RouteFactory(TApi api);

    Task ModifyAsync(Action<TParams> func, RequestOptions? options = null, CancellationToken token = default)
    {
        var args = new TParams();
        func(args);
        return Client.RestApiClient.ExecuteAsync(ModifyRoute(args.ToApiModel()), options ?? Client.DefaultRequestOptions, token);
    }

    internal RouteFactory ModifyRoute { get; }
}
