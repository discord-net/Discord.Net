using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class SyntaxUtils
{
    public static TypeDeclarationSyntax? CombineMembers(IEnumerable<TypeDeclarationSyntax> nodes)
    {
        var nodesList = nodes.ToList();

        if (nodesList.Count == 0) return null;

        var result = nodesList[0];

        for (var i = 1; i < nodesList.Count; i++)
        {
            var node = nodesList[i];

            if (result.ParameterList is null && node.ParameterList is not null)
                result = result.WithParameterList(node.ParameterList);

            if (result.ConstraintClauses.Count == 0 && node.ConstraintClauses.Any())
                result = result.WithConstraintClauses(node.ConstraintClauses);

            if (result.TypeParameterList is null && node.TypeParameterList is not null)
                result = result.WithTypeParameterList(node.TypeParameterList);

            if (result.BaseList is null && node.BaseList is not null)
                result = result.WithBaseList(node.BaseList);

            var typeMembers = node.Members
                .OfType<TypeDeclarationSyntax>()
                .ToArray();

            foreach (var typeMember in typeMembers)
            {
                if
                (
                    result.Members
                        .FirstOrDefault(x =>
                            x is TypeDeclarationSyntax existing
                            &&
                            existing.Identifier.ValueText == typeMember.Identifier.ValueText
                        )
                    is TypeDeclarationSyntax existing
                )
                {
                    result = result.ReplaceNode(
                        existing,
                        CombineMembers([existing, typeMember])!
                    );
                }
                else
                {
                    result = result.AddMembers(typeMember);
                }
            }

            result = result
                .AddMembers(
                    node.Members
                        .Where(x => x is not TypeDeclarationSyntax)
                        .ToArray()
                );
        }

        return result;
    }

    public static void ApplyNesting(ITypeSymbol type, ref TypeDeclarationSyntax syntax)
    {
        var container = type.ContainingType;

        while (container is not null)
        {
            if (
                container.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                is not TypeDeclarationSyntax containerSyntax
            ) break;

            syntax = CreateSourceGenClone(containerSyntax).AddMembers(syntax);

            container = container.ContainingType;
        }
    }

    public static string[] FlattenMemberAccess(MemberAccessExpressionSyntax syntax)
    {
        if (syntax.Expression is MemberAccessExpressionSyntax left)
            return [..FlattenMemberAccess(left), syntax.Name.Identifier.ValueText];

        return [syntax.Expression.ToString(), syntax.Name.Identifier.ValueText];
    }

    public static SyntaxNode GetMemberTargetOrSelf(SyntaxNode node)
    {
        if (node is MemberAccessExpressionSyntax memberAccess)
            return GetMemberTargetOrSelf(memberAccess.Name);
        return node;
    }

    public static T CreateSourceGenClone<T>(T syntax)
        where T : TypeDeclarationSyntax
    {
        return (T) SyntaxFactory.TypeDeclaration(
            syntax.Kind(),
            [],
            syntax.Modifiers,
            syntax.Keyword,
            syntax.Identifier,
            syntax.TypeParameterList,
            null,
            syntax.ConstraintClauses,
            SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            [],
            SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            default
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