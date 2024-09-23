using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils;

public static class TypeUtils
{
    public static IEnumerable<ITypeSymbol> SelfAndContaingTypes(ITypeSymbol symbol)
        => ContaingTypes(symbol).Prepend(symbol);
    
    public static IEnumerable<ITypeSymbol> ContaingTypes(ITypeSymbol symbol)
    {
        var current = symbol.ContainingType;

        while (current is not null)
        {
            yield return current;
            current = current.ContainingType;
        }
    }
    
    public static IEnumerable<INamedTypeSymbol> AllDirectInterfaces(ITypeSymbol symbol)
    {
        return symbol.Interfaces.SelectMany(x => (INamedTypeSymbol[])[x, ..x.AllInterfaces]);
    }

    public static string ToFullMetadataName(this ISymbol symbol)
    {
        var parts = new List<string>() {symbol.MetadataName};

        var container = symbol.ContainingType;

        while (container is not null)
        {
            parts.Add(symbol.ContainingType.MetadataName);
            container = container.ContainingType;
        }

        parts.Reverse();
        
        return $"{symbol.ContainingNamespace}.{string.Join(".", parts)}";
    }

    public static bool TypeLooselyEquals(ITypeSymbol first, ITypeSymbol second)
    {
        if (first.Equals(second, SymbolEqualityComparer.Default))
            return true;

        if (first is INamedTypeSymbol {IsGenericType: true} namedFirst &&
            second is INamedTypeSymbol {IsGenericType: true} namedSecond)
        {
            return
                (
                    namedFirst.IsUnboundGenericType &&
                    !namedSecond.IsUnboundGenericType &&
                    namedSecond.ConstructUnboundGenericType().Equals(namedFirst, SymbolEqualityComparer.Default)
                )
                ||
                (
                    !namedFirst.IsUnboundGenericType &&
                    namedSecond.IsUnboundGenericType &&
                    namedFirst.ConstructUnboundGenericType().Equals(namedSecond, SymbolEqualityComparer.Default)
                )
                ||
                (
                    !namedFirst.IsUnboundGenericType &&
                    !namedSecond.IsUnboundGenericType &&
                    namedFirst.ConstructUnboundGenericType().Equals(
                        namedSecond.ConstructUnboundGenericType(),
                        SymbolEqualityComparer.Default
                    )
                );
        }

        return false;
    }

    public static bool TypeContainsOtherAsGeneric(
        ITypeSymbol root,
        ITypeSymbol toFind,
        out ITypeSymbol? containedAt,
        HashSet<ITypeSymbol>? visited = null)
    {
        visited ??= new(SymbolEqualityComparer.Default);

        if (visited.Contains(root))
        {
            containedAt = null;
            return false;
        }

        visited.Add(root);

        if (root.Equals(toFind, SymbolEqualityComparer.Default))
        {
            containedAt = root;
            return true;
        }

        if (root is INamedTypeSymbol {IsGenericType: true} namedRoot)
        {
            foreach (var typeArg in namedRoot.TypeArguments)
            {
                if (!TypeContainsOtherAsGeneric(typeArg, toFind, out var container, visited)) continue;

                containedAt = container?.Equals(toFind, SymbolEqualityComparer.Default) ?? false ? typeArg : container;
                return true;
            }
        }

        foreach (var iface in root.AllInterfaces)
        {
            if (!TypeContainsOtherAsGeneric(iface, toFind, out var container, visited)) continue;
            containedAt = container?.Equals(toFind, SymbolEqualityComparer.Default) ?? false ? iface : container;
            return true;
        }

        containedAt = null;
        return false;
    }

    public static bool CanBeMisleadinglyAssigned(ITypeSymbol from, ITypeSymbol to, SemanticModel model)
    {
        if (model.Compilation.HasImplicitConversion(from, to))
            return true;

        // ensure all generics match
        if (
            from is INamedTypeSymbol {IsGenericType: true} namedFrom &&
            to is INamedTypeSymbol {IsGenericType: true} namedTo &&
            namedFrom.TypeArguments.Length == namedTo.TypeArguments.Length)
        {
            return !namedTo.TypeArguments
                .Where((t, i) =>
                    !CanBeMisleadinglyAssigned(namedFrom.TypeArguments[i], t, model)
                ).Any();
        }

        return false;
    }

    public static bool TypeIsOfProvidedOrDescendant(ITypeSymbol from, ITypeSymbol to, SemanticModel model)
    {
        return from.Equals(to, SymbolEqualityComparer.Default) ||
               model.Compilation.HasImplicitConversion(from, to);
    }

    public static IEnumerable<ITypeSymbol> GetBaseTypesAndThis(ITypeSymbol type)
    {
        var current = type;
        while (current is not null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static IEnumerable<INamedTypeSymbol> GetBaseTypesAndThis(INamedTypeSymbol type)
    {
        var current = type;
        while (current is not null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static IEnumerable<ITypeSymbol> GetBaseTypes(ITypeSymbol type)
    {
        var current = type.BaseType;
        while (current is not null)
        {
            yield return current;
            current = current.BaseType;
        }
    }

    public static INamedTypeSymbol? PairedWalkTypeSymbolForMatch(
        ITypeSymbol toFind,
        ITypeSymbol toWalk,
        INamedTypeSymbol toPair,
        SemanticModel model)
    {
        if (toWalk.Equals(toFind, SymbolEqualityComparer.Default))
            return toPair;

        if (toPair.Equals(toFind, SymbolEqualityComparer.Default))
            return toPair;

        if (toFind is INamedTypeSymbol {IsGenericType: false})
        {
            var found = Hierarchy.GetHierarchy(toPair)
                .FirstOrDefault(x =>
                    x.Type.Equals(toFind, SymbolEqualityComparer.Default)
                ).Type;

            if (found is not null)
                return found;
        }

        if (toWalk is INamedTypeSymbol {IsGenericType: true} namedToWalk)
        {
            // 'toPair' may not be generic, but implement the 'toWalk' type
            // if (!model.Compilation.HasImplicitConversion(toPair, toWalk))
            // {
            //     Hanz.Logger.Warn($"No conversion between {toWalk} : {toPair}");
            //     return null;
            // }

            INamedTypeSymbol pairImpl;

            if (
                toPair.IsGenericType &&
                toPair.ConstructUnboundGenericType()
                    .Equals(
                        namedToWalk.ConstructUnboundGenericType(),
                        SymbolEqualityComparer.Default
                    )
            )
            {
                pairImpl = toPair;
            }
            else
            {
                pairImpl = Hierarchy.GetHierarchy(toPair)
                    .FirstOrDefault(x =>
                        x.Type.IsGenericType &&
                        x.Type.ConstructUnboundGenericType()
                            .Equals(
                                namedToWalk.ConstructUnboundGenericType(),
                                SymbolEqualityComparer.Default
                            )
                    ).Type;
            }

            if (pairImpl is null)
            {
                //Hanz.Logger.Warn($"No implementation between {toWalk} : {toPair}");
                return null;
            }

            for (var i = 0; i < namedToWalk.TypeArguments.Length; i++)
            {
                if (pairImpl.TypeArguments[i] is not INamedTypeSymbol childToPair)
                    continue;

                var walked = PairedWalkTypeSymbolForMatch(
                    toFind,
                    namedToWalk.TypeArguments[i],
                    childToPair,
                    model
                );

                if (walked is not null)
                    return walked;
            }
        }

        //Hanz.Logger.Warn($"returning null for {toWalk} : {toPair}");

        return null;
    }
}
