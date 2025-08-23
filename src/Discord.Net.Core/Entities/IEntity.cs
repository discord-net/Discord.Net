global using SnowflakeEntity = Discord.IEntity<Discord.Snowflake>;
using Discord.Models;

namespace Discord;

public interface IEntity : IClientProvider;

public interface IEntity<out TId> : 
    IEntity,
    IIdentifiable<TId>
    where TId : IEquatable<TId>;

public interface IEntity<out TId, out TModel> :
    IEntity<TId>,
    IModeledBy<TModel>
    where TId : IEquatable<TId>
    where TModel : IModel;

public interface IModeledBy<out TModel> where TModel : IModel
{
    TModel Model { get; }
}

