using Discord.Models;

namespace Discord;

public interface IUpdatable<in TModel>
    where TModel : IModel
{
    ValueTask UpdateAsync(TModel model, CancellationToken token = default);
}
