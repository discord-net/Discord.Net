using Discord.Commands;
using Microsoft.CodeAnalysis;

namespace Discord.Analyzers
{
    internal static class SymbolExtensions
    {
        private static readonly string ModuleBaseName = typeof(ModuleBase<>).Name;

        public static bool DerivesFromModuleBase(this ITypeSymbol symbol)
        {
            for (var bType = symbol.BaseType; bType != null; bType = bType.BaseType)
                if (bType.MetadataName == ModuleBaseName)
                    return true;
            return false;
        }
    }
}
