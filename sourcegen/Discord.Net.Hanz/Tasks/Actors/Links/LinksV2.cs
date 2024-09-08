using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

public sealed class LinksV2 :
    ISyntaxGenerationCombineTask<LinksV2.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        InterfaceDeclarationSyntax syntax,
        INamedTypeSymbol entity,
        INamedTypeSymbol actor,
        INamedTypeSymbol model,
        ITypeSymbol id
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public InterfaceDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Entity { get; } = entity;
        public INamedTypeSymbol Actor { get; } = actor;
        public INamedTypeSymbol Model { get; } = model;
        public ITypeSymbol Id { get; } = id;

        public bool Equals(GenerationTarget other)
        {
            return Actor.GetAttributes().SequenceEqual(other.Actor.GetAttributes());
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not InterfaceDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax) is not INamedTypeSymbol symbol)
            return null;

        var actorInterface = Hierarchy.GetHierarchy(symbol)
            .Select(x => x.Type)
            .FirstOrDefault(x => x is {Name: "IActor", TypeArguments.Length: 2});

        if (actorInterface is null)
            return null;

        // don't apply to entities
        if (
            actorInterface.TypeArguments[1].Equals(symbol, SymbolEqualityComparer.Default) ||
            actorInterface.TypeArguments[1] is not INamedTypeSymbol entity)
            return null;

        var entityOfInterface = Hierarchy.GetHierarchy(actorInterface.TypeArguments[1])
            .Select(x => x.Type)
            .FirstOrDefault(x => x is {Name: "IEntityOf", TypeArguments.Length: 1});

        if (entityOfInterface?.TypeArguments.FirstOrDefault() is not INamedTypeSymbol model)
            return null;

        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            logger.Warn($"{symbol} is not partial, skipping");
            return null;
        }

        return new GenerationTarget(
            context.SemanticModel,
            syntax,
            entity,
            symbol,
            model,
            actorInterface.TypeArguments[0]
        );
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        foreach (var target in targets)
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            LinkMethods.Apply(context, target, logger);
            
            // add the default link types
            var syntax = SyntaxUtils
                .CreateSourceGenClone(target.Syntax);
            
            AddBackLink(ref syntax, target, logger);

            var formattedTypeArguments = string.Join(
                ", ",
                target.Actor, target.Id, target.Entity, target.Model
            );

            var linkTypeForTarget = target.SemanticModel.Compilation
                .GetTypeByMetadataName("Discord.ILinkType`4")
                ?.Construct(target.Actor, target.Id, target.Entity, target.Model);


            if (linkTypeForTarget is null)
            {
                targetLogger.Warn($"Failed to create link type for target {target.Actor}");
                continue;
            }

            var linkTypeSyntax = linkTypeForTarget.DeclaringSyntaxReferences
                    .FirstOrDefault()?
                    .GetSyntax()
                as InterfaceDeclarationSyntax;

            if (linkTypeSyntax is null)
            {
                targetLogger.Warn($"Failed to create link type syntax for target {target.Actor}");
                continue;
            }

            var linkTypes = linkTypeSyntax
                .Members
                .OfType<InterfaceDeclarationSyntax>()
                .Where(x => x.Identifier.ValueText is not "BackLink")
                .Select(x => x.WithMembers(
                    SyntaxFactory.List<MemberDeclarationSyntax>(
                        x.Members.OfType<InterfaceDeclarationSyntax>()
                    )
                ))
                .Select(x => x
                    .AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                $"{linkTypeForTarget}.{
                                    string.Join(
                                        ".",
                                        x.AncestorsAndSelf()
                                            .OfType<InterfaceDeclarationSyntax>()
                                            .TakeWhile(x =>
                                                x is not {Identifier.ValueText: "ILinkType"}
                                            )
                                            .Select(x =>
                                                $"{x.Identifier}{
                                                    (x.TypeParameterList?.Parameters.Count > 0
                                                        ? $"{x.TypeParameterList.WithParameters(
                                                            SyntaxFactory.SeparatedList(
                                                                x.TypeParameterList.Parameters
                                                                    .Select(x => x
                                                                        .WithVarianceKeyword(default)
                                                                    )
                                                            )
                                                        )}"
                                                        : string.Empty)
                                                }")
                                            .Reverse()
                                    )
                                }"
                            )
                        )
                    )
                    .WithMembers(
                        SyntaxFactory.List<MemberDeclarationSyntax>(
                            x.Members.OfType<InterfaceDeclarationSyntax>()
                                .Select(x => x.AddBaseListTypes(
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{linkTypeForTarget}.{
                                                    string.Join(
                                                        ".",
                                                        x.AncestorsAndSelf()
                                                            .OfType<InterfaceDeclarationSyntax>()
                                                            .TakeWhile(x =>
                                                                x is not {Identifier.ValueText: "ILinkType"}
                                                            )
                                                            .Select(x =>
                                                                $"{x.Identifier}{
                                                                    (x.TypeParameterList?.Parameters.Count > 0
                                                                        ? $"{x.TypeParameterList.WithParameters(
                                                                            SyntaxFactory.SeparatedList(
                                                                                x.TypeParameterList.Parameters
                                                                                    .Select(x => x
                                                                                        .WithVarianceKeyword(default)
                                                                                    )
                                                                            )
                                                                        )}"
                                                                        : string.Empty)
                                                                }")
                                                            .Reverse()
                                                    )
                                                }"
                                            )
                                        )
                                    )
                                )
                        )
                    )
                )
                .Select(x => x
                    .ReplaceNodes(
                        x.DescendantNodes().OfType<IdentifierNameSyntax>(),
                        (node, x) =>
                        {
                            switch (node.Identifier.Value)
                            {
                                case "TActor":
                                    return SyntaxFactory.IdentifierName(target.Actor.ToDisplayString());
                                case "TId":
                                    return SyntaxFactory.IdentifierName(target.Id.ToDisplayString());
                                case "TEntity":
                                    return SyntaxFactory.IdentifierName(target.Entity.ToDisplayString());
                                case "TModel":
                                    return SyntaxFactory.IdentifierName(target.Model.ToDisplayString());
                            }

                            return node;
                        }
                    )
                )
                .ToList();

            foreach (var linkType in linkTypes)
            {
                targetLogger.Log(
                    $"{linkType.Identifier}: {string.Join(", ", linkType.AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().Select(x => x.Identifier.ValueText))}");
            }

            if (targets.Any(x => x is not null && target.Actor.AllInterfaces.Contains(x.Actor)))
            {
                linkTypes = linkTypes
                    .Select(x => x.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword)))
                    .ToList();
            }

            var pagedAttribute = target.Entity
                .GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "PagedFetchableOfManyAttribute");

            if (pagedAttribute is not null)
            {
                var pageLinkType = linkTypes
                    .FirstOrDefault(x =>
                        x.Identifier.ValueText == "Paged" &&
                        x.TypeParameterList?.Parameters.Count == pagedAttribute.AttributeClass?.TypeArguments.Length
                    );

                if (pageLinkType is null || !linkTypes.Remove(pageLinkType))
                {
                    targetLogger.Warn($"Failed to create paged link type for target {target.Actor}");
                    continue;
                }

                pageLinkType = pageLinkType
                    .ReplaceNodes(
                        pageLinkType
                            .DescendantNodes()
                            .OfType<IdentifierNameSyntax>(),
                        (node, _) =>
                        {
                            if (node.Identifier.ValueText == "TParams")
                                return SyntaxFactory.IdentifierName(
                                    pagedAttribute.AttributeClass!.TypeArguments[0].ToDisplayString()
                                );

                            if (node.Identifier.ValueText == "TPaged")
                                return SyntaxFactory.IdentifierName(
                                    pagedAttribute.AttributeClass!.TypeArguments[1].ToDisplayString()
                                );

                            return node;
                        }
                    )
                    .WithConstraintClauses([])
                    .WithTypeParameterList(null);

                linkTypes.Add(pageLinkType);
            }


            if (TryGetFetchableRouteType(target, out var route) && route is IMethodSymbol methodSymbol)
            {
                var extraParameters = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);

                EntityTraits.ParseRouteArguments(
                    methodSymbol,
                    new EntityTraits.GenerationTarget(
                        target.SemanticModel,
                        target.Syntax,
                        target.Actor,
                        []
                    ),
                    targetLogger,
                    extra =>
                    {
                        if (!extra.IsOptional)
                            return null;

                        if (extra.DeclaringSyntaxReferences.Length == 0)
                            return null;

                        if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                            extraSyntax.Default is null)
                            return null;

                        extraParameters.Add(extra, extraSyntax);
                        return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(extraSyntax.Identifier));
                    }
                );

                if (extraParameters.Count > 0)
                {
                    var enumerableLink = linkTypes
                        .FirstOrDefault(x => x.Identifier.ValueText == "Enumerable");

                    if (enumerableLink is null || !linkTypes.Remove(enumerableLink))
                    {
                        targetLogger.Warn($"Failed to create enumerable link type for target {target.Actor}");
                        continue;
                    }

                    enumerableLink = enumerableLink
                        .AddMembers(
                            SyntaxFactory.ParseMemberDeclaration(
                                $"new ITask<IReadOnlyCollection<{target.Entity}>> AllAsync({string.Join(", ", extraParameters.Values)}, RequestOptions? options = null, CancellationToken token = default);"
                            )!,
                            SyntaxFactory.ParseMemberDeclaration(
                                $"ITask<IReadOnlyCollection<{target.Entity}>> {linkTypeForTarget}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options: options, token: token);"
                            )!
                        );

                    linkTypes.Add(enumerableLink);
                }
            }

            syntax = syntax
                .AddMembers(
                    linkTypes
                        .Select(x =>
                        {
                            AddBackLink(ref x, target, targetLogger);
                            return x;
                        })
                        .OfType<MemberDeclarationSyntax>()
                        .ToArray()
                );

            context.AddSource(
                $"Links/{target.Actor.ToFullMetadataName()}",
                $$"""
                  {{
                      string.Join(
                          "\n",
                          target.Syntax.GetUsingDirectives()
                              .Concat(linkTypeSyntax.GetUsingDirectives())
                              .Distinct()
                      )
                  }}

                  namespace {{target.Actor.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }

        var aliases = targets
            .OfType<GenerationTarget>()
            .Select(x =>
                $"global using {GetFriendlyName(x.Actor)}Link = Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;")
            .ToArray();

        if (aliases.Length > 0)
        {
            context.AddSource("Links/Aliases", string.Join(Environment.NewLine, aliases));
        }
    }

    private static void AddBackLink(ref InterfaceDeclarationSyntax syntax, GenerationTarget filler, Logger logger)
    {
        if (syntax.Identifier.ValueText == "BackLink")
        {
            logger.Log($"{filler.Actor}: Skipping {syntax.Identifier} (backlink)");
            return;
        }

        var typeMembers = syntax.Members.OfType<InterfaceDeclarationSyntax>().ToArray();

        if (typeMembers.Any(x => x.Identifier.ValueText == "BackLink"))
        {
            logger.Log($"{filler.Actor}: Skipping {syntax.Identifier} (has backlink)");
            return;
        }

        var baseIdent = new StringBuilder(syntax.Identifier.ValueText);

        if (syntax.TypeParameterList?.Parameters.Count > 0)
        {
            baseIdent
                .Append('<')
                .Append(
                    string.Join(
                        ", ",
                        syntax.TypeParameterList.Parameters.Select(x => x.Identifier)
                    )
                )
                .Append('>');
        }

        syntax = syntax
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                        public interface BackLink<out TSource> : 
                            {{baseIdent}}, 
                            Discord.IBackLink<TSource, {{string.Join(", ", filler.Actor, filler.Id, filler.Entity, filler.Model)}}>
                            where TSource : class, IPathable;
                      """
                )!
            );

        logger.Log($"{filler.Actor}: += {syntax.Identifier} backlink {baseIdent}");
        logger.Log(syntax.ToString());


        for (var i = 0; i < typeMembers.Length; i++)
        {
            if (syntax.Members[i] is not InterfaceDeclarationSyntax iface)
                continue;

            AddBackLink(ref iface, filler, logger);

            syntax = syntax.WithMembers(syntax.Members.RemoveAt(i).Insert(i, iface));
        }
    }

    private static bool TryGetFetchableRouteType(GenerationTarget target, out ISymbol route)
    {
        route = null!;

        var fetchableAttribute = target.Entity
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "FetchableOfManyAttribute");

        if (fetchableAttribute is null)
            return false;

        if (EntityTraits.GetNameOfArgument(fetchableAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return false;

        return (
            route = EntityTraits.GetRouteSymbol(
                routeMemberAccess,
                target.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
            )!
        ) is not null;
    }
    
    public static string GetFriendlyName(ITypeSymbol symbol)
    {
        if (symbol.TypeKind is TypeKind.Interface)
            return symbol.Name.Remove(0, 1).Replace("Actor", string.Empty);

        return symbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }
}