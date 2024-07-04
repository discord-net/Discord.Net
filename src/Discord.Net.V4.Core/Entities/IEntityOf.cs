using Discord.Models;

namespace Discord;

public interface IEntityOf<out TModel> : IEntity
    where TModel : IEntityModel
{
    TModel GetModel();
}
