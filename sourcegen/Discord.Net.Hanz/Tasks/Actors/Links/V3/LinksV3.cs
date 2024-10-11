using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Actors.V3.Impl;
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
    public static readonly Dictionary<string, ILinkTypeProcessor> Processors = new()
    {
        {nameof(Indexable), new Indexable()},
        {nameof(Enumerable), new Enumerable()},
        {nameof(Defined), new Defined()},
        {nameof(Paged), new Paged()}
    };

    public static readonly Dictionary<LinkActorTargets.AssemblyTarget, ILinkImplementer> Implementers = new()
    {
        {LinkActorTargets.AssemblyTarget.Rest, new RestLinkImplementer()}
    };


    public static Logger Logger =>
        Hanz.DefaultLogger.GetSubLogger(nameof(LinksV3));

    public sealed class Target(
        LinkTarget linkTarget,
        Schematic schematic,
        List<Target> ancestors,
        List<Target> children
    ) :
        IEquatable<Target>
    {
        public LinkTarget LinkTarget { get; } = linkTarget;
        public Schematic Schematic { get; } = schematic;
        public List<Target> Ancestors { get; } = ancestors;
        public List<Target> Children { get; } = children;
        public List<LinkExtensions.Extension>? Extensions { get; set; }
        public Target[]? Hierarchy { get; set; }

        public List<Target> EntityAssignableAncestors => Ancestors
            .Where(ancestor =>
                ancestor.LinkTarget.Entity.Equals(LinkTarget.Entity, SymbolEqualityComparer.Default) ||
                Net.Hanz.Hierarchy.Implements(LinkTarget.Entity, ancestor.LinkTarget.Entity)
            ).ToList();

        public Target? BaseTarget
            => LinkTarget.Actor.TypeKind is TypeKind.Class && LinkTarget.Actor.BaseType is not null
                ? Ancestors.FirstOrDefault(x =>
                    x.LinkTarget.Actor.Equals(LinkTarget.Actor.BaseType, SymbolEqualityComparer.Default)
                )
                : null;

        public string FormattedBackLinkType
            => $"Discord.IBackLink<TSource, {LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";
        public string FormattedCoreBackLinkType
            => $"Discord.IBackLink<TSource, {LinkTarget.GetCoreActor()}, {LinkTarget.Id}, {LinkTarget.GetCoreEntity()}, {LinkTarget.Model}>";
        
        public string FormattedLinkType
            => $"Discord.ILinkType<{LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";

        public string FormattedCoreLinkType
            =>
                $"Discord.ILinkType<{LinkTarget.GetCoreActor()}, {LinkTarget.Id}, {LinkTarget.GetCoreEntity()}, {LinkTarget.Model}>";

        public string FormattedCoreLink
            => $"Discord.ILink<{LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";

        public string FormattedRestLinkType =>
            $"Discord.Rest.IRestLinkType<{LinkTarget.Actor}, {LinkTarget.Id}, {LinkTarget.Entity}, {LinkTarget.Model}>";

        public string FormattedActorProvider
            => $"Discord.IActorProvider<{LinkTarget.Actor}, {LinkTarget.Id}>";

        public string FormattedCoreActorProvider
            => $"Discord.IActorProvider<{LinkTarget.GetCoreActor()}, {LinkTarget.Id}>";

        public string FormattedRestActorProvider
            => $"Discord.Rest.RestActorProvider<{LinkTarget.Actor}, {LinkTarget.Id}>";

        public string FormattedEntityProvider
            => $"Discord.IEntityProvider<{LinkTarget.Entity}, {LinkTarget.Model}>";

        public string FormattedCoreEntityProvider
            => $"Discord.IEntityProvider<{LinkTarget.GetCoreEntity()}, {LinkTarget.Model}>";

        public bool Equals(Target other)
        {
            return LinkTarget.Equals(other.LinkTarget) &&
                   Schematic.Equals(other.Schematic) &&
                   Ancestors.SequenceEqual(other.Ancestors);
        }

        public void Log(Logger logger)
        {
            logger.Log($"Target: {LinkTarget.Actor} | {LinkTarget.Id} | {LinkTarget.Entity} | {LinkTarget.Entity}");

            logger.Log($" - Base Target?: {BaseTarget?.LinkTarget.Actor.ToDisplayString() ?? "none"}");

            logger.Log($" - Ancestors: {Ancestors.Count}");

            foreach (var ancestor in Ancestors)
            {
                logger.Log($"    - {ancestor.LinkTarget.Actor}");
            }

            logger.Log($" - Entity Assignable Ancestors: {EntityAssignableAncestors.Count}");

            foreach (var ancestor in EntityAssignableAncestors)
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
            
            logger.Log($" - Interfaces: {LinkTarget.Actor.AllInterfaces.Length}");

            foreach (var iface in LinkTarget.Actor.AllInterfaces)
            {
                logger.Log($"   - {iface}");
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
            .Combine(context.CompilationProvider.Select(LinkSchematics.MapNonCoreSchematic))
            .Select((pair, _) => pair.Left ?? pair.Right)
            .Where(x => x is not null)
            .Collect()
            .SelectMany((x, _) =>
            {
                var set = new HashSet<string>();
                var result = new List<Schematic>();

                foreach (var schematic in x)
                {
                    if (schematic is null) continue;

                    if (set.Add(schematic.Root.Symbol.ToDisplayString()))
                        result.Add(schematic);
                }

                return result;
            });

        var actors = context.SyntaxProvider.CreateSyntaxProvider(
                LinkActorTargets.IsValid,
                LinkActorTargets.GetTargetForGeneration
            )
            .Where(x => x is not null)
            .Collect();

        var provider =
            schematics
                .Combine(actors)
                .SelectMany(GetTarget!);

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
        if (schematic.Compilation.Assembly.Name is not "Discord.Net.V4.Core") return;

        context.AddSource(
            $"LinksV3/{schematic.Root.Symbol.ToFullMetadataName()}",
            $$"""
              {{schematic.Root.Syntax.GetFormattedUsingDirectives()}}

              namespace {{schematic.Root.Symbol.ContainingNamespace}};

              #pragma warning disable CS0108
              #pragma warning disable CS0109
              {{Generate(schematic.Root, ImmutableList<LinkType>.Empty).NormalizeWhitespace()}}
              #pragma warning restore CS0108
              #pragma warning restore CS0109
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

    private static void GenerateSource(SourceProductionContext context, Target target)
    {
        var logger = Logger
            .WithCompilationContext(target.Schematic.Compilation)
            .GetSubLogger("Sources");

        logger.Log($"Generating {target.LinkTarget.Actor}...");

        context.AddSource(
            $"LinksV3/{target.LinkTarget.Actor.ToFullMetadataName()}",
            $"""
             {
                 string.Join(
                     Environment.NewLine,
                     target.LinkTarget.Syntax
                         .GetUsingDirectives()
                         .Concat(target.Schematic.Root.Syntax.GetUsingDirectives())
                         .Distinct()
                 )
             }

             namespace {target.LinkTarget.Actor.ContainingNamespace};

             #pragma warning disable CS0108
             #pragma warning disable CS0109
             {Run(target)}
             #pragma warning restore CS0108
             #pragma warning restore CS0109
             """
        );

        logger.Flush();
    }

    private static IEnumerable<Target> GetTarget(
        (Schematic Schematic, ImmutableArray<LinkTarget> Actors) target,
        CancellationToken token)
    {
        var logger = Logger.WithCompilationContext(target.Schematic.Compilation)
            .GetSubLogger("Targets")
            .WithCleanLogFile();

        Logger
            .WithCompilationContext(target.Schematic.Compilation)
            .GetSubLogger("Sources")
            .DeleteLogFile(true);

        var results = new Dictionary<INamedTypeSymbol, Target>(SymbolEqualityComparer.Default);

        try
        {
            logger.Log($"Processing {target.Actors.Length} actors...");

            foreach (var actor in target.Actors)
            {
                yield return CreateTarget(actor);
            }
        }
        finally
        {
            logger.Flush();
        }

        yield break;

        Target CreateTarget(LinkTarget link)
        {
            if (results.TryGetValue(link.Actor, out var value))
                return value;

            var result = results[link.Actor] = new(
                link,
                target.Schematic,
                [],
                []
            );

            result.Ancestors.AddRange(
                target.Actors
                    .Where(x => Hierarchy.Implements(link.Actor, x.Actor))
                    .Select(x =>
                        results.TryGetValue(x.Actor, out var existing)
                            ? existing
                            : results[x.Actor] = CreateTarget(x)
                    )
            );

            result.Children.AddRange(
                target.Actors
                    .Where(x =>
                        Hierarchy.Implements(x.Actor, link.Actor)
                        &&
                        !x.Actor.Equals(link.Actor, SymbolEqualityComparer.Default)
                    )
                    .Select(x =>
                        results.TryGetValue(x.Actor, out var existing)
                            ? existing
                            : results[x.Actor] = CreateTarget(x)
                    )
            );

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
        var logger = Logger.WithCompilationContext(target.Schematic.Compilation)
            .GetSubLogger("Generation")
            .GetSubLogger($"{target.LinkTarget.Actor.Name}")
            .WithCleanLogFile();

        try
        {
            var kind = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core
                ? "interface"
                : "class";

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

            var rootBackLink = Implementers.TryGetValue(target.LinkTarget.Assembly, out var implementer)
                ? implementer.CreateRootBackLink(target, logger)
                : BuildBackLink(target, ImmutableList<string>.Empty);
            
            return
                $$"""
                  public partial {{kind}} {{target.LinkTarget.Actor.Name}}
                  {
                      {{
                          LinkRelationships
                              .GenerateRelationship(target)
                              .WithNewlinePadding(4)
                      }}
                      {{linkMembers}}
                      {{
                          LinkExtensions
                              .FormatExtensions(ImmutableList<LinkType>.Empty, target, logger)
                              .WithNewlinePadding(4)
                      }}
                      {{
                          rootBackLink.WithNewlinePadding(4)
                      }}
                      {{LinkHierarchy.BuildHierarchy(target, ImmutableList<string>.Empty)}}
                  }
                  """;
        }
        catch (Exception x)
        {
            logger.Warn($"Failed to generate targets: {x}");
            return string.Empty;
        }
        finally
        {
            logger.Flush();
        }
    }

    private static string BuildLinkType(
        LinkType type,
        ImmutableList<LinkType> path,
        Target target,
        Logger logger)
    {
        logger.Log($" - {type.Symbol}");

        var isCore = target.LinkTarget.Assembly is LinkActorTargets.AssemblyTarget.Core;

        var childrenLinks = string.Join(
            Environment.NewLine,
            type.Children.Select(v => BuildLinkType(v, path.Add(type), target, logger))
        ).WithNewlinePadding(4);

        var bases = new List<string>();
        var members = new List<string>();
        ConstructorRequirements? constructorRequirements = null;
        
        if (isCore)
        {
            bases.Add($"{target.FormattedLinkType}{FormatPath(path.Add(type))}");

            if (path.Count > 0)
                bases.Add($"{target.LinkTarget.Actor}{FormatPath(path)}");

            foreach (var ancestor in target.EntityAssignableAncestors)
            {
                bases.Add($"{ancestor.LinkTarget.Actor}{FormatPath(path.Add(type))}");
                logger.Log($"{type.Symbol} += {ancestor.LinkTarget.Actor} -> {type.Symbol}");
            }

            if (path.Count > 0)
            {
                // add the root implementation to ours
                bases.Add($"{target.LinkTarget.Actor}.{FormatTypeName(type.Symbol)}");
            }

            if (Processors.TryGetValue(type.Symbol.Name, out var processor))
                processor.AddOverrideMembers(members, target, type, path);
            
            if (target.EntityAssignableAncestors.Count > 0)
            {
                foreach (var ancestor in target.EntityAssignableAncestors)
                {
                    members.AddRange([
                        $"{ancestor.LinkTarget.Actor} Discord.IActorProvider<{ancestor.LinkTarget.Actor}, {ancestor.LinkTarget.Id}>.GetActor({ancestor.LinkTarget.Id} id) => (this as IActorProvider<{target.LinkTarget.Actor}, {target.LinkTarget.Id}>).GetActor(id);"
                    ]);
                }
            }
        }
        else
        {
            var implementationLinkType = target.LinkTarget.Assembly switch
            {
                LinkActorTargets.AssemblyTarget.Rest => target.FormattedRestLinkType,
                _ => null
            };
            
            if (implementationLinkType is null) return string.Empty;
            
            bases.AddRange([
                $"{target.LinkTarget.GetCoreActor()}{FormatPath(path.Add(type))}",
                implementationLinkType
            ]);
            
            if (target.BaseTarget is not null)
            {
                bases.Insert(0, $"{target.BaseTarget.LinkTarget.Actor}{FormatPath(path.Add(type))}");
            }

            if (Implementers.TryGetValue(target.LinkTarget.Assembly, out var implementer))
            {
                constructorRequirements = implementer
                    .ImplementLink(members, target, type, path, logger);
            }

            if (path.Count > 0)
            {
                foreach (var entry in path.Add(type))
                {
                    bases.Add($"{target.LinkTarget.Actor}.{FormatTypeName(entry.Symbol)}");
                }
            }
        }

        return
            $$"""
              public interface {{FormatTypeName(type.Symbol)}}{{(
                  bases.Count > 0
                      ? $" :{Environment.NewLine}    {string.Join($",{Environment.NewLine}", bases).WithNewlinePadding(4)}"
                      : string.Empty
              )}}{{
              (
                  type.Syntax.ConstraintClauses.Count > 0
                      ? $"{Environment.NewLine}    {string.Join(Environment.NewLine, type.Syntax.ConstraintClauses).WithNewlinePadding(4)}"
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
                  {{BuildBackLink(target, type, path, logger, constructorRequirements: constructorRequirements).WithNewlinePadding(4)}}
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
        var usedAncestorPath = ancestorPath ?? path;

        var backlinkBaseType =
            $"Discord.IBackLink<TSource, {target.LinkTarget.Actor}, {target.LinkTarget.Id}, {target.LinkTarget.Entity}, {target.LinkTarget.Model}>";

        var bases = new List<string>()
        {
            $"{target.LinkTarget.Actor}{FormatPath(path)}",
            backlinkBaseType
        };

        foreach (var ancestor in target.EntityAssignableAncestors)
        {
            bases.Add($"{ancestor.LinkTarget.Actor}{FormatClosestPath(ancestor, path)}.BackLink<TSource>");
        }

        var members = new List<string>();

        var willHaveRedefinedSource = 
            target.EntityAssignableAncestors.Count > 0 || 
            target.LinkTarget.Assembly is not LinkActorTargets.AssemblyTarget.Core;

        if (willHaveRedefinedSource)
        {
            members.AddRange([
                "new TSource Source { get; }",
                $"TSource {backlinkBaseType}.Source => Source;"
            ]);
        }

        if (target.LinkTarget.Assembly is not LinkActorTargets.AssemblyTarget.Core)
        {
            var coreBase = $"{target.LinkTarget.GetCoreActor()}{FormatPath(path)}.BackLink<TSource>";
            bases.Add(coreBase);
            
            members.Add(
                target.EntityAssignableAncestors.Count > 0
                    ? $"TSource {coreBase}.Source => Source;"
                    : $"TSource Discord.IBackLink<TSource, {target.LinkTarget.GetCoreActor()}, {target.LinkTarget.Id}, {target.LinkTarget.GetCoreEntity()}, {target.LinkTarget.Model}>.Source => Source;"
            );
        }

        if (target.EntityAssignableAncestors.Count > 0)
        {
            foreach (var ancestor in target.EntityAssignableAncestors)
            {
                members.Add(
                    ancestor.EntityAssignableAncestors.Count > 0
                        ? $"TSource {ancestor.LinkTarget.Actor}{FormatPath(usedAncestorPath)}.BackLink<TSource>.Source => Source;"
                        : $"TSource Discord.IBackLink<TSource, {ancestor.LinkTarget.Actor}, {ancestor.LinkTarget.Id}, {ancestor.LinkTarget.Entity}, {ancestor.LinkTarget.Model}>.Source => Source;"
                );
            }
        }

        if (path.Count > 1)
        {
            foreach (var part in path)
            {
                bases.Add($"{target.LinkTarget.Actor}.{part}.BackLink<TSource>");

                if (willHaveRedefinedSource)
                    members.Add($"TSource {target.LinkTarget.Actor}.{part}.BackLink<TSource>.Source => Source;");
            }
        }
        
        return
            $$"""
              public interface BackLink<out TSource> :
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
        Logger logger,
        string? extraMembers = null,
        ConstructorRequirements? constructorRequirements = null)
    {
        if (Implementers.TryGetValue(target.LinkTarget.Assembly, out var implementer))
        {
            var members = new List<string>();
            
            implementer.ImplementBackLink(
                constructorRequirements,
                members,
                target,
                type,
                path,
                logger
            );

            extraMembers = extraMembers is null 
                ? string.Join(Environment.NewLine, members) 
                : $"{extraMembers}{Environment.NewLine}{string.Join(Environment.NewLine, members)}";
        }
        
        return BuildBackLink(
            target,
            path.Add(type).Select(x => FormatTypeName(x.Symbol)).ToImmutableList(),
            null,
            extraMembers
        );
    }

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

    public static string FormatClosestPath(
        Target target,
        ImmutableList<string> path,
        bool includeExtension = true,
        bool includeSchematic = true,
        bool includeHierarchy = true)
    {
        if (path.Count == 0) return string.Empty;

        var final = ImmutableList<string>.Empty;

        var node = target.Schematic.Root;

        foreach (var part in path)
        {
            var isExtensionPart = target.Extensions?.Any(v => v.Name == part) ?? false;
            var isSchematicPart = node.Children.Any(x => FormatTypeName(x.Symbol) == part);
            var isHierarchyPart = target.Hierarchy?.Any(x => GetFriendlyName(x.LinkTarget.Actor) == part) ?? false;

            if (isExtensionPart && includeExtension)
                final = final.Add(part);
            else if (isSchematicPart && includeSchematic)
                final = final.Add(part);
            else if (isHierarchyPart && includeHierarchy)
                final = final.Add(part);
        }

        return FormatPath(final);
    }

    public static string GetFriendlyName(ITypeSymbol symbol)
    {
        var name = symbol.Name;

        if (symbol.TypeKind is TypeKind.Interface)
            name = symbol.Name.Remove(0, 1);

        return name
            .Replace("Trait", string.Empty)
            .Replace("Actor", string.Empty)
            .Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }
}