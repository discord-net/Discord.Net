global using SnowflakeEntity = Discord.Models.IEntity<Discord.Snowflake>;
using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IEntity<out TId> :
    IIdentifiable<TId>,
    IClientProvider
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

