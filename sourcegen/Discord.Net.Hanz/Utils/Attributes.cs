using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Utils;

public static class Attributes
{
    public static string? GetAttributeName(AttributeSyntax attribute, SemanticModel semantic)
    {
        return ModelExtensions.GetSymbolInfo(semantic, attribute).Symbol is not IMethodSymbol attributeSymbol
            ? null
            : attributeSymbol.ContainingType.ToDisplayString();
    }

    public static ExpressionSyntax? GetAttributeNamedArg(AttributeSyntax attribute, string name)
    {
        var args = attribute
            .ChildNodes()
            .OfType<AttributeArgumentListSyntax>()
            .FirstOrDefault()
            ?.Arguments;

        return args?.FirstOrDefault(x => x.NameEquals?.Name.Identifier.ValueText == name)?.Expression;
    }

    public static string? GetAttributeNamedNameOfArg(AttributeSyntax attribute, string name)
    {
        var expression = GetAttributeNamedArg(attribute, name);

        if (expression is not InvocationExpressionSyntax invocation) return null;
        if (invocation.Expression is not IdentifierNameSyntax {Identifier.ValueText: "nameof"}) return null;

        return invocation.ArgumentList.Arguments.FirstOrDefault()?.Expression?.ToString();
    }

    public static bool? GetAttributeNamedBoolArg(AttributeSyntax attribute, string name)
    {
        var expression = GetAttributeNamedArg(attribute, name);

        if (expression is not LiteralExpressionSyntax literal) return null;

        return literal.Kind() switch
        {
            SyntaxKind.TrueLiteralExpression => true,
            SyntaxKind.FalseLiteralExpression => false,
            _ => null
        };
    }

    public static bool GetAttributeNamedBoolArg(AttributeSyntax attribute, string name, bool defaultVal)
        => GetAttributeNamedBoolArg(attribute, name) ?? defaultVal;
}
