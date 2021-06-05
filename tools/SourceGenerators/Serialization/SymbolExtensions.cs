using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Microsoft.CodeAnalysis;

namespace Discord.Net.SourceGenerators.Serialization
{
    internal static class SymbolExtensions
    {
        public static IEnumerable<IPropertySymbol> GetProperties(
            this INamedTypeSymbol symbol,
            bool includeInherited)
        {
            var s = symbol;
            do
            {
                foreach (var member in s.GetMembers())
                    if (member is IPropertySymbol property)
                        yield return property;

                s = s.BaseType;
            }
            while (includeInherited && s != null);
        }
    }
}
