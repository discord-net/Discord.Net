using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

public interface IEntityProvider<out TEntity, in TModel> : IClientProvider
    where TEntity : IEntity
    where TModel : IModel?
{
    internal TEntity CreateEntity(TModel model);
}

public interface IEntityProvider<out TEntity, in TModel, in TContext> : IClientProvider
    where TEntity : IEntity
    where TModel : IModel?
{
    internal TEntity CreateEntity(TModel model, TContext context);
}
