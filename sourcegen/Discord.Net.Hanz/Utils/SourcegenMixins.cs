using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Discord.Net.Hanz.Utils;

public readonly struct MixinSourceProductionContext(
    Dictionary<string, SourceText> sources
) : IEquatable<MixinSourceProductionContext>
{
    private readonly Dictionary<string, SourceText> _sources = sources;

    public void AddSource(string hintName, string source)
        => AddSource(hintName, SourceText.From(source, Encoding.UTF8));

    public void AddSource(string hintName, SourceText sourceText)
    {
        if (_sources.ContainsKey(hintName)) return;

        _sources[hintName] = sourceText;
    }

    public bool Equals(MixinSourceProductionContext other)
    {
        if (other._sources.Count != _sources.Count)
            return false;

        foreach (var entry in _sources)
        {
            if (!other._sources.TryGetValue(entry.Key, out var otherValue))
                return false;

            if (!entry.Value.ContentEquals(otherValue))
                return false;
        }

        return true;
    }
}

public static class SourcegenMixins
{
    public static IncrementalValueProvider<(ImmutableArray<T> Left, Compilation Compilation)> MixinSource<T>(
        this IncrementalValueProvider<ImmutableArray<T>> provider,
        IncrementalGeneratorInitializationContext context,
        Action<MixinSourceProductionContext, ImmutableArray<T>> generate,
        IEqualityComparer<T>? comparer = null
    ) => MixinSource(
        provider,
        context,
        generate,
        comparer is null ? ImmutableArrayComparer<T>.Default : new(comparer)
    );

    public static IncrementalValueProvider<(T Left, Compilation Compilation)> MixinSource<T>(
        this IncrementalValueProvider<T> provider,
        IncrementalGeneratorInitializationContext context,
        Action<MixinSourceProductionContext, T> generate,
        IEqualityComparer<T>? comparer = null)
    {
        var sourceProvider = provider.Select((state, _) =>
        {
            var sources = new Dictionary<string, SourceText>();
            var mixinContext = new MixinSourceProductionContext(sources);

            generate(mixinContext, state);

            return (InputState: state, Sources: sources);
        });

        context.RegisterSourceOutput(sourceProvider, (context, state) =>
        {
            foreach (var source in state.Sources)
            {
                context.AddSource(source.Key, source.Value);
            }
        });

        return sourceProvider
            .Combine(context.CompilationProvider)
            .WithComparer(comparer is null ? MixinComparer<T>.Default : new(comparer))
            .Select((x, token) =>
            {
                return (
                    x.Left.InputState,
                    x.Item2.AddSyntaxTrees(
                        x.Left.Item2.Values.Select(text =>
                            SyntaxFactory.ParseSyntaxTree(
                                text,
                                x.Item2.SyntaxTrees.First().Options,
                                cancellationToken: token
                            )
                        )
                    )
                );
            });
    }

    private sealed class MixinComparer<T> : IEqualityComparer<((T InputState, Dictionary<string, SourceText>) Left,
        Compilation Compilation)>
    {
        private readonly IEqualityComparer<T> _inner;
        public static readonly MixinComparer<T> Default = new(EqualityComparer<T>.Default);

        public MixinComparer(IEqualityComparer<T> inner)
        {
            _inner = inner;
        }

        public bool Equals(
            ((T InputState, Dictionary<string, SourceText>) Left, Compilation Compilation) x,
            ((T InputState, Dictionary<string, SourceText>) Left, Compilation Compilation) y
        ) => _inner.Equals(x.Left.InputState, y.Left.InputState);

        public int GetHashCode(((T InputState, Dictionary<string, SourceText>) Left, Compilation Compilation) obj)
            => _inner.GetHashCode(obj.Left.InputState);
    }

    private sealed class ImmutableArrayComparer<T> : IEqualityComparer<ImmutableArray<T>>
    {
        private readonly IEqualityComparer<T> _inner;
        public static readonly ImmutableArrayComparer<T> Default = new(EqualityComparer<T>.Default);

        public ImmutableArrayComparer(IEqualityComparer<T> inner)
        {
            _inner = inner;
        }

        public bool Equals(ImmutableArray<T> x, ImmutableArray<T> y)
            => x.GetHashCode() == y.GetHashCode();

        public int GetHashCode(ImmutableArray<T> obj)
        {
            unchecked
            {
                var hashCode = obj.IsEmpty.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Length;
                hashCode = (hashCode * 397) ^ obj.IsDefault.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsDefaultOrEmpty.GetHashCode();

                return Enumerable.Aggregate(
                    obj,
                    hashCode,
                    (current, entry) => (current * 397) ^ _inner.GetHashCode(entry)
                );
            }

            // unchecked
            // {
            //     var hashCode = obj.IsEmpty.GetHashCode();
            //     hashCode = (hashCode * 397) ^ obj.Length;
            //     hashCode = (hashCode * 397) ^ obj.IsDefault.GetHashCode();
            //     hashCode = (hashCode * 397) ^ obj.IsDefaultOrEmpty.GetHashCode();
            //     return hashCode;
            // }
        }
    }
}