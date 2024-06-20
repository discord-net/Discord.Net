using Microsoft.CodeAnalysis;
using System.Text;

namespace Discord.Net.SourceGenerators.Common;

public static class SymbolExtensions
{
    public static string GetFullMetadataName(this ISymbol? s)
    {
        if (s == null || IsRootNamespace(s))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(s.MetadataName);
        var last = s;

        s = s.ContainingSymbol;

        while (!IsRootNamespace(s))
        {
            if (s is ITypeSymbol && last is ITypeSymbol)
            {
                sb.Insert(0, '+');
            }
            else
            {
                sb.Insert(0, '.');
            }

            sb.Insert(0, s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
            //sb.Insert(0, s.MetadataName);
            s = s.ContainingSymbol;
        }

        return sb.ToString();
    }

    private static bool IsRootNamespace(ISymbol symbol)
    {
        INamespaceSymbol? s = null;
        return ((s = symbol as INamespaceSymbol) != null) && s.IsGlobalNamespace;
    }
}
