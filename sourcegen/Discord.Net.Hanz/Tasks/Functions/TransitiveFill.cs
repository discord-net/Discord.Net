using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks;

public static class TransitiveFill
{
    private static readonly string[] TypeBlacklists =
    [
        "IFactory"
    ];

    public static bool IsTargetMethod(IMethodSymbol methodSymbol)
    {
        return IsAppliedToExtensionMethodParameter(methodSymbol) || IsAppliedToGenericParameter(methodSymbol);
    }

    private static bool IsAppliedToGenericParameter(IMethodSymbol methodSymbol)
    {
        return methodSymbol.IsGenericMethod &&
               methodSymbol.TypeParameters
                   .Any(TypeParameterIsTransitiveFill);
    }

    private static bool TypeParameterIsTransitiveFill(ITypeParameterSymbol parameter)
    {
        return parameter
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.TransitiveFillAttribute");
    }

    private static bool IsAppliedToExtensionMethodParameter(IMethodSymbol methodSymbol)
    {
        return methodSymbol.Parameters.Length > 0 &&
               methodSymbol.IsExtensionMethod &&
               ParameterIsTransitiveFill((methodSymbol.ReducedFrom ?? methodSymbol).Parameters[0]);
    }

    private static bool ParameterIsTransitiveFill(IParameterSymbol? parameter)
    {
        if (parameter is null)
            return false;

        return parameter
            .GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.TransitiveFillAttribute");
    }

    public static void Apply(
        ref MethodDeclarationSyntax methodSyntax,
        InvocationExpressionSyntax invocationExpression,
        FunctionGenerator.MethodTarget target,
        SemanticModel semantic)
    {
        if (methodSyntax.TypeParameterList is null)
            return;

        var resolvedGenerics =
            new Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>>(SymbolEqualityComparer.Default);

        foreach (var typeParameter in target.MethodSymbol.TypeParameters.Where(TypeParameterIsTransitiveFill))
        {
            ResolveGenericForTypeParameter(
                target.MethodSymbol,
                typeParameter,
                invocationExpression,
                resolvedGenerics,
                semantic
            );

            if (resolvedGenerics.Count == target.MethodSymbol.TypeParameters.Length)
                break;
        }

        if (resolvedGenerics.Count != target.MethodSymbol.TypeParameters.Length)
        {
            foreach (var parameter in target.MethodSymbol.Parameters.Where(ParameterIsTransitiveFill))
            {
                ResolveGenericForParameter(
                    target.MethodSymbol,
                    parameter,
                    invocationExpression,
                    resolvedGenerics,
                    semantic
                );

                if (resolvedGenerics.Count == target.MethodSymbol.TypeParameters.Length)
                    break;
            }
        }

        if (resolvedGenerics.Count == 0)
            return;

        var generics = FlattenResults(target.MethodSymbol, resolvedGenerics, semantic);

        // we want to keep the [TransitiveFill] generic IF it's not explicitly used in the parameters
        var toKeep = generics.FirstOrDefault(x =>
            TypeParameterIsTransitiveFill(x.Key) &&
            target.MethodSymbol.Parameters.All(y => !y.Type.Equals(x.Key, SymbolEqualityComparer.Default))
        ).Key;

        if (toKeep is not null)
        {
            generics.Remove(toKeep);
        }

        var genericLookupTable = generics.ToDictionary(
            x => x.Key.Name,
            x => x.Value.WithNullableAnnotation(NullableAnnotation.NotAnnotated).ToDisplayString());

        var filledSyntax = methodSyntax;

        filledSyntax = filledSyntax.WithParameterList(
            filledSyntax.ParameterList.ReplaceNode(
                filledSyntax.ParameterList.Parameters[0],
                filledSyntax.ParameterList.Parameters[0].WithAttributeLists(default)
            )
        );

        filledSyntax = filledSyntax.WithConstraintClauses(
            SyntaxFactory.List(
                filledSyntax.ConstraintClauses
                    .Where(x =>
                        !genericLookupTable.ContainsKey(x.Name.Identifier.ValueText)
                    )
            )
        );

        filledSyntax = filledSyntax.WithTypeParameterList(
            generics.Count == methodSyntax.TypeParameterList.Parameters.Count || filledSyntax.TypeParameterList is null
                ? null
                : SyntaxFactory.TypeParameterList(
                    SyntaxFactory.SeparatedList(
                        filledSyntax.TypeParameterList.Parameters
                            .Where(x =>
                                !genericLookupTable.ContainsKey(x.Identifier.ValueText)
                            ).Select(x => x.WithAttributeLists(default))
                    )
                )
        );

        filledSyntax = filledSyntax.ReplaceNodes(
            filledSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(x =>
                    genericLookupTable.ContainsKey(x.Identifier.ValueText)
                ),
            (node, _) =>
            {
                return SyntaxFactory.IdentifierName(genericLookupTable[node.Identifier.ValueText]);
            }
        );

        methodSyntax = filledSyntax;
    }

