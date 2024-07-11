using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Exception = System.Exception;

namespace Discord.Net.Hanz.Tasks;

public static class SourceOfTruth
{
    public class GenerationTarget(
        SemanticModel semantic,
        TypeDeclarationSyntax typeDeclarationSyntax,
        MemberDeclarationSyntax memberDeclarationSyntax
    )
    {
        public SemanticModel Semantic { get; } = semantic;
        public TypeDeclarationSyntax TypeDeclarationSyntax { get; } = typeDeclarationSyntax;
        public MemberDeclarationSyntax MemberDeclarationSyntax { get; } = memberDeclarationSyntax;
    }

    public static bool IsValid(SyntaxNode node)
    {
        return node switch
        {
            MemberDeclarationSyntax member => member.AttributeLists.Count > 0,
            _ => false
        };
    }

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not MemberDeclarationSyntax member) return null;
        if (member.Parent is not TypeDeclarationSyntax type) return null;

        if (member.AttributeLists
            .SelectMany(x => x.Attributes)
            .All(x => Attributes.GetAttributeName(x, context.SemanticModel) != "Discord.SourceOfTruthAttribute"))
            return null;

        return new GenerationTarget(context.SemanticModel, type, member);
    }

    private class GenerationResult(string ns, string usingDirectives, TypeDeclarationSyntax syntax)
    {
        public string Namespace { get; } = ns;
        public string UsingDirectives { get; } = usingDirectives;
        public TypeDeclarationSyntax Syntax { get; set; } = syntax;
    }

    public static void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
    {
        var toGenerate = new Dictionary<string, GenerationResult>();

        foreach (var target in targets)
        {
            if (target is null) continue;

            var symbol = ModelExtensions.GetDeclaredSymbol(target.Semantic, target.TypeDeclarationSyntax);
            var sourceOfTruthType = (target.MemberDeclarationSyntax switch
            {
                MethodDeclarationSyntax method => ModelExtensions.GetTypeInfo(target.Semantic, method.ReturnType),
                PropertyDeclarationSyntax property => ModelExtensions.GetTypeInfo(target.Semantic, property.Type),
                _ => (TypeInfo?)null
            })?.Type;

            if (sourceOfTruthType is null) continue;

            var sourceOfTruthName = target.MemberDeclarationSyntax switch
            {
                MethodDeclarationSyntax method => method.Identifier.ValueText,
                PropertyDeclarationSyntax property => property.Identifier.ValueText,
                _ => null
            };

            if (sourceOfTruthName is null) continue;

            if (symbol is not INamedTypeSymbol typeSymbol) continue;

            List<ISymbol> validTargets = new();

            foreach (var iface in typeSymbol.AllInterfaces)
            {
                var targetMembers = iface.GetMembers(sourceOfTruthName);

                foreach (var member in targetMembers)
                {
                    switch (member)
                    {
                        case IMethodSymbol method when target.MemberDeclarationSyntax is MethodDeclarationSyntax:
                            if (!target.Semantic.Compilation.HasImplicitConversion(sourceOfTruthType,
                                    method.ReturnType))
                            {
                                Hanz.Logger.Warn(
                                    $"No conversion between {sourceOfTruthType.Name} -> {method.ReturnType.Name}");
                                break;
                            }

                            validTargets.Add(member);
                            break;
                        case IPropertySymbol property when target.MemberDeclarationSyntax is PropertyDeclarationSyntax:
                            if (!target.Semantic.Compilation.HasImplicitConversion(sourceOfTruthType, property.Type))
                            {
                                Hanz.Logger.Warn(
                                    $"No conversion between {sourceOfTruthType.Name} -> {property.Type.Name}");
                                break;
                            }

                            validTargets.Add(member);
                            break;
                    }
                }
            }

            try
            {
                var fullTypeName = target.Semantic.Compilation
                    .GetSemanticModel(target.TypeDeclarationSyntax.SyntaxTree)
                    .GetDeclaredSymbol(target.TypeDeclarationSyntax)!
                    .ToDisplayString();

                if (!toGenerate.TryGetValue(fullTypeName, out var targetTypeDeclaration))
                    targetTypeDeclaration = new GenerationResult(
                        target.Semantic.GetDeclaredSymbol(target.TypeDeclarationSyntax)!.ContainingNamespace.ToString(),
                        string.Join("\n",
                            target.TypeDeclarationSyntax.SyntaxTree.GetRoot().ChildNodes()
                                .OfType<UsingDirectiveSyntax>()),
                        target.TypeDeclarationSyntax
                            .WithBaseList(null)
                            .WithMembers([])
                            .WithAttributeLists([])
                    );

                foreach (var validTarget in validTargets)
                {
                    switch (validTarget)
                    {
                        case IPropertySymbol property:
                            targetTypeDeclaration.Syntax = targetTypeDeclaration.Syntax.AddMembers(
                                SyntaxFactory.PropertyDeclaration([], [],
                                    SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                                    SyntaxFactory.ExplicitInterfaceSpecifier(
                                        SyntaxFactory.IdentifierName(property.ContainingType.ToDisplayString())),
                                    SyntaxFactory.Identifier(property.Name),
                                    null,
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                        SyntaxFactory.IdentifierName(property.Name)
                                    ),
                                    null,
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                            );
                            break;
                        case IMethodSymbol method:
                            targetTypeDeclaration.Syntax = targetTypeDeclaration.Syntax.AddMembers(
                                SyntaxFactory.MethodDeclaration(
                                    [],
                                    [],
                                    SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                                    SyntaxFactory.ExplicitInterfaceSpecifier(
                                        SyntaxFactory.IdentifierName(method.ContainingType.ToDisplayString())
                                    ),
                                    SyntaxFactory.Identifier(method.Name),
                                    method.TypeParameters.Length > 0
                                        ? SyntaxFactory.TypeParameterList(
                                            SyntaxFactory.SeparatedList(
                                                method.TypeParameters.Select(x =>
                                                    SyntaxFactory.TypeParameter(x.ToDisplayString())
                                                )
                                            )
                                        )
                                        : null,
                                    SyntaxFactory.ParameterList(
                                        SyntaxFactory.SeparatedList(
                                            method.Parameters.Select(x =>
                                                SyntaxFactory.Parameter(
                                                    new SyntaxList<AttributeListSyntax>(),
                                                    new SyntaxTokenList(),
                                                    SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                                                    SyntaxFactory.Identifier(x.Name),
                                                    null
                                                )
                                            )
                                        )
                                    ),
                                    new SyntaxList<TypeParameterConstraintClauseSyntax>(),
                                    null,
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.IdentifierName(method.Name),
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList(
                                                    method.Parameters.Select(x =>
                                                        SyntaxFactory.Argument(
                                                            null,
                                                            SyntaxFactory.Token(SyntaxKind.None),
                                                            SyntaxFactory.IdentifierName(x.Name)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    ),
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                            );
                            break;
                    }
                }

                toGenerate[fullTypeName] = targetTypeDeclaration;
            }
            catch (Exception x)
            {
                Hanz.Logger.Log(LogLevel.Error, x.ToString());
            }
        }

        foreach (var target in toGenerate)
        {
            context.AddSource(
                $"SourceOfTruths/{target.Key}",
                $$"""
                  {{target.Value.UsingDirectives}}

                  namespace {{target.Value.Namespace}};

                  {{target.Value.Syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }
}
