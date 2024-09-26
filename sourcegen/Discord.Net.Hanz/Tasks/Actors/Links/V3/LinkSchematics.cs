using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors.V3;

public static class LinkSchematics
{
    public class Schematic : IEquatable<Schematic>
    {
        public Entry Root { get; }
        public SemanticModel Semantic { get; }

        public Schematic(Entry root, SemanticModel semantic)
        {
            Root = root;
            Semantic = semantic;
        }

        public bool Equals(Schematic other)
            => Root.Equals(other.Root);
    }

    public class Entry(
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        HashSet<Entry> children,
        AttributeData attribute
    ) : IEquatable<Entry>
    {
        public HashSet<Entry> Children { get; } = children;

        public INamedTypeSymbol Symbol { get; } = symbol;
        public TypeDeclarationSyntax Syntax { get; } = syntax;
        public AttributeData Attribute { get; } = attribute;

        public bool Equals(Entry other)
        {
            return
                Children.SetEquals(other.Children)
                &&
                Symbol.ToDisplayString().Equals(other.Symbol.ToDisplayString())
                &&
                Attribute.ToString().Equals(other.Attribute.ToString())
                &&
                Symbol.MemberNames.SequenceEqual(other.Symbol.MemberNames);
        }
    }

    public static bool IsPotentialSchematic(SyntaxNode node, CancellationToken token)
        => node is TypeDeclarationSyntax {AttributeLists.Count: > 0} && node.Parent is not TypeDeclarationSyntax;

    public static Schematic? MapSchematic(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not TypeDeclarationSyntax syntax) return null;

        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not INamedTypeSymbol symbol) return null;

        var logger = Hanz.RootLogger
            .GetSubLogger(nameof(LinkSchematics))
            .WithSemanticContext(context.SemanticModel)
            .WithCleanLogFile();

        logger.Log($"Processing {symbol}...");

        var lookup = new Dictionary<string, Entry>();

        var root = GetEntry(symbol, symbol, lookup, token, logger);

        if (root is null) return null;

        return new Schematic(root, context.SemanticModel);
    }

    private static Entry? GetEntry(
        INamedTypeSymbol root,
        INamedTypeSymbol symbol,
        Dictionary<string, Entry> lookup,
        CancellationToken token,
        Logger logger)
    {
        var attribute = symbol.GetAttributes()
            .FirstOrDefault(IsLinkSchematicAttribute);

        if (attribute is null) return null;

        var children = attribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Children")
            .Value;

        logger.Log($"{symbol}: Children?: {children.Kind}");

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

        if (symbol.DeclaringSyntaxReferences.First().GetSyntax(token) is not TypeDeclarationSyntax syntax)
            return null;

        return new(symbol, syntax, childrenEntries, attribute);
    }

    private static bool IsLinkSchematicAttribute(AttributeData attribute)
        => attribute.AttributeClass?.ToDisplayString() == "Discord.LinkSchematicAttribute";
}