using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway;

internal static class EntityModelStoreExtensions
{
    public static IEntityModelStore<TId, TNewModel> Cast<TId, TModel, TNewModel>(
        this IEntityModelStore<TId, TModel> store,
        Template<TNewModel> template
    )
        where TModel : class, IEntityModel<TId>
        where TNewModel : class, TModel
        where TId : IEquatable<TId>
    {
        return new CastingModelStore<TId, TModel, TNewModel>(store);
    }

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
        return await store.GetSubStoreAsync<TNextId, TNextModel>(id);
    }
}