    private static Dictionary<ITypeParameterSymbol, ITypeSymbol> FlattenResults(
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> filled,
        SemanticModel semanticModel)
    {
        return filled.ToDictionary<
            KeyValuePair<ITypeParameterSymbol, HashSet<ITypeSymbol>>,
            ITypeParameterSymbol,
            ITypeSymbol
        >(
            x => x.Key,
            filledTypeParameter =>
            {
                if (filledTypeParameter.Value.Count == 1)
                    return filledTypeParameter.Value.First();

                var references = method.TypeParameters
                    .Where(x => x
                            .ConstraintTypes
                            .Any(y =>
                                HasReferenceToOtherConstraint(y, filledTypeParameter.Key)
                            ) && filled.TryGetValue(x, out var resolved) && resolved.Count == 1
                    )
                    .ToArray();

                foreach (var candidate in filledTypeParameter.Value)
                {
                    if (
                        references.All(x => filled[x]
                            .OfType<INamedTypeSymbol>()
                            .Any(filledNamed => x
                                .ConstraintTypes
                                .Where(y =>
                                    HasReferenceToOtherConstraint(y, filledTypeParameter.Key)
                                )
                                .All(constraintType =>
                                {
                                    ITypeSymbol searchTarget = filledTypeParameter.Key;

                                    if (constraintType is ITypeParameterSymbol typeParameter)
                                    {
                                        if (typeParameter.Equals(filledTypeParameter.Key,
                                                SymbolEqualityComparer.Default))
                                        {
                                            constraintType = filledNamed;
                                            searchTarget = candidate;
                                        }
                                        else
                                        {
                                            constraintType = filled[typeParameter].First();
                                        }
                                    }

                                    var paired = TypeUtils.PairedWalkTypeSymbolForMatch(
                                        searchTarget,
                                        constraintType,
                                        filledNamed,
                                        semanticModel
                                    );

                                    return paired?.Equals(
                                        candidate,
                                        SymbolEqualityComparer.Default
                                    ) ?? false;
                                })
                            )
                        )
                    )
                    {
                        return candidate;
                    }
                }

                return filledTypeParameter.Value.First();
            },
            SymbolEqualityComparer.Default);
    }

    private static void PickSubstitute(
        ITypeParameterSymbol parameter,
        ITypeSymbol type,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved)
    {
        if (!resolved.TryGetValue(parameter, out var existing))
            existing = resolved[parameter] = new(SymbolEqualityComparer.Default);

        existing.Add(type);
    }

    private static bool HasReferenceToOtherConstraint(ITypeSymbol type, ITypeParameterSymbol toCheck)
    {
        if (type.Equals(toCheck, SymbolEqualityComparer.Default))
            return true;

        if (type is INamedTypeSymbol {IsGenericType: true} named)
            return named.TypeArguments.Any(x => HasReferenceToOtherConstraint(x, toCheck));

        return false;
    }

    private static void ResolveGenericForTypeParameter(
        IMethodSymbol method,
        ITypeParameterSymbol parameter,
        InvocationExpressionSyntax invocation,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved,
        SemanticModel semantic)
    {
        var fillType = GetGenericFillType(method.TypeArguments.IndexOf(parameter), invocation, semantic);

        if (fillType is null)
        {
            Hanz.Logger.Warn($"No fill type for type parameter {parameter} in {method}");
            return;
        }

        WalkTypeForConstraints(method, parameter, fillType, resolved);
    }

