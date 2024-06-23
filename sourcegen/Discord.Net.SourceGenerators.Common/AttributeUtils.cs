using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Discord.Net.SourceGenerators.Common;

public static class AttributeUtils
{
    public static ArgumentSyntax? PullArg(AttributeArgumentSyntax arg)
    {
        var argument = arg.DescendantNodes().OfType<ArgumentSyntax>().ToList();

        return argument.Count != 1 ? null : argument[0];
    }
}
