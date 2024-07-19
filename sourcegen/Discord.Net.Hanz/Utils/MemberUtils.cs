using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Utils;

public static class MemberUtils
{
    public static readonly ConflictEqualityComparer ConflictEquality = new();

    public class ConflictEqualityComparer : IEqualityComparer<ISymbol>
    {
        public bool Equals(ISymbol x, ISymbol y) => Conflicts(x, y) || SymbolEqualityComparer.Default.Equals(x, y);

        public int GetHashCode(ISymbol obj) => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    private static string GetMemberName<T>(T symbol, Func<T, ImmutableArray<T>> getExplicitInterfaces) where T : ISymbol
    {
        while (true)
        {
            if (getExplicitInterfaces(symbol).Length <= 0) return symbol.Name;
            symbol = getExplicitInterfaces(symbol)[0];
            continue;
        }
    }

    public static bool Conflicts(ISymbol a, ISymbol b)
    {
        return a switch
        {
            IPropertySymbol propA when b is IPropertySymbol propB =>
                GetMemberName(propA, x => x.ExplicitInterfaceImplementations) ==
                GetMemberName(propB, x => x.ExplicitInterfaceImplementations) ||
                propA.Type.Equals(
                    propB.Type,
                    SymbolEqualityComparer.Default
                ),
            IMethodSymbol methodA when b is IMethodSymbol methodB =>
                GetMemberName(methodA, x => x.ExplicitInterfaceImplementations) ==
                GetMemberName(methodB, x => x.ExplicitInterfaceImplementations) &&
                methodA.Parameters.Length == methodB.Parameters.Length &&
                methodA.Parameters
                    .Select((x, i) => (Parameter: x, Index: i))
                    .All(x => methodB.Parameters[x.Index].Type
                        .Equals(x.Parameter.Type, SymbolEqualityComparer.Default)),
            _ => false
        };
    }

    public static bool CanOverride(ISymbol targetSymbol, ISymbol baseSymbol, Compilation compilation)
    {
        return targetSymbol switch
        {
            IPropertySymbol propA when baseSymbol is IPropertySymbol propB
                => propA.Type.Equals(
                    propB.Type,
                    SymbolEqualityComparer.Default
                ) || compilation.HasImplicitConversion(propA.Type, propB.Type),
            IMethodSymbol methodA when baseSymbol is IMethodSymbol methodB =>
                (
                    methodA.ReturnType.Equals(
                        methodB.ReturnType,
                        SymbolEqualityComparer.Default
                    )
                    ||
                    compilation.HasImplicitConversion(methodA.ReturnType, methodB.ReturnType)
                ) &&
                methodA.Parameters.Length == methodB.Parameters.Length &&
                methodA.Parameters
                    .Select((x, i) => (Parameter: x, Index: i))
                    .All(x => methodB.Parameters[x.Index].Type
                        .Equals(x.Parameter.Type, SymbolEqualityComparer.Default)),
            _ => false
        };
    }
}
