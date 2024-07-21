using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils;

public static class TypeUtils
{
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
