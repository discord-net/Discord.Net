using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Utils;

public static class Hierarchy
{
    private static readonly Dictionary<ITypeSymbol, List<SortedHierarchySymbol>> _cache = new(SymbolEqualityComparer.Default);

    public readonly struct SortedHierarchySymbol(int distance, INamedTypeSymbol type)
    {
        public readonly int Distance = distance;
        public readonly INamedTypeSymbol Type = type;
    }

    public static IEnumerable<T> OrderByHierarchy<T>(
        in ImmutableArray<T?> targets,
        Func<T, INamedTypeSymbol> typeResolver,
        out Dictionary<INamedTypeSymbol, T> map,
        out HashSet<INamedTypeSymbol> bases)
    {
        var mapLocal = map = targets
            .Where(x => x is not null)
            .OfType<T>()
            .ToDictionary<T, INamedTypeSymbol, T>(typeResolver, x => x, SymbolEqualityComparer.Default);

        var basesLocal = bases = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        return map.OrderBy(x =>
        {
            var count = 0;

            switch (x.Key.TypeKind)
            {
                case TypeKind.Interface:
                    count = x.Key.Interfaces.Max(iface =>
                    {
                        var depth = 0;
                        CalculateInterfaceDepth(iface, ref depth, mapLocal, basesLocal);
                        return depth;
                    });
                    break;
                default:
                    var baseType = x.Key.BaseType;

                    if (baseType is null)
                        return 0;

                    do
                    {
                        if (mapLocal.ContainsKey(baseType))
                        {
                            basesLocal.Add(baseType);
                            count++;
                        }
                    } while ((baseType = baseType.BaseType) is not null);

                    break;
            }

            return count;
        }).Select(x => x.Value);
    }

    private static void CalculateInterfaceDepth<T>(
        INamedTypeSymbol type,
        ref int depth,
        Dictionary<INamedTypeSymbol, T> map,
        HashSet<INamedTypeSymbol> bases)
    {
        if (map.ContainsKey(type))
        {
            depth++;
            bases.Add(type);
        }

        foreach (var iface in type.Interfaces)
        {
            CalculateInterfaceDepth(iface, ref depth, map, bases);
        }

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
