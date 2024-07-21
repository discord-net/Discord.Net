using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class DeleteTrait
{
    // static IApiRoute IDeletable<ulong, IGuildActor>
    //     .DeleteRoute(IPathable path, ulong id) => Routes.DeleteGuild(id);
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Logger logger)
    {
        if (traitAttribute.ConstructorArguments.Length != 1)
            return;

        // get the id type
        var actorInterface = target.InterfaceSymbol.AllInterfaces
            .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

        if (actorInterface is null)
            return;

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is null) return;

        ExpressionSyntax? routeAccessBody = route switch
        {
            IMethodSymbol methodSymbol => SyntaxFactory.InvocationExpression(
                routeMemberAccess,
                EntityTraits.ParseRouteArguments(methodSymbol, target, logger)
            ),
            IPropertySymbol or IFieldSymbol => routeMemberAccess,
            _ => null
        };

        if (routeAccessBody is null) return;

        var idType = actorInterface.TypeArguments[0];

        var deletableInterface = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("Discord.IDeletable"),
            SyntaxFactory.TypeArgumentList(
                SyntaxFactory.SeparatedList([
                    SyntaxFactory.ParseTypeName(idType.ToDisplayString()),
                    SyntaxFactory.IdentifierName(target.InterfaceDeclarationSyntax.Identifier)
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
                    SyntaxFactory.IdentifierName("Discord.IApiRoute"),
                    SyntaxFactory.ExplicitInterfaceSpecifier(deletableInterface),
                    SyntaxFactory.Identifier("DeleteRoute"),
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
