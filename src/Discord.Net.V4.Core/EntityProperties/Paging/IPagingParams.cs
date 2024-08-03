using System.Runtime.CompilerServices;

namespace Discord;

public interface IPagingParams
{
    int? PageSize { get; }
    int? Total { get; }

    static abstract int MaxPageSize { get; }
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
