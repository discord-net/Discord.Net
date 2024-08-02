using Discord.Gateway;
using Discord.Models;

namespace Discord.Gateway;

internal static class EntityModelStoreExtensions
{
    public static IEntityModelStore<TId, TNewModel> CastUp<TId, TModel, TNewModel>(
        this IEntityModelStore<TId, TModel> store,
        Template<TNewModel> template
    )
        where TModel : class, TNewModel, IEntityModel<TId>
        where TNewModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if(typeof(TNewModel) == typeof(TModel))
            return (store as IEntityModelStore<TId, TNewModel>)!;

        return new CastUpModelStore<TId, TModel, TNewModel>(store);
    }

    public static IEntityModelStore<TId, TNewModel> CastDown<TId, TModel, TNewModel>(
        this IEntityModelStore<TId, TModel> store,
        Template<TNewModel> template
    )
        where TModel : class, IEntityModel<TId>
        where TNewModel : class, TModel
        where TId : IEquatable<TId>
    {
        if(typeof(TNewModel) == typeof(TModel))
            return (store as IEntityModelStore<TId, TNewModel>)!;

        return new CastDownModelStore<TId, TModel, TNewModel>(store);
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
