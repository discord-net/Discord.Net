using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public static class RestLoadableSource
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        PropertyDeclarationSyntax propertyDeclarationSyntax,
        ClassDeclarationSyntax classDeclarationSyntax
    )
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; } = propertyDeclarationSyntax;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
    }

    public static bool IsValid(SyntaxNode node)
    {
        if (node is not PropertyDeclarationSyntax property) return false;

        return property.AttributeLists.Count > 0;
    }

    public static GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        if (context.Node is not PropertyDeclarationSyntax property) return null;

        if (property.Parent is not ClassDeclarationSyntax classDeclarationSyntax)
        {
            return null;
        }

        if (property.Type is not GenericNameSyntax)
        {
            return null;
        }

        if (property.AttributeLists
            .SelectMany(x => x.Attributes)
            .All(x =>
                Attributes.GetAttributeName(x, context.SemanticModel) !=
                "Discord.Rest.RestLoadableActorSourceAttribute"
            )
           ) return null;

        return new GenerationTarget(
            context.SemanticModel,
            property,
            classDeclarationSyntax
        );
    }

    public static void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target?.PropertyDeclarationSyntax.Type is not GenericNameSyntax genericPropertyType) return;

        var interfaceName = SyntaxFactory.GenericName(
            SyntaxFactory.Identifier("IRestLoadableActor"),
            genericPropertyType.TypeArgumentList
        );

        var newCls = SyntaxFactory.ClassDeclaration(
            [],
            target.ClassDeclarationSyntax.Modifiers,
            target.ClassDeclarationSyntax.Keyword,
            target.ClassDeclarationSyntax.Identifier,
            null,
            SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList<BaseTypeSyntax>(
                    new []
                    {
                        SyntaxFactory.SimpleBaseType(
                            interfaceName
                        )
                    }
                )
            ),
            [],
            target.ClassDeclarationSyntax.OpenBraceToken,
            new SyntaxList<MemberDeclarationSyntax>(
                SyntaxFactory.PropertyDeclaration(
                    new SyntaxList<AttributeListSyntax>(),
                    new SyntaxTokenList(),
                    target.PropertyDeclarationSyntax.Type,
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        interfaceName,
                        SyntaxFactory.Token(SyntaxKind.DotToken)
                    ),
                    SyntaxFactory.Identifier("Loadable"),
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                        SyntaxFactory.IdentifierName("Loadable")
                    ),
                    null,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                )
            ),
            target.ClassDeclarationSyntax.CloseBraceToken,
            target.ClassDeclarationSyntax.SemicolonToken
        );

        context.AddSource(
            $"RestLoadableSource/{target.ClassDeclarationSyntax.Identifier}",
            $$"""
            using Discord.Rest.Actors;
            {{string.Join("\n", target.ClassDeclarationSyntax.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>())}}

            namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

            {{newCls.NormalizeWhitespace()}}
            """
        );
    }
}
