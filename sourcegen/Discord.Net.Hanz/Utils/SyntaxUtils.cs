using Microsoft.CodeAnalysis;
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

    public static LiteralExpressionSyntax CreateLiteral(ITypeSymbol type, object? value)
    {
        if (value is null)
            return type.IsValueType
                ? SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
                : SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);

        return value switch
        {
            bool val => val
                ? SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
                : SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression),
            string val => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(val)),
            char val => SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(val)),
            double val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            float val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            decimal val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            sbyte val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            byte val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            short val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            ushort val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            int val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(val)),
            uint val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            long val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            ulong val => SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression,
                SyntaxFactory.Literal(val)),
            _ => SyntaxFactory.LiteralExpression(SyntaxKind.DefaultLiteralExpression)
        };
    }
}
