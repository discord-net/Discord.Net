using System.Collections.Immutable;
using System.Reflection;
using Discord.Net.Hanz.Tasks.Actors.Links.V4;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

public class LinkSchematics : GenerationTask
{
    public record Schematic
    {
        public Entry Root { get; }

        public Schematic(Entry root)
        {
            Root = root;
        }
    }

    public record Entry(
        TypeRef type,
        HashSet<Entry> children
    )
    {
        public override int GetHashCode()
            => HashCode
                .Of(Type)
                .AndEach(Children);

        public HashSet<Entry> Children { get; } = children;

        public TypeRef Type { get; } = type;
    }

    public IncrementalValuesProvider<Schematic> SourceSchematics { get; }
    public IncrementalValueProvider<Schematic?> NonCoreSchematics { get; }
    public IncrementalValuesProvider<Schematic> Schematics { get; }

    private readonly Logger _logger;
    
    public LinkSchematics(IncrementalGeneratorInitializationContext context, Logger logger) : base(context, logger)
    {
        _logger = logger;
        
        SourceSchematics = context.SyntaxProvider
            .CreateSyntaxProvider(
                IsPotentialSchematic,
                MapSchematic
            )
            .Where(x => x is not null)!;

        NonCoreSchematics = context
            .CompilationProvider
            .Select(MapNonCoreSchematic);

        Schematics = SourceSchematics
            .Collect()
            .Combine(NonCoreSchematics)
            .SelectMany(IEnumerable<Schematic> (x, _) =>
            {
                logger.Log($"Schematic: {string.Join(" | ", x.Left)} <> {x.Right}");
                
                logger.Flush();
                
                return x.Left.Length > 0
                    ? x.Left
                    : x.Right is not null
                        ? new[] {x.Right}.ToImmutableArray()
                        : ImmutableArray<Schematic>.Empty;
            });
    }


    public static bool IsPotentialSchematic(SyntaxNode node, CancellationToken token)
        => node is TypeDeclarationSyntax {AttributeLists.Count: > 0} && node.Parent is not TypeDeclarationSyntax;

    public Schematic? MapSchematic(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not TypeDeclarationSyntax syntax) return null;

        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol symbol) return null;

        var logger = _logger
            .WithSemanticContext(context.SemanticModel)
            .WithCleanLogFile();

        logger.Log($"Processing {symbol}...");

        try
        {
            var lookup = new Dictionary<string, Entry>();

            var root = GetEntry(symbol, symbol, lookup, token, logger);

            if (root is null) return null;

            return new Schematic(root);
        }
        finally
        {
            logger.Flush();
        }
    }

    public Schematic? MapNonCoreSchematic(Compilation compilation, CancellationToken token)
    {
        if (LinkActorTargets.GetAssemblyTarget(compilation) is LinkActorTargets.AssemblyTarget.Core)
            return null;

        var logger = _logger
            .WithCompilationContext(compilation)
            .WithCleanLogFile();

        try
        {
            var symbol = compilation.GetTypeByMetadataName("Discord.ILinkType`4");

            if (symbol is null)
            {
                logger.Log("Failed to find ILinkType`4");
                return null;
            }

            return MapSchematic(symbol, compilation, token, logger);
        }
        catch (Exception x)
        {
            logger.Warn($"Failed to map non-core schematic: {x}");
            return null;
        }
        finally
        {
            logger.Flush();
        }
    }

    public Schematic? MapSchematic(
        INamedTypeSymbol symbol,
        Compilation compilation,
        CancellationToken token,
        Logger? logger = null)
    {
        var lookup = new Dictionary<string, Entry>();

        logger ??= _logger
            .WithCompilationContext(compilation)
            .WithCleanLogFile();

        try
        {
            var root = GetEntry(symbol, symbol, lookup, token, logger);

            if (root is null) return null;

            return new Schematic(root);
        }
        finally
        {
            logger.Flush();
        }
    }

    private static Entry? GetEntry(
        INamedTypeSymbol root,
        INamedTypeSymbol symbol,
        Dictionary<string, Entry> lookup,
        CancellationToken token,
        Logger? logger = null)
    {
        var attribute = symbol.GetAttributes()
            .FirstOrDefault(IsLinkSchematicAttribute);

        if (attribute is null) return null;

        var children = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Children")
            .Value;

        logger?.Log($"{symbol}: Children?: {children.Kind}");

        HashSet<Entry> childrenEntries = new();

        if (children.Kind is TypedConstantKind.Array)
        {
            foreach (var child in children.Values.Select(x => x.Value as string).OfType<string>())
            {
                if (child == symbol.Name) continue;

                if (!lookup.TryGetValue(child, out var entry))
                {
                    var childNode = root.GetTypeMembers().FirstOrDefault(x => x.Name == child);

                    if (childNode is null) continue;

                    entry = GetEntry(root, childNode, lookup, token, logger);

                    if (entry is null) continue;
                }

                childrenEntries.Add(entry);
            }
        }
        else if (root.Equals(symbol, SymbolEqualityComparer.Default))
        {
            foreach (var child in root.GetTypeMembers().Select(x => GetEntry(root, x, lookup, token, logger)))
            {
                if (child is null) continue;

                childrenEntries.Add(child);
            }
        }

        return new(new(symbol), childrenEntries);
    }

    private static bool IsLinkSchematicAttribute(AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString() == "Discord.LinkSchematicAttribute";
}