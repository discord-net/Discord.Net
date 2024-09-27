using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Actors.V3.Types;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Enumerable = Discord.Net.Hanz.Tasks.Actors.V3.Types.Enumerable;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

using LinkTarget = LinkActorTargets.GenerationTarget;
using Schematic = LinkSchematics.Schematic;
using LinkType = LinkSchematics.Entry;

public sealed class LinksV3
{
    private static readonly Dictionary<string, ILinkTypeProcessor> Processors = new()
    {
        {nameof(Indexable), new Indexable()},
        {nameof(Enumerable), new Enumerable()},
        {nameof(Defined), new Defined()},
        {nameof(Paged), new Paged()}
    };


    private static Logger Logger =>
        Hanz.DefaultLogger.GetSubLogger(nameof(LinksV3));

    public sealed class Target(
        LinkTarget linkTarget,
        Schematic schematic,
        List<Target> ancestors
    ) :
        IEquatable<Target>
    {
        public LinkTarget LinkTarget { get; } = linkTarget;
        public Schematic Schematic { get; } = schematic;
        public List<Target> Ancestors { get; } = ancestors;
        public List<LinkExtensions.Extension>? Extensions { get; set; }
        public Target[]? Hierarchy { get; set; }

        public string FormattedCoreLinkType
            => $"Discord.ILinkType<{LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";

        public string FormattedCoreLink
            => $"Discord.ILink<{LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";

        public bool Equals(Target other)
        {
            return LinkTarget.Equals(other.LinkTarget) &&
                   Schematic.Equals(other.Schematic) &&
                   Ancestors.SequenceEqual(other.Ancestors);
        }

        public void Log(Logger logger)
        {
            logger.Log($"Target: {LinkTarget.Actor} | {LinkTarget.Id} | {LinkTarget.Entity} | {LinkTarget.Entity}");
            
            logger.Log($" - Ancestors: {Ancestors.Count}");

            foreach (var ancestor in Ancestors)
            {
                logger.Log($"    - {ancestor.LinkTarget.Actor}");
            }
            
            logger.Log($" - Extensions: {Extensions?.Count ?? 0}");

            if (Extensions is not null)
            {
                foreach (var extension in Extensions)
                {
                    logger.Log($"    - {extension.Name}");
                }
            }
            
            logger.Log($" - Hierarchy: {Hierarchy?.Length ?? 0}");

            if (Hierarchy is not null)
            {
                foreach (var type in Hierarchy)
                {
                    logger.Log($"    - {type.LinkTarget.Actor}");
                }
            }
        }
    }

    public sealed class GenerativeTarget(Target target, TypeDeclarationSyntax syntax) :
        IEquatable<GenerativeTarget>
    {
        public Target Target { get; } = target;
        public TypeDeclarationSyntax Syntax { get; } = syntax;

        public bool Equals(GenerativeTarget other)
        {
            return Target.Equals(other.Target);
        }
    }

    public static void Register(IncrementalGeneratorInitializationContext context)
    {
        var schematics = context.SyntaxProvider
            .CreateSyntaxProvider(
                LinkSchematics.IsPotentialSchematic,
                LinkSchematics.MapSchematic
            )
            .Where(x => x is not null);

        var actors = context.SyntaxProvider.CreateSyntaxProvider(
                LinkActorTargets.IsValid,
                LinkActorTargets.GetTargetForGeneration
            )
            .Where(x => x is not null)
            .Collect();

        var provider =
            schematics
                .Combine(actors)
                .SelectMany(GetTarget!)
                .Select(GenerateLinks);

        context.RegisterSourceOutput(actors, GenerateAliases!);
        context.RegisterSourceOutput(provider, GenerateSource);
        context.RegisterSourceOutput(schematics, GenerateSchematics!);
    }

    private static void GenerateAliases(SourceProductionContext context, ImmutableArray<LinkTarget> targets)
    {
        var aliases = targets
            .Select(x =>
                $"""
                 global using {GetFriendlyName(x.Actor)}Link = Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 global using {GetFriendlyName(x.Actor)}LinkType = Discord.ILinkType<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 """
            )
            .ToArray();

        context.AddSource(
            "LinksV3/Aliases",
            string.Join(Environment.NewLine, aliases)
        );
    }

