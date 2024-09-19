using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;

namespace Discord.Net.Hanz;

public static class Hierarchy
{
    private static readonly Dictionary<ITypeSymbol, List<SortedHierarchySymbol>> _cache =
        new(SymbolEqualityComparer.Default);

    public static bool Implements(INamedTypeSymbol source, INamedTypeSymbol toCheck)
    {
        if (source.TypeKind is TypeKind.Interface && toCheck.TypeKind is TypeKind.Class)
            return false;

        switch (source.TypeKind, toCheck.TypeKind)
        {
            case (_, TypeKind.Interface):
                return source.AllInterfaces.Contains(toCheck);
            case (TypeKind.Class, TypeKind.Class):
                return TypeUtils.GetBaseTypes(source).Contains(toCheck, SymbolEqualityComparer.Default);
            default: return false;
        }
    }
    
    public static HashSet<INamedTypeSymbol> AllInterfacesWrtVariance(
        this ITypeSymbol seed,
        SemanticModel model,
        Logger? logger = null)
    {
        var result = new HashSet<INamedTypeSymbol>(seed.AllInterfaces, SymbolEqualityComparer.Default);

        var variance = seed.AllInterfaces
            .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
            .Where(x =>
                x.TypeKind is TypeKind.Interface &&
                x.IsGenericType &&
                x.ConstructedFrom.TypeParameters
                    .Any(x => x.Variance > 0)
            );

        var processed = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var variantInterface in variance)
        {
            if (!result.Contains(variantInterface) || processed.Contains(variantInterface.ConstructedFrom))
                goto end_iter;

            var implementations = seed.AllInterfaces
                .Where(x =>
                    x.ConstructedFrom
                        .Equals(variantInterface.ConstructedFrom, SymbolEqualityComparer.Default) &&
                    x.TypeParameters
                        .Select((x, i) => (Parameter: x, Index: i))
                        .All(arg =>
                            arg.Parameter.Variance > 0
                            ||
                            variantInterface.TypeArguments[arg.Index]
                                .Equals(
                                    x.TypeArguments[arg.Index],
                                    SymbolEqualityComparer.Default
                                )
                        )
                )
                .Where(x => result.Contains(x))
                .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                .ToList();

            if (implementations.Count <= 1)
                goto end_iter;

            logger?.Log($"{seed}: Computing lowest variant for {variantInterface.ConstructedFrom}");

            var lowestOrderBuckets = new List<INamedTypeSymbol>();

            for
            (
                var typeParameterIndex = 0;
                typeParameterIndex != variantInterface.TypeParameters.Length;
                typeParameterIndex++
            )
            {
                if (variantInterface.TypeParameters[typeParameterIndex].Variance == 0) continue;

                for
                (
                    var implementationIndex = 0;
                    implementationIndex < implementations.Count;
                    implementationIndex++
                )
                {
                    var implementation = implementations[implementationIndex];

                    if (lowestOrderBuckets.Count == 0)
                    {
                        lowestOrderBuckets.Add(implementation);
                        continue;
                    }

                    var bucketMissCount = 0;

                    for (var bucketIndex = 0; bucketIndex < lowestOrderBuckets.Count; bucketIndex++)
                    {
                        var lowestOrder = lowestOrderBuckets[bucketIndex];
                        var implementationArg = implementation.TypeArguments[typeParameterIndex];
                        var ourArg = lowestOrder.TypeArguments[typeParameterIndex];

                        if (implementationArg.Equals(ourArg, SymbolEqualityComparer.Default)) continue;

                        var isLower = model.Compilation.HasImplicitConversion(
                            implementationArg,
                            ourArg
                        );

                        var isUpper = model.Compilation.HasImplicitConversion(
                            ourArg,
                            implementationArg
                        );

                        if (isUpper && !isLower)
                        {
                            logger?.Log(
                                $"{variantInterface.ConstructedFrom}: Lower order exists for {variantInterface}, skipping");
                            continue;
                        }

                        if (!isLower && !isUpper)
                        {
                            bucketMissCount++;
                            logger?.Log($"{variantInterface.ConstructedFrom}: Miss #{bucketMissCount} {bucketIndex + 1}/{lowestOrderBuckets.Count}: {ourArg} <> {implementationArg}");
                            continue;
                        }

                        if (isLower)
                        {
                            logger?.Log(
                                $"{variantInterface.ConstructedFrom}: favouring argument {implementationArg} over {ourArg}");

                            lowestOrderBuckets[bucketIndex] = implementation;
                            break;
                        }
                    }

                    if (
                        bucketMissCount == lowestOrderBuckets.Count &&
                        !lowestOrderBuckets.Contains(implementation, SymbolEqualityComparer.Default))
                    {
                        logger?.Log($"{variantInterface.ConstructedFrom}: New bucket {implementation}");
                        lowestOrderBuckets.Add(implementation);
                    }
                }
            }

            if (implementations.Count == lowestOrderBuckets.Count) goto end_iter;


            logger?.Log(
                $"{seed}: reduced {implementations.Count - lowestOrderBuckets.Count} implementations of {variantInterface.ConstructedFrom}"
            );

            foreach (var toRemove in implementations.Where(x => !lowestOrderBuckets.Contains(x)))
            {
                logger?.Log($" - yeet {toRemove}?: {result.Remove(toRemove)}");
            }

            foreach (var type in lowestOrderBuckets)
            {
                logger?.Log($" - kept {type}");
            }

            end_iter:
            processed.Add(variantInterface.ConstructedFrom);
        }

