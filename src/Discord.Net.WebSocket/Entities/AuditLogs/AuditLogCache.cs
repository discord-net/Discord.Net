using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Discord.WebSocket;

internal class AuditLogCache
{
    private readonly ConcurrentDictionary<ulong, SocketAuditLogEntry> _entries;
    private readonly ConcurrentQueue<ulong> _orderedEntries;

    private readonly int _size;

    public IReadOnlyCollection<SocketAuditLogEntry> AuditLogs => _entries.ToReadOnlyCollection();

    public AuditLogCache(DiscordSocketClient client)
    {
        _size = client.AuditLogCacheSize;
        var dictSize = _size;
        if (dictSize * 1.05 > int.MaxValue)
            dictSize = int.MaxValue;
        else
            dictSize = (int)(dictSize * 1.05);

        _entries = new ConcurrentDictionary<ulong, SocketAuditLogEntry>(ConcurrentHashSet.DefaultConcurrencyLevel, dictSize);
        _orderedEntries = new ConcurrentQueue<ulong>();
    }

    public void Add(SocketAuditLogEntry entry)
    {
        if (_entries.TryAdd(entry.Id, entry))
        {
            _orderedEntries.Enqueue(entry.Id);

            while (_orderedEntries.Count > _size && _orderedEntries.TryDequeue(out var entryId))
                _entries.TryRemove(entryId, out _);
        }
    }

    public SocketAuditLogEntry Remove(ulong id)
    {
        _entries.TryRemove(id, out var entry);
        return entry;
    }

    public SocketAuditLogEntry Get(ulong id)
        => _entries.TryGetValue(id, out var result) ? result : null;

    /// <exception cref="ArgumentOutOfRangeException"><paramref name="limit"/> is less than 0.</exception>
    public IReadOnlyCollection<SocketAuditLogEntry> GetMany(ulong? fromEntryId, Direction dir, int limit = DiscordConfig.MaxAuditLogEntriesPerBatch, ActionType ? action = null)
    {
        if (limit < 0)
            throw new ArgumentOutOfRangeException(nameof(limit));
        if (limit == 0)
            return ImmutableArray<SocketAuditLogEntry>.Empty;

        IEnumerable<ulong> cachedEntriesIds;
        if (fromEntryId == null)
            cachedEntriesIds = _orderedEntries;
        else if (dir == Direction.Before)
            cachedEntriesIds = _orderedEntries.Where(x => x < fromEntryId.Value);
        else if (dir == Direction.After)
            cachedEntriesIds = _orderedEntries.Where(x => x > fromEntryId.Value);
        else //Direction.Around
        {
            if (!_entries.TryGetValue(fromEntryId.Value, out var entry))
                return ImmutableArray<SocketAuditLogEntry>.Empty;
            var around = limit / 2;
            var before = GetMany(fromEntryId, Direction.Before, around, action);
            var after = GetMany(fromEntryId, Direction.After, around, action).Reverse();

            return after.Concat(new [] { entry }).Concat(before).ToImmutableArray();
        }

        if (dir == Direction.Before)
            cachedEntriesIds = cachedEntriesIds.Reverse();
        if (dir == Direction.Around)
            limit = limit / 2 + 1;

        return cachedEntriesIds
            .Select(x => _entries.TryGetValue(x, out var entry) ? entry : null)
            .Where(x => x != null && (action is null || x.Action == action))
            .Take(limit)
            .ToImmutableArray();
    }
}
