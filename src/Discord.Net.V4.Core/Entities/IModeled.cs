using Discord.Models;

namespace Discord;

public interface IModeled<out TId, out TModel> : IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    TModel Model { get; }
}
