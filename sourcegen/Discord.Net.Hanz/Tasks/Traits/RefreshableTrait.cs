using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class RefreshableTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute)
    {
        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        var entityInterface = target.InterfaceSymbol.AllInterfaces
            .LastOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntity<"));

        if (entityInterface is null)
        {
            Hanz.Logger.Warn($"{target.InterfaceSymbol.Name}: Cannot find IEntity interface.");
            return;
        }

        var entityOfInterface = target.InterfaceSymbol.AllInterfaces
            .LastOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntityOf<"));

        if (entityOfInterface is null)
        {
            Hanz.Logger.Warn($"{target.InterfaceSymbol.Name}: Cannot find IEntityOf interface.");
            return;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is null) return;

        var extraParameters = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);

        ExpressionSyntax? routeAccessBody = route switch
        {
            IMethodSymbol methodSymbol => SyntaxFactory.InvocationExpression(
                routeMemberAccess,
                EntityTraits.ParseRouteArguments(methodSymbol, target, extra =>
                {
                    if (!extra.IsOptional)
                        return null;

                    if (extra.DeclaringSyntaxReferences.Length == 0)
                        return null;

                    if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                        extraSyntax.Default is null)
                        return null;

                    extraParameters.Add(extra, extraSyntax);
                    return SyntaxFactory.Argument(extraSyntax.Default.Value);
                })
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        var idType = entityInterface.TypeArguments[0];
        var modelType = entityOfInterface.TypeArguments[0];

        var refreshableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("Discord.IRefreshable"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    SyntaxFactory.IdentifierName(target.InterfaceDeclarationSyntax.Identifier),
                    SyntaxFactory.ParseTypeName(idType.ToDisplayString()),
                    SyntaxFactory.ParseTypeName(modelType.ToDisplayString())
                ])
            )
        );

        if (extraParameters.Count > 0 && route is IMethodSymbol routeMethod)
        {
            syntax = syntax.AddMembers(
                SyntaxFactory.MethodDeclaration(
                    [],
                    [],
                    SyntaxFactory.IdentifierName("Task"),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        refreshableInterface
                    ),
                    SyntaxFactory.Identifier("RefreshAsync"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.NullableType(SyntaxFactory.IdentifierName("RequestOptions")),
                                SyntaxFactory.Identifier("options"),
                                null
                            ),
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.IdentifierName("CancellationToken"),
                                SyntaxFactory.Identifier("token"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("RefreshAsync"),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList([
                                    ..extraParameters.Values.Select(x =>
                                        SyntaxFactory.Argument(
                                            x.Default?.Value ??
                                            SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                        )
                                    ),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("options")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ).WithLeadingTrivia(
                    SyntaxFactory.Comment("// point the default refresh method to our implementation")
                ),
                SyntaxFactory.MethodDeclaration(
                    [],
                    [],
                    SyntaxFactory.IdentifierName("Task"),
                    null,
                    SyntaxFactory.Identifier("RefreshAsync"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList([
                            ..extraParameters.Select(x =>
                                SyntaxFactory.Parameter(
                                    [],
                                    [],
                                    x.Value.Type,
                                    x.Value.Identifier,
                                    x.Value.Default
                                )
                            ),
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.NullableType(SyntaxFactory.IdentifierName("RequestOptions")),
                                SyntaxFactory.Identifier("options"),
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression)
                                )
                            ),
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.IdentifierName("CancellationToken"),
                                SyntaxFactory.Identifier("token"),
                                SyntaxFactory.EqualsValueClause(
                                    SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                )
                            )
                        ])
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("RefreshInternalAsync"),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList([
                                    SyntaxFactory.Argument(SyntaxFactory.ThisExpression()),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            routeMemberAccess,
                                            EntityTraits.ParseRouteArguments(
                                                routeMethod,
                                                target,
                                                extra =>
                                                {
                                                    if (extraParameters.TryGetValue(extra, out var extraSyntax))
                                                        return SyntaxFactory.Argument(
                                                            SyntaxFactory.IdentifierName(extraSyntax.Identifier));

                                                    return null;
                                                },
                                                SyntaxFactory.ThisExpression(),
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.ThisExpression(),
                                                    SyntaxFactory.IdentifierName("Id")
                                                )
                                            )
                                        )
                                    ),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("options")),
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                )
            );
        }

        if (routeAccessBody is null) return;

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(refreshableInterface)
            )
            .AddMembers(
                SyntaxFactory.MethodDeclaration(
                    [],
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                    ),
                    SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("Discord.IApiOutRoute"),
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList([
                                SyntaxFactory.ParseTypeName(modelType.ToDisplayString())
                            ])
                        )
                    ),
                    SyntaxFactory.ExplicitInterfaceSpecifier(refreshableInterface),
                    SyntaxFactory.Identifier("RefreshRoute"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.IdentifierName("Discord.IPathable"),
                                SyntaxFactory.Identifier("path"),
                                null
                            ),
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.ParseTypeName(idType.ToDisplayString()),
                                SyntaxFactory.Identifier("id"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        routeAccessBody
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                )
            );
    }
}
