using Microsoft.CodeAnalysis;

namespace Discord.Net.Hanz.Utils;

public static class Generics
{
    public static ITypeSymbol ResolveGeneric(ITypeSymbol arg, INamedTypeSymbol root, TypeInfo info)
    {
        if (root.TypeArguments.Any(arg.Equals))
        {
            var typeArg = ((info.Type as INamedTypeSymbol)!).TypeArguments[root.TypeArguments.IndexOf(arg)];
            return typeArg.WithNullableAnnotation(arg.NullableAnnotation);
        }

        if (arg is not INamedTypeSymbol namedArg)
            return arg;

        if (namedArg is {IsGenericType: false, IsUnboundGenericType: false})
            return arg;

        var newTypes = namedArg.TypeArguments
            .Select(x => ResolveGeneric(x, root, info)).ToArray();

        if (!newTypes.SequenceEqual(namedArg.TypeArguments, SymbolEqualityComparer.Default))
        {
            return namedArg.ConstructedFrom.Construct(newTypes);
        }

        return arg;
    }
}
