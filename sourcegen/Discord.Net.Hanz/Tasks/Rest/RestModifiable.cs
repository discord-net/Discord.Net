using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Runtime.InteropServices.ComTypes;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace Discord.Net.Hanz.Tasks;

public class RestModifiable : IGenerationCombineTask<RestModifiable.GenerationTarget>
{
    public static readonly DiagnosticDescriptor EntityTypeCannotBeFound = new(
        "HZ0001",
        "Cannot resolve entity type of actor",
        "Cannot resolve the entity type of the actor {0}",
        "Rest Modifiable",
        DiagnosticSeverity.Error,
        true
    );

    public class GenerationTarget(
        ClassDeclarationSyntax classDeclaration,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol[] interfaceSymbols,
        SemanticModel semanticModel
    ) : IEquatable<GenerationTarget>
    {
        public ClassDeclarationSyntax ClassDeclaration { get; } = classDeclaration;
        public INamedTypeSymbol ClassSymbol { get; } = classSymbol;
        public INamedTypeSymbol[] InterfaceSymbols { get; } = interfaceSymbols;
        public SemanticModel SemanticModel { get; } = semanticModel;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclaration.IsEquivalentTo(other.ClassDeclaration) &&
                   ClassSymbol.Equals(other.ClassSymbol, SymbolEqualityComparer.Default) &&
                   InterfaceSymbols.SequenceEqual(other.InterfaceSymbols, SymbolEqualityComparer.Default);
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
                var hashCode = ClassDeclaration.GetHashCode();
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ClassSymbol);
                hashCode = (hashCode * 397) ^ InterfaceSymbols.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
        => node is ClassDeclarationSyntax;

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax classSyntax) return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classSyntax) is not INamedTypeSymbol classSymbol)
            return null;

        if (classSymbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.ILoadable")))
            return null;

        var interfaces = classSymbol.AllInterfaces
            .Where(x => x.ToDisplayString().StartsWith("Discord.IModifiable"))
            .ToArray();

        if (interfaces.Length == 0) return null;

        return new GenerationTarget(
            classSyntax,
            classSymbol,
            interfaces,
            context.SemanticModel
        );
    }

    private static ITypeSymbol? ResolveActorEntityType(INamedTypeSymbol actor, ClassDeclarationSyntax syntax,
        SemanticModel semanticModel)
    {
        var actorInterface = actor.Interfaces.FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

        if (actorInterface is not null)
            return actorInterface.TypeArguments[1];

        if (actor.BaseType?.Name == "RestActor")
            return actor.BaseType.TypeArguments[1];

        if (syntax.Identifier.ValueText.EndsWith("Actor"))
        {
            var entity = syntax.SyntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault(x =>
                    x.Identifier.ValueText == syntax.Identifier.ValueText.Replace("Actor", string.Empty));

            if (entity is not null)
                return semanticModel.GetDeclaredSymbol(entity);
        }

        return null;
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets)
    {
        foreach (var target in targets)
        {
            if (target is null) continue;

            var baseTarget = targets.FirstOrDefault(x =>
                target.ClassSymbol.BaseType is not null &&
                (x?.ClassSymbol.Equals(target.ClassSymbol.BaseType, SymbolEqualityComparer.Default) ?? false)
            );

            GenerateModifyForClass(context, target, baseTarget);
        }
    }

    private static void GenerateModifyForClass(
        SourceProductionContext context,
        GenerationTarget target,
        GenerationTarget? baseTarget)
    {
        var declaration = ClassDeclaration(
            [],
            target.ClassDeclaration.Modifiers,
            target.ClassDeclaration.Identifier,
            target.ClassDeclaration.TypeParameterList,
            null,
            target.ClassDeclaration.ConstraintClauses,
            []
        );

        var actorInterface = target.ClassSymbol.Interfaces.FirstOrDefault(x =>
            x.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IActor")) &&
            x.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.IModifiable"))
        );

        // determine the type (actor vs. entity)
        var entityModifiable = target.InterfaceSymbols.FirstOrDefault(x => x.TypeArguments.Length == 5);

        var actorModifiable =
            actorInterface?.Interfaces.FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IModifiable"));

        if (entityModifiable is null && actorModifiable is null) return;

        if (actorModifiable is not null && entityModifiable is null)
        {
            // don't remove this, or you'll be fired
            if (!actorModifiable.TypeArguments[1].Equals(actorInterface, SymbolEqualityComparer.Default))
                return;

            // get the entity type
            var actorType = ResolveActorEntityType(target.ClassSymbol, target.ClassDeclaration, target.SemanticModel);

            if (actorType is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    EntityTypeCannotBeFound,
                    target.ClassDeclaration.GetLocation(),
                    target.ClassDeclaration.Identifier.ValueText
                ));
                Hanz.Logger.Log($"No entity found for actor {target.ClassSymbol.Name}");
                return;
            }

            var paramsType = actorModifiable.TypeArguments[2];

            var modifiers = TokenList(
                Token(SyntaxKind.PublicKeyword)
            );

            if (baseTarget is not null)
                modifiers = modifiers.Add(Token(SyntaxKind.NewKeyword));

            modifiers = modifiers.AddRange([
                Token(SyntaxKind.AsyncKeyword)
            ]);

            #region ModifyAsync

            declaration = declaration.AddMembers(
                MethodDeclaration(
                    [],
                    modifiers,
                    GenericName(
                        Identifier("Task"),
                        TypeArgumentList(
                            SeparatedList([
                                ParseTypeName(actorType.ToDisplayString())
                            ])
                        )
                    ),
                    null,
                    Identifier("ModifyAsync"),
                    null,
                    ParameterList(
                        SeparatedList([
                            Parameter(
                                [],
                                [],
                                GenericName(
                                    Identifier("Action"),
                                    TypeArgumentList(
                                        SeparatedList([
                                            ParseTypeName(paramsType.ToDisplayString())
                                        ])
                                    )
                                ),
                                Identifier("func"),
                                null
                            ),
                            Parameter(
                                [],
                                [],
                                NullableType(
                                    IdentifierName("global::Discord.RequestOptions")),
                                Identifier("options"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.NullLiteralExpression))
                            ),
                            Parameter(
                                [],
                                [],
                                IdentifierName("global::System.Threading.CancellationToken"),
                                Identifier("token"),
                                EqualsValueClause(
                                    LiteralExpression(SyntaxKind.DefaultLiteralExpression))
                            )
                        ])
                    ),
                    [],
                    Block(
                        LocalDeclarationStatement(
                            [],
                            VariableDeclaration(
                                IdentifierName("var"),
                                SeparatedList([
                                    VariableDeclarator(
                                        Identifier("model"),
                                        null,
                                        EqualsValueClause(
                                            AwaitExpression(
                                                InvocationExpression(
                                                    MemberAccessExpression(
                                                        SyntaxKind.SimpleMemberAccessExpression,
                                                        ParseTypeName(actorModifiable.ToDisplayString()),
                                                        IdentifierName("ModifyAndReturnModelAsync")
                                                    ),
                                                    ArgumentList(
                                                        SeparatedList([
                                                            Argument(
                                                                IdentifierName("Client")),
                                                            Argument(ThisExpression()),
                                                            Argument(IdentifierName("Id")),
                                                            Argument(
                                                                IdentifierName("func")),
                                                            Argument(
                                                                IdentifierName("options")),
                                                            Argument(
                                                                IdentifierName("token"))
                                                        ])
                                                    )
                                                )
                                            )
                                        )
                                    )
                                ])
                            )
                        ),
                        ReturnStatement(
                            InvocationExpression(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName("CreateEntity")
                                ),
                                ArgumentList(
                                    SeparatedList([
                                        Argument(
                                            IdentifierName("model")
                                        )
                                    ])
                                )
                            )
                        )
                    ),
                    null
                )
            );

            #endregion
        }

        if (declaration.Members.Count == 0) return;

        context.AddSource(
            $"RestModifiables/{target.ClassSymbol.Name}",
            $$"""
              {{target.ClassDeclaration.GetFormattedUsingDirectives()}}

              namespace {{target.ClassSymbol.ContainingNamespace}};

              {{declaration.NormalizeWhitespace()}}
              """
        );
    }
}
