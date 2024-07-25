using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using CSharpExtensions = Microsoft.CodeAnalysis.CSharp.CSharpExtensions;

namespace Discord.Net.Hanz.Tasks;

public class ExtendInterfaceDefaults : IGenerationCombineTask<ExtendInterfaceDefaults.GenerationTarget>
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

        var targetInterfaces = GetTargetInterfaces(symbol, logger);

        logger.Log($"{symbol} has {targetInterfaces?.Length} targets");

        if (targetInterfaces.HasValue)
        {
            foreach (var iface in targetInterfaces.Value)
            {
                logger.Log($" - {iface}");
            }
        }

        if (targetInterfaces is null)
            return null;

        return new GenerationTarget(
            context.SemanticModel,
            target,
            symbol,
            targetInterfaces.Value
        );
    }

    private static bool IsTemplateExtensionInterface(INamedTypeSymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Any(y => y.AttributeClass?.ToDisplayString() == "Discord.TemplateExtensionAttribute");
    }

    public static HashSet<INamedTypeSymbol> CorrectTemplateExtensionInterfaces(IEnumerable<INamedTypeSymbol> elements)
    {
        var rootSet = new HashSet<INamedTypeSymbol>(elements, SymbolEqualityComparer.Default);
        var templates = rootSet
            .Where(IsTemplateExtensionInterface)
            .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);
        // we only want to return the lower-order version of the template extension, this computation isn't fast but
        // it works.
        foreach (var element in templates)
        {
            if (templates.Any(x => x.AllInterfaces.Contains(element, SymbolEqualityComparer.Default)))
            {
                rootSet.Remove(element);
            }

            // take precedence into effect
            var attribute = element.GetAttributes().First(x =>
                x.AttributeClass?.ToDisplayString() == "Discord.TemplateExtensionAttribute"
            );

            var precedenceOverType = attribute.NamedArguments
                .FirstOrDefault(x => x.Key == "TakesPrecedenceOver")
                .Value.Value;

            if (precedenceOverType is INamedTypeSymbol namedType)
            {
                foreach (var toRemove in templates.Where(x => TypeUtils.TypeLooselyEquals(x, namedType)))
                {
                    rootSet.Remove(toRemove);
                }
            }
        }

        return rootSet;
    }

    public static HashSet<INamedTypeSymbol> GetTemplateExtensionInterfaces(
        ITypeSymbol symbol)
    {
        return CorrectTemplateExtensionInterfaces(
            symbol.AllInterfaces
                .Where(IsTemplateExtensionInterface)
        );
    }

    public static ImmutableArray<ITypeSymbol>? GetTargetInterfaces(
        ITypeSymbol symbol,
        Logger logger)
    {
        var attribute = symbol.GetAttributes().FirstOrDefault(x =>
            x.AttributeClass?.ToDisplayString() == "Discord.ExtendInterfaceDefaultsAttribute"
        );

        if (attribute is null)
            return null;

        if (
            attribute.ConstructorArguments.Length == 0 ||
            attribute.ConstructorArguments[0].Values.Length == 0)
        {
            return
                attribute.NamedArguments
                    .FirstOrDefault(x => x.Key == "ExtendAll")
                    .Value.Value as bool? ?? false
                    ? symbol.AllInterfaces.CastArray<ITypeSymbol>()
                    : symbol.Interfaces.CastArray<ITypeSymbol>()
                        .AddRange(GetTemplateExtensionInterfaces(symbol));
        }

        return attribute.ConstructorArguments[0].Values
            .Select(x => x.Value)
            .OfType<ITypeSymbol>()
            .Concat(GetTemplateExtensionInterfaces(symbol))
            .ToImmutableArray();
    }

    public static IEnumerable<ISymbol> GetTargetMembersForImplementation(
        IEnumerable<ITypeSymbol> symbols,
        ITypeSymbol classSymbol)
        => symbols.SelectMany(x => GetTargetMembersForImplementation(x, classSymbol));

    public static IEnumerable<ISymbol> GetTargetMembersForImplementation(
        ITypeSymbol interfaceSymbol,
        ITypeSymbol classSymbol)
    {
        var willHaveFetchMethods = RestLoadable.WillHaveFetchMethods(classSymbol);
        var isTemplateExtensionInterface =
            interfaceSymbol is INamedTypeSymbol namedInterfaceSymbol &&
            IsTemplateExtensionInterface(namedInterfaceSymbol);

        return interfaceSymbol.GetMembers()
            .Where(x =>
                {
                    if (willHaveFetchMethods &&
                        x is IMethodSymbol memberMethod &&
                        MemberUtils.GetMemberName(
                            memberMethod,
                            x => x.ExplicitInterfaceImplementations
                        ) == "FetchAsync")
                        return false;

                    if (
                        isTemplateExtensionInterface &&
                        (
                            !classSymbol.FindImplementationForInterfaceMember(x)
                                ?.ContainingType
                                .Equals(x.ContainingType, SymbolEqualityComparer.Default) ?? false
                        )
                    )
                    {
                        // template has a different definition, don't include this member
                        return false;
                    }

                    return x switch
                    {
                        IPropertySymbol prop => prop is
                                                {
                                                    IsStatic: false, ExplicitInterfaceImplementations.Length: 0
                                                } &&
                                                (prop.IsVirtual || prop is {IsVirtual: false, IsAbstract: false}),
                        IMethodSymbol method => method is
                        {
                            MethodKind: MethodKind.Ordinary,
                            ExplicitInterfaceImplementations.Length: 0,
                            IsStatic: false,
                        } && (method.IsVirtual || method is {IsVirtual: false, IsAbstract: false}),
                        _ => false
                    };
                }
            );
    }

    private static ITypeSymbol ReplaceTypeReference(
        ITypeSymbol toReplaceIn,
        ITypeSymbol toReplace,
        ITypeSymbol replacement)
    {
        if (toReplaceIn.Equals(toReplace, SymbolEqualityComparer.Default))
            return replacement;

        if (toReplaceIn is not INamedTypeSymbol {IsGenericType: true} namedToReplaceIn) return toReplaceIn;

        foreach (var typeArg in namedToReplaceIn.TypeArguments)
        {
            if (typeArg is not INamedTypeSymbol namedTypeArg) continue;

            var replaced = ReplaceTypeReference(namedTypeArg, toReplace, replacement);

            if (!replaced.Equals(namedTypeArg, SymbolEqualityComparer.Default))
            {
                toReplaceIn = namedToReplaceIn
                    .ConstructedFrom
                    .Construct(
                        namedToReplaceIn.TypeArguments.Replace(namedTypeArg, replaced).ToArray()
                    );
            }
        }

        return toReplaceIn;
    }

    public static ITypeSymbol GetTargetTypeForImplementation(
        ISymbol targetMember,
        ITypeSymbol exampleCase,
        INamedTypeSymbol target,
        SemanticModel semanticModel,
        Logger logger)
    {
        var heuristic = (
                targetMember is IMethodSymbol method
                    ? method.GetReturnTypeAttributes()
                    : target.GetAttributes()
            )
            .FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString().StartsWith("Discord.TypeHeuristicAttribute") ?? false);

        if (heuristic is null)
            return exampleCase;

        logger.Log($"{targetMember}: Found heuristic data {heuristic}");

        if (heuristic?.ConstructorArguments.FirstOrDefault().Value is not string memberName)
        {
            logger.Log($"{targetMember}: no heuristic member name supplied");
            return exampleCase;
        }

        var targetMemberType = MemberUtils.GetMemberType(targetMember);

        if (targetMemberType is null)
        {
            logger.Log($"{targetMember}: no target member type supplied");
            return exampleCase;
        }

        ISymbol? heuristicMember = null;

        // also need to lookup type if it's there
        if (heuristic.AttributeClass?.IsGenericType ?? false)
        {
            var genericType = heuristic.AttributeClass.TypeArguments[0];

            // make sure the target type implements the interface
            if (!semanticModel.Compilation.HasImplicitConversion(target, genericType))
            {
                logger.Warn($"{targetMember}: Heuristic type {genericType} is not implemented by {target}");
                return exampleCase;
            }

            heuristicMember = genericType.GetMembers(memberName).FirstOrDefault();
        }
        else
        {
            // otherwise we do a search for the heuristic member and find the first match by name

            foreach (var element in Hierarchy.GetHierarchy(targetMember.ContainingType)
                         .Select(x => x.Type)
                         .Prepend(targetMember.ContainingType))
            {
                var member = element
                    .GetMembers()
                    .FirstOrDefault(x => x.Name == memberName);

                if (member is null)
                    continue;

                heuristicMember = member;
            }
        }

        if (heuristicMember is null)
        {
            logger.Warn($"{targetMember}: Heuristic attributes' member couldn't be resolved");
            return exampleCase;
        }

        var heuristicMemberType = MemberUtils.GetMemberType(heuristicMember);

        if (heuristicMemberType is null)
        {
            logger.Warn($"{targetMember}: Heuristics' type couldn't be resolved");
            return exampleCase;
        }

        logger.Log($"{targetMember}: found heuristic {heuristicMember}: ({heuristicMemberType} -> {targetMemberType})");

        var implementation = target.FindImplementationForInterfaceMember(heuristicMember);

        // impl can be source of truth'd
        if (implementation is null)
        {
            foreach (var targetSymbol in TypeUtils.GetBaseTypesAndThis(target))
            {
                var searchMembers = targetSymbol.GetMembers(memberName);

                foreach (var searchMember in searchMembers)
                {
                    if (searchMember is null || heuristicMember.Kind != searchMember.Kind ||
                        !SourceOfTruth.IsTarget(searchMember))
                        continue;

                    var searchMemberType = MemberUtils.GetMemberType(searchMember);

                    if (searchMemberType is null)
                    {
                        logger.Warn($"{targetMember}: no result type can be pulled for {searchMember}");
                        continue;
                    }

                    if (!TypeUtils.CanBeMisleadinglyAssigned(searchMemberType, heuristicMemberType, semanticModel))
                    {
                        logger.Log(
                            $"{targetMember}: no result conversion between candidate {searchMember} -> {heuristicMemberType}");
                        continue;
                    }

                    implementation = searchMember;
                    break;
                }

                if (implementation is not null)
                    break;
            }
        }

        if (implementation is null)
        {
            logger.Warn($"{targetMember}: Heuristic attributes' implementation couldn't be resolved");
            return exampleCase;
        }

        var implementationType = MemberUtils.GetMemberType(implementation);

        logger.Log($"{targetMember}: Found heuristic implementation {implementation} ({implementationType})");

        if (implementationType is null)
        {
            logger.Warn($"{targetMember}: Heuristic attributes' implementation type couldn't be resolved");
            return exampleCase;
        }

        // the 'exampleCase' type could be apart of the heuristic
        if (TypeUtils.TypeContainsOtherAsGeneric(exampleCase, heuristicMemberType, out _))
        {
            logger.Log(
                $"{targetMember}: Doing type substitution:\n" +
                $"Template: {exampleCase}\n" +
                $"ToReplace: {heuristicMemberType}\n" +
                $"Replacement: {implementationType}"
            );

            return ReplaceTypeReference(exampleCase, heuristicMemberType, implementationType);
        }

        if (heuristicMemberType is INamedTypeSymbol {IsGenericType: true} namedHeuristic &&
            TypeUtils.TypeContainsOtherAsGeneric(heuristicMemberType, exampleCase, out _))
        {
            if (
                Hierarchy.GetHierarchy(implementationType)
                    .Select(x => x.Type)
                    .Prepend(implementation)
                    .FirstOrDefault(x =>
                        x is INamedTypeSymbol {IsGenericType: true} generic &&
                        generic.ConstructUnboundGenericType().Equals(
                            namedHeuristic.ConstructUnboundGenericType(),
                            SymbolEqualityComparer.Default
                        )
                    )
                is not INamedTypeSymbol match
            )
            {
                logger.Warn($"{targetMember}: Unable to find implementation of {namedHeuristic} on {implementation}");
                return exampleCase;
            }

            logger.Log(
                $"{targetMember}: Performing paired walk for type match:\n" +
                $"ToFind: {exampleCase}\n" +
                $"ToWalk: {heuristicMemberType}\n" +
                $"ToPair: {match}"
            );

            var walked = TypeUtils.PairedWalkTypeSymbolForMatch(
                exampleCase,
                heuristicMemberType,
                match,
                semanticModel
            );

            if (walked is null)
            {
                logger.Warn($"{targetMember}: Failed to find match after walk");
                return exampleCase;
            }

            return walked;
        }

        logger.Warn($"{targetMember}: no heuristic implementation was resolved, using default case");

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

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            targetLogger.Log($"Processing {target.ClassSymbol}:");

            foreach (var item in GetTemplateExtensionInterfaces(target.ClassSymbol))
            {
                targetLogger.Log($" - {item}");
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
                        ? GetTargetMembersForImplementation(baseTarget.TargetInterfaces, baseTarget.ClassSymbol)
                        : []
                ]
            ).ToArray();

            foreach (var targetInterface in target.TargetInterfaces)
            {
                var toImplement = GetTargetMembersForImplementation(
                        targetInterface, target.ClassSymbol
                    )
                    .Where(x =>
                        !InterfaceProxy.WillHaveProxiedMemberFor(target.ClassDeclarationSyntax, target.ClassSymbol,
                            target.SemanticModel, x)
                    );

                var implemented = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

                foreach (var member in toImplement)
                {
                    if (implemented.Any(x => MemberUtils.Conflicts(x, member)) || !implemented.Add(member))
                    {
                        targetLogger.Log($"{target.ClassSymbol}: Skipping {member} (conflicting member)");
                        continue;
                    }

                    targetLogger.Log($"{target.ClassSymbol}: Adding {member}");

                    var baseOverridableImplementation = baseTreeMembers
                        .FirstOrDefault(y =>
                            y.ContainingType.TypeKind is not TypeKind.Interface &&
                            MemberUtils.Conflicts(member, y) &&
                            MemberUtils.CanOverride(member, y, target.SemanticModel.Compilation)
                        );

                    var shouldBeNew = baseOverridableImplementation is null &&
                                      baseTreeMembers
                                          .Any(x =>
                                              MemberUtils.Conflicts(x, member)
                                          );

                    var modifiers = SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    );

                    if (bases.Contains(target.ClassSymbol) && baseOverridableImplementation is null)
                    {
                        targetLogger.Log($"{target.ClassSymbol}: {member} -> virtual");
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));
                    }

                    if (baseOverridableImplementation is not null)
                    {
                        targetLogger.Log(
                            $"{target.ClassSymbol}: {member} -> override ({baseOverridableImplementation})");
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
                    }

                    if (shouldBeNew)
                    {
                        targetLogger.Log($"{target.ClassSymbol}: {member} -> new");
                        modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword));
                    }

                    switch (member)
                    {
                        case IPropertySymbol property:
                            var propName = property.ExplicitInterfaceImplementations.Length > 0
                                ? property.ExplicitInterfaceImplementations[0].Name
                                : property.Name;

                            var propertyType = GetTargetTypeForImplementation(
                                property,
                                property.Type,
                                target.ClassSymbol,
                                target.SemanticModel,
                                targetLogger
                            );

                            declaration = declaration.AddMembers(
                                SyntaxFactory.PropertyDeclaration(
                                    [],
                                    modifiers,
                                    SyntaxFactory.ParseTypeName(propertyType.ToDisplayString()),
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
                                target.SemanticModel,
                                targetLogger
                            );

                            targetLogger.Log($"Chose return type {returnType}");

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
