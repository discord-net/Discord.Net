using Discord.Models;

namespace Discord;

public interface IUpdatable<TModel>
    where TModel : IEntityModel
{
    ValueTask UpdateAsync(TModel model, CancellationToken token = default);
}
