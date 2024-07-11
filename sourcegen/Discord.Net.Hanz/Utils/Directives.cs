using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class Directives
{
    public static string GetFormattedUsingDirectives(this SyntaxNode node)
    {
        return string.Join("\n", node.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>());
    }
}
