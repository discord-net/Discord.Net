namespace Discord.Models;

public static class ModelUtils
{
    public static TEntity? GetReferencedEntityModel<TId, TEntity>(this IEntityModel model, TId id)
        where TEntity : class, IEntityModel<TId>
        where TId : IEquatable<TId>
    {
        if (model is not IEntityModelSource source)
            return null;

        foreach (var entity in source.GetEntities())
        {
            if (entity is TEntity target && target.Id.Equals(id))
                return target;
        }

        return null;
    }
}
