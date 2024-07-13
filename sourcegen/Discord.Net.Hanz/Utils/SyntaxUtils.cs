using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class SyntaxUtils
{
    public static ClassDeclarationSyntax CreateSourceGenClone(ClassDeclarationSyntax classDeclarationSyntax)
    {
        return SyntaxFactory.ClassDeclaration(
            [],
            classDeclarationSyntax.Modifiers,
            classDeclarationSyntax.Identifier,
            classDeclarationSyntax.TypeParameterList,
            null,
            classDeclarationSyntax.ConstraintClauses,
            []
        );
    }
}
