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

        var results = new Dictionary<GenerationTarget, InterfaceDeclarationSyntax>();

        foreach (var target in targets)
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            targetLogger.Log($"Processing {target.Actor}");
            
            LinkMethods.Apply(context, target, logger);

            // add the default link types
            var syntax = SyntaxUtils
                .CreateSourceGenClone(target.Syntax);

            AddBackLink(ref syntax, target, logger);

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
                .Select(x => x
                    .WithMembers(
                        SyntaxFactory.List<MemberDeclarationSyntax>(
                            x.Members.OfType<InterfaceDeclarationSyntax>()
                                .Select(x => x
                                    .WithAttributeLists([])
                                    .WithModifiers(
                                        SyntaxFactory.TokenList(
                                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                                        )
                                    )
                                )
                        )
                    )
                    .WithAttributeLists([])
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                        )
                    )
                )
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
                                .Select(x =>
                                    {
                                        ApplyTargetedBases(ref x, linkTypeForTarget, target);
                                        return x;
                                    }
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
                .Where(x => x.AttributeClass?.Name == "PagedFetchableOfManyAttribute")
                .ToArray();

            if (pagedAttribute.Length > 0)
            {
                foreach (var attribute in pagedAttribute)
                {
                    var pageLinkType = linkTypes
                        .FirstOrDefault(x =>
                            x.Identifier.ValueText == "Paged" &&
                            x.TypeParameterList?.Parameters.Count == attribute.AttributeClass?.TypeArguments.Length
                        );

                    if (pageLinkType is null)
                    {
                        targetLogger.Warn($"Failed to create paged link type for target {target.Actor}");
                        continue;
                    }

                    var sb = new StringBuilder("Paged");

                    if (attribute.AttributeClass!.TypeArguments.Length >= 1)
                    {
                        var identifier = attribute.AttributeClass!
                            .TypeArguments[0]
                            .Name
                            .Replace("Page", string.Empty)
                            .Replace("Params", string.Empty);

                        sb.Append($"{identifier}");
                    }

                    if (attribute.AttributeClass!.TypeArguments.Length >= 2)
                    {
                        var identifier = GetFriendlyName(
                            attribute.AttributeClass!
                                .TypeArguments[1]
                        );

                        sb.Append($"As{identifier}");
                    }

                    pageLinkType = pageLinkType
                        .WithBaseList(
                            SyntaxFactory.BaseList(
                                SyntaxFactory.SeparatedList((BaseTypeSyntax[])
                                [
                                    SyntaxFactory.SimpleBaseType(
                                        SyntaxFactory.ParseTypeName(
                                            $"{pageLinkType.Identifier}{
                                                (pageLinkType.TypeParameterList?.Parameters.Count > 0
                                                    ? $"{pageLinkType.TypeParameterList.WithParameters(
                                                        SyntaxFactory.SeparatedList(
                                                            pageLinkType.TypeParameterList.Parameters
                                                                .Select(x => x
                                                                    .WithVarianceKeyword(default)
                                                                )
                                                        )
                                                    )}"
                                                    : string.Empty)
                                            }"
                                        )
                                    )
                                ])
                            )
                        );


                    pageLinkType = pageLinkType
                        .ReplaceNodes(
                            pageLinkType
                                .DescendantNodes()
                                .OfType<IdentifierNameSyntax>(),
                            (node, _) =>
                            {
                                return node.Identifier.ValueText switch
                                {
                                    "TParams" => SyntaxFactory.IdentifierName(
                                        attribute.AttributeClass!.TypeArguments[0]
                                            .ToDisplayString()),
                                    "TPaged" => SyntaxFactory.IdentifierName(attribute.AttributeClass!.TypeArguments[1]
                                        .ToDisplayString()),
                                    _ => node
                                };
                            }
                        )
                        .WithConstraintClauses([])
                        .WithTypeParameterList(null)
                        .WithIdentifier(
                            SyntaxFactory.Identifier($"{sb}")
                        );

                    linkTypes.Add(pageLinkType);
                }
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

            ApplyHierarchicalRoot(ref syntax, target, targets, targetLogger);
            LinkExtensions.Process(ref syntax, target, targets, targetLogger);

            results[target] = syntax;
        }

        foreach (var result in results)
        {
            var target = result.Key;
            var syntax = result.Value;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            var ancestors = results
                .Where(x => target
                    .Actor
                    .AllInterfaces
                    .Contains(x.Key.Actor)
                );

            foreach (var ancestor in ancestors)
            {
                targetLogger.Log($"{target.Actor}: Anscestor check on {ancestor.Key.Actor}");
                ApplyNewWhereNeeded(ref syntax, ancestor.Value, targetLogger);
            }

            try
            {
                context.AddSource(
                    $"Links/{target.Actor.ToFullMetadataName()}",
                    $$"""
                      {{target.Syntax.GetFormattedUsingDirectives("MorseCode.ITask")}}

                      namespace {{target.Actor.ContainingNamespace}};

                      #pragma warning disable CS0108
                      #pragma warning disable CS0109
                      {{syntax.NormalizeWhitespace()}}
                      #pragma warning restore CS0108
                      #pragma warning restore CS0109
                      """
                );
            }
            catch (Exception x)
            {
                targetLogger.Warn($"{target.Actor}: {x}");
            }
        }

        var aliases = targets
            .OfType<GenerationTarget>()
            .Select(x =>
                $"""
                 global using {GetFriendlyName(x.Actor)}Link = Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 global using {GetFriendlyName(x.Actor)}LinkType = Discord.ILinkType<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 """
            )
            .ToArray();

        if (aliases.Length > 0)
        {
            context.AddSource("Links/Aliases", string.Join(Environment.NewLine, aliases));
        }
    }

    private static void ApplyNewWhereNeeded(
        ref InterfaceDeclarationSyntax source,
        InterfaceDeclarationSyntax toCheckAgainst,
        Logger logger,
        int depth = 0)
    {
        for (var i = 0; i != source.Members.Count; i++)
        {
            var member = source.Members[i];

            if (member is not InterfaceDeclarationSyntax iface)
                continue;

            var companion = toCheckAgainst
                .Members
                .FirstOrDefault(x =>
                    x is InterfaceDeclarationSyntax target &&
                    target.Identifier.ValueText == iface.Identifier.ValueText
                ) as InterfaceDeclarationSyntax;

            if (companion is null) continue;

            logger.Log($"{"".PadLeft(depth * 2)} - {iface.Identifier} <> {companion.Identifier}");

            if(iface.Modifiers.IndexOf(SyntaxKind.NewKeyword) == -1)
                iface = iface.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            
            ApplyNewWhereNeeded(ref iface, companion, logger, depth + 1);

            source = source.WithMembers(
                source.Members.RemoveAt(i).Insert(i, iface)
            );
        }
    }

    private static string ToReferenceName(InterfaceDeclarationSyntax syntax)
    {
        return $"{syntax.Identifier}{
            (syntax.TypeParameterList?.Parameters.Count > 0
                ? $"{syntax.TypeParameterList.WithParameters(
                    SyntaxFactory.SeparatedList(
                        syntax.TypeParameterList.Parameters
                            .Select(x => x
                                .WithVarianceKeyword(default)
                            )
                    )
                )}"
                : string.Empty)
        }";
    }

    private static void ApplyTargetedBases(
        ref InterfaceDeclarationSyntax syntax,
        INamedTypeSymbol linkType,
        GenerationTarget target)
    {
        if (syntax.Identifier.ValueText is not "Indexable" and not "Enumerable" and not "Defined" and not "Paged")
            return;

        var path = syntax.AncestorsAndSelf()
            .OfType<InterfaceDeclarationSyntax>()
            .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
            .ToArray();

        switch (path.Length)
        {
            case <= 1: break;
            default:
                syntax = syntax.WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SeparatedList(
                            [
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(
                                        $"{linkType}.{string.Join(".", path.Select(ToReferenceName).Reverse())}"
                                    )
                                ),
                                ..path.SelectMany(x =>
                                    (BaseTypeSyntax[])
                                    [
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{target.Actor}.{ToReferenceName(x)}"
                                            )
                                        ),
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{linkType}.{ToReferenceName(x)}"
                                            )
                                        )
                                    ]
                                )
                            ]
                        )
                    )
                );
                break;
        }

        for (var i = 0; i < syntax.Members.Count; i++)
        {
            var member = syntax.Members[i];

            if (member is not InterfaceDeclarationSyntax child)
                continue;

            ApplyTargetedBases(ref child, linkType, target);

            syntax = syntax.WithMembers(
                syntax.Members.RemoveAt(i).Insert(i, child)
            );
        }
    }

    private static void ApplyHierarchicalRoot(
        ref InterfaceDeclarationSyntax syntax,
        GenerationTarget target,
        ImmutableArray<GenerationTarget?> targets,
        Logger logger)
    {
        if (!target.Actor.GetAttributes().Any(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute"))
            return;

        //if (syntax.Identifier.ValueText is "BackLink") return;

        var children = targets
            .Where(x => x?.Actor.AllInterfaces.Contains(target.Actor) ?? false)
            .OfType<GenerationTarget>()
            .ToArray();

        if (children.Length == 0) return;

        syntax = syntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  public interface Hierarchy
                  {
                      {{
                          string.Join(
                              Environment.NewLine,
                              children
                                  .Select(x =>
                                  {
                                      var name = string.Join(
                                          string.Empty,
                                          ToNameParts(GetFriendlyName(x.Actor))
                                              .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                      );

                                      return SyntaxFactory.ParseMemberDeclaration(
                                          $"Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}> {name} {{ get; }}"
                                      )!;
                                  })
                          )
                      }}
                  }  
                  """
            )!
        );

        syntax = syntax
            .ReplaceNodes(
                syntax.DescendantNodes().OfType<InterfaceDeclarationSyntax>(),
                (old, node) =>
                {
                    if (node.Identifier.ValueText == "Hierarchy")
                        return node;

                    if (node.Identifier.ValueText is "BackLink") return node;

                    var anscestors = old.AncestorsAndSelf()
                        .OfType<InterfaceDeclarationSyntax>()
                        .ToList();

                    var path = string.Join(
                        ".",
                        anscestors
                            .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
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
                                }"
                            )
                            .Reverse()
                    );

                    logger.Log($"{target.Actor} += {children.Length} children to {path}");
                    foreach (var ancestor in anscestors)
                    {
                        logger.Log($" - {ancestor.Identifier}");
                    }

                    var hierarchy = SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public interface Hierarchy : {{target.Actor}}.Hierarchy, {{path}}
                          {
                              {{
                                  string.Join(
                                      Environment.NewLine,
                                      children.Select(x => {
                                          var name = string.Join(
                                              string.Empty,
                                              ToNameParts(GetFriendlyName(x.Actor))
                                                  .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                          );

                                          return
                                              $$"""
                                                new {{x.Actor}}.{{path}} {{name}} { get; }
                                                Discord.ILink<{{x.Actor}}, {{x.Id}}, {{x.Entity}}, {{x.Model}}> {{target.Actor}}.Hierarchy.{{name}} => {{name}};
                                                """;
                                      })
                                  )
                              }}
                          }
                          """
                    ) as InterfaceDeclarationSyntax;

                    AddBackLink(ref hierarchy!, target, logger, false, false, transformer: backlink =>
                    {
                        return backlink
                            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                            .WithSemicolonToken(default)
                            .AddMembers(
                                children.SelectMany(x =>
                                {
                                    var name = string.Join(
                                        string.Empty,
                                        ToNameParts(GetFriendlyName(x.Actor))
                                            .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                    );

                                    return (MemberDeclarationSyntax[])
                                    [
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"new {x.Actor}.{path}.BackLink<TSource> {name} {{ get; }}"
                                        )!,
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"{x.Actor}.{path} {target.Actor}.{path}.Hierarchy.{name} => {name};"
                                        )!
                                    ];
                                }).ToArray()
                            );
                    });

                    return node
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                        .WithSemicolonToken(default)
                        .AddMembers(
                            hierarchy
                        );
                }
            );
    }

    public static void AddBackLink(
        ref InterfaceDeclarationSyntax syntax,
        GenerationTarget filler,
        Logger logger,
        bool searchChildren = true,
        bool applyBaseBackLinks = true,
        Func<InterfaceDeclarationSyntax, InterfaceDeclarationSyntax>? transformer = null)
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

        var backlink = (InterfaceDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
            $$"""
                public interface BackLink<out TSource> : 
                    {{baseIdent}}, 
                    Discord.ILinkType<{{string.Join(", ", filler.Actor, filler.Id, filler.Entity, filler.Model)}}>.BackLink<TSource>,
                    Discord.IBackLink<TSource, {{string.Join(", ", filler.Actor, filler.Id, filler.Entity, filler.Model)}}>
                    where TSource : class, IPathable;
              """
        )!;

        if (applyBaseBackLinks && syntax.BaseList?.Types.Count > 0)
        {
            backlink = backlink.AddBaseListTypes(
                syntax.BaseList.Types
                    .Where(x => x.ToString().StartsWith($"{filler.Actor}"))
                    .Select(x =>
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName($"{x.Type}.BackLink<TSource>")
                        )
                    )
                    .ToArray<BaseTypeSyntax>()
            );
        }

        if (transformer is not null)
            backlink = transformer(backlink);

        syntax = syntax
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
            .AddMembers(
                backlink
            );

        logger.Log($"{filler.Actor}: += {syntax.Identifier} backlink {baseIdent}");

        if (!searchChildren) return;

        for (var i = 0; i < typeMembers.Length; i++)
        {
            if (syntax.Members[i] is not InterfaceDeclarationSyntax iface)
                continue;

            AddBackLink(ref iface, filler, logger);

            syntax = syntax.WithMembers(syntax.Members.RemoveAt(i).Insert(i, iface));
        }
    }

    private static string[] ToNameParts(string str)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < str.Length; i++)
        {
            var ch = str[i];
            if (char.IsUpper(ch) && i > 0)
                sb.Append(' ');

            sb.Append(ch);
        }

        return sb.ToString().Split(' ');
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