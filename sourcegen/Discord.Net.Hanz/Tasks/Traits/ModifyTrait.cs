using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class ModifyTrait
{
    // static IApiInOutRoute<ModifyGuildParams, IEntityModel> IModifiable.ModifyRoute(
    //     IPathable path,
    //     ulong id,
    //     ModifyGuildParams args
    // ) => Routes.ModifyGuild(id, args);

    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Dictionary<string, InterfaceDeclarationSyntax> entitiesSyntax)
    {
        var paramsType = traitAttribute.AttributeClass?.TypeArguments.FirstOrDefault();

        if (paramsType is null) return;

        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        var actorInterface = target.InterfaceSymbol.Interfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

        if (actorInterface is null)
        {
            Hanz.Logger.Warn($"{target.InterfaceSymbol.Name}: Cannot resolve actor interface");
            return;
        }

        // get the api params type
        var entityPropertiesInterface = paramsType.AllInterfaces
            .LastOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntityProperties"));

        if (entityPropertiesInterface is null)
        {
            Hanz.Logger.Warn($"{target.InterfaceSymbol.Name}: Cannot resolve entity properties");
            return;
        }

        var entityModel = actorInterface.TypeArguments[1].Interfaces
            .Where(x => x.ToDisplayString().StartsWith("Discord.IEntityOf"))
            .ToArray();

        if (entityModel.Length != 1)
        {
            Hanz.Logger.Warn($"{target.InterfaceSymbol.Name}: Failed to find model, {entityModel.Length} candidates returned");

            // TODO: resolve direct representation
            return;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        if (EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel) is not IMethodSymbol routeMethod)
            return;

        var entityType = actorInterface.TypeArguments[1];

        var idType = SyntaxFactory.ParseTypeName(actorInterface.TypeArguments[0].ToDisplayString());
        var userPropertiesType = SyntaxFactory.ParseTypeName(paramsType.ToDisplayString());
        var apiParamsType = SyntaxFactory.ParseTypeName(entityPropertiesInterface.TypeArguments[0].ToDisplayString());
        var entityTypeSyntax = SyntaxFactory.ParseTypeName(entityType.ToDisplayString());
        var modelType = SyntaxFactory.ParseTypeName(entityModel[0].TypeArguments[0].ToDisplayString());

        var actorModifiableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("Discord.IModifiable"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    idType,
                    SyntaxFactory.IdentifierName(target.InterfaceSymbol.Name),
                    userPropertiesType,
                    apiParamsType,
                    entityTypeSyntax,
                    modelType
                ])
            )
        );

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(actorModifiableInterface)
            );

        GenerateModifyOverload(
            ref syntax,
            actorModifiableInterface,
            apiParamsType,
            idType,
            routeMemberAccess,
            routeMethod,
            target,
            entityPropertiesInterface.TypeArguments[0]
        );

        var entityModifiableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("Discord.IModifiable"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    idType,
                    entityTypeSyntax,
                    userPropertiesType,
                    apiParamsType,
                    modelType
                ])
            )
        );

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
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(entityModifiableInterface))
                ?? throw new KeyNotFoundException($"Couldn't find entity syntax for {entityType}");

        GenerateModifyOverload(
            ref entitySyntax,
            entityModifiableInterface,
            apiParamsType,
            idType,
            routeMemberAccess,
            routeMethod,
            target,
            entityPropertiesInterface.TypeArguments[0]
        );

        entitiesSyntax[entityType.ToDisplayString()] = entitySyntax;
    }

    private static void GenerateModifyOverload(
        ref InterfaceDeclarationSyntax syntax,
        GenericNameSyntax modifiableInterface,
        TypeSyntax apiParamsType,
        TypeSyntax idType,
        MemberAccessExpressionSyntax routeMemberAccess,
        IMethodSymbol routeMethod,
        EntityTraits.GenerationTarget target,
        ITypeSymbol userPropertiesType)
    {
        syntax = syntax.AddMembers(
            SyntaxFactory.MethodDeclaration(
                [],
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                ),
                SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Discord.IApiInOutRoute"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList([
                            apiParamsType,
                            SyntaxFactory.IdentifierName("Discord.Models.IEntityModel")
                        ])
                    )
                ),
                SyntaxFactory.ExplicitInterfaceSpecifier(modifiableInterface),
                SyntaxFactory.Identifier("ModifyRoute"),
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
                            idType,
                            SyntaxFactory.Identifier("id"),
                            null
                        ),
                        SyntaxFactory.Parameter(
                            [],
                            [],
                            apiParamsType,
                            SyntaxFactory.Identifier("args"),
                            null
                        )
                    ])
                ),
                [],
                null,
                SyntaxFactory.ArrowExpressionClause(
                    SyntaxFactory.InvocationExpression(
                        routeMemberAccess,
                        EntityTraits.ParseRouteArguments(routeMethod, target, extra =>
                        {
                            if (
                                extra.Type.Equals(userPropertiesType,
                                    SymbolEqualityComparer.Default) ||
                                target.SemanticModel.Compilation
                                    .ClassifyCommonConversion(
                                        userPropertiesType,
                                        extra.Type
                                    ).Exists)
                            {
                                return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                            }

                            if (extra.Type.TypeKind is TypeKind.TypeParameter && extra.Type.Name == "TArgs")
                            {
                                return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                            }

                            Hanz.Logger.Warn($"Couldn't resolve route argument type {extra.Type} for {target.InterfaceSymbol.Name}");

                            return null;
                        })
                    )
                ),
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            )
        );
    }
}
