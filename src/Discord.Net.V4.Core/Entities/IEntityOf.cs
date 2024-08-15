using Discord.Models;

namespace Discord;

public interface IEntityOf<out TModel> : IEntity
    where TModel : IModel
{
    TModel GetModel();
}
