using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class EntityHierarchies : IGenerationCombineTask<EntityHierarchies.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol classSymbol,
        ImmutableArray<ITypeSymbol> targetInterfaces
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public INamedTypeSymbol ClassSymbol { get; } = classSymbol;
        public ImmutableArray<ITypeSymbol> TargetInterfaces { get; } = targetInterfaces;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax) &&
                   ClassSymbol.Equals(other.ClassSymbol, SymbolEqualityComparer.Default) &&
                   TargetInterfaces.Length == other.TargetInterfaces.Length &&
                   TargetInterfaces.SequenceEqual(
                       other.TargetInterfaces,
                       (a, b) => SymbolEqualityComparer.Default.Equals(a, b)
                   );
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = ClassDeclarationSyntax.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetInterfaces.GetHashCode();
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ClassSymbol);
                return hashCode;
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax {AttributeLists.Count: > 0};

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        var symbol = context.SemanticModel.GetDeclaredSymbol(target, token);

        if (symbol is null)
            return null;

        var targetInterfaces = GetTargetInterfaces(symbol);

        if (targetInterfaces is null)
            return null;

        return new GenerationTarget(
            context.SemanticModel,
            target,
            symbol,
            targetInterfaces.Value
        );
    }

    private static ImmutableArray<ITypeSymbol>? GetTargetInterfaces(ITypeSymbol symbol)
    {
        var attribute = symbol.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass?.ToDisplayString() == "Discord.ExtendInterfaceDefaultsAttribute"
        );

        if (attribute?.ConstructorArguments.Length != 1)
        {
            return null;
        }

        if (attribute.ConstructorArguments[0].Values.Length == 0)
        {
            return
                attribute.NamedArguments
                    .FirstOrDefault(x => x.Key == "ExtendAll")
                    .Value.Value as bool? ?? false
                    ? symbol.AllInterfaces.CastArray<ITypeSymbol>()
                    : symbol.Interfaces.CastArray<ITypeSymbol>();
        }

        return attribute.ConstructorArguments[0].Values.Select(x => x.Value).OfType<ITypeSymbol>().ToImmutableArray();
    }

    private static IEnumerable<ISymbol> GetTargetMembersForImplementation(IEnumerable<ITypeSymbol> symbols)
        => symbols.SelectMany(GetTargetMembersForImplementation);

    private static IEnumerable<ISymbol> GetTargetMembersForImplementation(ITypeSymbol symbols)
    {
        return symbols.GetMembers().Where(x =>
            {
                return x switch
                {
                    IPropertySymbol prop => prop is {IsVirtual: true, ExplicitInterfaceImplementations.Length: 0},
                    IMethodSymbol method => method is
                    {
                        IsVirtual: true, IsAsync: false, MethodKind: MethodKind.Ordinary,
                        ExplicitInterfaceImplementations.Length: 0
                    },
                    _ => false
                };
            }
        );
    }

    private static ITypeSymbol UnAsyncType(ITypeSymbol type)
    {
        return type is INamedTypeSymbol
        {
            IsGenericType: true,
            Name: "ValueTask" or "Task"
        } asyncType
            ? asyncType.TypeArguments[0]
            : type;
    }

    private static ITypeSymbol GetTargetTypeForImplementation(
        ISymbol targetMember,
        ITypeSymbol exampleCase,
        INamedTypeSymbol target,
        SemanticModel semanticModel)
    {
        var heuristic = (
                targetMember is IMethodSymbol method
                    ? method.GetReturnTypeAttributes()
                    : target.GetAttributes()
            )
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == "Discord.TypeHeuristicAttribute");

        if (heuristic?.ConstructorArguments.FirstOrDefault().Value is not string memberName)
            return exampleCase;

        if (UnAsyncType(exampleCase) is not INamedTypeSymbol unAsyncType)
            return exampleCase;

        foreach (var element in Hierarchy.GetHierarchy(targetMember.ContainingType)
                     .Select(x => x.Type)
                     .Prepend(targetMember.ContainingType))
        {
            var member = element
                .GetMembers()
                .FirstOrDefault(x => x.Name == memberName);

            if (member is null)
            {
                continue;
            }

            var implementation = target.FindImplementationForInterfaceMember(member);

            // impl can be source of truth'd
            if (implementation is null)
            {
                foreach (var targetSymbol in TypeUtils.GetBaseTypesAndThis(target))
                {
                    var searchMember = targetSymbol.GetMembers(memberName).FirstOrDefault();
                    if (searchMember is null || member.Kind != searchMember.Kind ||
                        !SourceOfTruth.IsTarget(searchMember))
                        continue;

                    implementation = searchMember;
                    break;
                }
            }

            if (implementation is null)
            {
                continue;
            }

            // resolve the type for the heuristic
            var heuristicType = member switch
            {
                IPropertySymbol {Type: INamedTypeSymbol sourceType} when
                    implementation is IPropertySymbol {Type: INamedTypeSymbol pairType}
                    => TypeUtils.PairedWalkTypeSymbolForMatch(
                        unAsyncType,
                        sourceType,
                        pairType,
                        semanticModel
                    ),
                IMethodSymbol {ReturnType: INamedTypeSymbol sourceType} when
                    implementation is IMethodSymbol {ReturnType: INamedTypeSymbol pairType} &&
                    UnAsyncType(pairType) is INamedTypeSymbol unAsyncPairType
                    => TypeUtils.PairedWalkTypeSymbolForMatch(
                        unAsyncType,
                        sourceType,
                        unAsyncPairType,
                        semanticModel
                    ),
                _ => null
            };

            if (heuristicType is null)
            {
                break;
            }

            //correct async
            if (
                heuristicType is not {IsGenericType: true, Name: "ValueTask" or "Task"} &&
                exampleCase is INamedTypeSymbol {IsGenericType: true, Name: "ValueTask" or "Task"} asyncType
            )
            {
                return asyncType
                    .ConstructedFrom
                    .Construct(heuristicType.WithNullableAnnotation(asyncType.TypeArguments[0]
                        .NullableAnnotation));
            }

            return heuristicType;
        }

        return exampleCase;
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var map = targets
            .Where(x => x is not null)
            .OfType<GenerationTarget>()
            .ToDictionary(x => x.ClassSymbol, SymbolEqualityComparer.Default);

        var bases = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        var hierarchyOrder = map.Values.OrderBy(x =>
        {
            var baseType = x.ClassSymbol.BaseType;

            if (baseType is null)
                return 0;

            var count = 0;

            do
            {
                if (map.ContainsKey(baseType))
                {
                    bases.Add(baseType);
                    count++;
                }
            } while ((baseType = baseType.BaseType) is not null);

            return count;
        }).ToArray();

        for (var index = 0; index < hierarchyOrder.Length; index++)
        {
            var target = hierarchyOrder[index];
            if (target is null)
            {
                continue;
            }

            var declaration = SyntaxFactory.ClassDeclaration(
                [],
                target.ClassDeclarationSyntax.Modifiers,
                target.ClassDeclarationSyntax.Identifier,
                target.ClassDeclarationSyntax.TypeParameterList,
                null,
                target.ClassDeclarationSyntax.ConstraintClauses,
                []
            );

            var baseTree =
                target.ClassSymbol.BaseType is not null
                    ? TypeUtils.GetBaseTypesAndThis(target.ClassSymbol.BaseType).ToArray()
                    : [];

            var baseTreeMembers = baseTree.SelectMany(x =>
                (ISymbol[])
                [
                    ..x.GetMembers(),
                    ..map.TryGetValue(x, out var baseTarget)
                        ? GetTargetMembersForImplementation(baseTarget.TargetInterfaces)
                        : []
                ]
            ).ToArray();

            foreach (var targetInterface in target.TargetInterfaces)
            {
                var toImplement = GetTargetMembersForImplementation(targetInterface);

                var implemented = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

                foreach (var member in toImplement)
                {
                    if (target.ClassSymbol.GetMembers(member.Name).Length > 0)
                    {
                        continue;
                    }

                    if (implemented.Any(x => MemberUtils.Conflicts(x, member)) || !implemented.Add(member))
                        continue;

                    var baseImplementation = baseTreeMembers
                        .FirstOrDefault(y =>
                            MemberUtils.CanOverride(member, y, target.SemanticModel.Compilation)
                        );

                    var shouldBeNew = baseImplementation is null &&
                                      baseTreeMembers
                                          .Any(x =>
                                              MemberUtils.Conflicts(x, member)
                                          );

                    var modifiers = SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    );

                    if (bases.Contains(target.ClassSymbol) && baseImplementation is null)
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

                    if (baseImplementation is not null &&
                        MemberUtils.CanOverride(
                            member,
                            baseImplementation,
                            target.SemanticModel.Compilation
                        ))
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));

                    if (shouldBeNew)
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword));

                    switch (member)
                    {
                        case IPropertySymbol property:
                            var propName = property.ExplicitInterfaceImplementations.Length > 0
                                ? property.ExplicitInterfaceImplementations[0].Name
                                : property.Name;

                            declaration = declaration.AddMembers(
                                SyntaxFactory.PropertyDeclaration(
                                    [],
                                    modifiers,
                                    SyntaxFactory.ParseTypeName(property.Type.ToDisplayString()),
                                    null,
                                    SyntaxFactory.Identifier(propName),
                                    null,
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.ParenthesizedExpression(
                                                SyntaxFactory.BinaryExpression(
                                                    SyntaxKind.AsExpression,
                                                    SyntaxFactory.ThisExpression(),
                                                    SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString())
                                                )
                                            ),
                                            SyntaxFactory.IdentifierName(propName)
                                        )
                                    ),
                                    null,
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                ).WithLeadingTrivia(
                                    SyntaxFactory.Comment($"// {property}")
                                )
                            );

                            break;

                        case IMethodSymbol methodSymbol:
                            var methodName = methodSymbol.ExplicitInterfaceImplementations.Length > 0
                                ? methodSymbol.ExplicitInterfaceImplementations[0].Name
                                : methodSymbol.Name;

                            ExpressionSyntax body = SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.ParenthesizedExpression(
                                        SyntaxFactory.BinaryExpression(
                                            SyntaxKind.AsExpression,
                                            SyntaxFactory.ThisExpression(),
                                            SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString())
                                        )
                                    ),
                                    SyntaxFactory.IdentifierName(methodName)
                                ),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList(
                                        methodSymbol.Parameters.Select(x =>
                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Name))
                                        )
                                    )
                                )
                            );

                            var returnType = GetTargetTypeForImplementation(
                                methodSymbol,
                                methodSymbol.ReturnType,
                                target.ClassSymbol,
                                target.SemanticModel
                            );

                            var hasDifferentReturnType =
                                !returnType.Equals(methodSymbol.ReturnType, SymbolEqualityComparer.Default);

                            if (hasDifferentReturnType &&
                                returnType is INamedTypeSymbol namedReturnType)
                            {
                                var isAsync = namedReturnType is {IsGenericType: true, Name: "Task" or "ValueTask"};
                                if (isAsync)
                                    modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

                                body = SyntaxFactory.CastExpression(
                                    SyntaxFactory.ParseTypeName(
                                        isAsync
                                            ? namedReturnType.TypeArguments[0].ToDisplayString()
                                            : namedReturnType.ToDisplayString()
                                    ),
                                    SyntaxFactory.ParenthesizedExpression(
                                        isAsync ? SyntaxFactory.AwaitExpression(body) : body
                                    )
                                );
                            }

                            declaration = declaration.AddMembers(
                                SyntaxFactory.MethodDeclaration(
                                        [],
                                        modifiers,
                                        SyntaxFactory.ParseTypeName(
                                            returnType.ToDisplayString()
                                        ),
                                        null,
                                        SyntaxFactory.Identifier(methodName),
                                        methodSymbol.TypeParameters.Length > 0
                                            ? SyntaxFactory.TypeParameterList(
                                                SyntaxFactory.SeparatedList(
                                                    methodSymbol.TypeParameters.Select(x =>
                                                        SyntaxFactory.TypeParameter(
                                                            x.Name
                                                        )
                                                    )
                                                )
                                            )
                                            : null,
                                        SyntaxFactory.ParameterList(
                                            SyntaxFactory.SeparatedList(
                                                methodSymbol.Parameters.Select(x =>
                                                    SyntaxFactory.Parameter(
                                                        [],
                                                        [],
                                                        SyntaxFactory.ParseTypeName(x.Type.ToDisplayString()),
                                                        SyntaxFactory.Identifier(x.Name),
                                                        x.HasExplicitDefaultValue
                                                            ? SyntaxFactory.EqualsValueClause(
                                                                SyntaxUtils.CreateLiteral(x.Type,
                                                                    x.ExplicitDefaultValue)
                                                            )
                                                            : null
                                                    )
                                                )
                                            )
                                        ),
                                        [],
                                        null,
                                        SyntaxFactory.ArrowExpressionClause(
                                            body
                                        ),
                                        SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                    )
                                    .WithLeadingTrivia(
                                        SyntaxFactory.Comment($"// {methodSymbol}")
                                    )
                            );
                            break;
                    }
                }
            }

            if (declaration.Members.Count == 0)
                continue;

            context.AddSource(
                $"EntityHierarchies/{target.ClassDeclarationSyntax.Identifier}",
                $$"""
                  {{target.ClassDeclarationSyntax.GetFormattedUsingDirectives()}}

                  namespace {{target.SemanticModel.GetDeclaredSymbol(target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  #nullable enable

                  {{declaration.NormalizeWhitespace()}}

                  #nullable restore
                  """
            );
        }
    }
}
