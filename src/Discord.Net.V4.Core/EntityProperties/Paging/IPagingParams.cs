using System.Runtime.CompilerServices;
using Discord.Rest;

namespace Discord;

public interface IPagingParams<TRoute, in TApiModel> : IPagingParams
    where TRoute : IRouteOperation<TRoute>
    where TApiModel : class
{
    TRoute? GetNext(
        IPathable path,
        TRoute route, 
        TApiModel? previousApiResult
        );
}

public interface IPagingParams
{
    int? PageSize { get; }
    int? Total { get; }

    static abstract int MaxPageSize { get; }

    protected static int GetPageSize<T>(T? pagingParams)
        where T : class, IPagingParams
    {
        return pagingParams?.PageSize.HasValue ?? false
            ? Math.Max(0, Math.Min(pagingParams.PageSize.Value, T.MaxPageSize))
            : T.MaxPageSize;
    }
}

public interface IDirectionalPagingParams<TId> : IPagingParams
{
    Direction? Direction { get; }
    Optional<TId> From { get; }
}

public interface IBetweenPagingParams<TId> : IDirectionalPagingParams<TId>
    where TId : IEquatable<TId>
{
    Optional<TId> Before { get; }
    Optional<TId> After { get; }

    bool IsBetween => Before.IsSpecified && After.IsSpecified;

    Direction? IDirectionalPagingParams<TId>.Direction => IsBetween
        ? null
        : Before.Map(Discord.Direction.Before) ?? After.Map(Discord.Direction.After);

    Optional<TId> IDirectionalPagingParams<TId>.From => IsBetween ? default : Before | After;
}
