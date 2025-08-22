using Discord.Models;
using Discord.Models.Models;

namespace Discord;

public interface IPagedLink<in TParams, out TEntity> :
    IAsyncEnumerable<TEntity>
    where TParams : IParametersModel
{
    // TODO: page params
}