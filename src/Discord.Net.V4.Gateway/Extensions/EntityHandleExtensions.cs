using JetBrains.Annotations;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord.Gateway;

internal static class EntityHandleExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [return: NotNullIfNotNull(nameof(handle))]
    public static TEntity? ConsumeAsReference<TId, TEntity>(this IEntityHandle<TId, TEntity>? handle)
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        if (handle is null)
            return null;

        var entity = handle.Entity;
        handle.Dispose();
        return entity;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEnumerable<TEntity> ConsumeAsReference<TId, TEntity>(
        this IEnumerable<IEntityHandle<TId, TEntity>> handles
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        => handles.Select(ConsumeAsReference)!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncEnumerable<TEntity> ConsumeAsReferenceAsync<TId, TEntity>(
        this IAsyncEnumerable<IEntityHandle<TId, TEntity>> handles
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        => handles.Select(ConsumeAsReference)!;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IAsyncEnumerable<IReadOnlyCollection<TEntity>> ConsumeAsReferenceAsync<TId, TEntity>(
        this IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> handles
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
        => handles.Select(x =>
            x.Select(ConsumeAsReference).ToImmutableList()
        )!;

    public static async ValueTask<IReadOnlyCollection<TEntity>> ConsumeAsReferenceAndFlattenAsync<TId, TEntity>(
        this IAsyncEnumerable<IReadOnlyCollection<IEntityHandle<TId, TEntity>>> handles
    )
        where TEntity : class, IEntity<TId>
        where TId : IEquatable<TId>
    {
        var result = new List<TEntity>();

        await foreach (var batch in handles)
            result.AddRange(batch.ConsumeAsReference());

        return result.AsReadOnly();
    }
}
