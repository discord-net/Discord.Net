using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class FetchableAttribute(string route) : Attribute;

[AttributeUsage(AttributeTargets.Interface)]
internal sealed class FetchableOfManyAttribute(string route) : Attribute;
#pragma warning restore CS9113 // Parameter is unread.

public interface IFetchable<TId, out TModel> : IEntity<TId>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal static abstract IApiOutRoute<TModel> FetchRoute(IPathable path, TId id);
}

public interface IFetchableOfMany<TId, out TModel> : IEntity<TId>, IEntityOf<TModel>
    where TId : IEquatable<TId>
    where TModel : IEntityModel<TId>
{
    internal static abstract IApiOutRoute<IEnumerable<TModel>> FetchRoute(IPathable path, TId id);
}
