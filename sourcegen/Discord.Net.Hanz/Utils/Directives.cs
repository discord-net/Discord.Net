using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class Directives
{
    public static string GetFormattedUsingDirectives(this SyntaxNode node)
    {
        return string.Join("\n", GetUsingDirectives(node));
    }

    public static IEnumerable<string> GetUsingDirectives(this SyntaxNode node)
        => node.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>().Select(x => x.ToString());
}