    private static void ResolveGenericForParameter(
        IMethodSymbol method,
        IParameterSymbol parameter,
        InvocationExpressionSyntax invocation,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved,
        SemanticModel semantic)
    {
        var fillType = GetFillType(method.Parameters.IndexOf(parameter), invocation, semantic);

        if (fillType is null)
        {
            Hanz.Logger.Warn($"Cannot resolve fill type for {parameter}");
            return;
        }

        if (method.Parameters.IndexOf(parameter) == 0)
        {
            if (parameter.Type is not ITypeParameterSymbol typeParameter)
            {
                Hanz.Logger.Warn($"parameter type is not a type parameter for 'this' arg");
                return;
            }

            if (!CanSubstitute(typeParameter, fillType))
            {
                Hanz.Logger.Warn($"Cannot substitute {typeParameter} to {fillType}");
                return;
            }

            // we've resolved the 'this' parameter to 'fillType'
            PickSubstitute(typeParameter, fillType, resolved);

            // check for any generics within the constraints
            foreach (var constraintType in typeParameter.ConstraintTypes)
            {
                if (!WalkTypeForConstraints(method, constraintType, fillType, resolved))
                {
                    Hanz.Logger.Warn($"Cannot substitute constraint {constraintType} to {fillType}");
                    return;
                }
            }
        }
        else
        {
            WalkTypeForConstraints(method, parameter.Type, fillType, resolved);
        }
    }

    private static bool GetInterfaceConstraint(ITypeParameterSymbol parameter)
    {
        return parameter
            .GetAttributes()
            .Any(x =>
                x.AttributeClass?.ToDisplayString() == "Discord.InterfaceAttribute"
            );
    }

