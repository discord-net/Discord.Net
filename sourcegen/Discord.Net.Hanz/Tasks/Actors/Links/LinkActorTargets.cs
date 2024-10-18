using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

public class LinkActorTargets
{
    public enum AssemblyTarget
    {
        Core,
        Rest
    }

    public static readonly string[] AllowedAssemblies =
    [
        "Discord.Net.V4.Core",
        "Discord.Net.V4.Rest"
    ];

    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol entity,
        INamedTypeSymbol actor,
        INamedTypeSymbol model,
        ITypeSymbol id,
        AssemblyTarget assembly
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; private set; } = semanticModel;
        public TypeDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Entity { get; } = entity;
        public INamedTypeSymbol Actor { get; } = actor;
        public INamedTypeSymbol Model { get; } = model;
        public ITypeSymbol Id { get; } = id;
        public AssemblyTarget Assembly { get; } = assembly;

        public GenerationTarget? Update(Compilation compilation)
        {
            var tree = compilation.SyntaxTrees.FirstOrDefault(x => x.IsEquivalentTo(Syntax.SyntaxTree));

            if (tree is null) return null;

            var newNode = tree.GetRoot()
                .DescendantNodes()
                .OfType<TypeDeclarationSyntax>()
                .FirstOrDefault(x => x.IsEquivalentTo(Syntax));

            if (newNode is null) return null;
            
            return GetTargetForGeneration(
                compilation.GetSemanticModel(tree),
                newNode
            );
        }

        public bool Equals(GenerationTarget other)
            => GetHashCode() == other.GetHashCode();

        public override int GetHashCode()
            => HashCode
                .Of(Actor.ToDisplayString())
                .And((int)Assembly)
                .AndEach(Actor
                    .GetAttributes()
                    .Select(x => HashCode
                        .Of(x.AttributeClass?.ToDisplayString())
                        // .AndEach(x.ConstructorArguments)
                        // .AndEach(x.NamedArguments.Select(x => x.Key))
                        // .AndEach(x.NamedArguments.Select(x => x.Value))
                    )
                );

        public INamedTypeSymbol GetCoreActor()
        {
            if (Assembly is AssemblyTarget.Core) return Actor;

            return Hierarchy.GetHierarchy(Actor, false)
                .First(x =>
                    x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                    &&
                    x.Type.AllInterfaces.Any(y => y is {Name: "IActor", TypeArguments.Length: 2})
                ).Type;
        }

        public INamedTypeSymbol GetCoreEntity()
        {
            if (Assembly is AssemblyTarget.Core) return Entity;

            return Hierarchy.GetHierarchy(Entity, false)
                .First(x =>
                    x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                    &&
                    x.Type.AllInterfaces.Any(y => y is {Name: "IEntity"})
                ).Type;
        }
    }

    public static bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax;
    }

    public static AssemblyTarget? GetAssemblyTarget(
        Compilation compilation
    ) => GetAssemblyTarget(compilation.Assembly.Name);

    public static AssemblyTarget? GetAssemblyTarget(string name)
    {
        return name switch
        {
            "Discord.Net.V4.Core" => AssemblyTarget.Core,
            "Discord.Net.V4.Rest" => AssemblyTarget.Rest,
            _ => null
        };
    }

    public static GenerationTarget? GetTargetForSymbol(
        INamedTypeSymbol symbol,
        Compilation compilation,
        CancellationToken token = default)
    {
        if (!AllowedAssemblies.Contains(symbol.ContainingAssembly.Name)) return null;

        var assembly = GetAssemblyTarget(symbol.ContainingAssembly.Name) ?? throw new NotSupportedException();

        if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax(token) is not TypeDeclarationSyntax syntax)
            return null;

        var actorType = assembly switch
        {
            AssemblyTarget.Core => "IActor",
            AssemblyTarget.Rest => "IRestActor",
            _ => throw new NotSupportedException()
        };

        var actorInterface = Hierarchy.GetHierarchy(symbol)
            .Select(x => x.Type)
            .FirstOrDefault(x => x.Name == actorType && x is {TypeArguments.Length: 2});

        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            return null;
        }

        if (actorInterface is null)
            return null;

        // don't apply to entities
        if (
            actorInterface.TypeArguments[1].Equals(symbol, SymbolEqualityComparer.Default) ||
            actorInterface.TypeArguments[1] is not INamedTypeSymbol entity ||
            symbol.AllInterfaces.Contains(entity))
            return null;

        var entityOfInterface = Hierarchy.GetHierarchy(actorInterface.TypeArguments[1])
            .Select(x => x.Type)
            .FirstOrDefault(x => x is {Name: "IEntityOf", TypeArguments.Length: 1});

        if (entityOfInterface?.TypeArguments.FirstOrDefault() is not INamedTypeSymbol model)
            return null;

        if (!compilation.ContainsSyntaxTree(syntax.SyntaxTree))
        {
            compilation = compilation.AddSyntaxTrees(syntax.SyntaxTree);
        }

        return new GenerationTarget(
            compilation.GetSemanticModel(syntax.SyntaxTree),
            syntax,
            entity,
            symbol,
            model,
            actorInterface.TypeArguments[0],
            assembly
        );
    }

    public static GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        CancellationToken token = default
    ) => GetTargetForGeneration(context.SemanticModel, context.Node);

    public static GenerationTarget? GetTargetForGeneration(
        SemanticModel semantic,
        SyntaxNode node)
    {
        if (!AllowedAssemblies.Contains(semantic.Compilation.Assembly.Name)) return null;

        var assembly = GetAssemblyTarget(semantic.Compilation) ?? throw new NotSupportedException();

        if (node is not TypeDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(semantic, syntax) is not INamedTypeSymbol symbol)
            return null;
        
        var actorType = assembly switch
        {
            AssemblyTarget.Core => "IActor",
            AssemblyTarget.Rest => "IRestActor",
            _ => throw new NotSupportedException()
        };

        var actorInterface = Hierarchy.GetHierarchy(symbol)
            .Select(x => x.Type)
            .FirstOrDefault(x => x.Name == actorType && x is {TypeArguments.Length: 2});

        if (actorInterface is null)
            return null;

        // don't apply to entities
        if (
            actorInterface.TypeArguments[1].Equals(symbol, SymbolEqualityComparer.Default) ||
            actorInterface.TypeArguments[1] is not INamedTypeSymbol entity ||
            symbol.AllInterfaces.Contains(entity))
            return null;

        var entityOfInterface = Hierarchy.GetHierarchy(actorInterface.TypeArguments[1])
            .Select(x => x.Type)
            .FirstOrDefault(x => x is {Name: "IEntityOf", TypeArguments.Length: 1});

        if (entityOfInterface?.TypeArguments.FirstOrDefault() is not INamedTypeSymbol model)
            return null;

        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            return null;
        }

        var parent = syntax.Parent;

        while (parent is TypeDeclarationSyntax parentSyntax)
        {
            if (parentSyntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
            {
                return null;
            }

            parent = parentSyntax.Parent;
        }

        return new GenerationTarget(
            semantic,
            syntax,
            entity,
            symbol,
            model,
            actorInterface.TypeArguments[0],
            assembly
        );
    }
}