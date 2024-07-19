using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public class ModelEquality : IGenerationTask<ModelEquality.GenerationTarget>
{
    public class GenerationTarget(
        SemanticModel semanticModel,
        INamedTypeSymbol typeSymbol,
        TypeDeclarationSyntax typeDeclarationSyntax
    ) : IEquatable<GenerationTarget>
    {
        public INamedTypeSymbol TypeSymbol { get; } = typeSymbol;
        public TypeDeclarationSyntax TypeDeclarationSyntax { get; } = typeDeclarationSyntax;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return TypeSymbol.Equals(other.TypeSymbol, SymbolEqualityComparer.Default) &&
                   TypeDeclarationSyntax.IsEquivalentTo(other.TypeDeclarationSyntax);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GenerationTarget)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (SymbolEqualityComparer.Default.GetHashCode(TypeSymbol) * 397) ^ TypeDeclarationSyntax.GetHashCode();
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        return node is TypeDeclarationSyntax {AttributeLists.Count: > 0, BaseList: not null};
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not TypeDeclarationSyntax {AttributeLists.Count: > 0, BaseList: not null} type) return null;

        if (type.AttributeLists
            .SelectMany(x => x.Attributes)
            .All(x => Attributes.GetAttributeName(x, context.SemanticModel) != "Discord.ModelEqualityAttribute"))
            return null;

        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(type);

        if (typeSymbol is null) return null;

        if (typeSymbol.AllInterfaces.All(x => x.ToDisplayString() != "Discord.Models.IEntityModel"))
            return null;

        if (type.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

        return new GenerationTarget(context.SemanticModel, typeSymbol, type);
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target)
    {
        if (target is null) return;

        try
        {
            var iEquatableInterfaceSyntax = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier("IEquatable"),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList(
                        new TypeSyntax[] {SyntaxFactory.IdentifierName(target.TypeSymbol.Name)}
                    )
                )
            );

            var syntax = target.TypeDeclarationSyntax
                .WithMembers([])
                .WithAttributeLists([])
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                .WithBaseList(SyntaxFactory.BaseList(
                    SyntaxFactory.SeparatedList(
                        new BaseTypeSyntax[]
                        {
                            SyntaxFactory.SimpleBaseType(
                                iEquatableInterfaceSyntax
                            )
                        }
                    )
                ));

            var otherEquatableInterfaces = target.TypeSymbol
                .AllInterfaces
                .Where(x => x.Name != "IEquatable")
                .Where(iface =>
                    iface
                        .GetMembers("Equals")
                        .OfType<IMethodSymbol>()
                        .Any()
                    || iface
                        .GetAttributes()
                        .Any(x =>
                            x.AttributeClass?.ToDisplayString() == "Discord.ModelEqualityAttribute"
                        )
                )
                .Where(x => x.DeclaringSyntaxReferences.Length > 0)
                .ToArray();

            var properties = target.TypeSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.ExplicitInterfaceImplementations.Length == 0)
                .ToArray();

            var interfaceConditions = otherEquatableInterfaces
                .SelectMany(x => x
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(x => properties.All(y => y.Name != x.Name))
                )
                .Select(x => GenerateEqualityCheckForProperty(
                        x,
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName(x.Name)
                        )
                    )
                );

            var equalsMethod = GenerateEqualsMethod(target, properties, interfaceConditions);

            if (equalsMethod is null)
            {
                Hanz.Logger.Warn(
                    $"No properties exist on {target.TypeSymbol.ToDisplayString()} that can be used for equality");
                return;
            }

            syntax = syntax.AddMembers([
                equalsMethod,
                SyntaxFactory.MethodDeclaration(
                    [],
                    [],
                    SyntaxFactory.IdentifierName("bool"),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        iEquatableInterfaceSyntax
                    ),
                    SyntaxFactory.Identifier("Equals"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(
                            [
                                SyntaxFactory.Parameter(
                                    [],
                                    [],
                                    SyntaxFactory.NullableType(SyntaxFactory.IdentifierName(target.TypeSymbol.Name)),
                                    SyntaxFactory.Identifier("other"),
                                    null
                                )
                            ]
                        )
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.InvocationExpression(
                            SyntaxFactory.IdentifierName("Equals"),
                            SyntaxFactory.ArgumentList(
                                SyntaxFactory.SeparatedList([
                                    SyntaxFactory.Argument(
                                        SyntaxFactory.IdentifierName("other")
                                    )
                                ])
                            )
                        )
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ),
                SyntaxFactory.MethodDeclaration(
                    [],
                    [],
                    SyntaxFactory.IdentifierName("bool"),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        SyntaxFactory.GenericName(
                            SyntaxFactory.Identifier("IEquatable"),
                            SyntaxFactory.TypeArgumentList(
                                SyntaxFactory.SeparatedList(
                                    new TypeSyntax[] {SyntaxFactory.IdentifierName("IEntityModel")}
                                )
                            )
                        )
                    ),
                    SyntaxFactory.Identifier("Equals"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList(
                            [
                                SyntaxFactory.Parameter(
                                    [],
                                    [],
                                    SyntaxFactory.NullableType(SyntaxFactory.IdentifierName("IEntityModel")),
                                    SyntaxFactory.Identifier("other"),
                                    null
                                )
                            ]
                        )
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            SyntaxFactory.IsPatternExpression(
                                SyntaxFactory.IdentifierName("other"),
                                SyntaxFactory.DeclarationPattern(
                                    SyntaxFactory.IdentifierName(target.TypeSymbol.Name),
                                    SyntaxFactory.SingleVariableDesignation(
                                        SyntaxFactory.Identifier("otherThis")
                                    )
                                )
                            ),
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("Equals"),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList([
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.IdentifierName("otherThis")
                                        )
                                    ])
                                )
                            )
                        )
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ),
                ..otherEquatableInterfaces.Select(x => SyntaxFactory.MethodDeclaration(
                    [],
                    [],
                    SyntaxFactory.IdentifierName("bool"),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        SyntaxFactory.IdentifierName(x.Name)
                    ),
                    SyntaxFactory.Identifier("Equals"),
                    null,
                    SyntaxFactory.ParameterList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.Parameter(
                                [],
                                [],
                                SyntaxFactory.NullableType(SyntaxFactory.IdentifierName(x.Name)),
                                SyntaxFactory.Identifier("other"),
                                null
                            )
                        ])
                    ),
                    [],
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            SyntaxFactory.IsPatternExpression(
                                SyntaxFactory.IdentifierName("other"),
                                SyntaxFactory.DeclarationPattern(
                                    SyntaxFactory.IdentifierName(target.TypeSymbol.Name),
                                    SyntaxFactory.SingleVariableDesignation(
                                        SyntaxFactory.Identifier("otherThis")
                                    )
                                )
                            ),
                            SyntaxFactory.InvocationExpression(
                                SyntaxFactory.IdentifierName("Equals"),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList([
                                        SyntaxFactory.Argument(
                                            SyntaxFactory.IdentifierName("otherThis")
                                        )
                                    ])
                                )
                            )
                        )
                    ),
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                ))
            ]);

            context.AddSource(
                $"ModelEquality/{target.TypeSymbol.Name}",
                $$"""
                  {{target.TypeDeclarationSyntax.GetFormattedUsingDirectives()}}

                  namespace {{target.TypeSymbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
        catch (Exception x)
        {
            Hanz.Logger.Log(LogLevel.Error, x.ToString());
        }
    }

    private static MemberDeclarationSyntax? GenerateEqualsMethod(
        GenerationTarget target,
        IEnumerable<IPropertySymbol> properties,
        IEnumerable<ExpressionSyntax> otherConditions)
    {
        var otherParameterSyntax = SyntaxFactory.Parameter(
            [],
            [],
            SyntaxFactory.IdentifierName($"{target.TypeSymbol.ToDisplayString()}?"),
            SyntaxFactory.Identifier("other"),
            null
        );

        var comparisons = properties
            .Select(property => GenerateEqualityCheckForProperty(
                property,
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.ThisExpression(),
                    SyntaxFactory.IdentifierName(property.Name)
                )
            ))
            .Concat(otherConditions)
            .ToArray();

        if (comparisons.Length == 0)
            return null;

        return SyntaxFactory.MethodDeclaration(
            [],
            SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.NewKeyword)),
            SyntaxFactory.IdentifierName("bool"),
            null,
            SyntaxFactory.Identifier("Equals"),
            null,
            SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    [otherParameterSyntax]
                )
            ),
            [],
            SyntaxFactory.Block(
                SyntaxFactory.IfStatement(
                    SyntaxFactory.IsPatternExpression(
                        SyntaxFactory.IdentifierName("other"),
                        SyntaxFactory.ConstantPattern(
                            SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))
                    ),
                    SyntaxFactory.ReturnStatement(
                        SyntaxFactory.LiteralExpression(SyntaxKind.FalseLiteralExpression))
                ),
                SyntaxFactory.ReturnStatement(comparisons
                    .Aggregate((a, b) => SyntaxFactory.BinaryExpression(
                            SyntaxKind.LogicalAndExpression,
                            a,
                            b
                        )
                    )
                )
            ),
            null
        );
    }

    private static ExpressionSyntax GenerateEqualityCheckForProperty(
        IPropertySymbol property,
        ExpressionSyntax compareAgainst)
    {
        // this.A?.Equals(other.A) ?? other.A is null

        var isNullable = property.NullableAnnotation is NullableAnnotation.Annotated ||
                         property.Type.NullableAnnotation is NullableAnnotation.Annotated;


        var propertyAccess = SyntaxFactory.MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            SyntaxFactory.ThisExpression(),
            SyntaxFactory.IdentifierName(property.Name)
        );

        var comparisonMethod = property.Type.SpecialType switch
        {
            SpecialType.System_Array or SpecialType.System_Collections_IEnumerable
                or SpecialType.System_Collections_Generic_IEnumerable_T
                or SpecialType.System_Collections_Generic_IList_T
                or SpecialType.System_Collections_Generic_ICollection_T
                or SpecialType.System_Collections_Generic_IReadOnlyList_T
                or SpecialType.System_Collections_Generic_IReadOnlyCollection_T
                => "SequenceEqual",
            _ => "Equals"
        };

        var invocation = SyntaxFactory.InvocationExpression(
            isNullable
                ? SyntaxFactory.MemberBindingExpression(SyntaxFactory.IdentifierName(comparisonMethod))
                : SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    propertyAccess,
                    SyntaxFactory.IdentifierName(comparisonMethod)
                ),
            SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList([SyntaxFactory.Argument(compareAgainst)])
            )
        );

        if (!isNullable) return invocation;

        return SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.CoalesceExpression,
                SyntaxFactory.ConditionalAccessExpression(
                    propertyAccess,
                    invocation
                ),
                SyntaxFactory.IsPatternExpression(
                    compareAgainst,
                    SyntaxFactory.UnaryPattern(
                        SyntaxFactory.Token(SyntaxKind.NotKeyword),
                        SyntaxFactory.ConstantPattern(SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression))
                    )
                )
            )
        );
    }
}