    private static string? GetNotConstraint(ITypeParameterSymbol parameter)
    {
        return parameter
            .GetAttributes()
            .FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == "Discord.NotAttribute"
            )
            ?.ConstructorArguments[0].Value as string;
    }

    private static bool WalkTypeForConstraints(
        IMethodSymbol method,
        ITypeSymbol type,
        ITypeSymbol filledType,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved)
    {
        if (type is ITypeParameterSymbol constraintTypeParameter &&
            CanSubstitute(constraintTypeParameter, filledType))
        {
            var notConstraint = GetNotConstraint(constraintTypeParameter);
            var interfaceConstraint = GetInterfaceConstraint(constraintTypeParameter);

            if (notConstraint is not null)
            {
                var notGeneric = resolved
                    .FirstOrDefault(x => x.Key.Name == notConstraint)
                    .Value;

                var shouldResubstitute = notGeneric is not null &&
                                         notGeneric.Any(x => x.Equals(filledType, SymbolEqualityComparer.Default));

                if (shouldResubstitute || (interfaceConstraint && filledType.TypeKind is not TypeKind.Interface))
                {
                    var newFillType = Hierarchy
                        .GetHierarchy(filledType)
                        .FirstOrDefault(x =>
                            (!shouldResubstitute || CanSubstitute(constraintTypeParameter, x.Type)) &&
                            (!interfaceConstraint || x.Type.TypeKind is TypeKind.Interface)
                        ).Type;

                    if (newFillType is null)
                    {
                        Hanz.Logger.Warn($"Couldn't resolve 'NOT' case for {filledType} ({notConstraint})");
                        return false;
                    }

                    filledType = newFillType;
                }
            }

            var hasWalked = resolved.ContainsKey(constraintTypeParameter);

            PickSubstitute(constraintTypeParameter, filledType, resolved);

            if (hasWalked) return true;

            foreach (var constraints in constraintTypeParameter.ConstraintTypes)
            {
                WalkTypeForConstraints(
                    method,
                    constraints,
                    filledType,
                    resolved
                );
            }

            return true;
        }

        if (type is INamedTypeSymbol namedConstraint)
        {
            var expectedSubstitute = TryMatchTo(filledType, namedConstraint);

            // 'fillType' doesn't implement the type
            if (expectedSubstitute is null)
            {
                Hanz.Logger.Warn($"{filledType} doesn't implement {namedConstraint}");
                return false;
            }

            if (namedConstraint.IsGenericType)
            {
                for (var i = 0; i < namedConstraint.TypeArguments.Length; i++)
                {
                    WalkTypeForConstraints(
                        method,
                        namedConstraint.TypeArguments[i],
                        expectedSubstitute.TypeArguments[i],
                        resolved
                    );
                }
            }

            return true;
        }

        return false;
    }

    private static INamedTypeSymbol? TryMatchTo(
        ITypeSymbol fillType,
        INamedTypeSymbol toMatchTo,
        Func<INamedTypeSymbol, bool>? additionalPredicate = null)
    {
        if (fillType.Equals(toMatchTo, SymbolEqualityComparer.Default))
            return toMatchTo;

        if (
            fillType is INamedTypeSymbol {IsGenericType: true} namedFillType &&
            namedFillType.TypeArguments.Length == toMatchTo.TypeArguments.Length &&
            namedFillType.ConstructUnboundGenericType().Equals(
                toMatchTo.ConstructUnboundGenericType(),
                SymbolEqualityComparer.Default
            ))
        {
            return namedFillType;
        }

        var fillTypeHierarchy = Hierarchy.GetHierarchy(fillType);

        return fillTypeHierarchy.FirstOrDefault(x =>
        {
            var isMatch = x.Type.IsGenericType && toMatchTo.IsGenericType
                ? x.Type.ConstructUnboundGenericType()
                    .Equals(
                        toMatchTo.ConstructUnboundGenericType(),
                        SymbolEqualityComparer.Default
                    )
                : x.Type.Equals(toMatchTo, SymbolEqualityComparer.Default);

            return isMatch && (additionalPredicate?.Invoke(x.Type) ?? true);
        }).Type;
    }

    private static bool CanSubstitute(ITypeParameterSymbol parameter, ITypeSymbol requested,
        HashSet<ITypeSymbol>? checkedType = null)
    {
        checkedType ??= new(SymbolEqualityComparer.Default);

        if (checkedType.Contains(parameter))
            return true;

        if (parameter.HasReferenceTypeConstraint && !requested.IsReferenceType) return false;
        if (parameter.HasValueTypeConstraint && !requested.IsValueType) return false;
        if (parameter.HasUnmanagedTypeConstraint && !requested.IsUnmanagedType) return false;

        foreach (var constraintType in parameter.ConstraintTypes)
        {
            if (checkedType.Contains(constraintType))
                continue;

            if (!CanSubstituteConstraint(requested, constraintType, checkedType))
            {
                // check for IFactory
                if (TypeBlacklists.Any(x => constraintType.ToDisplayString().StartsWith(x)))
                {
                    continue;
                }

                Hanz.Logger.Warn($"CONSTRAINT: {requested} doesn't match {constraintType}");
                return false;
            }

            checkedType.Add(constraintType);
        }

        return true;
    }

    private static bool CanSubstituteConstraint(ITypeSymbol requested, ITypeSymbol constraint,
        HashSet<ITypeSymbol>? checkedType = null)
    {
        checkedType ??= new(SymbolEqualityComparer.Default);

        // add to prevent recursion
        checkedType.Add(constraint);

        var result = constraint switch
        {
            ITypeParameterSymbol constraintParameter => CanSubstitute(constraintParameter, requested, checkedType),
            INamedTypeSymbol named =>
                TryMatchTo(requested, named, matched =>
                    checkedType.Contains(matched) ||
                    !named.IsGenericType ||
                    named.TypeArguments
                        .Select((x, i) => (Type: x, Index: i))
                        .All(x =>
                            CanSubstituteConstraint(matched.TypeArguments[x.Index], x.Type, checkedType)
                        )
                ) is not null,
            _ => true
        };

        if (!result)
            checkedType.Remove(constraint);

        return result;
    }

    private static ITypeSymbol? GetGenericFillType(int index, InvocationExpressionSyntax invocation,
        SemanticModel semantic)
    {
        var syntax = invocation.Expression as GenericNameSyntax;

        if (syntax is null)
        {
            syntax = invocation.Expression switch
            {
                MemberAccessExpressionSyntax memberAccess => memberAccess.Name as GenericNameSyntax,
                _ => syntax
            };
        }

        if (syntax is null)
        {
            Hanz.Logger.Warn($"No fill type for {invocation.Expression} ");
            return null;
        }

        if (syntax.TypeArgumentList.Arguments.Count <= index)
            return null;

        return semantic.GetTypeInfo(syntax.TypeArgumentList.Arguments[index]).Type;
    }

    private static ITypeSymbol? GetFillType(int index, InvocationExpressionSyntax invocation, SemanticModel semantic)
    {
        switch (index)
        {
            case -1:
                Hanz.Logger.Warn("Cannot pull parameter: -1 index");
                return null;
            case 0:
                return FunctionGenerator.GetInvocationTarget(invocation, semantic);
            case > 0 when invocation.ArgumentList.Arguments.Count >= index:
                var expression = invocation.ArgumentList.Arguments[index - 1].Expression;
                var type = ModelExtensions.GetTypeInfo(semantic, expression).Type;

                if (type is null)
                    Hanz.Logger.Warn($"Couldn't resolve type for expression {expression}");

                return type;
            default:
                Hanz.Logger.Warn($"Cannot pull parameter: not enough arguments for index {index}");
                return null;
        }
    }
}
