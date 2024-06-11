using Discord.Models;

namespace Discord;

public interface IEntityProvider<out TEntity, in TModel> : IClientProvider
    where TEntity : IEntity
    where TModel : IEntityModel?
{
    TEntity Create(TModel model);
}