        return result;
    }

    public readonly struct SortedHierarchySymbol(int distance, INamedTypeSymbol type)
        : IEquatable<SortedHierarchySymbol>
    {
        public readonly int Distance = distance;
        public readonly INamedTypeSymbol Type = type;

        public bool Equals(SortedHierarchySymbol other)
            => Distance == other.Distance && Type.Equals(other.Type, SymbolEqualityComparer.Default);

        public override bool Equals(object? obj) => obj is SortedHierarchySymbol other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Distance * 397) ^ SymbolEqualityComparer.Default.GetHashCode(Type);
            }
        }

        public static bool operator ==(SortedHierarchySymbol left, SortedHierarchySymbol right) => left.Equals(right);

        public static bool operator !=(SortedHierarchySymbol left, SortedHierarchySymbol right) => !left.Equals(right);
    }

    public static HashSet<ITypeSymbol> GetHierarchyBetween(
        ITypeSymbol child,
        ITypeSymbol root,
        bool includeRoot = true,
        bool includeChild = true,
        bool searchBaseClasses = true,
        bool searchAllPaths = true)
    {
        var path = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        HasParent(
            child,
            root,
            new(SymbolEqualityComparer.Default),
            path,
            searchBaseClasses,
            searchAllPaths
        );

        if (!includeChild)
            path.Remove(child);

        if (!includeRoot)
            path.Remove(root);

        return path;

        static bool HasParent(
            ITypeSymbol child,
            ITypeSymbol root,
            HashSet<ITypeSymbol> visited,
            HashSet<ITypeSymbol> path,
            bool searchBaseClasses,
            bool searchAllPaths)
        {
            if (child.TypeKind is TypeKind.Class && root.TypeKind is TypeKind.Class)
            {
                if (child.BaseType?.Equals(root, SymbolEqualityComparer.Default) ?? false)
                {
                    path.Add(root);
                    return true;
                }
            }

            var result = false;

            if (searchBaseClasses && child.TypeKind is TypeKind.Class && child.BaseType is not null)
            {
                result = HasParent(child.BaseType, root, visited, path, searchBaseClasses, searchAllPaths);
            }

            if (root.TypeKind is TypeKind.Interface)
            {
                if (child.Interfaces.Contains(root, SymbolEqualityComparer.Default))
                {
                    path.Add(root);

                    if (!searchAllPaths)
                        return true;

                    result = true;
                }

                foreach (var iface in child.Interfaces.Where(x => !visited.Contains(x)))
                {
                    result |= HasParent(iface, root, visited, path, searchBaseClasses, searchAllPaths);

                    if (result && !searchAllPaths)
                        break;

                    visited.Add(iface);
                }
            }

            if (result)
                path.Add(child);

            return result;
        }
    }

    public static IEnumerable<T> OrderByHierarchy<T>(
        ImmutableArray<T?> targets,
        Func<T, INamedTypeSymbol> typeResolver,
        out Dictionary<INamedTypeSymbol, T> map,
        out HashSet<INamedTypeSymbol> bases)
    {
        var targetsCopy = targets;
        targets = targets
            .Where(x =>
                x is not null && targetsCopy.Count(y =>
                    y is not null && typeResolver(y).Equals(typeResolver(x), SymbolEqualityComparer.Default)
                ) == 1
            )
            .ToImmutableArray();

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

    public static List<SortedHierarchySymbol> GetHierarchy(ITypeSymbol symbol, bool cache = true)
    {
        return cache && _cache.TryGetValue(symbol, out var cached)
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
