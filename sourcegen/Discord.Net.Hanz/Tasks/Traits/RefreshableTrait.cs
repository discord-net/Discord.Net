using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
namespace Discord.Net.Hanz.Tasks.Traits;

public static class RefreshableTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Logger logger)
    {
        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        var entityInterface = target.InterfaceSymbol.AllInterfaces
            .LastOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntity<"));

        if (entityInterface is null)
        {
            logger.Warn($"{target.InterfaceSymbol.Name}: Cannot find IEntity interface.");
            return;
        }

        var entityOfInterface = EntityTraits.GetEntityModelOfInterface(target.InterfaceSymbol);

        if (entityOfInterface is null)
        {
            logger.Warn($"{target.InterfaceSymbol.Name}: Cannot find IEntityOf interface.");
            return;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is null) return;

        var extraParameters = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);

        ExpressionSyntax? routeAccessBody = route switch
        {
            IMethodSymbol methodSymbol => InvocationExpression(
                routeMemberAccess,
                EntityTraits.ParseRouteArguments(methodSymbol, target, logger, extra =>
                {
                    if (!extra.IsOptional)
                        return null;

                    if (extra.DeclaringSyntaxReferences.Length == 0)
                        return null;

                    if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                        extraSyntax.Default is null)
                        return null;

                    extraParameters.Add(extra, extraSyntax);
                    return Argument(extraSyntax.Default.Value);
                })
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        var idType = entityInterface.TypeArguments[0];
        var modelType = entityOfInterface.TypeArguments[0] as INamedTypeSymbol;

        if (modelType is null) return;

        var refreshableInterface = GenericName(
            Identifier("Discord.IRefreshable"),
            TypeArgumentList(
                SeparatedList([
                    IdentifierName(target.InterfaceDeclarationSyntax.Identifier),
                    ParseTypeName(idType.ToDisplayString()),
                    ParseTypeName(modelType.ToDisplayString())
                ])
            )
        );

        if (extraParameters.Count > 0 && route is IMethodSymbol routeMethod)
        {
            syntax = syntax.AddMembers(
                MethodDeclaration(
                    [],
                    [],
                    IdentifierName("Task"),
                    ExplicitInterfaceSpecifier(
                        refreshableInterface
                    ),
                    Identifier("RefreshAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                NullableType(IdentifierName("RequestOptions")),
                                Identifier("options"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("CancellationToken"),
                                Identifier("token"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("RefreshAsync"),
                            ArgumentList(
                                SeparatedList([
                                    ..extraParameters.Values.Select(x =>
                                        Argument(
                                            x.Default?.Value ??
                                            LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                        )
                                    ),
                                    Argument(IdentifierName("options")),
                                    Argument(IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    Token(SyntaxKind.SemicolonToken)
                ).WithLeadingTrivia(
                    Comment("// point the default refresh method to our implementation")
                ),
                MethodDeclaration(
                    [],
                    [],
                    IdentifierName("Task"),
                    null,
                    Identifier("RefreshAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            ..extraParameters.Select(x =>
                                Parameter(
                                    [],
                                    [],
                                    x.Value.Type,
                                    x.Value.Identifier,
                                    x.Value.Default
                                )
                            ),
                            Parameter(
                                [],
                                [],
                                NullableType(IdentifierName("RequestOptions")),
                                Identifier("options"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.NullLiteralExpression)
                                )
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("CancellationToken"),
                                Identifier("token"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                )
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("RefreshInternalAsync"),
                            ArgumentList(
                                SeparatedList([
                                    Argument(ThisExpression()),
                                    Argument(
                                        InvocationExpression(
                                            routeMemberAccess,
                                            EntityTraits.ParseRouteArguments(
                                                routeMethod,
                                                target,
                                                logger,
                                                extra =>
                                                {
                                                    if (extraParameters.TryGetValue(extra, out var extraSyntax))
                                                        return Argument(
                                                            IdentifierName(extraSyntax.Identifier));

                                                    return null;
                                                },
                                                ThisExpression(),
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    ThisExpression(),
                                                    IdentifierName("Id")
                                                )
                                            )
                                        )
                                    ),
                                    Argument(IdentifierName("options")),
                                    Argument(IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    Token(SyntaxKind.SemicolonToken)
                )
            );
        }

        if (routeAccessBody is null) return;

        syntax = syntax
            .AddBaseListTypes(
                SimpleBaseType(refreshableInterface)
            );

        FetchableTrait.DefineFetchableRoute(
            ref syntax,
            target,
            idType,
            modelType,
            route,
            routeMemberAccess,
            "Discord.IFetchable",
            "FetchRoute",
            logger.GetSubLogger("IFetchable"),
            out _
        );
            //AddRefreshRouteMethod(ref syntax, idType, modelType, routeAccessBody, refreshableInterface);

        ApplyParentRefreshOverrides(ref syntax, target, modelType, refreshableInterface);
    }

    private static void ApplyParentRefreshOverrides(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        INamedTypeSymbol targetModelType,
        GenericNameSyntax refreshableInterface)
    {
        var bases = target.InterfaceSymbol.AllInterfaces.Where(IsRefreshTarget);

        var memberCount = syntax.Members.Count;

        foreach (var baseRefreshable in bases)
        {
            var entityIdType = EntityTraits.GetEntityInterface(baseRefreshable)?.TypeArguments.FirstOrDefault();
            var entityModelType = EntityTraits.GetEntityModelOfInterface(baseRefreshable)?.TypeArguments.FirstOrDefault();

            if (entityModelType is null || entityIdType is null)
                continue;

            if (!target.SemanticModel.Compilation.ClassifyCommonConversion(targetModelType, entityModelType).Exists)
                continue;

            syntax = syntax.AddMembers(
                MethodDeclaration(
                    [],
                    [],
                    IdentifierName("Task"),
                    ExplicitInterfaceSpecifier(
                        GenericName(
                            Identifier("Discord.IRefreshable"),
                            TypeArgumentList(
                                SeparatedList([
                                    ParseTypeName(baseRefreshable.ToDisplayString()),
                                    ParseTypeName(entityIdType.ToDisplayString()),
                                    ParseTypeName(entityModelType.ToDisplayString())
                                ])
                            )
                        )
                    ),
                    Identifier("RefreshAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                NullableType(IdentifierName("RequestOptions")),
                                Identifier("options"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("CancellationToken"),
                                Identifier("token"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("RefreshAsync"),
                            ArgumentList(
                                SeparatedList([
                                    Argument(IdentifierName("options")),
                                    Argument(IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    Token(SyntaxKind.SemicolonToken)
                ).WithLeadingTrivia(Comment($"// point {baseRefreshable.Name}'s RefreshAsync method to ours"))
            );
        }

        if (memberCount != syntax.Members.Count)
        {
            syntax = syntax.AddMembers(
                MethodDeclaration(
                    [],
                    TokenList([
                        Token(SyntaxKind.NewKeyword)
                    ]),
                    IdentifierName("Task"),
                    null,
                    Identifier("RefreshAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                NullableType(IdentifierName("RequestOptions")),
                                Identifier("options"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.NullLiteralExpression)
                                )
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("CancellationToken"),
                                Identifier("token"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                                )
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("RefreshInternalAsync"),
                            ArgumentList(
                                SeparatedList([
                                    Argument(ThisExpression()),
                                    Argument(
                                        InvocationExpression(
                                            IdentifierName("FetchRoute"),
                                            ArgumentList(
                                                SeparatedList([
                                                    Argument(ThisExpression()),
                                                    Argument(IdentifierName("Id"))
                                                ])
                                            )
                                        )
                                    ),
                                    Argument(IdentifierName("options")),
                                    Argument(IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    Token(SyntaxKind.SemicolonToken)
                ).WithLeadingTrivia(Comment("// redeclare RefreshAsync to allow overloading to point to ours")),
                MethodDeclaration(
                    [],
                    [],
                    IdentifierName("Task"),
                    ExplicitInterfaceSpecifier(refreshableInterface),
                    Identifier("RefreshAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                NullableType(IdentifierName("RequestOptions")),
                                Identifier("options"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("CancellationToken"),
                                Identifier("token"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        InvocationExpression(
                            IdentifierName("RefreshAsync"),
                            ArgumentList(
                                SeparatedList([
                                    Argument(IdentifierName("options")),
                                    Argument(IdentifierName("token"))
                                ])
                            )
                        )
                    ),
                    Token(SyntaxKind.SemicolonToken)
                ).WithLeadingTrivia(Comment("// point direct RefreshAsync to ours"))
            );
        }
    }

    private static bool IsRefreshTarget(INamedTypeSymbol symbol)
    {
        if (symbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.RefreshableAttribute"))
            return true;

        return false;
    }
}
