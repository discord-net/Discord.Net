using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class InvitableTrait
{
    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        Dictionary<string, InterfaceDeclarationSyntax> entitiesSyntax,
        Logger logger)
    {
        if (traitAttribute.AttributeClass is null || traitAttribute.ConstructorArguments.Length != 1)
            return;

        var actorInterface = EntityTraits.GetActorInterface(target.InterfaceSymbol);

        if (actorInterface is null)
            return;

        var entityType = actorInterface.TypeArguments[1];

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return;

        var route = EntityTraits.GetRouteSymbol(routeMemberAccess, target.SemanticModel);

        if (route is not IMethodSymbol
            {
                ReturnType: INamedTypeSymbol {TypeArguments.Length: 2} apiRoute
            } routeMethod) return;

        var apiType = apiRoute.TypeArguments[0];

        var routeAccessBody = SyntaxFactory.InvocationExpression(
            routeMemberAccess,
            EntityTraits.ParseRouteArguments(routeMethod, target, logger, extra =>
            {
                if (
                    extra.Type.Equals(
                        apiType,
                        SymbolEqualityComparer.Default
                    )
                    ||
                    target.SemanticModel.Compilation
                        .ClassifyCommonConversion(
                            extra.Type,
                            apiType
                        )
                        .Exists
                )
                {
                    return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                }

                if (extra.Type.TypeKind is TypeKind.TypeParameter && extra.Type.Name == "TArgs")
                {
                    return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                }

                logger.Warn(
                    $"Couldn't resolve route argument type {extra.Type} for {target.InterfaceSymbol.Name}"
                );

                return null;
            })
        );

        var idType = actorInterface.TypeArguments[0];
        var inviteType =
            traitAttribute.AttributeClass.TypeArguments.ElementAtOrDefault(1)?.ToDisplayString()
            ?? "Discord.IInvite";
        var paramsType = traitAttribute.AttributeClass.TypeArguments[0];

        AddInvitable(
            ref syntax,
            target.InterfaceSymbol,
            inviteType,
            idType,
            paramsType,
            apiType,
            in routeAccessBody
        );

        if (!entitiesSyntax.TryGetValue(entityType.ToDisplayString(), out var entitySyntax))
            entitySyntax
                = (
                      target.SemanticModel.Compilation
                              .GetTypeByMetadataName(entityType.ToDisplayString())
                              ?.DeclaringSyntaxReferences
                              .FirstOrDefault()?
                              .GetSyntax()
                          as InterfaceDeclarationSyntax
                  )
                  ?.WithMembers([])
                  .WithBaseList(null)
                  .WithAttributeLists([])
                  .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                  .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                  ?? throw new KeyNotFoundException($"Couldn't find entity syntax for {entityType}");

        logger.Log($"{target.InterfaceSymbol}: Adding invitable trait to {entityType} ({entitySyntax.Identifier})");

        AddInvitable(
            ref entitySyntax,
            entityType,
            inviteType,
            idType,
            paramsType,
            apiType,
            in routeAccessBody,
            target.InterfaceSymbol
        );

        entitiesSyntax[entityType.ToDisplayString()] = entitySyntax;
    }

    private static void AddInvitable(
        ref InterfaceDeclarationSyntax syntax,
        ITypeSymbol self,
        string inviteType,
        ITypeSymbol idType,
        ITypeSymbol paramsType,
        ITypeSymbol apiType,
        in InvocationExpressionSyntax routeSyntax,
        ITypeSymbol? actor = null
    )
    {
        var invitableInterfaceName =
            $"Discord.IInvitable<" +
            $"{self}," +
            $"{inviteType}," +
            $"{idType}," +
            $"{paramsType}," +
            $"{apiType}" +
            $">";

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(invitableInterfaceName)
                )
            )
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static IApiInOutRoute<{{apiType}}, Discord.Models.IInviteModel> {{invitableInterfaceName}}.CreateInviteRoute(IPathable path, {{idType}} id, {{apiType}} args)
                          => CreateInviteRoute(path, id, args);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static new IApiInOutRoute<{{apiType}}, Discord.Models.IInviteModel> CreateInviteRoute(IPathable path, {{idType}} id, {{apiType}} args)
                          => {{routeSyntax}};
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      [return: TypeHeuristic<IEntityProvider<{{inviteType}}, Discord.Models.IInviteModel>>(nameof(CreateEntity))]
                      new Task<{{inviteType}}> CreateInviteAsync(
                          {{paramsType}} args,
                          RequestOptions? options = null,
                          CancellationToken token = default)
                          => {{invitableInterfaceName}}.CreateInviteInternalAsync(
                              Client,
                              CreateInviteRoute(this, Id, args.ToApiModel()),
                              this,
                              options,
                              token
                          );
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      async Task<Discord.IInvite> IInvitable<{{paramsType}}>.CreateInviteAsync(
                        {{paramsType}} args,
                        RequestOptions? options,
                        CancellationToken token
                      ) => await CreateInviteAsync(args, options, token);
                      """
                )!
            );

        if (actor is null) return;

        invitableInterfaceName =
            $"Discord.IInvitable<" +
            $"{actor}," +
            $"{inviteType}," +
            $"{idType}," +
            $"{paramsType}," +
            $"{apiType}" +
            $">";

        syntax = syntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  static IApiInOutRoute<{{apiType}}, Discord.Models.IInviteModel> {{invitableInterfaceName}}.CreateInviteRoute(IPathable path, {{idType}} id, {{apiType}} args)
                      => CreateInviteRoute(path, id, args);
                  """
            )!
        );
    }
}
