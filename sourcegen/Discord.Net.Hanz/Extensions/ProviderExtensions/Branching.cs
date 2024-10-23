using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz;

public readonly record struct Branch<TValue, TSource>(
    int SourceVersion,
    TValue Value
)
{
    public Branch<TNewValue, TSource> Mutate<TNewValue>(TNewValue newValue)
        => new(SourceVersion, newValue);
}

public static partial class ProviderExtensions
{
    private readonly struct BranchGroup<TBranch, TSource>
    {
        private readonly ImmutableArray<Branch<TBranch, TSource>> _branches;

        private readonly Dictionary<int, List<TBranch>> _entries;
        
        public BranchGroup(ImmutableArray<Branch<TBranch, TSource>> branches)
        {
            _branches = branches;
            _entries = [];

            foreach (var branch in _branches)
            {
                if(!_entries.TryGetValue(branch.SourceVersion, out var entries))
                    _entries[branch.SourceVersion] = entries = [];
                
                entries.Add(branch.Value);
            }
        }

        public bool TryGetEntries(TSource source, out List<TBranch> entries)
            => _entries.TryGetValue(source?.GetHashCode() ?? 0, out entries);

        public override int GetHashCode()
            => HashCode.OfEach(_branches);
    }
    
    public static IncrementalValuesProvider<Branch<T, T>> Branch<T>(
        this IncrementalValuesProvider<T> source
    ) => source.Select((x, _) => new Branch<T, T>(x?.GetHashCode() ?? 0, x));

    public static IncrementalValuesProvider<TResult> Merge<TSource, TBranch, TResult>(
        this IncrementalValuesProvider<Branch<TBranch, TSource>> branch,
        IncrementalValuesProvider<TSource> source,
        Func<TSource, TBranch?, CancellationToken, TResult> selector)
    {
        return source
            .Combine(branch
                .Collect()
                .Select((x, _) => new BranchGroup<TBranch, TSource>(x))
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
}