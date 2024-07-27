using Discord.Gateway.Cache;

namespace Discord.Gateway;

internal static class EntityModelStoreExtensions
{
    public static async ValueTask<IEntityModelStore<TNextId, TNextModel>> Chain<TId, TModel, TNextId, TNextModel>(
        this ValueTask<IEntityModelStore<TId, TModel>> task,
        TId id
    )
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
        where TNextModel : class, IEntityModel<TNextId>
        where TNextId : IEquatable<TNextId>
    {
        var store = await task;
        return await store.GetStoreAsync<TNextId, TNextModel>(id);
    }
}
