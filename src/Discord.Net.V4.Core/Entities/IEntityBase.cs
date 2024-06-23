using Discord.Models;

namespace Discord;

public interface IEntityBase<out TId, TModel> : IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
}
