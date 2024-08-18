using Discord.Net.Hanz.Utils;
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
        Logger logger,
        out Dictionary<IParameterSymbol, ParameterSyntax> extraParameters,
        ITypeSymbol? targetInterface = null,
        ITypeSymbol? pageParams = null)
    {
        targetInterface ??= target.InterfaceSymbol;

        var extraParametersLocal = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);
        extraParameters = extraParametersLocal;

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

                    extraParametersLocal.Add(extra, extraSyntax);
                    return Argument(IdentifierName(extraSyntax.Identifier));
                })
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        if (routeAccessBody is null)
        {
            logger.Warn($"{target.InterfaceSymbol}: {fetchableType}: No route access body");
            return false;
        }

        var modelTypeSyntax = ParseTypeName(modelType.ToDisplayString());

        var fetchableInterfaceTypeArguments = TypeArgumentList(
            SeparatedList([
                ParseTypeName(idType.ToDisplayString()),
                modelTypeSyntax
            ])
        );

        if (fetchableType is "Discord.IPagedFetchableOfMany")
        {
            if (pageParams is null)
            {
                logger.Warn("pageParams was null on 'Discord.IPagedFetchableOfMany'");
                return false;
            }

            var pagingInterface = pageParams
                .AllInterfaces
                .FirstOrDefault(x => x.Name == "IPagingParams" && x.IsGenericType);

            if (pagingInterface is null)
            {
                logger.Warn($"Unable to find paging params interface on {pageParams}");
                return false;
            }

            fetchableInterfaceTypeArguments = fetchableInterfaceTypeArguments.AddArguments(
                ParseTypeName(pageParams.ToDisplayString()),
                ParseTypeName(pagingInterface.TypeArguments[1].ToDisplayString())
            );
        }

        var fetchableInterface = GenericName(
            Identifier(fetchableType),
            fetchableInterfaceTypeArguments
        );

        if (fetchableType is "Discord.IPagedFetchableOfMany")
        {
            syntax = syntax.AddBaseListTypes(SimpleBaseType(fetchableInterface));
            return true;
        }
        
        var apiOutInterface = GenericName(
            Identifier("IApiOutRoute"),
            TypeArgumentList(
                SeparatedList(new TypeSyntax[]
                {
                    fetchableType switch
                    {
                        "Discord.IFetchableOfMany" => GenericName(
                            Identifier("IEnumerable"),
                            TypeArgumentList(
                                SeparatedList([
                                    modelTypeSyntax
                                ])
                            )
                        ),
                        _ => modelTypeSyntax
                    }
                })
            )
        );

        var modifiers = TokenList(
            Token(SyntaxKind.InternalKeyword),
            Token(SyntaxKind.NewKeyword),
            Token(SyntaxKind.StaticKeyword)
        );

        if (fetchMethod is not "FetchRoute" and not "FetchManyRoute" || extraParameters.Count > 0 && !(
                targetInterface.AllInterfaces.Any(IsFetchableTarget) ||
                (targetInterface.AllInterfaces.Any(LoadableTrait.IsLoadable) && fetchableType == "Discord.IFetchable")
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
                        ..fetchableType == "Discord.IFetchable"
                            ? (ParameterSyntax[])
                            [
                                Parameter(
                                    [],
                                    [],
                                    ParseTypeName(idType.ToDisplayString()),
                                    Identifier("id"),
                                    null
                                )
                            ]
                            : [],
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
                    ..fetchableType == "Discord.IFetchable"
                        ? (ArgumentSyntax[]) [Argument(IdentifierName("id"))]
                        : [],
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

        // update 'fetchMethod' to point to the interface method since it can be anything provided by the callee, future
        // use of it should point to the interface method.
        fetchMethod = fetchableType switch
        {
            "Discord.IFetchable" => "FetchRoute",
            "Discord.IFetchableOfMany" => "FetchManyRoute",
            _ => null!
        };

        if (syntax.BaseList?.Types.All(x => !x.Type.IsEquivalentTo(fetchableInterface)) ?? true)
        {
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
                                ..fetchMethod == "FetchRoute"
                                    ? (ParameterSyntax[])
                                    [
                                        Parameter(
                                            [],
                                            [],
                                            ParseTypeName(idType.ToDisplayString()),
                                            Identifier("id"),
                                            null
                                        )
                                    ]
                                    : [],
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

            if (
                !baseModel.Equals(modelType, SymbolEqualityComparer.Default) &&
                !target.SemanticModel.Compilation.HasImplicitConversion(modelType, baseModel)
            ) continue;

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
                            ..fetchMethod == "FetchRoute"
                                ? (ParameterSyntax[])
                                [
                                    Parameter(
                                        [],
                                        [],
                                        ParseTypeName(idType.ToDisplayString()),
                                        Identifier("id"),
                                        null
                                    )
                                ]
                                : [],
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
        AttributeData[] traitsAttribute,
        Logger logger)
    {
        foreach (var traitAttribute in traitsAttribute)
        {
            logger.Log($"Processing trait {traitAttribute}");
            
            if (traitAttribute.ConstructorArguments.Length != 1)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} doesn't have 1 argument");
                continue;
            }

            // refreshable/loadable implements fetchable
            if ((IsRefreshable(target.InterfaceSymbol) || LoadableTrait.IsLoadable(target.InterfaceSymbol)) &&
                traitAttribute.AttributeClass?.ToDisplayString() == "Discord.Fetchable")
            {
                logger.Log($"{target.InterfaceSymbol}: {traitAttribute} skipped, implemented by loadable");
                continue;
            }

            var entityInterface = EntityTraits.GetEntityInterface(target.InterfaceSymbol);

            if (entityInterface is null)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} no 'IEntity' interface");
                continue;
            }

            var entityOfInterface = EntityTraits.GetEntityModelOfInterface(target.InterfaceSymbol);

            if (entityOfInterface is null)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} no 'IEntityOf' interface");
                continue;
            }

            if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} no route member access");
                continue;
            }

            var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

            if (route is null)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} no route");
                continue;
            }

            var idType = entityInterface.TypeArguments[0];

            var fetchableType = traitAttribute.AttributeClass?.Name switch
            {
                "FetchableAttribute" => "Discord.IFetchable",
                "FetchableOfManyAttribute" => "Discord.IFetchableOfMany",
                "PagedFetchableOfManyAttribute" => "Discord.IPagedFetchableOfMany",
                _ => null
            };

            var fetchMethod =
                traitsAttribute.Length > 1
                    ? $"{route.Name}Route"
                    : fetchableType switch
                    {
                        "Discord.IFetchable" => "FetchRoute",
                        "Discord.IFetchableOfMany" => "FetchManyRoute",
                        "Discord.IPagedFetchableOfMany" => "FetchPagedRoute",
                        _ => null
                    };

            if (fetchableType is null || fetchMethod is null)
            {
                logger.Warn($"{target.InterfaceSymbol}: {traitAttribute} no fetchable type/method");
                continue;
            }

            var result = DefineFetchableRoute(
                ref syntax,
                target,
                idType,
                entityOfInterface.TypeArguments[0],
                route,
                routeMemberAccess,
                fetchableType,
                fetchMethod,
                logger,
                out _,
                pageParams: fetchableType is "Discord.IPagedFetchableOfMany"
                    ? traitAttribute.AttributeClass!.TypeArguments[0]
                    : null
            );
            
            logger.Log($"{target.InterfaceSymbol}: {traitAttribute}: {result}");
        }
    }

    public static bool WillBeFetchableTarget(ITypeSymbol interfaceSymbol)
    {
        return IsFetchableTarget(interfaceSymbol) || IsRefreshable(interfaceSymbol) ||
               LoadableTrait.IsLoadable(interfaceSymbol);
    }

    public static bool WillBeFetchable(ITypeSymbol interfaceSymbol)
        => IsFetchable(interfaceSymbol) || IsRefreshable(interfaceSymbol) || LoadableTrait.IsLoadable(interfaceSymbol);

    public static bool WillBeFetchableOfMany(ITypeSymbol interfaceSymbol)
        => IsFetchableOfMany(interfaceSymbol);

    public static bool IsRefreshable(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.RefreshableAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IRefreshable"));
    }

    public static bool IsFetchableTarget(ITypeSymbol interfaceSymbol)
    {
        return IsFetchable(interfaceSymbol) || IsFetchableOfMany(interfaceSymbol);
    }

    public static bool IsFetchable(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.FetchableAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IFetchable"));
    }

    public static bool IsFetchableOfMany(ITypeSymbol interfaceSymbol)
    {
        if (
            interfaceSymbol
            .GetAttributes()
            .Any(x =>
                x.AttributeClass?.ToDisplayString() is "Discord.FetchableOfManyAttribute"
                ||
                (
                    x.AttributeClass?.ToDisplayString().StartsWith("Discord.PagedFetchableOfManyAttribute")
                    ?? false
                )
            )
        )
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IFetchableOfMany"));
    }

    public static bool IsPagedFetchableOfMany(ITypeSymbol interfaceSymbol)
    {
        return interfaceSymbol
            .GetAttributes()
            .Any(x =>
                x.AttributeClass?
                    .ToDisplayString()
                    .StartsWith("Discord.PagedFetchableOfManyAttribute")
                ?? false
            );
    }
}