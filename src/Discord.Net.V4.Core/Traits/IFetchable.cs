using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class FetchableAttribute(string route) : Attribute;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class FetchableOfManyAttribute(string route) : Attribute;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class PagedFetchableOfManyAttribute<TPageParams>(string route) : Attribute
    where TPageParams : IPagingParams;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class PagedFetchableOfManyAttribute<TPageParams, TPagedEntity>(string route) : Attribute
    where TPageParams : IPagingParams;
#pragma warning restore CS9113 // Parameter is unread.

public interface IFetchable<in TId, out TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal static abstract IApiOutRoute<TModel> FetchRoute(IPathable path, TId id);
}

public interface IFetchableOfMany<in TId, out TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal static abstract IApiOutRoute<IEnumerable<TModel>> FetchManyRoute(IPathable path);
}

public interface IPagedFetchableOfMany<in TId, out TModel, TParams, TApi>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
    where TParams : class, IPagingParams<TParams, TApi>
    where TApi : class;

