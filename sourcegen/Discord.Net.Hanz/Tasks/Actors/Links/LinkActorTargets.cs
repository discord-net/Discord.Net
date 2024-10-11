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
        public SemanticModel SemanticModel { get; } = semanticModel;
        public TypeDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Entity { get; } = entity;
        public INamedTypeSymbol Actor { get; } = actor;
        public INamedTypeSymbol Model { get; } = model;
        public ITypeSymbol Id { get; } = id;
        public AssemblyTarget Assembly { get; } = assembly;

        public bool Equals(GenerationTarget other)
        {
            return Actor.GetAttributes().SequenceEqual(other.Actor.GetAttributes());
        }

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
        Compilation compilation)
    {
        return compilation.Assembly.Name switch
        {
            "Discord.Net.V4.Core" => AssemblyTarget.Core,
            "Discord.Net.V4.Rest" => AssemblyTarget.Rest,
            _ => null
        };
    }
    
    public static GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        CancellationToken token = default)
    {
        if (!AllowedAssemblies.Contains(context.SemanticModel.Compilation.Assembly.Name)) return null;

        var assembly = GetAssemblyTarget(context.SemanticModel.Compilation) ?? throw new NotSupportedException();

        if (context.Node is not TypeDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax) is not INamedTypeSymbol symbol)
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

        return new GenerationTarget(
            context.SemanticModel,
            syntax,
            entity,
            symbol,
            model,
            actorInterface.TypeArguments[0],
            assembly
        );
    }
}