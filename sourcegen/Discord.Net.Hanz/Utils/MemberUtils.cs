using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Utils;

public static class MemberUtils
{
    public static readonly ConflictEqualityComparer ConflictEquality = new();

    public static ParameterListSyntax CreateParameterList(
        IMethodSymbol? method, 
        bool withDefaults = true,
        string requestOptions = "RequestOptions")
    {
        return method is null
            ? SyntaxFactory.ParseParameterList(
                $"({requestOptions}? options = null, CancellationToken token = default)"
            )
            : SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    method.Parameters.Select(x =>
                        SyntaxFactory.Parameter(
                            [],
                            [],
                            SyntaxFactory.ParseTypeName(
                                x.Type.Name is "RequestOptions" 
                                    ? $"{requestOptions}?" 
                                    : x.Type.ToDisplayString()),
                            SyntaxFactory.Identifier(x.Name),
                            x.HasExplicitDefaultValue && withDefaults
                                ? SyntaxFactory.EqualsValueClause(
                                    SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)
                                )
                                : null
                        )
                    )
                )
            );
    }

    public static bool IsExplicitInterfaceImplementation(ISymbol symbol)
    {
        return symbol switch
        {
            IPropertySymbol {ExplicitInterfaceImplementations.Length: > 0} => true,
            IMethodSymbol {ExplicitInterfaceImplementations.Length: > 0} => true,
            _ => false
        };
    }

    public static bool CanImplementMemberExplicitly(ISymbol symbol)
    {
        return
            symbol.ContainingType.TypeKind is TypeKind.Interface &&
            (
                symbol.IsVirtual || symbol.IsAbstract
            );
    }

    public class ConflictEqualityComparer : IEqualityComparer<ISymbol>
    {
        public bool Equals(ISymbol x, ISymbol y) => Conflicts(x, y) || SymbolEqualityComparer.Default.Equals(x, y);

        public int GetHashCode(ISymbol obj) => SymbolEqualityComparer.Default.GetHashCode(obj);
    }

    public static string GetMemberName(ISymbol symbol)
    {
        return symbol switch
        {
            IPropertySymbol prop => GetMemberName(prop, x => x.ExplicitInterfaceImplementations),
            IMethodSymbol method => GetMemberName(method, x => x.ExplicitInterfaceImplementations),
            _ => null!
        };
    }

    public static string GetMemberName<T>(T symbol, Func<T, ImmutableArray<T>> getExplicitInterfaces) where T : ISymbol
    {
        while (true)
        {
            if (getExplicitInterfaces(symbol).Length <= 0) return symbol.Name;
            symbol = getExplicitInterfaces(symbol)[0];
            continue;
        }
    }

    public static ITypeSymbol? GetMemberType(ISymbol? member)
    {
        return member switch
        {
            IPropertySymbol prop => prop.Type,
            IMethodSymbol method => method.ReturnType,
            _ => null
        };
    }

    public static bool Conflicts(ISymbol a, ISymbol b)
    {
        return a switch
        {
            IPropertySymbol propA when b is IPropertySymbol propB =>
                GetMemberName(propA) == GetMemberName(propB),
            IMethodSymbol methodA when b is IMethodSymbol methodB =>
                GetMemberName(methodA) == GetMemberName(methodB)
                &&
                methodA.Parameters.Length == methodB.Parameters.Length
                &&
                methodA.Parameters
                    .Select((x, i) => (Parameter: x, Index: i))
                    .All(x => methodB
                        .Parameters[x.Index]
                        .Type
                        .Equals(x.Parameter.Type, SymbolEqualityComparer.Default)),
            _ => false
        };
    }

    public static bool CanOverride(ISymbol targetSymbol, ISymbol baseSymbol, Compilation compilation)
    {
        return targetSymbol switch
        {
            IPropertySymbol propA when baseSymbol is IPropertySymbol propB =>
                CanOverrideProperty(
                    propA.Type, propA.Name,
                    propB.Type, propB.Name,
                    compilation
                ),
            IMethodSymbol methodA when baseSymbol is IMethodSymbol methodB =>
                CanOverrideMethod(
                    methodA.ReturnType, methodA.Name, methodA.Parameters,
                    methodB.ReturnType, methodB.Name, methodB.Parameters,
                    compilation
                ),
            _ => false
        };
    }

    public static bool CanOverrideProperty(
        ITypeSymbol propertyTypeA, string nameA,
        ITypeSymbol propertyTypeB, string nameB,
        Compilation compilation
    )
    {
        return
            nameA == nameB
            &&
            (
                propertyTypeA.Equals(
                    propertyTypeB,
                    SymbolEqualityComparer.Default
                )
                || compilation.HasImplicitConversion(propertyTypeA, propertyTypeB)
            );
    }

    public static bool CanOverrideMethod(
        ITypeSymbol returnTypeA, string nameA, IList<IParameterSymbol> parametersA,
        ITypeSymbol returnTypeB, string nameB, IList<IParameterSymbol> parametersB,
        Compilation compilation)
    {
        return
            nameA == nameB
            &&
            (
                returnTypeA.Equals(
                    returnTypeB,
                    SymbolEqualityComparer.Default
                )
                ||
                compilation.HasImplicitConversion(returnTypeA, returnTypeB)
            )
            &&
            parametersA.Count == parametersB.Count &&
            parametersA
                .Select((x, i) => (Parameter: x, Index: i))
                .All(x => parametersB[x.Index].Type
                    .Equals(x.Parameter.Type, SymbolEqualityComparer.Default)
                );
    }
}
