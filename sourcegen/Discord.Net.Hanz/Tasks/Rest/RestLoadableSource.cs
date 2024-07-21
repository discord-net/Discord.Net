using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public class RestLoadableSource : IGenerationTask<RestLoadableSource.GenerationTarget>
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        PropertyDeclarationSyntax propertyDeclarationSyntax,
        ClassDeclarationSyntax classDeclarationSyntax
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public PropertyDeclarationSyntax PropertyDeclarationSyntax { get; } = propertyDeclarationSyntax;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return PropertyDeclarationSyntax.IsEquivalentTo(other.PropertyDeclarationSyntax) &&
                   ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GenerationTarget) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (PropertyDeclarationSyntax.GetHashCode() * 397) ^ ClassDeclarationSyntax.GetHashCode();
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        if (node is not PropertyDeclarationSyntax property) return false;

        return property.AttributeLists.Count > 0;
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
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

    public void Execute(SourceProductionContext context, GenerationTarget? target, Logger logger)
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
            {{string.Join("\n", target.ClassDeclarationSyntax.SyntaxTree.GetRoot().ChildNodes().OfType<UsingDirectiveSyntax>())}}

            namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

            {{newCls.NormalizeWhitespace()}}
            """
        );
    }
}
