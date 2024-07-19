using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils;

public static class Hierarchy
{
    private static readonly Dictionary<ITypeSymbol, List<SortedHierarchySymbol>> _cache = new(SymbolEqualityComparer.Default);

    public readonly struct SortedHierarchySymbol(int distance, INamedTypeSymbol type)
    {
        public readonly int Distance = distance;
        public readonly INamedTypeSymbol Type = type;
    }

    public static List<SortedHierarchySymbol> GetHierarchy(ITypeSymbol symbol)
    {
        return _cache.TryGetValue(symbol, out var cached)
            ? cached
            : _cache[symbol] = CreateHierarchyMap(symbol);
    }

    internal static List<SortedHierarchySymbol> CreateHierarchyMap(ITypeSymbol symbol)
    {
        return MapHierarchy(symbol, 0).OrderBy(x => x.Distance).ToList();
    }

    private static IEnumerable<SortedHierarchySymbol> MapHierarchy(ITypeSymbol symbol, int depth)
    {
        if (symbol.BaseType is not null)
        {
            yield return new SortedHierarchySymbol(depth, symbol.BaseType);
            foreach (var extended in MapHierarchy(symbol.BaseType, depth + 1))
                yield return extended;
        }

        foreach (var iface in symbol.Interfaces)
        {
            yield return new SortedHierarchySymbol(depth, iface);
            foreach (var extended in MapHierarchy(iface, depth + 1))
                yield return extended;
        }
    }
}
