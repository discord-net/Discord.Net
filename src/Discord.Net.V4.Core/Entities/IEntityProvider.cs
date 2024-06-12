using Discord.Models;
using System.Diagnostics.CodeAnalysis;

namespace Discord;

public interface IEntityProvider<out TEntity, in TModel> : IClientProvider
    where TEntity : IEntity
    where TModel : IEntityModel?
{
    TEntity CreateEntity(TModel model);

    [return: NotNullIfNotNull(nameof(model))]
    TEntity? CreateNullableEntity(TModel? model) => model is null ? default : CreateEntity(model);
}
