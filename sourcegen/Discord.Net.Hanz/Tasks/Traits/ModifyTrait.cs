using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class ModifyTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Dictionary<string, InterfaceDeclarationSyntax> entitiesSyntax,
        Logger logger)
    {
        var paramsType = traitAttribute.AttributeClass?.TypeArguments.FirstOrDefault();

        if (paramsType is null) return;

        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        var actorInterface = target.InterfaceSymbol.Interfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

        if (actorInterface is null)
        {
            logger.Warn($"{target.InterfaceSymbol.Name}: Cannot resolve actor interface");
            return;
        }

        // get the api params type
        var entityPropertiesInterface = Hierarchy.GetHierarchy(paramsType)
            .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IEntityProperties"))
            .Type;

        if (entityPropertiesInterface is null)
        {
            logger.Warn($"{target.InterfaceSymbol.Name}: Cannot resolve entity properties");
            return;
        }

        var entityModel = EntityTraits.GetEntityModelOfInterface(actorInterface.TypeArguments[1] as INamedTypeSymbol);

        if (entityModel is null)
        {
            logger.Warn($"{target.InterfaceSymbol.Name}: Failed to find model");

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
        var modelType = SyntaxFactory.ParseTypeName(entityModel.TypeArguments[0].ToDisplayString());

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
            entityPropertiesInterface.TypeArguments[0],
            logger
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
                ?? throw new KeyNotFoundException($"Couldn't find entity syntax for {entityType}");

        entitySyntax = entitySyntax.AddBaseListTypes(SyntaxFactory.SimpleBaseType(entityModifiableInterface));

        GenerateModifyOverload(
            ref entitySyntax,
            entityModifiableInterface,
            apiParamsType,
            idType,
            routeMemberAccess,
            routeMethod,
            target,
            entityPropertiesInterface.TypeArguments[0],
            logger
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
        ITypeSymbol userPropertiesType,
        Logger logger)
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
                        EntityTraits.ParseRouteArguments(routeMethod, target, logger, extra =>
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

                            logger.Warn($"Couldn't resolve route argument type {extra.Type} for {target.InterfaceSymbol.Name}");

                            return null;
                        })
                    )
                ),
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            )
        );
    }
}
