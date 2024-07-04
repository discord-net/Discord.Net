using Discord.Models;

namespace Discord;

public interface IUpdatable<TModel> : IEntityOf<TModel>
    where TModel : IEntityModel
{
    ValueTask UpdateAsync(TModel model, CancellationToken token = default);
}
