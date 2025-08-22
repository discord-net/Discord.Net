using Discord.Models.Models;

namespace Discord.Models;

public interface IPagedLink<in TParams, out TEntity> :
    IAsyncEnumerable<TEntity>
    where TParams : IParametersModel
{
    // TODO: page params
}