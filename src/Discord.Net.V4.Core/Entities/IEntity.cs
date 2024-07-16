using Discord.Models;

namespace Discord;

public interface IEntity<out TId, out TModel> : IEntity<TId>, IEntityOf<TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>;

public interface IEntity<out T> : IEntity, IIdentifiable<T> where T : IEquatable<T>;

public interface IEntity : IClientProvider;
