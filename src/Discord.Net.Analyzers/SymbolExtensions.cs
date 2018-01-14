using System;
using Microsoft.CodeAnalysis;
using Discord.Commands;

namespace Discord.Analyzers
{
    internal static class SymbolExtensions
    {
        private static readonly string _moduleBaseName = typeof(ModuleBase<>).Name;

        public static bool DerivesFromModuleBase(this ITypeSymbol symbol)
        {
            for (var bType = symbol.BaseType; bType != null; bType = bType.BaseType)
            {
                if (bType.MetadataName == _moduleBaseName)
                    return true;
            }
            return false;
        }
    }
}
