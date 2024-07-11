using Discord.Paging;

namespace Discord;

public interface IPagedActor<in TId, out TPaged, in TPageParams>
    where TId : IEquatable<TId>
    where TPaged : class, IEntity<TId>
    where TPageParams : IPagingParams
{
    IAsyncPaged<TPaged> PagedAsync(TPageParams? args = default, RequestOptions? options = null);
}
