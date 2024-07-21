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
        SemanticModel semantic,
        Logger logger)
    {
        var resolvedGenerics =
            new Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>>(SymbolEqualityComparer.Default);

        foreach (var typeParameter in target.MethodSymbol.TypeParameters.Where(TypeParameterIsTransitiveFill))
        {
            ResolveGenericForTypeParameter(
                target.MethodSymbol,
                typeParameter,
                invocationExpression,
                resolvedGenerics,
                semantic,
                logger
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
                    semantic,
                    logger
                );

                if (resolvedGenerics.Count == target.MethodSymbol.TypeParameters.Length)
                    break;
            }
        }

        if (resolvedGenerics.Count == 0)
        {
            logger.Warn($"No generics could be resolved for {target.MethodSymbol}");
            return;
        }

        var generics = FlattenResults(target.MethodSymbol, resolvedGenerics, semantic, logger);

        // we want to keep the [TransitiveFill] generic IF it's not explicitly used in the parameters
        var toKeep = generics.FirstOrDefault(x =>
            TypeParameterIsTransitiveFill(x.Key) &&
            target.MethodSymbol.Parameters.All(y => HasReferenceToOtherConstraint(y.Type, x.Key))
        ).Key;

        if (toKeep is not null)
        {
            generics.Remove(toKeep);
        }

        var genericLookupTable = generics.ToDictionary(
            x => x.Key.Name,
            x => x.Value.ToDisplayString());

        logger.Log($"Computed {genericLookupTable.Count} filled generics for {target.MethodSymbol}");
        foreach (var entry in genericLookupTable)
        {
            logger.Log($". {entry.Key} -> {entry.Value}");
        }

        var filledSyntax = methodSyntax;

        logger.Log("Syntax 0:\n" + filledSyntax);

        filledSyntax = filledSyntax.WithParameterList(
            filledSyntax.ParameterList.ReplaceNode(
                filledSyntax.ParameterList.Parameters[0],
                filledSyntax.ParameterList.Parameters[0].WithAttributeLists([])
            )
        );

        logger.Log("Syntax 1:\n" + filledSyntax.NormalizeWhitespace().ToString());


        filledSyntax = filledSyntax.WithConstraintClauses(
            SyntaxFactory.List(
                filledSyntax.ConstraintClauses
                    .Where(x =>
                        !genericLookupTable.ContainsKey(x.Name.Identifier.ValueText)
                    )
            )
        );

        logger.Log("Syntax 2:\n" + filledSyntax.NormalizeWhitespace().ToString());


        if (methodSyntax.TypeParameterList is not null)
        {
            filledSyntax = filledSyntax.WithTypeParameterList(
                generics.Count == methodSyntax.TypeParameterList.Parameters.Count ||
                filledSyntax.TypeParameterList is null
                    ? null
                    : SyntaxFactory.TypeParameterList(
                        SyntaxFactory.SeparatedList(
                            filledSyntax.TypeParameterList.Parameters
                                .Where(x =>
                                    !genericLookupTable.ContainsKey(x.Identifier.ValueText)
                                ).Select(x => x.WithAttributeLists([]))
                        )
                    )
            );
        }

        logger.Log("Syntax 3:\n" + filledSyntax.NormalizeWhitespace().ToString());

        filledSyntax = filledSyntax.ReplaceNodes(
            filledSyntax.DescendantNodes()
                .OfType<IdentifierNameSyntax>()
                .Where(x =>
                    genericLookupTable.ContainsKey(x.Identifier.ValueText)
                )
                .ToArray(),
            (node, _) => SyntaxFactory.IdentifierName(genericLookupTable[node.Identifier.ValueText]));

        logger.Log("Syntax 4:\n" + filledSyntax.NormalizeWhitespace().ToString());

        methodSyntax = filledSyntax;
    }

    private static ITypeSymbol ApplyMinimumNullableAnnotation(ITypeSymbol symbol, NullableAnnotation minimum)
    {
        if (symbol.NullableAnnotation > minimum)
            return symbol.WithNullableAnnotation(minimum);

        return symbol;
    }

    private static Dictionary<ITypeParameterSymbol, ITypeSymbol> FlattenResults(
        IMethodSymbol method,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> filled,
        SemanticModel semanticModel,
        Logger logger)
    {
        return filled.ToDictionary<
            KeyValuePair<ITypeParameterSymbol, HashSet<ITypeSymbol>>,
            ITypeParameterSymbol,
            ITypeSymbol
        >(
            x => x.Key,
            filledTypeParameter =>
            {
                logger.Log($"{filledTypeParameter.Key}");
                foreach (var filled in filledTypeParameter.Value)
                {
                    logger.Log($" - {filled}");
                }

                // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
                var annotation = filledTypeParameter.Key
                    .ConstraintNullableAnnotations
                    .Aggregate((a, b) => a | b);

                annotation = (NullableAnnotation)Math.Min((byte)annotation, (byte)NullableAnnotation.Annotated);

                if (filledTypeParameter.Value.Count == 1)
                    return ApplyMinimumNullableAnnotation(filledTypeParameter.Value.First(), annotation);

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
                        return ApplyMinimumNullableAnnotation(candidate, annotation);
                    }
                }

                return ApplyMinimumNullableAnnotation(filledTypeParameter.Value.First(), annotation);
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

        //Hanz.Logger.Log($"picked {parameter} : {type}");
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
        SemanticModel semantic,
        Logger logger)
    {
        var fillType = GetGenericFillType(method, method.TypeArguments.IndexOf(parameter), invocation, semantic, logger);

        if (fillType is null)
        {
            logger.Log($"Couldn't resolve fill type for {parameter}");
            return;
        }

        logger.Log($"Walking generic {parameter} : {fillType}");

        WalkTypeForConstraints(method, parameter, fillType, semantic, resolved, logger);
    }

    private static void ResolveGenericForParameter(
        IMethodSymbol method,
        IParameterSymbol parameter,
        InvocationExpressionSyntax invocation,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved,
        SemanticModel semantic,
        Logger logger)
    {
        var fillType = GetFillType(method, method.Parameters.IndexOf(parameter), invocation, semantic, logger);

        if (fillType is null)
        {
            return;
        }

        if (method.Parameters.IndexOf(parameter) == 0)
        {
            if (parameter.Type is not ITypeParameterSymbol typeParameter)
            {
                logger.Warn($"parameter type is not a type parameter for 'this' arg");
                return;
            }

            if (!CanSubstitute(typeParameter, fillType))
            {
                logger.Warn($"Cannot substitute {typeParameter} to {fillType}");
                return;
            }

            // we've resolved the 'this' parameter to 'fillType'
            PickSubstitute(typeParameter, fillType, resolved);

            // check for any generics within the constraints
            foreach (var constraintType in typeParameter.ConstraintTypes)
            {
                if (!WalkTypeForConstraints(method, constraintType, fillType, semantic, resolved, logger))
                {
                    logger.Warn($"Cannot substitute constraint {constraintType} to {fillType}");
                    return;
                }
            }
        }
        else
        {
            WalkTypeForConstraints(method, parameter.Type, fillType, semantic, resolved, logger);
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
        SemanticModel semanticModel,
        Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> resolved,
        Logger logger,
        int depth = 0)
    {
        logger.LogWithDepth($"Processing {type} : {filledType}", depth);

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
                        logger.LogWithDepth($"Couldn't resolve 'NOT' case for {filledType} ({notConstraint})", depth,
                            LogLevel.Warning);
                        return false;
                    }

                    filledType = newFillType;
                }
            }

            var hasWalked = resolved.ContainsKey(constraintTypeParameter);

            PickSubstitute(constraintTypeParameter, filledType, resolved);
            logger.LogWithDepth($"Picked {filledType} for {constraintTypeParameter}", depth);

            if (hasWalked) return true;

            logger.LogWithDepth(
                $"Walking {constraintTypeParameter.ConstraintTypes.Length} constraints for {filledType}",
                depth);
            foreach (var constraints in constraintTypeParameter.ConstraintTypes)
            {
                WalkTypeForConstraints(
                    method,
                    constraints,
                    filledType,
                    semanticModel,
                    resolved,
                    logger,
                    depth + 1
                );
            }

            return true;
        }

        if (type is INamedTypeSymbol namedConstraint)
        {
            var expectedSubstitute = TryMatchTo(filledType, namedConstraint, candidate =>
                FurtherClassifyCandidate(namedConstraint, candidate, semanticModel, logger, depth: depth)
            );


            // 'fillType' doesn't implement the type
            if (expectedSubstitute is null)
            {
                logger.LogWithDepth($"{filledType} doesn't implement {namedConstraint}", depth);
                return false;
            }

            logger.LogWithDepth($"Picked expected substitute {expectedSubstitute}", depth);

            if (namedConstraint.IsGenericType)
            {
                logger.LogWithDepth($"Walking {namedConstraint.TypeArguments.Length} type arguments for {filledType}", depth);

                for (var i = 0; i < namedConstraint.TypeArguments.Length; i++)
                {
                    WalkTypeForConstraints(
                        method,
                        namedConstraint.TypeArguments[i],
                        expectedSubstitute.TypeArguments[i],
                        semanticModel,
                        resolved,
                        logger,
                        depth + 1
                    );
                }
            }

            return true;
        }

        logger.LogWithDepth($"Couldn't resolve a match for {type} : {filledType}", depth);
        return false;
    }


    private static bool FurtherClassifyCandidate(
        ITypeSymbol source,
        ITypeSymbol candidate,
        SemanticModel semantic,
        Logger logger,
        HashSet<ITypeSymbol>? processed = null,
        int depth = 0)
    {
        processed ??= new(SymbolEqualityComparer.Default);

        logger.LogWithDepth($"{source}: Checking candidate {candidate}", depth);

        if (processed.Contains(source))
        {
            logger.LogWithDepth($"{source}: Precomputed validation", depth);
            return true;
        }

        if (source is ITypeParameterSymbol constraint)
        {
            var canSubstitute = CanSubstitute(constraint, candidate);
            logger.LogWithDepth($"{source}: Can substitute? {canSubstitute}", depth);
            return canSubstitute;
        }

        if (
            source is INamedTypeSymbol {IsGenericType: true} namedSource &&
            candidate is INamedTypeSymbol {IsGenericType: true} namedCandidate)
        {
            if (namedSource.TypeParameters.Length != namedCandidate.TypeParameters.Length)
            {
                logger.LogWithDepth(
                    $"{source}: Mismatch type parameter arguments {namedSource.TypeParameters.Length} <> {namedCandidate.TypeParameters.Length}",
                    depth);
                return false;
            }

            for (int i = 0; i < namedSource.TypeParameters.Length; i++)
            {
                if (!FurtherClassifyCandidate(
                        namedSource.TypeArguments[i],
                        namedCandidate.TypeArguments[i],
                        semantic,
                        logger,
                        processed,
                        depth + 1
                    ))
                {
                    logger.LogWithDepth(
                        $"{source}: Invalid classification {namedSource.TypeArguments[i]} : {namedCandidate.TypeArguments[i]}",
                        depth);
                    return false;
                }
            }

            return true;
        }

        var isValid =
            source.Equals(candidate, SymbolEqualityComparer.Default) ||
            semantic.Compilation.HasImplicitConversion(candidate, source);

        logger.LogWithDepth($"{source}: Valid? {isValid}", depth);

        if (isValid)
            processed.Add(source);

        return isValid;
    }

    private static void LogWithDepth(
        this Logger logger,
        string msg,
        int depth,
        LogLevel level = LogLevel.Information)
    {
        logger.Log(level, "".PadLeft(depth, '.') + $" {msg}");
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

    private static ITypeSymbol? GetGenericFillType(
        IMethodSymbol method,
        int index,
        InvocationExpressionSyntax invocation,
        SemanticModel semantic,
        Logger logger)
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
            var typeConstraint = method.TypeParameters[index];
            // look at the parameters
            var candidateParameter = method.Parameters.FirstOrDefault(x =>
                HasReferenceToOtherConstraint(x.Type, typeConstraint)
            );

            if (candidateParameter is null)
            {
                logger.Warn(
                    $"No fill type for {invocation.Expression} through both type parameters and supplied arguments");
                return null;
            }


            if (GetFillType(method, method.Parameters.IndexOf(candidateParameter), invocation, semantic, logger) is not
                INamedTypeSymbol candidateFillType)
                return null;

            return TypeUtils.PairedWalkTypeSymbolForMatch(
                typeConstraint,
                candidateParameter.Type,
                candidateFillType,
                semantic
            );
        }

        if (syntax.TypeArgumentList.Arguments.Count <= index)
            return null;

        return semantic.GetTypeInfo(syntax.TypeArgumentList.Arguments[index]).Type;
    }

    private static ITypeSymbol? GetFillType(
        IMethodSymbol symbol,
        int index,
        InvocationExpressionSyntax invocation,
        SemanticModel semantic,
        Logger logger)
    {
        switch (index)
        {
            case -1:
                logger.Warn("Cannot pull parameter: -1 index");
                return null;
            case 0 when symbol.IsExtensionMethod:
                return FunctionGenerator.GetInvocationTarget(invocation, semantic);
            case >= 0 when invocation.ArgumentList.Arguments.Count > index - (symbol.IsExtensionMethod ? 1 : 0):
                if (symbol.IsExtensionMethod)
                    index = -1;

                var expression = invocation.ArgumentList.Arguments[index].Expression;
                var type = ModelExtensions.GetTypeInfo(semantic, expression).Type;

                if (type is null)
                    logger.Warn($"Couldn't resolve type for expression {expression}");

                return type;
            default:
                logger.Warn($"Cannot pull parameter: not enough arguments for index {index}");
                return null;
        }
    }
}
