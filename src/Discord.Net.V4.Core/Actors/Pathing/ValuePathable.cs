using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Discord;

internal sealed class ValuePathable :
    IPathable
{
    private readonly Dictionary<Type, IPathEntry> _entries = [];

    public Optional<TId> Get<TId, TEntity>() where TId : IEquatable<TId> where TEntity : IIdentifiable<TId>
    {
        if (
            !_entries.TryGetValue(typeof(TEntity), out var entry) ||
            entry is not IPathEntry<TId> {Type: PathEntryType.Id} pathEntry
        ) return default;

        return pathEntry.Value.Some();
    }

    IEnumerator<IPathEntry> IEnumerable<IPathEntry>.GetEnumerator() => _entries.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _entries.Values.GetEnumerator();
}