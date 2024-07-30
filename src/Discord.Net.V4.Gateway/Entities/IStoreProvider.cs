using Discord.Gateway.Cache;
using Discord.Models;

namespace Discord.Gateway;

[NoExposure]
public interface IStoreProvider<TId, TModel>
    where TModel : class, IEntityModel<TId>
    where TId : IEquatable<TId>
{
    internal ValueTask<IEntityModelStore<TId, TModel>> GetStoreAsync(CancellationToken token = default);
}
