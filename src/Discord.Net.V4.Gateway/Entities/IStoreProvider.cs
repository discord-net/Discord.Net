using Discord.Gateway;
using Discord.Gateway.State;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IStoreInfoProvider<TId, TModel> : IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal static abstract ValueTask<IStoreInfo<TId, TModel>> GetStoreInfoAsync(
        DiscordGatewayClient client,
        IPathable? path = null,
        CancellationToken token = default
    );
}

[NoExposure]
public interface IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal static abstract ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(
        DiscordGatewayClient client,
        IPathable? path = null,
        CancellationToken token = default
    );
}