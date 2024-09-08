using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public class ModelLinking : ISyntaxGenerationCombineTask<ModelLinking.GenerationTarget>
{
    public const string LinkAttribute = "Discord.LinkAttribute";


    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        INamedTypeSymbol model,
        InterfaceDeclarationSyntax syntax,
        Dictionary<IPropertySymbol, INamedTypeSymbol> links
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public INamedTypeSymbol Model { get; } = model;
        public InterfaceDeclarationSyntax Syntax { get; } = syntax;
        public Dictionary<IPropertySymbol, INamedTypeSymbol> Links { get; } = links;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Model.Equals(other.Model, SymbolEqualityComparer.Default) && Links.Equals(other.Links);
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SymbolEqualityComparer.Default.GetHashCode(Model) * 397) ^ Links.GetHashCode();
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
        => node is InterfaceDeclarationSyntax iface && iface.Members.Any(x => x.AttributeLists.Count > 0);

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not InterfaceDeclarationSyntax interfaceSyntax) return null;

        if (!interfaceSyntax.Modifiers.Any(x => x.IsKind(SyntaxKind.PartialKeyword)))
            return null;

        if (context.SemanticModel.GetDeclaredSymbol(interfaceSyntax) is not { } symbol) return null;

        if (!symbol.AllInterfaces.Any(x => x.Name is "IModel")) return null;

        var links = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x
                .GetAttributes()
                .Any(x =>
                    (
                        x.AttributeClass?
                            .ToDisplayString()
                            .StartsWith(LinkAttribute)
                        ?? false
                    )
                    &&
                    x.AttributeClass.TypeArguments[0] is INamedTypeSymbol
                )
            )
            .ToDictionary(
                x => x,
                x => (INamedTypeSymbol) x
                    .GetAttributes()
                    .First(x => x
                        .AttributeClass!
                        .ToDisplayString()
                        .StartsWith(LinkAttribute)
                    ).AttributeClass!.TypeArguments[0],
                (IEqualityComparer<IPropertySymbol>) SymbolEqualityComparer.Default
            );

        if (links.Count == 0) return null;

        return new GenerationTarget(
            context.SemanticModel,
            symbol,
            interfaceSyntax,
            links
        );
    }


    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        foreach (var target in targets)
        {
            if (target is null) continue;

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            Dictionary<ITypeSymbol, List<(IPropertySymbol Property, ITypeSymbol LinkType)>> linksByType = new(SymbolEqualityComparer.Default);

            foreach (var link in target.Links)
            {
                var isNullableValueType = link.Key.Type.Name == "Nullable";

                var isNullable =
                    isNullableValueType ||
                    link.Key.Type.NullableAnnotation is NullableAnnotation.Annotated;

                var idType = isNullableValueType
                    ? ((INamedTypeSymbol) link.Key.Type).TypeArguments[0]
                    : link.Key.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

                var interfaceName = $"ILinkingModel<{idType}, {link.Value}>";

                if (!linksByType.TryGetValue(idType, out var links))
                    linksByType[idType] = links = new();

                links.Add((link.Key, link.Value));

                syntax = syntax
                    .AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                interfaceName
                            )
                        )
                    )
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              private bool TryGetIdFor{{link.Key.Name}}([MaybeNullWhen(false)] out {{idType}} id)
                              {
                                  {{(
                                      isNullable
                                          ? $$"""
                                              if({{link.Key.Name}} is not null)
                                              {
                                                  id = {{link.Key.Name}}{{(isNullableValueType ? ".Value" : string.Empty)}};
                                                  return true;
                                              }

                                              id = default;
                                              return false;

                                              """
                                          : $"""
                                             id = {link.Key.Name};
                                             return true;
                                             """
                                  )}}
                              }      
                              """
                        )!,
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              bool {{interfaceName}}.TryGetId([MaybeNullWhen(false)] out {{idType}} id)
                                  => TryGetIdFor{{link.Key.Name}}(out id);
                              """
                        )!
                    );
            }

            foreach (var link in linksByType)
            {
                syntax = syntax
                    .AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                $"ILinkingModel<{link.Key}>"
                            )
                        )
                    )
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              bool ILinkingModel<{{link.Key}}>.TryGetId(Type modelType, [MaybeNullWhen(false)] out {{link.Key}} id)
                              {
                                  {{
                                      string.Join(
                                          "\n",
                                          link.Value.Select(x =>
                                              $$"""
                                                if(modelType == typeof({{x.LinkType.ToDisplayString()}}))
                                                    return TryGetIdFor{{x.Property.Name}}(out id);
                                                """
                                          )
                                      )
                                  }}
                                  
                                  id = default;
                                  return false;
                              } 
                              """
                        )!
                    );
            }

            context.AddSource(
                $"ModelLinks/{target.Model.ToFullMetadataName()}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives("System.Diagnostics.CodeAnalysis")}}

                  namespace {{target.Model.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}