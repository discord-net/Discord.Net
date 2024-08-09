namespace Discord.Models;

public static class ModelUtils
{
    public static TModel? GetReferencedEntityModel<TId, TModel>(this IEntityModel model, TId id)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        return model is IModelSource source
            ? source.GetReferencedEntityModel<TId, TModel>(id)
            : null;
    }

    public static TModel? GetReferencedEntityModel<TId, TModel>(this IModelSource model, TId id)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if (model is IModelSourceOf<TModel> singleSource && singleSource.Model.Id.Equals(id))
            return singleSource.Model;

        if (
            model is IModelSourceOfMultiple<TModel> multiSource &&
            multiSource.GetModels().FirstOrDefault(x => x.Id.Equals(id)) is {} targetModel)
            return targetModel;

        foreach (var entity in model.GetDefinedModels())
        {
            if (entity is TModel target && target.Id.Equals(id))
                return target;
        }

        return null;
    }
}
