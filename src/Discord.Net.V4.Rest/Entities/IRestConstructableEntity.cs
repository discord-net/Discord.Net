namespace Discord.Rest;

public interface IRestConstructable<out TEntity, in TActor, in TModel> :
    IConstructable<TEntity, TActor, TModel, DiscordRestClient>
    where TEntity : IRestConstructable<TEntity, TActor, TModel>;