using Discord.Models;

namespace Discord;

public interface IEntityProvider<TEntity, TAbstraction, TModel> : IClientProvider
    where TEntity : class, TAbstraction, IConstructable<TEntity, TModel>
    where TModel : IEntityModel?
{
}
