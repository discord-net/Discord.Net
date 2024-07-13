using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public class InterfaceProxy : IGenerationTask<InterfaceProxy.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> properties)
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> Properties { get; } = properties;
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        if (node is not ClassDeclarationSyntax classDeclarationSyntax)
            return false;

        return classDeclarationSyntax.Members.Any(x => x is PropertyDeclarationSyntax {AttributeLists.Count: > 0});
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        var dict = new Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>>();

        foreach (var member in target.Members
                     .Where(x => x is PropertyDeclarationSyntax {AttributeLists.Count: > 0}))
        {
            if (member is not PropertyDeclarationSyntax property) continue;

            foreach (var attribute in property.AttributeLists.SelectMany(x => x.Attributes))
            {
                if (attribute.ArgumentList is null) continue;

                if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attribute).Symbol is not IMethodSymbol
                    attributeSymbol) continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Discord.ProxyInterfaceAttribute") continue;

                dict.Add(property, attribute.ArgumentList.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList());
            }
        }

        if (dict.Count > 0)
        {
            return new GenerationTarget(
                context.SemanticModel,
                target,
                dict
            );
        }

        return null;
    }


    public void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        if (ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax) is not
            INamedTypeSymbol targetSymbol)
            return;

        var syntax = SyntaxUtils.CreateSourceGenClone(target.ClassDeclarationSyntax);

        foreach (var kvp in target.Properties)
        {
            var property = target.SemanticModel.GetDeclaredSymbol(kvp.Key);

            if (property is null)
                continue;

            foreach (var typeofInterface in kvp.Value)
            {
                var genericNode = typeofInterface.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();

                var semanticInterface =
                    ModelExtensions.GetTypeInfo(target.SemanticModel, genericNode ?? typeofInterface.Type);

                if (semanticInterface.Type is null)
                {
                    return;
                }

                if (semanticInterface.Type is not INamedTypeSymbol typeSymbol)
                {
                    return;
                }

                AddProxiedMembers(ref syntax, typeSymbol, targetSymbol, property);

                var proxiedInterfaceTypeSyntax = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Discord.IProxied"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.ParseTypeName(typeSymbol.ToDisplayString())
                        ])
                    )
                );
                syntax = syntax.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        proxiedInterfaceTypeSyntax
                    )
                );

                syntax = syntax.AddMembers([
                    SyntaxFactory.PropertyDeclaration(
                        [],
                        [],
                        SyntaxFactory.ParseTypeName(typeSymbol.ToDisplayString()),
                        SyntaxFactory.ExplicitInterfaceSpecifier(
                            proxiedInterfaceTypeSyntax
                        ),
                        SyntaxFactory.Identifier("ProxiedValue"),
                        null,
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.ThisExpression(),
                                SyntaxFactory.IdentifierName(property.Name)
                            )
                        ),
                        null,
                        SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                    )
                ]);
            }

            var rootProxiedInterfaceTypeSyntax = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier("Discord.IProxied"),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList([
                        SyntaxFactory.ParseTypeName(property.Type.ToDisplayString())
                    ])
                )
            );
            syntax = syntax.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    rootProxiedInterfaceTypeSyntax
                )
            );

            syntax = syntax.AddMembers([
                SyntaxFactory.PropertyDeclaration(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName(property.Type.ToDisplayString()),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        rootProxiedInterfaceTypeSyntax
                    ),
                    SyntaxFactory.Identifier("ProxiedValue"),
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName(property.Name)
                        )
                    ),
                    null,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                )
            ]);

            context.AddSource(
                $"InterfaceProxy/{target.ClassDeclarationSyntax.Identifier}_Proxied.g.cs",
                $$"""
                  namespace {{ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  #nullable enable

                  {{syntax.NormalizeWhitespace()}}

                  #nullable restore
                  """
            );
        }
    }

    private static void AddProxiedMembers(
        ref ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol toProxy,
        INamedTypeSymbol targetSymbol,
        IPropertySymbol proxiedTo)
    {
        var castedSyntax = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AsExpression,
                SyntaxFactory.IdentifierName(proxiedTo.Name),
                SyntaxFactory.ParseTypeName(toProxy.ToDisplayString())
            )
        );

        foreach (var member in toProxy.GetMembers())
        {
            if (targetSymbol.GetMembers(member.Name).Length > 0)
                continue;

            switch (member)
            {
                case IPropertySymbol property:
                    if (property.ExplicitInterfaceImplementations.Length > 0 || property.IsStatic)
                        break;

                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.PropertyDeclaration(
                            [],
                            CreateAccessors(property.DeclaredAccessibility),
                            SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                            null,
                            SyntaxFactory.Identifier(property.Name),
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    castedSyntax,
                                    SyntaxFactory.IdentifierName(property.Name)
                                )
                            ),
                            null,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    );

                    if (property.DeclaredAccessibility is not Accessibility.Public)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.PropertyDeclaration(
                                [],
                                [],
                                SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                    SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
                                ),
                                SyntaxFactory.Identifier(property.Name),
                                null,
                                SyntaxFactory.ArrowExpressionClause(
                                    SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ThisExpression(),
                                        SyntaxFactory.IdentifierName(property.Name)
                                    )
                                ),
                                null,
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                            )
                        );
                    }

                    break;

                case IMethodSymbol method:
                    if (method.IsStatic || method.MethodKind is not MethodKind.Ordinary)
                        break;

                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.MethodDeclaration(
                            [],
                            CreateAccessors(method.DeclaredAccessibility),
                            SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                            null,
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
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        castedSyntax,
                                        SyntaxFactory.IdentifierName(method.Name)
                                    ),
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
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    );

                    if (method.DeclaredAccessibility is not Accessibility.Public)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.MethodDeclaration(
                                [],
                                [],
                                SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                    SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
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
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.ThisExpression(),
                                            SyntaxFactory.IdentifierName(method.Name)
                                        ),
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
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                            )
                        );
                    }

                    break;
            }
        }
    }

    private static SyntaxTokenList CreateAccessors(Accessibility accessibility)
    {
        return SyntaxFactory.TokenList(accessibility switch
        {
            Accessibility.Private => [SyntaxFactory.Token(SyntaxKind.PrivateKeyword)],
            Accessibility.Protected => [SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)],
            Accessibility.ProtectedOrInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
            ],
            Accessibility.Internal => [SyntaxFactory.Token(SyntaxKind.InternalKeyword)],
            Accessibility.ProtectedAndInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.InternalKeyword)
            ],
            Accessibility.Public => [SyntaxFactory.Token(SyntaxKind.PublicKeyword)],
            _ => []
        });
    }
}
