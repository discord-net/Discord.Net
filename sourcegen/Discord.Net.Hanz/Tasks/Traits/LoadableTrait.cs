using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class LoadableTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Dictionary<string, InterfaceDeclarationSyntax> entitiesSyntax)
    {
        if (traitAttribute.ConstructorArguments.Length != 2)
            return;

        var actorInterface = EntityTraits.GetActorInterface(target.InterfaceSymbol);

        if (actorInterface is null)
        {
            Hanz.Logger.Warn($"Cannot find actor interface for {target.InterfaceSymbol}");
            return;
        }

        var modelType = EntityTraits.GetEntityModelOfInterface(actorInterface.TypeArguments[1])?.TypeArguments[0];

        if (modelType is null)
        {
            Hanz.Logger.Warn($"Cannot find model interface for {target.InterfaceSymbol}");
            return;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
        {
            Hanz.Logger.Warn($"Cannot find route nameof for loadable {target.InterfaceSymbol}");
            return;
        }

        var route = EntityTraits.GetRouteSymbol(
            routeMemberAccess,
            target.SemanticModel,
            traitAttribute.ConstructorArguments[1].Values.Length
        );

        if (route is null)
        {
            Hanz.Logger.Warn($"Cannot find route for loadable {target.InterfaceSymbol}");
            return;
        }

        if (traitAttribute.ConstructorArguments[1].Values.Length > 0)
        {
            var genericNames = traitAttribute.ConstructorArguments[1]
                .Values
                .Select(x => x.Value!.ToString());

            routeMemberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                routeMemberAccess.Expression,
                SyntaxFactory.GenericName(
                    routeMemberAccess.Name.Identifier,
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(
                            genericNames
                                .Select(x => SyntaxFactory.ParseTypeName(x))
                        )
                    )
                )
            );
        }

        FetchableTrait.DefineFetchableRoute(
            ref syntax,
            target,
            actorInterface.TypeArguments[0],
            modelType,
            route,
            routeMemberAccess,
            "Discord.IFetchable",
            "FetchRoute",
            out var extraParameters
        );

        var returnType = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("ValueTask"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList<TypeSyntax>(new TypeSyntax[]
                {
                    SyntaxFactory.NullableType(
                        SyntaxFactory.ParseTypeName(actorInterface.TypeArguments[1].ToDisplayString()))
                })
            )
        );

        var loadableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("Discord.ILoadable"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    SyntaxFactory.ParseTypeName(target.InterfaceSymbol.ToDisplayString()),
                    SyntaxFactory.ParseTypeName(actorInterface.TypeArguments[0].ToDisplayString()),
                    SyntaxFactory.ParseTypeName(actorInterface.TypeArguments[1].ToDisplayString()),
                    SyntaxFactory.ParseTypeName(modelType.ToDisplayString())
                ])
            )
        );

        CreateOverloadToFetchAsync(
            ref syntax,
            returnType,
            loadableInterface
        );

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(loadableInterface)
            )
            .AddMembers(
                SyntaxFactory.MethodDeclaration(
                    [],
                    extraParameters.Count > 0 && !target.InterfaceSymbol.AllInterfaces.Any(IsLoadable)
                        ? []
                        : SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.NewKeyword)
                        ),
                    returnType,
                    null,
                    SyntaxFactory.Identifier("FetchAsync"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList([
                            ..extraParameters.Select(x =>
                                x.Value
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
                            SyntaxFactory.IdentifierName("FetchInternalAsync"),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList([
                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("Client")),
                                    SyntaxFactory.Argument(SyntaxFactory.ThisExpression()),
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.IdentifierName("FetchRoute"),
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList([
                                                    SyntaxFactory.Argument(SyntaxFactory.ThisExpression()),
                                                    SyntaxFactory.Argument(SyntaxFactory.IdentifierName("Id"))
                                                ])
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

        foreach (var baseLoadable in target.InterfaceSymbol.AllInterfaces.Where(IsLoadable))
        {
            var baseActorInterface = EntityTraits.GetActorInterface(baseLoadable);
            var baseModelType = EntityTraits.GetEntityModelOfInterface(baseActorInterface?.TypeArguments[1])
                ?.TypeArguments[0];

            if (baseActorInterface is null || baseModelType is null) continue;

            CreateOverloadToFetchAsync(
                ref syntax,
                SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("ValueTask"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList<TypeSyntax>(new TypeSyntax[]
                        {
                            SyntaxFactory.NullableType(
                                SyntaxFactory.ParseTypeName(baseActorInterface.TypeArguments[1].ToDisplayString()))
                        })
                    )
                ),
                SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Discord.ILoadable"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.ParseTypeName(baseLoadable.ToDisplayString()),
                            SyntaxFactory.ParseTypeName(baseActorInterface.TypeArguments[0].ToDisplayString()),
                            SyntaxFactory.ParseTypeName(baseActorInterface.TypeArguments[1].ToDisplayString()),
                            SyntaxFactory.ParseTypeName(baseModelType.ToDisplayString())
                        ])
                    )
                ),
                !baseActorInterface.TypeArguments[1]
                    .Equals(actorInterface.TypeArguments[1], SymbolEqualityComparer.Default)
            );
        }

        var entityType = actorInterface.TypeArguments[1];

        if (FetchableTrait.WillBeFetchable(entityType))
            return;

        if (!entitiesSyntax.TryGetValue(entityType.ToDisplayString(), out var entitySyntax))
            entitySyntax = (target.SemanticModel.Compilation
                                   .GetTypeByMetadataName(entityType.ToDisplayString())
                                   ?.DeclaringSyntaxReferences
                                   .FirstOrDefault()?
                                   .GetSyntax()
                               as InterfaceDeclarationSyntax)
                           ?.WithMembers([])
                           .WithBaseList(null)
                           .WithAttributeLists([])
                           .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                           .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                           ?? throw new KeyNotFoundException($"Couldn't find entity syntax for {entityType}");


        FetchableTrait.DefineFetchableRoute(
            ref entitySyntax,
            target,
            actorInterface.TypeArguments[0],
            modelType,
            route,
            routeMemberAccess,
            "Discord.IFetchable",
            "FetchRoute",
            out _,
            actorInterface.TypeArguments[1]
        );

        entitiesSyntax[entityType.ToDisplayString()] = entitySyntax;
    }

    private static void CreateOverloadToFetchAsync(
        ref InterfaceDeclarationSyntax syntax,
        GenericNameSyntax returnType,
        NameSyntax interfaceToOverload,
        bool async = false)
    {
        ExpressionSyntax invocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.IdentifierName("FetchAsync"),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList([
                    SyntaxFactory.Argument(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("options")),
                        default,
                        SyntaxFactory.IdentifierName("options")
                    ),
                    SyntaxFactory.Argument(
                        SyntaxFactory.NameColon(SyntaxFactory.IdentifierName("token")),
                        default,
                        SyntaxFactory.IdentifierName("token")
                    )
                ])
            )
        );

        if (async)
            invocation = SyntaxFactory.AwaitExpression(invocation);

        syntax = syntax.AddMembers(
            SyntaxFactory.MethodDeclaration(
                [],
                async
                    ? SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
                    )
                    : [],
                returnType,
                SyntaxFactory.ExplicitInterfaceSpecifier(interfaceToOverload),
                SyntaxFactory.Identifier("FetchAsync"),
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
                    invocation
                ),
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            )
        );
    }

    public static bool IsLoadable(ITypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.LoadableAttribute"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.ILoadable"));
    }
}
