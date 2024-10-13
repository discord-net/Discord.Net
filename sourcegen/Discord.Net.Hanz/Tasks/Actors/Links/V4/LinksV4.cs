using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Actors.Links.V4.Nodes;
using Discord.Net.Hanz.Tasks.Actors.V3;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.Links.V4;

using LinkTarget = LinkActorTargets.GenerationTarget;
using Schematic = LinkSchematics.Schematic;
using LinkType = LinkSchematics.Entry;

public class LinksV4
{
    public static Logger Logger =>
        Hanz.DefaultLogger.GetSubLogger(nameof(LinksV4));

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
                .Select(GetTarget!);
                //.SelectMany((x, _) => x.Nodes.Select(y => (x, y)));
                
                
        context.RegisterSourceOutput(actors, GenerateAliases!);
        context.RegisterSourceOutput(provider, GenerateSource);
        context.RegisterSourceOutput(schematics, GenerateSchematics!);
    }

    private static void GenerateSource(SourceProductionContext context, LinkGraph graph)
    {
        var logger = Logger.WithCompilationContext(graph.Compilation)
            .GetSubLogger("Build")
            .WithCleanLogFile();

        var start = DateTimeOffset.UtcNow;
        
        foreach (var entry in graph.Nodes)
        {
            var entryLogger = logger.GetSubLogger(entry.Key.ToFullMetadataName()).WithCleanLogFile();

            try
            {
                logger.Log($"Bulding {entry.Key}...");
                var ts = DateTimeOffset.UtcNow;

                var built = entry.Value.Build(graph.Context, entryLogger);
                
                var d = DateTimeOffset.UtcNow - ts;
                logger.Log($"Done in {d.TotalMilliseconds}ms");
                
                entryLogger.Log($"Generated Code:\n{built}");

                context.AddSource(
                    $"LinksV4/{entry.Key.ToFullMetadataName()}",
                    $$"""
                      {{graph.Schematic.Root.Syntax.GetFormattedUsingDirectives("Discord")}}

                      namespace {{entry.Key.ContainingNamespace}};

                      #pragma warning disable CS0108
                      #pragma warning disable CS0109
                      
                      {{entry.Value.Build(graph.Context, logger)}}
                      
                      #pragma warning restore CS0108
                      #pragma warning restore CS0109
                      """
                );
            }
            catch (Exception x)
            {
                entryLogger.Log($"Failed to generate: {x}");
                logger.Log($"Failed to generate: {x}");
            }
            
            entryLogger.Flush();
        }

        var dt = DateTimeOffset.UtcNow - start;
        
        logger.Log($"Build complete in {dt.TotalMilliseconds}ms");
        
        logger.Flush();
    }

    private static LinkGraph GetTarget(
        (Schematic Schematic, ImmutableArray<LinkTarget> Actors) targets,
        CancellationToken token)
    {
        var logger = Logger.WithCompilationContext(targets.Schematic.Compilation)
            .GetSubLogger("VisitStep")
            .WithCleanLogFile();

        var nodes = new Dictionary<INamedTypeSymbol, ActorNode>(SymbolEqualityComparer.Default);
        
        foreach (var target in targets.Actors)
        {
            if(nodes.ContainsKey(target.Actor)) continue;
            nodes[target.Actor] = new ActorNode(target, targets.Schematic);
        }
        
        var graph = new LinkGraph(nodes, targets.Schematic.Compilation, targets.Schematic);
        graph.Visit(logger);
        logger.Flush();
        return graph;
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
            "LinksV4/Aliases",
            string.Join(Environment.NewLine, aliases)
        );
    }

    private static void GenerateSchematics(
        SourceProductionContext context,
        Schematic schematic)
    {
        if (schematic.Compilation.Assembly.Name is not "Discord.Net.V4.Core") return;

        context.AddSource(
            $"LinksV4/{schematic.Root.Symbol.ToFullMetadataName()}",
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

    public static string GetFriendlyName(ITypeSymbol symbol, bool forceInterfaceRules = false)
    {
        var name = symbol.Name;

        if (forceInterfaceRules || symbol.TypeKind is TypeKind.Interface)
            name = symbol.Name.Remove(0, 1);

        return name
            .Replace("Trait", string.Empty)
            .Replace("Actor", string.Empty)
            .Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
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
}