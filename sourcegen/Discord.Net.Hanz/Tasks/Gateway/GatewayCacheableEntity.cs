using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks.Gateway;

public sealed class GatewayCacheableEntity : IGenerationCombineTask<GatewayCacheableEntity.GenerationContext>
{
    public sealed class GenerationContext(
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        ITypeSymbol idType,
        ITypeSymbol modelType
    ) : IEquatable<GenerationContext>
    {
        public ClassDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public ITypeSymbol IdType { get; } = idType;
        public ITypeSymbol ModelType { get; } = modelType;

        public bool Equals(GenerationContext? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                Syntax.IsEquivalentTo(other.Syntax) &&
                Symbol.Equals(other.Symbol, SymbolEqualityComparer.Default) &&
                IdType.Equals(other.IdType, SymbolEqualityComparer.Default) &&
                ModelType.Equals(other.ModelType, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is GenerationContext other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Syntax.GetHashCode();
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(Symbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ModelType);
                return hashCode;
            }
        }

        public static bool operator ==(GenerationContext? left, GenerationContext? right) => Equals(left, right);

        public static bool operator !=(GenerationContext? left, GenerationContext? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax;

    public static bool TryGetCacheableType(ITypeSymbol type, out INamedTypeSymbol? cacheable)
    {
        cacheable = type.Interfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<"));

        if (cacheable is null && (type.BaseType?.ToDisplayString().StartsWith("Discord.Gateway.GatewayCacheableEntity<") ?? false))
        {
            cacheable = type.BaseType.Interfaces
                .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<"));
        }

        return cacheable is not null;
    }

    public GenerationContext? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not ClassDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax, token) is not INamedTypeSymbol symbol) return null;

        if (symbol.IsAbstract) return null;

        if (!TryGetCacheableType(symbol, out var cacheable))
            return null;

        return new GenerationContext(
            syntax,
            symbol,
            cacheable!.TypeArguments[1],
            cacheable.TypeArguments[2]
        );
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationContext?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        foreach (var target in targets)
        {
            if (target is null)
                continue;

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var contextConstructSyntax = SyntaxFactory.ParseTypeName(
                $"IContextConstructable<{target.Symbol}, {target.ModelType}, ICacheConstructionContext<{target.IdType}, {target.Symbol}>, DiscordGatewayClient>"
            );

            syntax = syntax.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IStoreProvider<{target.IdType}, {target.ModelType}>")),
                SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName($"IBrokerProvider<{target.IdType}, {target.Symbol}, {target.ModelType}>")),
                SyntaxFactory.SimpleBaseType(contextConstructSyntax)
            );

            context.AddSource(
                $"CacheableEntities/{target.Symbol.Name}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.Symbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    // private static bool TryCreateConstructMethod(INamedTypeSymbol symbol, out MemberDeclarationSyntax syntax)
    // {
    //     syntax = default!;
    //
    //     if (symbol.Constructors.Length != 1)
    //         return false;
    //
    //     var invocationList = SyntaxFactory.ArgumentList();
    //
    //     foreach (var parameter in symbol.Constructors[0].Parameters)
    //     {
    //         switch (parameter.Type.Name)
    //         {
    //             case "DiscordGatewayClient":
    //                 invocationList = invocationList.AddArguments(
    //                     SyntaxFactory.Argument(SyntaxFactory.IdentifierName("client"))
    //                 );
    //                 break;
    //             default:
    //                 if(parameter.Type.Name.StartsWith("IIdentifiable"))
    //         }
    //     }
    // }
}
