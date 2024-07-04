namespace Discord.Rest;

public static class EntityUtils
{
    public static Func<DiscordRestClient, TId, TModel?, TEntity?> FactoryOfDescendantModel<TId, TModel, TEntity, TDescendingModel>(
        Func<TId, TDescendingModel, TEntity> factory
    )
        where TDescendingModel : TModel
        where TEntity : class
    {
        return (id, model) =>
        {
            if (model is null)
                return null;

            if (model is not TDescendingModel desc)
                throw new DiscordException(
                    $"Entity model was not of correct type, expecting '{typeof(TDescendingModel)}' but got '{typeof(TModel)}'");

            return factory(id, desc);
        };
    }
}
