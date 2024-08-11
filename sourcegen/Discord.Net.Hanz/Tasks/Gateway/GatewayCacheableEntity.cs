using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks.Gateway;

public sealed class GatewayCacheableEntity : IGenerationCombineTask<GatewayCacheableEntity.GenerationTarget>
{
    public sealed class GenerationTarget(
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        ITypeSymbol idType,
        ITypeSymbol modelType,
        ITypeSymbol actorType
    ) : IEquatable<GenerationTarget>
    {
        public ClassDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public ITypeSymbol IdType { get; } = idType;
        public ITypeSymbol ModelType { get; } = modelType;
        public ITypeSymbol ActorType { get; } = actorType;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                Syntax.IsEquivalentTo(other.Syntax) &&
                Symbol.Equals(other.Symbol, SymbolEqualityComparer.Default) &&
                IdType.Equals(other.IdType, SymbolEqualityComparer.Default) &&
                ActorType.Equals(other.ActorType, SymbolEqualityComparer.Default) &&
                ModelType.Equals(other.ModelType, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Syntax.GetHashCode();
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(Symbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ActorType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ModelType);
                return hashCode;
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax;

    public static bool TryGetCacheableType(ITypeSymbol type, out INamedTypeSymbol? cacheable)
    {
        cacheable = type.Interfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<"));

        if (cacheable is null &&
            (type.BaseType?.ToDisplayString().StartsWith("Discord.Gateway.GatewayCacheableEntity<") ?? false))
        {
            cacheable = type.BaseType.Interfaces
                .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<"));
        }

        return cacheable is not null;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not ClassDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax, token) is not INamedTypeSymbol symbol)
            return null;

        if (symbol.IsAbstract) return null;

        if (!TryGetCacheableType(symbol, out var cacheable))
            return null;

        var actorType = context.SemanticModel.Compilation.GetTypeByMetadataName($"{symbol.ToDisplayString()}Actor");

        if (actorType is null)
            return null;

        return new GenerationTarget(
            syntax,
            symbol,
            cacheable!.TypeArguments[1],
            cacheable.TypeArguments[2],
            actorType
        );
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var generated = new Dictionary<string, (ClassDeclarationSyntax Syntax, GenerationTarget Target)>();

        foreach (var target in targets)
        {
            if (target is null)
                continue;

            if (generated.TryGetValue(target.Symbol.Name, out _))
            {
                logger.Warn($"{target.Symbol}: Already generated sources");
                continue;
            }

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var contextConstructSyntax = SyntaxFactory.ParseTypeName(
                $"IContextConstructable<{target.Symbol}, {target.ModelType}, IGatewayConstructionContext, DiscordGatewayClient>"
            );

            syntax = syntax.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName($"IStoreProvider<{target.IdType}, {target.ModelType}>")),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName($"IStoreInfoProvider<{target.IdType}, {target.ModelType}>")),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        $"IBrokerProvider<{target.IdType}, {target.Symbol}, {target.ModelType}>")),
                SyntaxFactory.SimpleBaseType(contextConstructSyntax)
            );

            var hasStoreRoot = GatewayLoadable.TryGetStoreRootMap(target.ActorType, out var storeRootMap);

            var storeToInfo = hasStoreRoot
                ? $".ToInfo(Client, await ({storeRootMap.Last().Property.Name} as IStoreProvider<{storeRootMap.Last().IdType}, {storeRootMap.Last().ModelType}>).GetStoreAsync(token), {storeRootMap.Last().Property.Name}.Id, Template.Of<{storeRootMap.Last().Actor}>());"
                : $".ToInfo(Client, Template.Of<{target.ActorType}>());";

            syntax = syntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                    static ValueTask<IStoreInfo<{{target.IdType}}, {{target.ModelType}}>> IStoreInfoProvider<{{target.IdType}}, {{target.ModelType}}>.GetStoreInfoAsync(DiscordGatewayClient client, IPathable path, CancellationToken token)
                        => {{target.ActorType}}.GetStoreInfoAsync(client, path, token);
                    """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      ValueTask<IStoreInfo<{{target.IdType}}, {{target.ModelType}}>> IStoreInfoProvider<{{target.IdType}}, {{target.ModelType}}>.GetStoreInfoAsync(CancellationToken token)
                          => Actor.GetStoreInfoAsync(token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> IStoreProvider<{{target.IdType}}, {{target.ModelType}}>.GetStoreAsync(DiscordGatewayClient client, IPathable path, CancellationToken token)
                          => {{target.ActorType}}.GetStoreAsync(client, path, token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static ValueTask<IEntityBroker<{{target.IdType}}, {{target.Symbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.Symbol}}, {{target.ModelType}}>.GetBrokerAsync(DiscordGatewayClient client, CancellationToken token)
                          => {{target.ActorType}}.GetBrokerAsync(client, token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static ValueTask<IManageableEntityBroker<{{target.IdType}}, {{target.Symbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.Symbol}}, {{target.ModelType}}>.GetBrokerForModelAsync(
                          DiscordGatewayClient client,
                          Type modelType,
                          CancellationToken token
                      ) => {{target.Symbol}}Actor.GetBrokerForModelAsync(client, modelType, token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $"static IReadOnlyCollection<BrokerProviderDelegate<{target.IdType}, {target.Symbol}, {target.ModelType}>> IBrokerProvider<{target.IdType}, {target.Symbol}, {target.ModelType}>.GetBrokerHierarchy() => {target.Symbol}Actor.GetBrokerHierarchy();"
                )!
            );

            generated.Add(target.Symbol.Name, (syntax, target));
        }

        foreach (var item in generated)
        {
            context.AddSource(
                $"CacheableEntities/{item.Key}",
                $$"""
                  {{item.Value.Target.Syntax.GetFormattedUsingDirectives("Discord.Gateway.State")}}

                  namespace {{item.Value.Target.Symbol.ContainingNamespace}};

                  {{item.Value.Syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}
