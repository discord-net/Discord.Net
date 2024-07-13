using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class EntityHierarchies : IGenerationTask<EntityHierarchies.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        List<TypeOfExpressionSyntax> targetInterfaces,
        bool extendAll
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public List<TypeOfExpressionSyntax> TargetInterfaces { get; } = targetInterfaces;
        public bool ExtendAll { get; } = extendAll;
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax {AttributeLists.Count: > 0};

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        foreach (var attribute in target.AttributeLists.SelectMany(x => x.Attributes))
        {
            if (ModelExtensions.GetSymbolInfo(context.SemanticModel, attribute).Symbol is not IMethodSymbol
                attributeSymbol) continue;

            var attributeName = attributeSymbol.ContainingType.ToDisplayString();

            if (attributeName == "Discord.ExtendInterfaceDefaultsAttribute")
            {
                var targetTypes = attribute.ArgumentList?.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList() ?? [];

                return new GenerationTarget(
                    context.SemanticModel,
                    target,
                    targetTypes,
                    Attributes.GetAttributeNamedBoolArg(attribute, "ExtendAll", false)
                );
            }
        }

        return null;
    }


    public void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        var declaration = SyntaxFactory.ClassDeclaration(
            [],
            target.ClassDeclarationSyntax.Modifiers,
            target.ClassDeclarationSyntax.Identifier,
            target.ClassDeclarationSyntax.TypeParameterList,
            null,
            target.ClassDeclarationSyntax.ConstraintClauses,
            []
        );


        if (ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax) is not
            INamedTypeSymbol targetTypeSymbol) return;

        ICollection<INamedTypeSymbol?> targetInterfaces = target.TargetInterfaces.Select(x =>
        {
            var genericNode = x.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();

            return ModelExtensions.GetTypeInfo(target.SemanticModel, genericNode ?? x.Type).Type as INamedTypeSymbol;
        }).ToList();

        if (targetInterfaces.Count == 0)
            targetInterfaces = target.ExtendAll ? targetTypeSymbol.AllInterfaces : targetTypeSymbol.Interfaces;

        if (targetInterfaces.Any(x => x is null)) return;

        foreach (var targetInterface in targetInterfaces)
        foreach (var member in targetInterface!.GetMembers())
        {
            if (targetTypeSymbol.GetMembers(member.Name).Length > 0) continue;

            switch (member)
            {
                case IPropertySymbol property:
                    if (property is {IsVirtual: false, ExplicitInterfaceImplementations.Length: 0})
                        continue;

                    var propName = property.ExplicitInterfaceImplementations.Length > 0
                        ? property.ExplicitInterfaceImplementations[0].Name
                        : property.Name;

                    declaration = declaration.AddMembers(
                        SyntaxFactory.PropertyDeclaration(
                            [],
                            SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                            ),
                            SyntaxFactory.ParseTypeName(property.Type.ToDisplayString()),
                            null,
                            SyntaxFactory.Identifier(propName),
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ParenthesizedExpression(
                                        SyntaxFactory.BinaryExpression(
                                            SyntaxKind.AsExpression,
                                            SyntaxFactory.ThisExpression(),
                                            SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString())
                                        )
                                    ),
                                    SyntaxFactory.IdentifierName(propName)
                                )
                            ),
                            null,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    );

                    break;

                case IMethodSymbol
                {
                    IsStatic: false,
                    MethodKind: MethodKind.Ordinary or MethodKind.ExplicitInterfaceImplementation
                } methodSymbol:
                    if (methodSymbol is {IsVirtual: false, ExplicitInterfaceImplementations.Length: 0})
                        continue;

                    var methodName = methodSymbol.ExplicitInterfaceImplementations.Length > 0
                        ? methodSymbol.ExplicitInterfaceImplementations[0].Name
                        : methodSymbol.Name;

                    declaration = declaration.AddMembers(
                        SyntaxFactory.MethodDeclaration(
                            [],
                            SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                            ),
                            SyntaxFactory.ParseTypeName(methodSymbol.ReturnType.ToDisplayString()),
                            null,
                            SyntaxFactory.Identifier(methodName),
                            methodSymbol.TypeParameters.Length > 0
                                ? SyntaxFactory.TypeParameterList(
                                    SyntaxFactory.SeparatedList(
                                        methodSymbol.TypeParameters.Select(x =>
                                            SyntaxFactory.TypeParameter(
                                                x.Name
                                            )
                                        )
                                    )
                                )
                                : null,
                            SyntaxFactory.ParameterList(
                                SyntaxFactory.SeparatedList(
                                    methodSymbol.Parameters.Select(x =>
                                        SyntaxFactory.Parameter(
                                            [],
                                            [],
                                            SyntaxFactory.ParseTypeName(x.Type.ToDisplayString()),
                                            SyntaxFactory.Identifier(x.Name),
                                            null
                                        )
                                    )
                                )
                            ),
                            [],
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.ParenthesizedExpression(
                                            SyntaxFactory.BinaryExpression(
                                                SyntaxKind.AsExpression,
                                                SyntaxFactory.ThisExpression(),
                                                SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString())
                                            )
                                        ),
                                        SyntaxFactory.IdentifierName(methodName)
                                    ),
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList(
                                            methodSymbol.Parameters.Select(x =>
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name))
                                            )
                                        )
                                    )
                                )
                            ),
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    );
                    break;
            }
        }

        if (declaration.Members.Count == 0)
            return;

        context.AddSource(
            $"EntityHierarchies/{target.ClassDeclarationSyntax.Identifier}",
            $$"""
              namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

              #nullable enable

              {{declaration.NormalizeWhitespace()}}

              #nullable restore
              """
        );
    }
}
