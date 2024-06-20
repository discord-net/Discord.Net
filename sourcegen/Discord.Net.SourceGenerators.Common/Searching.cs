using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace Discord.Net.SourceGenerators.Common;

public static class Searching
{
    public static ClassDeclarationSyntax[] FindClassesWithAttribute(ref GeneratorExecutionContext context, string attributeName)
    {
        return context.Compilation.SyntaxTrees
            .SelectMany(x => x
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(x => x.AttributeLists
                    .SelectMany(x => x.Attributes)
                    .Any(x => x.Name.GetText().ToString() == attributeName)
                )
            ).ToArray();
    }

    public static ClassDeclarationSyntax[] FindPropertiesWithAttribute(ref GeneratorExecutionContext context, string attributeName)
    {
        return context.Compilation.SyntaxTrees
            .SelectMany(x => x
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Where(x => x
                    .DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Any(x => x.AttributeLists
                        .SelectMany(x => x.Attributes)
                        .Any(x => x.Name.GetText().ToString() == attributeName)
                    )
                )
            ).ToArray();
    }
}
