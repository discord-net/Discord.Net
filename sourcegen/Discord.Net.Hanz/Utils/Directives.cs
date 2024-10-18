using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class Directives
{
    public static string GetFormattedUsingDirectives(this SyntaxNode node, params string[] extras)
    {
        return string.Join("\n", GetUsingDirectives(node).Concat(extras.Select(x => $"using {x};")).Distinct());
    }

    public static IEnumerable<string> GetUsingDirectives(this SyntaxNode node)
        => GetUsingDirectivesSyntax(node).Select(x => x.ToString());

    public static IEnumerable<UsingDirectiveSyntax> GetUsingDirectivesSyntax(this SyntaxNode node)
        => node.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>().Where(x => x.GlobalKeyword == default);
}
