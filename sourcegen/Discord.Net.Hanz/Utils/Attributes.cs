using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class Attributes
{
    public static string? GetAttributeName(AttributeSyntax attribute, SemanticModel semantic)
    {
        return semantic.GetSymbolInfo(attribute).Symbol is not IMethodSymbol attributeSymbol
            ? null
            : attributeSymbol.ContainingType.ToDisplayString();
    }
}
