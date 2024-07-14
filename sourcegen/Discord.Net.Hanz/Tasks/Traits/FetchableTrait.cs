using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class FetchableTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute)
    {
        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        // refreshable implements fetchable
        if (IsRefreshable(target.InterfaceSymbol) && traitAttribute.AttributeClass?.ToDisplayString() == "Discord.Fetchable")
            return;

        var entityInterface = target.InterfaceSymbol.AllInterfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntity"));

        if (entityInterface is null)
            return;

        var entityOfInterface = target.InterfaceSymbol.AllInterfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IEntityOf"));

        if (entityOfInterface is null)
            return;

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is null) return;

         ExpressionSyntax? routeAccessBody = route switch
        {
            IMethodSymbol methodSymbol => SyntaxFactory.InvocationExpression(
                routeMemberAccess,
                EntityTraits.ParseRouteArguments(methodSymbol, target)
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        if (routeAccessBody is null) return;

        var idType = entityInterface.TypeArguments[0];

        var fetchableType = traitAttribute.AttributeClass?.Name switch
        {
            "FetchableAttribute" => "Discord.IFetchable",
            "FetchableOfManyAttribute" => "Discord.IFetchableOfMany",
            _ => null
        };

        if (fetchableType is null) return;

        var modelType = SyntaxFactory.ParseTypeName(entityOfInterface.TypeArguments[0].ToDisplayString());

        var deletableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier(fetchableType),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    SyntaxFactory.ParseTypeName(idType.ToDisplayString()),
                    modelType
                ])
            )
        );

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(deletableInterface)
            )
            .AddMembers(
                SyntaxFactory.MethodDeclaration(
                    [],
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                    ),
                    SyntaxFactory.GenericName(
                        SyntaxFactory.Identifier("IApiOutRoute"),
                        SyntaxFactory.TypeArgumentList(
                            SyntaxFactory.SeparatedList(new TypeSyntax[]{
                                SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier("IEnumerable"),
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SeparatedList([
                                            modelType
                                        ])
                                    )
                                )
                            })
                        )
                    ),
                    SyntaxFactory.ExplicitInterfaceSpecifier(deletableInterface),
                    SyntaxFactory.Identifier("FetchRoute"),
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

    private static bool IsRefreshable(INamedTypeSymbol interfaceSymbol)
    {
        if (interfaceSymbol.GetAttributes().Any(x => x.AttributeClass?.ToDisplayString() == "Discord.Refreshable"))
            return true;

        return interfaceSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IRefreshable"));
    }
}