    private static void GenerateSchematics(
        SourceProductionContext context,
        Schematic schematic)
    {
        context.AddSource(
            $"LinksV3/{schematic.Root.Symbol.ToFullMetadataName()}",
            $$"""
              {{schematic.Root.Syntax.GetFormattedUsingDirectives()}}

              namespace {{schematic.Root.Symbol.ContainingNamespace}};

              {{Generate(schematic.Root, ImmutableList<LinkType>.Empty).NormalizeWhitespace()}}
              """
        );

        return;

        TypeDeclarationSyntax Generate(LinkType type, ImmutableList<LinkType> path)
        {
            var clone = (TypeDeclarationSyntax) SyntaxUtils.CreateSourceGenClone(type.Syntax)
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                .WithSemicolonToken(default)
                .WithBaseList(
                    path.Count > 0
                        ? SyntaxFactory.BaseList(
                            SyntaxFactory.SeparatedList(
                                path
                                    .SelectMany(IEnumerable<BaseTypeSyntax> (x, i) =>
                                    [
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{schematic.Root.Symbol}.{FormatTypeName(x.Symbol)}"
                                            )
                                        ),
                                    ])
                                    .Append(
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{schematic.Root.Symbol}.{FormatTypeName(type.Symbol)}"
                                            )
                                        )
                                    )
                            )
                        )
                        : null
                );

            return clone.WithMembers(
                SyntaxFactory.List(
                    type.Children
                        .Select(x =>
                            Generate(
                                x,
                                type.Symbol.Name == "ILinkType" ? path : path.Add(type)
                            )
                        )
                        .ToArray<MemberDeclarationSyntax>()
                )
            );
        }
    }

    private static void GenerateSource(SourceProductionContext context, GenerativeTarget target)
    {
        context.AddSource(
            $"LinksV3/{target.Target.LinkTarget.Actor.ToFullMetadataName()}",
            $"""
             {
                 string.Join(
                     Environment.NewLine,
                     target.Target.LinkTarget.Syntax
                         .GetUsingDirectives()
                         .Concat(target.Target.Schematic.Root.Syntax.GetUsingDirectives())
                         .Distinct()
                 )
             }

             namespace {target.Target.LinkTarget.Actor.ContainingNamespace};

             {target.Syntax.NormalizeWhitespace()}
             """
        );
    }

    private static GenerativeTarget GenerateLinks(Target target, CancellationToken token)
        => new(
            target,
            (TypeDeclarationSyntax) SyntaxFactory.ParseMemberDeclaration(
                Run(target)
            )!
        );

    private static IEnumerable<Target> GetTarget(
        (Schematic Schematic, ImmutableArray<LinkTarget> Actors) target,
        CancellationToken token)
    {
        var results = new Dictionary<INamedTypeSymbol, Target>(SymbolEqualityComparer.Default);

        foreach (var actor in target.Actors)
        {
            yield return CreateTarget(actor);
        }

        yield break;

        Target CreateTarget(LinkTarget link)
        {
            if (results.TryGetValue(link.Actor, out var value))
                return value;

            var ancestors = target.Actors
                .Where(x => Hierarchy.Implements(link.Actor, x.Actor))
                .Select(x =>
                    results.TryGetValue(x.Actor, out var existing)
                        ? existing
                        : results[x.Actor] = CreateTarget(x)
                )
                .ToList();

            var result = results[link.Actor] = new(
                link,
                target.Schematic,
                ancestors
            );

            var logger = Logger.WithSemanticContext(link.SemanticModel);

            result.Extensions = LinkExtensions.GetExtensions(result, symbol =>
                    results.TryGetValue(symbol, out var existing)
                        ? existing
                        : target.Actors.FirstOrDefault(x =>
                                x.GetCoreActor().Equals(symbol, SymbolEqualityComparer.Default))
                            is { } actor
                            ? CreateTarget(actor)
                            : null,
                logger
            );

            result.Hierarchy = LinkHierarchy
                .GetHierarchy(result, target.Actors, logger)
                ?.Select(CreateTarget)
                .ToArray();

            return result;
        }
    }

    private static string Run(Target target)
    {
        var logger = Logger.WithSemanticContext(target.Schematic.Semantic);

        var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core ? "interface" : "class";

        target.Log(logger);

        var linkMembers = string.Join(
            Environment.NewLine,
            target.Schematic.Root.Children
                .Select(x =>
                    BuildLinkType(
                        x,
                        ImmutableList<LinkType>.Empty,
                        target,
                        logger
                    )
                )
        ).WithNewlinePadding(4);

        return
            $$"""
              public partial {{kind}} {{target.LinkTarget.Actor.Name}}
              {
                  {{
                      string
                          .Join(
                              Environment.NewLine,
                              Processors.Values
                                  .Select(x => x.CreateProvider(target, logger))
                                  .Where(x => x is not null)
                          )
                          .WithNewlinePadding(4)
                  }}
                  {{linkMembers}}
                  {{
                      LinkExtensions
                          .FormatExtensions(ImmutableList<LinkType>.Empty, target, logger)
                          .WithNewlinePadding(4)
                  }}
                  {{
                      BuildBackLink(target, ImmutableList<string>.Empty)
                          .WithNewlinePadding(4)
                  }}
                  {{LinkHierarchy.BuildHierarchy(target, ImmutableList<string>.Empty)}}
              }
              """;
    }

    private static string BuildLinkType(
        LinkType type,
        ImmutableList<LinkType> path,
        Target target,
        Logger logger)
    {
        logger.Log($" - {type.Symbol}");

        var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core ? "interface" : "class";

        var childrenLinks = string.Join(
            Environment.NewLine,
            type.Children.Select(v => BuildLinkType(v, path.Add(type), target, logger))
        ).WithNewlinePadding(4);

        var bases = new List<string>
        {
            $"{target.FormattedCoreLinkType}{FormatPath(path.Add(type))}"
        };

        foreach (var ancestor in target.Ancestors)
        {
            bases.Add($"{ancestor.LinkTarget.Actor}{FormatPath(path.Add(type))}");
            logger.Log($"{type.Symbol} += {ancestor.LinkTarget.Actor} -> {type.Symbol}");
        }

        var members = new List<string>();

        if (target.Ancestors.Count > 0)
        {
            // we need to redefine the link members
            if (Processors.TryGetValue(type.Symbol.Name, out var processor))
                processor.AddOverrideMembers(members, target, type, path);

            foreach (var ancestor in target.Ancestors)
            {
                members.AddRange([
                    $"{ancestor.LinkTarget.Actor} Discord.IActorProvider<{ancestor.LinkTarget.Actor}, {ancestor.LinkTarget.Id}>.GetActor({ancestor.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
                ]);
            }
        }

        return
            $$"""
              public {{kind}} {{FormatTypeName(type.Symbol)}}{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}    {string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}"
                      : string.Empty
              )}}{{
              (
                  type.Syntax.ConstraintClauses.Count > 0
                      ? $"{Environment.NewLine}{string.Join(Environment.NewLine, type.Syntax.ConstraintClauses)}"
                      : string.Empty
              )}}
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
                  {{childrenLinks}}
                  {{
                      LinkExtensions
                          .FormatExtensions(path.Add(type), target, logger)
                          .WithNewlinePadding(4)
                  }}
                  {{BuildBackLink(target, type, path).WithNewlinePadding(4)}}
                  {{
                      LinkHierarchy
                          .BuildHierarchy(
                              target,
                              path.Add(type)
                                  .Select(x =>
                                      FormatTypeName(x.Symbol)
                                  )
                                  .ToImmutableList()
                          )
                          .WithNewlinePadding(4)
                  }}
              }
              """;
    }

    public static string BuildBackLink(
        Target target,
        ImmutableList<string> path,
        ImmutableList<string>? ancestorPath = null,
        string? extraMembers = null)
    {
        ancestorPath ??= path;

        var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core ? "interface" : "class";

        var backlinkBaseType =
            $"Discord.IBackLink<TSource, {target.LinkTarget.Actor}, {target.LinkTarget.Id}, {target.LinkTarget.Entity}, {target.LinkTarget.Model}>";

        var bases = new List<string>()
        {
            $"{target.LinkTarget.Actor}{FormatPath(path)}",
            backlinkBaseType
        };

        foreach (var ancestor in target.Ancestors)
        {
            bases.Add($"{ancestor.LinkTarget.Actor}{FormatPath(path)}.BackLink<TSource>");
        }

        var members = new List<string>();

        if (target.Ancestors.Count > 0)
        {
            members.AddRange([
                "new TSource Source { get; }",
                $"TSource {backlinkBaseType}.Source => Source;"
            ]);

            foreach (var ancestor in target.Ancestors)
            {
                members.Add(
                    ancestor.Ancestors.Count > 0
                        ? $"TSource {ancestor.LinkTarget.Actor}{FormatPath(ancestorPath)}.BackLink<TSource>.Source => Source;"
                        : $"TSource Discord.IBackLink<TSource, {ancestor.LinkTarget.Actor}, {ancestor.LinkTarget.Id}, {ancestor.LinkTarget.Entity}, {ancestor.LinkTarget.Model}>.Source => Source;"
                );
            }
        }

        return
            $$"""
              public {{kind}} BackLink<{{(kind is "interface" ? "out " : string.Empty)}}TSource> :
                  {{string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}}
                  where TSource : class, IPathable
              {
                  {{string.Join(Environment.NewLine, members).WithNewlinePadding(4)}}
                  {{extraMembers?.WithNewlinePadding(4)}}
              }
              """;
    }

    public static string BuildBackLink(
        Target target,
        LinkType type,
        ImmutableList<LinkType> path,
        string? extraMembers = null
    ) => BuildBackLink(
        target,
        path.Add(type).Select(x => FormatTypeName(x.Symbol)).ToImmutableList(),
        null,
        extraMembers
    );

    public static string FormatTypeName(INamedTypeSymbol symbol)
    {
        var sb = new StringBuilder(symbol.Name);

        if (symbol.TypeParameters.Length > 0)
        {
            sb.Append('<')
                .Append(string.Join(", ", symbol.TypeParameters.Select(x => x.ToDisplayString())))
                .Append('>');
        }

        return sb.ToString();
    }

    public static string FormatPath(ImmutableList<LinkType> path)
    {
        if (path.Count == 0) return string.Empty;

        return $".{string.Join(".", path.Select(x => FormatTypeName(x.Symbol)))}";
    }

    public static string FormatPath(ImmutableList<string> path)
    {
        if (path.Count == 0) return string.Empty;

        return $".{string.Join(".", path)}";
    }

    public static string GetFriendlyName(ITypeSymbol symbol)
    {
        if (symbol.TypeKind is TypeKind.Interface)
            return symbol.Name.Remove(0, 1).Replace("Actor", string.Empty);

        return symbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }
}