using System;
using Microsoft.CodeAnalysis;
using Discord.Commands;

namespace Discord.Analyzers
{
    internal static class SymbolExtensions
    {
        private static readonly string _moduleBaseName = typeof(ModuleBase<>).Name;

        public static bool DerivesFromModuleBase(this INamedTypeSymbol symbol)
        {
            var bType = symbol.BaseType;
            while (bType != null)
            {
                if (bType.MetadataName == _moduleBaseName)
                    return true;

                bType = bType.BaseType;
            }
            return false;
        }
    }
}
