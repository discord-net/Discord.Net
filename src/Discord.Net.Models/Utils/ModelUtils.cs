namespace Discord.Models;

public static class ModelUtils
{
    public static TModel? GetReferencedEntityModel<TId, TModel>(this IEntityModel model, TId id)
        where TModel : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if (model is not IEntityModelSource source)
            return null;

        foreach (var entity in source.GetEntities())
        {
            if (entity is TModel target && target.Id.Equals(id))
                return target;
        }

        return null;
    }
}
