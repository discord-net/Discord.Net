using Discord.Models;

namespace Discord;

#pragma warning disable CS9113 // Parameter is unread.
[AttributeUsage(AttributeTargets.Interface)]
internal sealed class FetchableAttribute(string route) : Attribute;

[AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
internal sealed class FetchableOfManyAttribute(string route) : Attribute;
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
