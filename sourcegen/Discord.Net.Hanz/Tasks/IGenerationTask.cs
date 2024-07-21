using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks;

public interface IGenerationTask<T>
    where T : class, IEquatable<T>
{
    bool IsValid(SyntaxNode node, CancellationToken token = default);

    T? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger, CancellationToken token = default);

    void Execute(SourceProductionContext context, T? target, Logger logger);
}

public interface IGenerationCombineTask<T> where T : class, IEquatable<T>
{
    bool IsValid(SyntaxNode node, CancellationToken token = default);

    T? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger, CancellationToken token = default);

    void Execute(SourceProductionContext context, ImmutableArray<T?> targets, Logger logger);
}
