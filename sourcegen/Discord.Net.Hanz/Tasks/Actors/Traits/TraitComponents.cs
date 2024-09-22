using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public sealed class TraitComponents : ISyntaxGenerationTask<TraitComponents.GenerationTarget>
{
    public sealed class TraitComponentTarget(
        INamedTypeSymbol component
    ) : IEquatable<TraitComponentTarget>
    {
        public INamedTypeSymbol Component { get; } = component;
        public Dictionary<IPropertySymbol, (INamedTypeSymbol Type, string? Getter)> LinkExtensions { get; } = [];
        public HashSet<TraitComponentTarget> Children { get; } = [];

        public bool Equals(TraitComponentTarget other)
        {
            return SymbolEqualityComparer.Default.Equals(Component, other.Component) &&
                   LinkExtensions.Count == other.LinkExtensions.Count;
        }
    }

    public sealed class GenerationTarget(
        Compilation compilation,
        INamedTypeSymbol trait,
        HashSet<TraitComponentTarget> components
    ) : IEquatable<GenerationTarget>
    {
        public Compilation Compilation { get; } = compilation;
        public INamedTypeSymbol Trait { get; } = trait;
        public HashSet<TraitComponentTarget> Components { get; } = components;


        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return
                Trait.Equals(other.Trait, SymbolEqualityComparer.Default) &&
                Components.SetEquals(other.Components);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return
                    (SymbolEqualityComparer.Default.GetHashCode(Trait) * 397) ^
                    Components.GetHashCode();
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right)
        {
            return !Equals(left, right);
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is InterfaceDeclarationSyntax iface && iface.Modifiers.IndexOf(SyntaxKind.PartialKeyword) != -1;

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.SemanticModel.GetDeclaredSymbol(context.Node) is not INamedTypeSymbol symbol) return null;

        if (!symbol.GetAttributes().Any(v => v.AttributeClass?.ToDisplayString() == "Discord.TraitAttribute"))
            return null;

        var traits = GetTraitComponentTargets(symbol);

        if (traits.Count == 0) return null;

        return new GenerationTarget(context.SemanticModel.Compilation, symbol, traits);
    }

    private HashSet<TraitComponentTarget> GetTraitComponentTargets(INamedTypeSymbol trait)
    {
        var components = trait.GetTypeMembers()
            .Where(x => x
                .GetAttributes()
                .Any(v => v.AttributeClass?.ToDisplayString() == "Discord.TraitComponentAttribute")
            )
            .ToArray();

        if (components.Length == 0) return [];

        var remaining = new HashSet<INamedTypeSymbol>(components, SymbolEqualityComparer.Default);

        var result = new HashSet<TraitComponentTarget>();

        while (remaining.Count > 0)
        {
            var current = remaining.First();
            remaining.Remove(current);
        }

        foreach (var component in components)
        {
            var target = GetTraitComponentTarget(component, trait);

            var componentAttribute = component.GetAttributes()
                .First(x => x.AttributeClass?.ToDisplayString() == "Discord.TraitComponentAttribute");

            var parentComponent = componentAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "Parent")
                .Value.Value as INamedTypeSymbol;

            if (parentComponent is not null)
            {
                var parent = result
                    .FirstOrDefault(x => x
                        .Component
                        .Equals(parentComponent, SymbolEqualityComparer.Default)
                    );

                if (parent is null)
                {
                    if (!remaining.Remove(parentComponent))
                        continue;

                    parent = GetTraitComponentTarget(parentComponent, trait);
                    result.Add(parent);
                }

                parent.Children.Add(target);
            }

            result.Add(target);
        }

        return result;
    }

    private TraitComponentTarget GetTraitComponentTarget(INamedTypeSymbol component, INamedTypeSymbol trait)
    {
        var target = new TraitComponentTarget(component);

        var linkExtensionAttributes = component.GetAttributes()
            .Where(x => x.AttributeClass?.ToDisplayString() == "Discord.TraitLinkExtendsAttribute")
            .ToArray();

        if (linkExtensionAttributes.Length > 0)
        {
            foreach (var extension in linkExtensionAttributes)
            {
                if (extension.ConstructorArguments.Length != 2) continue;

                var memberName = extension.ConstructorArguments[0].Value as string;
                var extensionType = extension.ConstructorArguments[1].Value as INamedTypeSymbol;

                if (memberName is null || extensionType is null) continue;

                if (trait.GetMembers(memberName).FirstOrDefault() is not IPropertySymbol property) continue;

                target.LinkExtensions[property] = (
                    Type: extensionType,
                    Getter: extension.NamedArguments
                        .FirstOrDefault(x => x.Key == "Getter")
                        .Value.Value as string
                );
            }
        }

        return target;
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target, Logger logger)
    {
        if (target is null) return;

        if (target.Components.Count == 0) return;

        logger.Log($"Processing {target.Trait}");

        var extensions = GenerateExtensions(target);

        if (extensions is null) return;

        context.AddSource(
            $"TraitComponents/{target.Trait.ToFullMetadataName()}",
            $$"""
              using Discord;

              namespace {{target.Trait.ContainingNamespace.ToDisplayString()}};

              public partial interface {{target.Trait.Name}}
              {
                  {{extensions.WithNewlinePadding(4)}}
              }
              """
        );
    }

    private static string? GenerateExtensions(GenerationTarget target)
    {
        var extensionComponents = target.Components
            .Where(x => x.LinkExtensions.Count > 0)
            .ToImmutableHashSet();

        if (extensionComponents.Count == 0) return null;

        var result = new StringBuilder();

        foreach
        (
            var component
            in extensionComponents
        )
        {
            result.AppendLine(GenerateType(component, bases: [target.Trait.Name]));
        }

        return result.ToString();

        string GenerateType(
            TraitComponentTarget component,
            ImmutableHashSet<string>? path = null,
            IEnumerable<string>? bases = null,
            int depth = 0)
        {
            var name = component.Component.Name.Replace("Component", string.Empty);

            var members = string.Join(
                Environment.NewLine,
                component
                    .LinkExtensions
                    .Select(x =>
                    {
                        var linkType = x.Key.Type.ToDisplayString();
                        var containingType = x.Key.ContainingType.ToDisplayString();

                        if (path is not null)
                        {
                            linkType += $".{string.Join(".", path)}";
                            containingType += $".{string.Join(".", path)}";
                        }

                        var getter = x.Value.Getter is not null
                            ? $"=> {x.Value.Getter};"
                            : "{ get; }";
                                                    
                        return
                            $"new {linkType}.{x.Value.Type.Name} {x.Key.Name} {getter}{Environment.NewLine}" +
                            $"{linkType} {containingType}.{x.Key.Name} => {x.Key.Name};";
                    })
            );

            var childTypes = component.Children.Count == 0
                ? string.Empty
                : string.Join(
                    Environment.NewLine,
                    component.Children.Select(x =>
                        GenerateType(
                            x,
                            (path ??= ImmutableHashSet<string>.Empty).Add(name),
                            (bases ?? []).Append(name),
                            depth + 1
                        )
                    )
                );

            return $$"""
                     public interface {{name}}{{(
                         bases is not null ? $" : {string.Join(", ", bases)}" : string.Empty
                     )}}
                     {
                         {{members.WithNewlinePadding(4)}}
                         {{childTypes}}
                     }
                     """.WithNewlinePadding(depth * 4);
        }
    }
}