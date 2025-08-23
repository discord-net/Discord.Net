using Discord.Models;

namespace Discord;

public interface ILink<TId, TEntity, TActor, TModel> :
    IClientProvider
    where TId : IEquatable<TId>
    where TEntity : IEntity<TId>, IModeledBy<TModel>
    where TActor : IActor<TId, TEntity>
    where TModel : IModel;