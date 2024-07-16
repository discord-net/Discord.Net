using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class FetchableTrait
{
    public static bool DefineFetchableRoute(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        ITypeSymbol idType,
        ITypeSymbol modelType,
        ISymbol route,
        MemberAccessExpressionSyntax routeMemberAccess,
        string fetchableType,
        string fetchMethod,
        out Dictionary<IParameterSymbol, ParameterSyntax> extraParameters,
        ITypeSymbol? targetInterface = null)
    {
        targetInterface ??= target.InterfaceSymbol;

        var extraParametersLocal = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);
        extraParameters = extraParametersLocal;

        ExpressionSyntax? routeAccessBody = route switch
        {
            IMethodSymbol methodSymbol => InvocationExpression(
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

                    extraParametersLocal.Add(extra, extraSyntax);
                    return Argument(IdentifierName(extraSyntax.Identifier));
                })
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        if (routeAccessBody is null) return false;

        var modelTypeSyntax = ParseTypeName(modelType.ToDisplayString());

        var fetchableInterface = GenericName(
            Identifier(fetchableType),
            TypeArgumentList(
                SeparatedList([
                    ParseTypeName(idType.ToDisplayString()),
                    modelTypeSyntax
                ])
            )
        );

        var apiOutInterface = GenericName(
            Identifier("IApiOutRoute"),
            TypeArgumentList(
                SeparatedList(new TypeSyntax[]
                {
                    fetchMethod == "FetchManyRoute"
                        ? GenericName(
                            Identifier("IEnumerable"),
                            TypeArgumentList(
                                SeparatedList([
                                    modelTypeSyntax
                                ])
                            )
                        )
                        : modelTypeSyntax
                })
            )
        );

        var modifiers = TokenList(
            Token(SyntaxKind.InternalKeyword),
            Token(SyntaxKind.NewKeyword),
            Token(SyntaxKind.StaticKeyword)
        );

        if (extraParameters.Count > 0 && !(
                targetInterface.AllInterfaces.Any(IsFetchableTarget) ||
                (targetInterface.AllInterfaces.Any(LoadableTrait.IsLoadable) && fetchMethod == "FetchRoute")
            ))
        {
            modifiers = modifiers.RemoveAt(modifiers.IndexOf(SyntaxKind.NewKeyword));
        }

        syntax = syntax.AddMembers(
            MethodDeclaration(
                [],
                modifiers,
                apiOutInterface,
                null,
                Identifier(fetchMethod),
                null,
                ParameterList(
                    SeparatedList([
                        Parameter(
                            [],
                            [],
                            IdentifierName("Discord.IPathable"),
                            Identifier("path"),
                            null
                        ),
                        Parameter(
                            [],
                            [],
                            ParseTypeName(idType.ToDisplayString()),
                            Identifier("id"),
                            null
                        ),
                        ..extraParameters.Select(x =>
                            Parameter(
                                [],
                                [],
                                x.Value.Type,
                                x.Value.Identifier,
                                x.Value.Default
                            )
                        )
                    ])
                ),
                [],
                null,
                ArrowExpressionClause(
                    routeAccessBody
                ),
                Token(SyntaxKind.SemicolonToken)
            )
        );

        // update 'routeAccessBody' to point to the declared method above
        routeAccessBody = InvocationExpression(
            IdentifierName(fetchMethod),
            ArgumentList(
                SeparatedList([
                    Argument(IdentifierName("path")),
                    Argument(IdentifierName("id")),
                    ..extraParameters.Select(x =>
                        Argument(
                            NameColon(x.Key.Name),
                            default,
                            x.Value.Default?.Value ?? LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                        )
                    )
                ])
            )
        );

        syntax = syntax
            .AddBaseListTypes(
                SimpleBaseType(fetchableInterface)
            )
            .AddMembers(
                MethodDeclaration(
                    [],
                    TokenList(
                        Token(SyntaxKind.StaticKeyword)
                    ),
                    apiOutInterface,
                    ExplicitInterfaceSpecifier(fetchableInterface),
                    Identifier(fetchMethod),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                IdentifierName("Discord.IPathable"),
                                Identifier("path"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                ParseTypeName(idType.ToDisplayString()),
                                Identifier("id"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        routeAccessBody
                    ),
                    Token(SyntaxKind.SemicolonToken)
                )
            );

        var implementedBases = new HashSet<ITypeSymbol>([modelType], SymbolEqualityComparer.Default);

        foreach (var baseFetchable in targetInterface.AllInterfaces.Where(WillBeFetchableTarget))
        {
            switch (fetchMethod)
            {
                case "FetchRoute" when !WillBeFetchable(baseFetchable):
                    continue;
                case "FetchManyRoute" when !WillBeFetchableOfMany(baseFetchable):
                    continue;
            }

            var baseModel = EntityTraits.GetModelInterface(baseFetchable);

            if (baseModel is null)
                continue;

            var baseModelSyntax = ParseTypeName(baseModel.ToDisplayString());

            if (!implementedBases.Add(baseModel))
            {
                continue;
            }

            syntax = syntax.AddMembers(
                MethodDeclaration(
                    [],
                    TokenList(
                        Token(SyntaxKind.StaticKeyword)
                    ),
                    GenericName(
                        Identifier("IApiOutRoute"),
                        TypeArgumentList(
                            SeparatedList(new TypeSyntax[]
                            {
                                IsFetchableOfMany(baseFetchable)
                                    ? GenericName(
                                        Identifier("IEnumerable"),
                                        TypeArgumentList(
                                            SeparatedList([
                                                baseModelSyntax
                                            ])
                                        )
                                    )
                                    : baseModelSyntax
                            })
                        )
                    ),
                    ExplicitInterfaceSpecifier(
                        GenericName(
                            Identifier(fetchableType),
                            TypeArgumentList(
                                SeparatedList([
                                    ParseTypeName(idType.ToDisplayString()),
                                    baseModelSyntax
                                ])
                            )
                        )
                    ),
                    Identifier(fetchMethod),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                IdentifierName("Discord.IPathable"),
                                Identifier("path"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                ParseTypeName(idType.ToDisplayString()),
                                Identifier("id"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    ArrowExpressionClause(
                        routeAccessBody
                    ),
                    Token(SyntaxKind.SemicolonToken)
                )
            );
        }

        return true;
    }

    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute)
    {
        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        // refreshable/loadable implements fetchable
        if ((IsRefreshable(target.InterfaceSymbol) || LoadableTrait.IsLoadable(target.InterfaceSymbol)) &&
            traitAttribute.AttributeClass?.ToDisplayString() == "Discord.Fetchable")
            return;

        var entityInterface = EntityTraits.GetEntityInterface(target.InterfaceSymbol);

        if (entityInterface is null)
            return;

        var entityOfInterface = EntityTraits.GetEntityModelOfInterface(target.InterfaceSymbol);

        if (entityOfInterface is null)
            return;

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is null) return;

        var idType = entityInterface.TypeArguments[0];

        var fetchableType = traitAttribute.AttributeClass?.Name switch
        {
            "FetchableAttribute" => "Discord.IFetchable",
            "FetchableOfManyAttribute" => "Discord.IFetchableOfMany",
            _ => null
        };

        var fetchMethod = fetchableType switch
        {
            "Discord.IFetchable" => "FetchRoute",
            "Discord.IFetchableOfMany" => "FetchManyRoute",
            _ => null
        };

        if (fetchableType is null || fetchMethod is null) return;

        DefineFetchableRoute(
            ref syntax,
            target,
            idType,
            entityOfInterface.TypeArguments[0],
            route,
            routeMemberAccess,
            fetchableType,
            fetchMethod,
            out _
        );
    }

    public static bool WillBeFetchableTarget(ITypeSymbol interfaceSymbol)
    {
        return IsFetchableTarget(interfaceSymbol) || IsRefreshable(interfaceSymbol) || LoadableTrait.IsLoadable(interfaceSymbol);
    }

    public static bool WillBeFetchable(ITypeSymbol interfaceSymbol)
        => IsFetchable(interfaceSymbol) || IsRefreshable(interfaceSymbol) || LoadableTrait.IsLoadable(interfaceSymbol);

    public static bool WillBeFetchableOfMany(ITypeSymbol interfaceSymbol)
        => IsFetchableOfMany(interfaceSymbol);

    public static bool IsRefreshable(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.RefreshableAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IRefreshable"));
    }

    public static bool IsFetchableTarget(ITypeSymbol interfaceSymbol)
    {
        return IsFetchable(interfaceSymbol) || IsFetchableOfMany(interfaceSymbol);
    }

    public static bool IsFetchable(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.FetchableAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IFetchable"));
    }

    public static bool IsFetchableOfMany(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.FetchableOfManyAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IFetchableOfMany"));
    }
}
