using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class ModifyTrait
{
    private static bool ResolveModifiableTypes(
        INamedTypeSymbol symbol,
        SemanticModel semanticModel,
        AttributeData? traitAttribute,
        Logger logger,
        out TypeSyntax idType,
        out TypeSyntax userPropertiesType,
        out TypeSyntax apiParamsType,
        out TypeSyntax entityTypeSyntax,
        out TypeSyntax modelType,
        out IMethodSymbol routeMethod,
        out MemberAccessExpressionSyntax routeMemberAccess,
        out INamedTypeSymbol entityPropertiesInterface,
        out ITypeSymbol entityType
    )
    {
        entityType = default!;
        entityPropertiesInterface = default!;
        routeMemberAccess = default!;
        routeMethod = default!;
        idType = default!;
        userPropertiesType = default!;
        apiParamsType = default!;
        entityTypeSyntax = default!;
        modelType = default!;

        if (symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not InterfaceDeclarationSyntax syntax)
            return false;

        var symbolSemantic = semanticModel.Compilation.GetSemanticModel(syntax.SyntaxTree);

        if (traitAttribute is null)
        {
            traitAttribute = symbol.GetAttributes()
                .FirstOrDefault(x =>
                    x.AttributeClass?.ToDisplayString().StartsWith("Discord.ModifiableAttribute") ?? false
                );

            if (traitAttribute is null)
                return false;
        }

        var paramsType = traitAttribute.AttributeClass?.TypeArguments.FirstOrDefault();

        if (paramsType is null) return false;

        if (traitAttribute.ConstructorArguments.Length != 1)
            return false;

        var actorInterface = symbol.Interfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

        if (actorInterface is null)
        {
            logger.Warn($"{symbol.Name}: Cannot resolve actor interface");
            return false;
        }

        // get the api params type
        entityPropertiesInterface = Hierarchy.GetHierarchy(paramsType)
            .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IEntityProperties"))
            .Type;

        if (entityPropertiesInterface is null)
        {
            logger.Warn($"{symbol.Name}: Cannot resolve entity properties");
            return false;
        }

        var entityModel = EntityTraits.GetEntityModelOfInterface(actorInterface.TypeArguments[1] as INamedTypeSymbol);

        if (entityModel is null)
        {
            logger.Warn($"{symbol.Name}: Failed to find model");

            // TODO: resolve direct representation
            return false;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccessCheck)
            return false;

        routeMemberAccess = routeMemberAccessCheck;

        if (EntityTraits.GetRouteSymbol(routeMemberAccess, symbolSemantic) is not IMethodSymbol routeMethodCheck)
            return false;

        routeMethod = routeMethodCheck;

        entityType = actorInterface.TypeArguments[1];

        idType = SyntaxFactory.ParseTypeName(actorInterface.TypeArguments[0].ToDisplayString());
        userPropertiesType = SyntaxFactory.ParseTypeName(paramsType.ToDisplayString());
        apiParamsType = SyntaxFactory.ParseTypeName(entityPropertiesInterface.TypeArguments[0].ToDisplayString());
        entityTypeSyntax = SyntaxFactory.ParseTypeName(entityType.ToDisplayString());
        modelType = SyntaxFactory.ParseTypeName(entityModel.TypeArguments[0].ToDisplayString());

        return true;
    }

    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Dictionary<string, InterfaceDeclarationSyntax> entitiesSyntax,
        Logger logger)
    {
        if (!ResolveModifiableTypes(
                target.InterfaceSymbol,
                target.SemanticModel,
                traitAttribute,
                logger,
                out var idType,
                out var userPropertiesType,
                out var apiParamsType,
                out var entityTypeSyntax,
                out var modelType,
                out var routeMethod,
                out var routeMemberAccess,
                out var entityPropertiesInterface,
                out var entityType)
           ) return;

        foreach (var baseInterface in target.InterfaceSymbol.AllInterfaces)
        {
            if (!ResolveModifiableTypes(
                    baseInterface,
                    target.SemanticModel,
                    null,
                    logger,
                    out var baseIdType,
                    out var baseUserPropertiesType,
                    out var baseApiParamsType,
                    out var baseEntityTypeSyntax,
                    out var baseModelType,
                    out var baseRouteMethod,
                    out var baseRouteMemberAccess,
                    out var baseEntityPropertiesInterface,
                    out var baseEntityType)
               ) continue;

            // base is modifiable, we should override its method if its parameters are assignable from ours

        }

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

                            logger.Warn(
                                $"Couldn't resolve route argument type {extra.Type} for {target.InterfaceSymbol.Name}");

                            return null;
                        })
                    )
                ),
                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
            )
        );
    }
}
