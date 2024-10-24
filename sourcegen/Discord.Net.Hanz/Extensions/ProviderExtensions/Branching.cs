using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public readonly record struct Branch<TValue>(
    int SourceVersion,
    TValue Value
)
{
    public Branch<TNewValue> Mutate<TNewValue>(TNewValue newValue)
        => new(SourceVersion, newValue);

    public Branch<TNewValue> CreateNestedBranch<TNewValue>(
        TNewValue value
    ) => new(GetHashCode(), value);

    public override int GetHashCode()
        => HashCode.Of(SourceVersion).And(Value);
}

public static partial class ProviderExtensions
{
    private readonly record struct Conjunction<TSource, TBranch>(
        TSource Source,
        ImmutableArray<TBranch> Branches
    )
    {
        public override int GetHashCode()
            => HashCode.Of(Source).AndEach(Branches);

        public bool Equals(Conjunction<TSource, TBranch>? other)
        {
            if (!other.HasValue) return false;

            return EqualityComparer<TSource>.Default.Equals(Source, other.Value.Source)
                   && Branches.SequenceEqual(other.Value.Branches);
        }
    }

    private readonly struct BranchGroup<TBranch>
    {
        private readonly ImmutableArray<Branch<TBranch>> _branches;

        private readonly Dictionary<int, List<TBranch>> _entries;

        public BranchGroup(ImmutableArray<Branch<TBranch>> branches)
        {
            _branches = branches;
            _entries = [];

            foreach (var branch in _branches)
            {
                if (!_entries.TryGetValue(branch.SourceVersion, out var entries))
                    _entries[branch.SourceVersion] = entries = [];

                entries.Add(branch.Value);
            }
        }

        public bool TryGetEntries<TSource>(TSource source, out List<TBranch> entries)
            => TryGetEntries(EqualityComparer<TSource>.Default.GetHashCode(source), out entries);

        public bool TryGetEntries(int hash, out List<TBranch> entries)
            => _entries.TryGetValue(hash, out entries);

        public ImmutableArray<TBranch> GetEntriesOrEmpty<TSource>(TSource source)
            => GetEntriesOrEmpty(EqualityComparer<TSource>.Default.GetHashCode(source));

        public ImmutableArray<TBranch> GetEntriesOrEmpty(int hash)
            => _entries.TryGetValue(hash, out var entries)
                ? entries.ToImmutableArray()
                : ImmutableArray<TBranch>.Empty;

        public override int GetHashCode()
            => HashCode.OfEach(_branches);
    }

    public static IncrementalValuesProvider<Branch<T>> Branch<T>(
        this IncrementalValuesProvider<T> source
    ) => source.Select((x, _) => new Branch<T>(x?.GetHashCode() ?? 0, x));

    public static IncrementalValuesProvider<TResult> Merge<TSource, TBranch, TResult>(
        this IncrementalValuesProvider<Branch<TBranch>> branch,
        IncrementalValuesProvider<TSource> source,
        Func<TSource, TBranch?, CancellationToken, TResult> selector)
    {
        return source
            .Combine(branch
                .Collect()
                .Select((x, _) => new BranchGroup<TBranch>(x))
            )
            .SelectMany((tuple, token) =>
            {
                if (tuple.Right.TryGetEntries(tuple.Left, out var entries))
                {
                    return entries.Select(x => selector(tuple.Left, x, token));
                }

                return [selector(tuple.Left, default, token)];
            });
    }

    public static IncrementalValuesProvider<TResult> Collect<TSource, TBranch, TResult>(
        this IncrementalValuesProvider<Branch<TBranch>> branch,
        IncrementalValuesProvider<TSource> source,
        Func<TSource, ImmutableArray<TBranch>, CancellationToken, TResult> selector,
        Func<TSource, int>? getHash = null)
    {
        return source
            .Combine(branch
                .Collect()
                .Select((x, _) => new BranchGroup<TBranch>(x))
            )
            .Select((tuple, token) =>
                new Conjunction<TSource, TBranch>(
                    tuple.Left,
                    tuple.Right.GetEntriesOrEmpty(
                        getHash?.Invoke(tuple.Left) ?? EqualityComparer<TSource>.Default.GetHashCode(tuple.Left)
                    )
                )
            )
            .Select((conjunction, token) =>
                selector(conjunction.Source, conjunction.Branches, token)
            );
    }

    public static IncrementalValuesProvider<Branch<T>> WhereNonNull<T>(
        this IncrementalValuesProvider<Branch<T?>> source
    ) where T : class
    {
        return source.Where(x => x is not null)!;
    }

    public static IncrementalValuesProvider<Branch<T>> WhereNonNull<T>(
        this IncrementalValuesProvider<Branch<T?>> source
    ) where T : struct
    {
        return source.Where(x => x.HasValue).Select((x, _) => x!.Value);
    }

    // 1 => 1 transform 
    public static IncrementalValueProvider<Branch<TResult>> Select<TSource, TResult>(
        this IncrementalValueProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, TResult> selector
    ) => source.Select((branch, token) => branch.Mutate(selector(branch.Value, token)));

    public static IncrementalValuesProvider<Branch<TResult>> Select<TSource, TResult>(
        this IncrementalValuesProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, TResult> selector
    ) => source.Select((branch, token) => branch.Mutate(selector(branch.Value, token)));

    // 1 => many (or none) transform
    public static IncrementalValuesProvider<Branch<TResult>> SelectMany<TSource, TResult>(
        this IncrementalValueProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, ImmutableArray<TResult>> selector
    ) => source.SelectMany((branch, token) => selector(branch.Value, token).Select(branch.Mutate).ToImmutableArray());

    public static IncrementalValuesProvider<Branch<TResult>> SelectMany<TSource, TResult>(
        this IncrementalValueProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, IEnumerable<TResult>> selector
    ) => source.SelectMany((branch, token) => selector(branch.Value, token).Select(branch.Mutate));

    public static IncrementalValuesProvider<Branch<TResult>> SelectMany<TSource, TResult>(
        this IncrementalValuesProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, ImmutableArray<TResult>> selector
    ) => source.SelectMany((branch, token) => selector(branch.Value, token).Select(branch.Mutate).ToImmutableArray());

    public static IncrementalValuesProvider<Branch<TResult>> SelectMany<TSource, TResult>(
        this IncrementalValuesProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, IEnumerable<TResult>> selector
    ) => source.SelectMany((branch, token) => selector(branch.Value, token).Select(branch.Mutate));

    // helper for filtering
    public static IncrementalValuesProvider<Branch<TSource>> Where<TSource>(
        this IncrementalValuesProvider<Branch<TSource>> source,
        Func<TSource, bool> predicate
    ) => source.SelectMany(
        (item, _) =>
            predicate(item)
                ? ImmutableArray.Create(item)
                : ImmutableArray<TSource>.Empty
    );

    internal static IncrementalValuesProvider<Branch<TSource>> Where<TSource>(
        this IncrementalValuesProvider<Branch<TSource>> source,
        Func<TSource, CancellationToken, bool> predicate
    ) => source.SelectMany(
        (item, c) =>
            predicate(item, c)
                ? ImmutableArray.Create(item)
                : ImmutableArray<TSource>.Empty
    );
}