using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public static class TransitiveFill
{
    private static readonly string[] TypeBlacklists =
    [
        "IFactory",
        "Discord.IFactory",
        "Discord.IProxied",
        "Discord.Gateway.IStoreProvider",
        "Discord.Gateway.IStoreInfoProvider",
        "Discord.Gateway.IRootStoreProvider",
        "Discord.Gateway.ISubStoreProvider",
        "Discord.Gateway.IBrokerProvider",
        "Discord.IContextConstructable"
    ];

    private sealed class Context
    {
        public Dictionary<ITypeParameterSymbol, ParsedTypeParameter> TypeParameters { get; }

        public Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> Resolved { get; }
            = new(SymbolEqualityComparer.Default);

        public Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> AdditionalChecks { get; }
            = new(SymbolEqualityComparer.Default);

        public Dictionary<ITypeParameterSymbol, HashSet<ITypeSymbol>> NotAllowed { get; }
            = new(SymbolEqualityComparer.Default);

        public SemanticModel SemanticModel { get; }

        public IMethodSymbol Method { get; }

        public InvocationExpressionSyntax Invocation { get; }

        public Dictionary<int, int> VariableFuncArgsMap { get; }

        public Logger Logger { get; }

        public Context(
            SemanticModel model,
            IMethodSymbol method,
            InvocationExpressionSyntax invocation,
            Dictionary<int, int> variableFuncArgsMap,
            Logger logger)
        {
            SemanticModel = model;
            Method = method;
            Invocation = invocation;
            VariableFuncArgsMap = variableFuncArgsMap;
            Logger = logger;

            TypeParameters = method.TypeParameters
                .ToDictionary(
                    x => x,
                    x => new ParsedTypeParameter(this, x)
                );
        }

        public void RemoveCandidate(ITypeParameterSymbol parameter, ITypeSymbol candidate)
        {
            if (!Resolved.TryGetValue(parameter, out var candidates))
                return;

            if (candidates.Remove(candidate))
                Logger.Log($"{parameter} -= {candidate}");

            if (candidates.Count == 0)
                Resolved.Remove(parameter);
        }

        public bool PassesAdditionalChecks(ITypeParameterSymbol parameter, ITypeSymbol substitute)
        {
            return
                !AdditionalChecks.TryGetValue(parameter, out var checks)
                ||
                checks.All(x => SemanticModel.Compilation.HasImplicitConversion(substitute, x));
        }

        public void AddAdditionalConstraints(ITypeParameterSymbol symbol, params ITypeSymbol[] constraints)
        {
            if (!AdditionalChecks.TryGetValue(symbol, out var constraintsSet))
                AdditionalChecks[symbol] = constraintsSet = new(SymbolEqualityComparer.Default);

            constraintsSet.UnionWith(constraints);
        }

        public void PickSubstitute(
            ITypeParameterSymbol parameter,
            params ITypeSymbol[] types)
        {
            if (!TypeParameters.TryGetValue(parameter, out var typeParameter))
            {
                Logger.Warn($"Unknown type parameter '{parameter}' attempted to be substituted for {Method}");
                return;
            }

            if (typeParameter.Ignored)
                return;

            if (!Resolved.TryGetValue(parameter, out var existing))
                existing = Resolved[parameter] = new(SymbolEqualityComparer.Default);

            foreach (var type in types.Except(existing, SymbolEqualityComparer.Default))
                Logger.Log($"{parameter} += {type}");

            existing.UnionWith(types);
        }

        public void MarkAsIllegal(ITypeParameterSymbol parameter, params ITypeSymbol[] symbols)
        {
            foreach (var type in symbols)
                RemoveCandidate(parameter, type);

            if (NotAllowed.TryGetValue(parameter, out var already) && symbols.All(already.Contains))
                return;

            if (!NotAllowed.TryGetValue(parameter, out var existing))
                existing = NotAllowed[parameter] = new(SymbolEqualityComparer.Default);

            foreach (var type in symbols.Except(existing))
                Logger.Log($"{parameter} can no longer be {type}");

            existing.UnionWith(symbols);
        }
    }

    private sealed class ParsedTypeParameter
    {
        public ITypeParameterSymbol Parameter { get; }

        public bool TransitiveFilled { get; }
        public bool Ignored { get; }
        public bool Shrink { get; }
        public bool Expand { get; }
        public bool ShouldBeInterface { get; }
        public bool RequiredToBeResolve { get; }
        public HashSet<ITypeParameterSymbol> Not { get; }

        public ParsedTypeParameter(Context context, ITypeParameterSymbol symbol)
        {
            Parameter = symbol;
            Not = new HashSet<ITypeParameterSymbol>(SymbolEqualityComparer.Default);

            foreach (var attribute in symbol.GetAttributes())
            {
                switch (attribute.AttributeClass?.ToDisplayString())
                {
                    case "Discord.InterfaceAttribute":
                        ShouldBeInterface = true;
                        break;
                    case "Discord.NotAttribute" when attribute.ConstructorArguments.Length > 0:
                        var notTarget = context.Method.TypeParameters
                            .FirstOrDefault(x =>
                                x.Name == attribute.ConstructorArguments[0].Value as string
                            );
                        if (notTarget is not null)
                            Not.Add(notTarget);
                        break;
                    case "Discord.RequireResolveAttribute":
                        RequiredToBeResolve = true;
                        break;
                    case "Discord.IgnoreAttribute":
                        Ignored = true;
                        break;
                    case "Discord.ShrinkAttribute":
                        Shrink = true;
                        break;
                    case "Discord.TransitiveFillAttribute":
                        TransitiveFilled = true;
                        break;
                    case "Discord.ExpandAttribute":
                        Expand = true;
                        break;
                }
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (TransitiveFilled)
                sb.Append("Transitive ");

            if (Ignored)
                sb.Append("Ignored ");

            if (Shrink)
                sb.Append("Shrunk ");

            if (Expand)
                sb.Append("Expanded ");

            if (ShouldBeInterface)
                sb.Append("Interface ");

            if (RequiredToBeResolve)
                sb.Append("Required ");

            if (Not.Count > 0)
                sb.Append($"Not({string.Join(" | ", Not.Select(x => x.Name))}) ");

            sb.Append(Parameter.Name);

            return sb.ToString();
        }
    }

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

    private static bool InvocationArgumentsAreInRange(IMethodSymbol method, InvocationExpressionSyntax expression)
    {
        var upper = method.Parameters.Length;
        var lower = upper;

        if (method.IsExtensionMethod)
            lower--;

        lower -= Math.Max(0, method.Parameters.Count(x => x.IsOptional));

        return
            expression.ArgumentList.Arguments.Count >= lower
            &&
            (
                VariableFuncArgs.IsTargetMethod(method)
                ||
                expression.ArgumentList.Arguments.Count <= upper
            );
    }

    public static void Apply(
        ref MethodDeclarationSyntax methodSyntax,
        InvocationExpressionSyntax invocationExpression,
        IMethodSymbol method,
        SemanticModel semantic,
        Logger logger,
        Dictionary<int, int> variableFuncArgsMap)
    {
        logger.Log($"Executing TransitiveFill on {method}");
        logger.Log($"Variable function map length: {variableFuncArgsMap.Count}");

        foreach (var entry in variableFuncArgsMap)
            logger.Log($" - {entry.Key}:{entry.Value}");

        if (!InvocationArgumentsAreInRange(method, invocationExpression))
        {
            logger.Log($"Skipping this method, mismatch argument count:\n" +
                       $"Expression: {invocationExpression.ArgumentList.Arguments.Count}\n" +
                       $"Symbol: {method.Parameters.Length}\n" +
                       $"IsExtension?: {method.IsExtensionMethod}");
            return;
        }

        var context = new Context(semantic, method, invocationExpression, variableFuncArgsMap, logger);

        foreach (var typeParameter in method.TypeParameters.Where(TypeParameterIsTransitiveFill))
        {
            ResolveGenericForTypeParameter(
                typeParameter,
                context,
                logger
            );

            if (context.Resolved.Count == method.TypeParameters.Length)
                break;
        }

        if (context.Resolved.Count != method.TypeParameters.Length)
        {
            foreach (var parameter in method.Parameters)
            {
                ResolveGenericForParameter(
                    context,
                    parameter,
                    logger
                );

                if (context.Resolved.Count == method.TypeParameters.Length)
                    break;
            }
        }

        if (context.Resolved.Count == 0)
        {
            logger.Warn($"No generics could be resolved for {method}");
            return;
        }

        // do a check for any remaining generics specified by the user
        if (
            context.Resolved.Count < method.TypeParameters.Length &&
            SyntaxUtils.GetMemberTargetOrSelf(invocationExpression.Expression) is GenericNameSyntax genericNameSyntax
        )
        {
            var remainingParameters = new Queue<ITypeParameterSymbol>(
                method.TypeParameters
                    .Where(x =>
                        !context.Resolved.ContainsKey(x)
                    )
            );

            logger.Log("User supplied generics and we've got unresolved:");

            foreach (var generic in remainingParameters)
            {
                logger.Log($" - {generic}");
            }

            for (var i = 0; i != genericNameSyntax.TypeArgumentList.Arguments.Count; i++)
            {
                var typeInfo = semantic.GetTypeInfo(genericNameSyntax.TypeArgumentList.Arguments[i]).Type;

                if (typeInfo is null)
                {
                    logger.Warn(
                        $"Could not resolve user-specified generic argument {genericNameSyntax.TypeArgumentList.Arguments[i]}");
                    continue;
                }

                try_resolve:

                if (remainingParameters.Count == 0)
                    break;

                var typeParameter = remainingParameters.Dequeue();

                if (context.Resolved.TryGetValue(typeParameter, out var matched))
                {
                    logger.Log($"Post-resolved {typeParameter}:");
                    foreach (var match in matched)
                    {
                        logger.Log($" - {match}");
                    }

                    goto try_resolve;
                }

                if (WalkTypeForConstraints(context, typeParameter, typeInfo, logger))
                {
                    // update the references to the substituted parameter
                }
            }
        }

        var generics = FlattenResults(context, logger);

        foreach (var required in context.TypeParameters.Values.Where(x => x.RequiredToBeResolve))
        {
            if (!generics.ContainsKey(required.Parameter))
            {
                logger.Warn($"Missing required generic '{required}'");
                return;
            }
        }

        // we want to keep the [TransitiveFill] generic IF it's not explicitly used in the parameters
        var toKeep = generics.FirstOrDefault(x =>
            TypeParameterIsTransitiveFill(x.Key) &&
            method.Parameters.All(y => HasReferenceToOtherConstraint(y.Type, x.Key))
        ).Key;

        if (toKeep is not null)
        {
            generics.Remove(toKeep);
        }

        var genericLookupTable = generics.ToDictionary(
            x => x.Key.Name,
            x => x.Value.ToDisplayString());

        logger.Log($"Computed {genericLookupTable.Count} filled generics for {method}");
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
        Context context,
        Logger logger)
    {
        var result = new Dictionary<ITypeParameterSymbol, ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (
            var filledTypeParameter in context.Resolved
                .Where(x => x.Value.Count > 0))
        {
            logger.Log($"{filledTypeParameter.Key}");

            foreach (var filled in filledTypeParameter.Value)
                logger.Log($" - {filled}");

            var parsed = context.TypeParameters[filledTypeParameter.Key];

            // ReSharper disable once BitwiseOperatorOnEnumWithoutFlags
            var annotation = filledTypeParameter.Key
                .ConstraintNullableAnnotations
                .Aggregate((a, b) => a | b);

            annotation = (NullableAnnotation)Math.Min((byte)annotation, (byte)NullableAnnotation.Annotated);

            if (filledTypeParameter.Value.Count == 1)
            {
                result.Add(
                    filledTypeParameter.Key,
                    ApplyMinimumNullableAnnotation(filledTypeParameter.Value.First(), annotation)
                );
                continue;
            }

            var references = context.Method.TypeParameters
                .Where(x => x
                        .ConstraintTypes
                        .Any(y =>
                            HasReferenceToOtherConstraint(y, filledTypeParameter.Key)
                        ) && context.Resolved.ContainsKey(x)
                )
                .ToArray();

            var candidates = filledTypeParameter.Value
                .Where(candidate => references
                    .All(x => context.Resolved[x]
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
                                        constraintType = context.Resolved[typeParameter].First();
                                    }
                                }

                                var paired = TypeUtils.PairedWalkTypeSymbolForMatch(
                                    searchTarget,
                                    constraintType,
                                    filledNamed,
                                    context.SemanticModel
                                );

                                return paired?.Equals(
                                    candidate,
                                    SymbolEqualityComparer.Default
                                ) ?? false;
                            })
                        )
                    )
                )
                .ToArray();

            if (candidates.Length <= 1)
            {
                logger.Log($"{filledTypeParameter.Key}: Candidates: {candidates.Length}");

                result.Add(
                    filledTypeParameter.Key,
                    ApplyMinimumNullableAnnotation(
                        candidates.FirstOrDefault() ?? filledTypeParameter.Value.First(),
                        annotation
                    )
                );
                continue;
            }

            if (parsed.Shrink || parsed.Expand)
            {
                var descendantReference = references
                    .FirstOrDefault(x =>
                        x.ConstraintTypes.Contains(filledTypeParameter.Key) &&
                        result.ContainsKey(x)
                    );

                if (descendantReference is null)
                {
                    logger.Warn($"Failed to find references that inherit '{filledTypeParameter.Key}'");
                    foreach (var reference in references)
                    {
                        logger.Warn($" - {reference} : {result.ContainsKey(reference)}");
                    }

                    goto add_default;
                }

                var hierarchy = Hierarchy.GetHierarchy(result[descendantReference])
                    .Where(x => candidates.Contains(x.Type, SymbolEqualityComparer.Default));

                var chosen = parsed.Shrink
                    ? hierarchy.First()
                    : hierarchy.Last();

                logger.Log($"Chose {chosen.Type} for {parsed}");

                result.Add(
                    filledTypeParameter.Key,
                    ApplyMinimumNullableAnnotation(chosen.Type, annotation)
                );
                continue;
            }

            add_default:
            result.Add(
                filledTypeParameter.Key,
                ApplyMinimumNullableAnnotation(filledTypeParameter.Value.First(), annotation)
            );
        }


        return result;
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
        ITypeParameterSymbol parameter,
        Context context,
        Logger logger)
    {
        var fillType =
            GetGenericFillType(context.Method.TypeArguments.IndexOf(parameter), context, logger);

        if (fillType is null)
        {
            logger.Log($"Couldn't resolve fill type for {parameter}");
            return;
        }

        logger.Log($"Walking generic {parameter} : {fillType}");

        WalkTypeForConstraints(context, parameter, fillType, logger);
    }

    private static void ResolveGenericForParameter(
        Context context,
        IParameterSymbol parameter,
        Logger logger)
    {
        var fillType = GetFillType(context.Method.Parameters.IndexOf(parameter), context, logger);

        if (fillType is null)
        {
            logger.Log($"{parameter}: Skipping, no fill type resolved.");
            return;
        }

        if (context.Method.Parameters.IndexOf(parameter) == 0)
        {
            if (parameter.Type is not ITypeParameterSymbol typeParameter)
            {
                logger.Warn($"{parameter}: Parameter type is not a type parameter for 'this' arg");
                return;
            }

            if (!CanSubstitute(typeParameter, fillType, context, logger))
            {
                logger.Warn($"{parameter}: Cannot substitute {typeParameter} to {fillType}");
                return;
            }

            // we've resolved the 'this' parameter to 'fillType'
            context.PickSubstitute(typeParameter, fillType);

            // check for any generics within the constraints
            foreach (var constraintType in typeParameter.ConstraintTypes)
            {
                if (!WalkTypeForConstraints(context, constraintType, fillType, logger))
                {
                    logger.Warn($"Cannot substitute constraint {constraintType} to {fillType}");
                    return;
                }
            }
        }
        else
        {
            logger.Log($"{parameter}: Walking fill type {fillType}");
            WalkTypeForConstraints(context, parameter.Type, fillType, logger);
        }
    }

    private static IEnumerable<ITypeSymbol> WeakResolveGenerics(ITypeSymbol source, ITypeSymbol toResolve,
        Context context)
    {
        if (toResolve is ITypeParameterSymbol typeParameter)
        {
            return context.Resolved.TryGetValue(typeParameter, out var resolved)
                ? resolved
                : [source];
        }

        if (toResolve is INamedTypeSymbol {IsGenericType: true} named)
        {
            return source.AllInterfaces
                .Where(x =>
                    x.IsGenericType &&
                    x.ConstructedFrom.Equals(named.ConstructedFrom, SymbolEqualityComparer.Default)
                );
        }

        return [toResolve];
    }

    private static bool WalkTypeForConstraints(
        Context context,
        ITypeSymbol type,
        ITypeSymbol filledType,
        Logger logger,
        int depth = 0)
    {
        logger.LogWithDepth($"Processing {type} : {filledType}", depth);

        if (
            type is ITypeParameterSymbol constraintTypeParameter &&
            CanSubstitute(constraintTypeParameter, filledType, context, logger, depth: depth))
        {
            var parsedParameter = context.TypeParameters[constraintTypeParameter];

            if (parsedParameter.Not.Count > 0)
            {
                var shouldResubstitute =
                    parsedParameter.ShouldBeInterface && filledType.TypeKind is not TypeKind.Interface
                    ||
                    parsedParameter
                        .Not
                        .SelectMany(x =>
                            context.Resolved.TryGetValue(x, out var value)
                                ? value
                                : []
                        )
                        .Any(x =>
                            x.Equals(filledType, SymbolEqualityComparer.Default)
                        );


                if (shouldResubstitute)
                {
                    logger.LogWithDepth(
                        $"{constraintTypeParameter}: Computing substitution for {type} : {filledType}",
                        depth
                    );

                    var constraintSubstitutions = constraintTypeParameter.ConstraintTypes
                        .SelectMany(x =>
                        {
                            var resolved = WeakResolveGenerics(filledType, x, context)
                                .ToArray();

                            logger.LogWithDepth($"{constraintTypeParameter}: Weak resolved constraints:", depth);
                            foreach (var resolvedConstraint in resolved)
                            {
                                logger.LogWithDepth($" - {filledType} -> {x} -> {resolvedConstraint}", depth);
                            }

                            return resolved;
                        })
                        .SelectMany(x =>
                        {
                            var hierarchy = Hierarchy
                                .GetHierarchyBetween(
                                    filledType,
                                    x,
                                    false,
                                    false,
                                    !parsedParameter.ShouldBeInterface
                                );

                            logger.LogWithDepth(
                                $"{constraintTypeParameter}: Hierarchy for {filledType} -> {x}:",
                                depth
                            );

                            foreach (var element in hierarchy)
                            {
                                logger.LogWithDepth($" - {element}", depth);
                            }

                            return hierarchy;
                        })
                        .Distinct<ITypeSymbol>(SymbolEqualityComparer.Default)
                        .Where(x =>
                        {
                            var result = WalkTypeForConstraints(
                                context,
                                constraintTypeParameter,
                                x,
                                logger,
                                depth + 1
                            );

                            if (!result) context.MarkAsIllegal(constraintTypeParameter, x);

                            return result;
                        })
                        .ToArray();

                    if (constraintSubstitutions.Length == 0)
                    {
                        logger.LogWithDepth(
                            $"{constraintTypeParameter}: Couldn't resolve 'NOT' case for {filledType} ({string.Join(" | ", parsedParameter.Not)})",
                            depth,
                            LogLevel.Warning
                        );
                        context.MarkAsIllegal(constraintTypeParameter, filledType);

                        return false;
                    }

                    logger.LogWithDepth(
                        $"{constraintTypeParameter}: {constraintSubstitutions.Length} valid candidates:",
                        depth
                    );

                    foreach (var element in constraintSubstitutions)
                        logger.LogWithDepth($"- {element}", depth);

                    context.PickSubstitute(constraintTypeParameter, constraintSubstitutions);
                    return true;
                }
            }

            var hasWalked = context.Resolved.TryGetValue(constraintTypeParameter, out var set) &&
                            set.Contains(filledType);

            if (hasWalked) return true;

            logger.LogWithDepth(
                $"Walking {constraintTypeParameter.ConstraintTypes.Length} constraints for {filledType}",
                depth
            );

            // prevent recursion
            context.PickSubstitute(constraintTypeParameter, filledType);

            var otherCandidates = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);
            foreach (var constraint in constraintTypeParameter.ConstraintTypes)
            {
                var temp = WeakResolveGenerics(
                        filledType,
                        constraint,
                        context
                    )
                    .SelectMany(x => Hierarchy
                        .GetHierarchyBetween(
                            filledType,
                            x,
                            includeRoot: false,
                            includeChild: false
                        )
                    );

                otherCandidates.UnionWith(temp);

                if (WalkTypeForConstraints(context, constraint, filledType, logger, depth + 1))
                    continue;

                logger.LogWithDepth(
                    $"{filledType} doesn't match constraint {constraintTypeParameter} on {constraint}",
                    depth
                );

                context.RemoveCandidate(constraintTypeParameter, filledType);
                context.MarkAsIllegal(constraintTypeParameter, filledType);
                return false;
            }

            logger.LogWithDepth($"Picked {filledType} for {constraintTypeParameter}", depth);

            if (otherCandidates.Count > 0)
            {
                logger.LogWithDepth(
                    $"Possible other candidates found in hierarchy between {filledType} -> {type}:",
                    depth
                );

                foreach (var candidate in otherCandidates)
                    logger.LogWithDepth($" - {candidate}", depth);

                // context.PickSubstitute(constraintTypeParameter, otherCandidates.ToArray());

                foreach (var candidate in otherCandidates)
                {
                    if (WalkTypeForConstraints(context, constraintTypeParameter, candidate, logger, depth + 1))
                    {
                        logger.LogWithDepth($"Picked additional fill type {candidate} for {constraintTypeParameter}",
                            depth);
                        context.PickSubstitute(constraintTypeParameter, candidate);
                    }
                    else
                    {
                        context.RemoveCandidate(constraintTypeParameter, candidate);
                        context.MarkAsIllegal(constraintTypeParameter, candidate);
                    }
                }
            }


            return true;
        }

        if (type is INamedTypeSymbol namedConstraint)
        {
            var processed = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default) {filledType};

            var expectedSubstitute = TryMatchTo(filledType, namedConstraint, candidate =>
                FurtherClassifyCandidate(context, namedConstraint, candidate, logger, processed, depth: depth)
            );

            // 'fillType' doesn't implement the type
            if (expectedSubstitute is null)
            {
                if (TypeBlacklists.Any(x => namedConstraint.ToDisplayString().StartsWith(x)))
                {
                    logger.LogWithDepth($"Skipping {namedConstraint}, blacklisted type", depth);
                    return true;
                }

                logger.LogWithDepth($"{filledType} doesn't implement {namedConstraint}", depth);
                return false;
            }

            logger.LogWithDepth($"Picked expected substitute {expectedSubstitute}", depth);

            if (namedConstraint.IsGenericType)
            {
                logger.LogWithDepth($"Walking {namedConstraint.TypeArguments.Length} type arguments for {filledType}",
                    depth);

                for (var i = 0; i < namedConstraint.TypeArguments.Length; i++)
                {
                    if (processed.Contains(namedConstraint.TypeArguments[i]) ||
                        processed.Contains(expectedSubstitute.TypeArguments[i]))
                    {
                        logger.LogWithDepth($"Skipping {namedConstraint.TypeArguments[i]}: precompute", depth);
                        continue;
                    }

                    var result = WalkTypeForConstraints(
                        context,
                        namedConstraint.TypeArguments[i],
                        expectedSubstitute.TypeArguments[i],
                        logger,
                        depth + 1
                    );

                    logger.LogWithDepth(
                        $"{namedConstraint}: {expectedSubstitute.TypeArguments[i]} : {namedConstraint.TypeArguments[i]}?: {result}",
                        depth
                    );

                    if (!result) return false;
                }
            }

            return true;
        }

        logger.LogWithDepth($"Couldn't resolve a match for {type} : {filledType}", depth);
        return false;
    }


    private static bool FurtherClassifyCandidate(
        Context context,
        ITypeSymbol source,
        ITypeSymbol candidate,
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
            var canSubstitute = CanSubstitute(constraint, candidate, context, logger, depth: depth);

            if (
                canSubstitute &&
                context.Resolved.TryGetValue(constraint, out var candidates))
            {
                context.AddAdditionalConstraints(constraint, candidate);
                var hasMatching = false;

                logger.LogWithDepth($"{source}: Checking {candidates.Count} existing candidates for {candidate}",
                    depth);

                foreach (var existingCandidate in candidates.ToArray())
                {
                    var hasConversion =
                        context.SemanticModel.Compilation.HasImplicitConversion(candidate, existingCandidate);

                    hasMatching |= hasConversion;

                    logger.LogWithDepth(
                        $"{source}: can comply with existing substitute {existingCandidate}?: {hasConversion}",
                        depth + 1
                    );
                }

                if (!hasMatching)
                {
                    return false;
                }

                // only remove candidates that don't match when this type can be used
                // foreach (
                //     var existing in candidates
                //         .Where(x =>
                //             !context.SemanticModel.Compilation.HasImplicitConversion(candidate, x)
                //         )
                //         .ToArray()
                // )
                // {
                //     context.RemoveCandidate(constraint, existing);
                //     context.MarkAsIllegal(constraint, existing);
                // }
            }

            //if(canSubstitute) context.PickSubstitute(constraint, candidate);

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
                        context,
                        namedSource.TypeArguments[i],
                        namedCandidate.TypeArguments[i],
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
            context.SemanticModel.Compilation.HasImplicitConversion(candidate, source);

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

        var fillTypeHierarchy = Hierarchy
            .GetHierarchy(fillType)
            .Select(x => x.Type)
            .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        return fillTypeHierarchy.FirstOrDefault(x =>
        {
            var isMatch = x.IsGenericType && toMatchTo.IsGenericType
                ? x.ConstructUnboundGenericType()
                    .Equals(
                        toMatchTo.ConstructUnboundGenericType(),
                        SymbolEqualityComparer.Default
                    )
                : x.Equals(toMatchTo, SymbolEqualityComparer.Default);

            return isMatch && (additionalPredicate?.Invoke(x) ?? true);
        });
    }

    private static bool CanSubstitute(
        ITypeParameterSymbol parameter,
        ITypeSymbol requested,
        Context context,
        Logger logger,
        HashSet<ITypeSymbol>? checkedType = null,
        int depth = 0)
    {
        if (context.NotAllowed.TryGetValue(parameter, out var already) && already.Contains(requested))
            return false;

        checkedType ??= new(SymbolEqualityComparer.Default);

        if (checkedType.Contains(parameter))
            return true;

        if (parameter.HasReferenceTypeConstraint && !requested.IsReferenceType) return false;
        if (parameter.HasValueTypeConstraint && !requested.IsValueType) return false;
        if (parameter.HasUnmanagedTypeConstraint && !requested.IsUnmanagedType) return false;

        if (!context.PassesAdditionalChecks(parameter, requested))
        {
            logger.LogWithDepth($"{requested} does not meet the additional checks for {parameter}", depth);
            checkedType.Add(parameter);
            return false;
        }

        foreach (var constraintType in parameter.ConstraintTypes)
        {
            if (checkedType.Contains(constraintType))
                continue;

            if (!CanSubstituteConstraint(requested, constraintType, context, logger, checkedType, depth + 1))
            {
                // check for blacklisted
                if (TypeBlacklists.Any(x => constraintType.ToDisplayString().StartsWith(x)))
                {
                    logger.LogWithDepth($"Blacklisted constraint type {constraintType}, ignoring", depth);
                    continue;
                }

                logger.LogWithDepth($"type {requested} doesn't satisfy constraint {constraintType}", depth);
                return false;
            }

            logger.LogWithDepth($"{requested} vaguely satisfies constraint {constraintType}", depth);

            checkedType.Add(constraintType);
        }

        return true;
    }

    private static bool CanSubstituteConstraint(
        ITypeSymbol requested,
        ITypeSymbol constraint,
        Context context,
        Logger logger,
        HashSet<ITypeSymbol>? checkedType = null,
        int depth = 0)
    {
        checkedType ??= new(SymbolEqualityComparer.Default);

        // add to prevent recursion
        checkedType.Add(constraint);

        var result = constraint switch
        {
            ITypeParameterSymbol constraintParameter => CanSubstitute(
                constraintParameter,
                requested,
                context,
                logger,
                checkedType,
                depth + 1
            ),
            INamedTypeSymbol named =>
                TryMatchTo(requested, named, matched =>
                    checkedType.Contains(matched) ||
                    !named.IsGenericType ||
                    named.TypeArguments
                        .Select((x, i) => (Type: x, Index: i))
                        .All(x =>
                            CanSubstituteConstraint(
                                matched.TypeArguments[x.Index],
                                x.Type,
                                context,
                                logger,
                                checkedType,
                                depth + 1
                            )
                        )
                ) is not null,
            _ => true
        };

        if (!result)
            checkedType.Remove(constraint);

        return result;
    }

    private static ITypeSymbol? GetGenericFillType(
        int index,
        Context context,
        Logger logger)
    {
        var typeConstraint = context.Method.TypeParameters[index];

        // look at the parameters
        var candidateParameter = context.Method.Parameters.FirstOrDefault(x =>
            HasReferenceToOtherConstraint(x.Type, typeConstraint)
        );

        if (candidateParameter is null)
        {
            logger.Warn(
                $"No fill type for {context.Invocation.Expression} through both type parameters and supplied arguments");
            return null;
        }

        if (GetFillType(context.Method.Parameters.IndexOf(candidateParameter), context, logger) is not
            INamedTypeSymbol candidateFillType)
            return null;

        return TypeUtils.PairedWalkTypeSymbolForMatch(
            typeConstraint,
            candidateParameter.Type,
            candidateFillType,
            context.SemanticModel
        );
    }

    private static ITypeSymbol? GetFillType(
        int index,
        Context context,
        Logger logger)
    {
        foreach (
            var vargParameter in context.VariableFuncArgsMap
                .Where(x => x.Key < index)
        )
        {
            logger.Log($"Offsetting {index} by {vargParameter.Value} (vararg at index {vargParameter.Key})");
            //index += vargParameter.Value;
        }


        switch (index)
        {
            case -1:
                logger.Warn("Cannot pull parameter: -1 index");
                return null;
            case 0 when context.Method.IsExtensionMethod:
                return FunctionGenerator.GetInvocationTarget(context.Invocation, context.SemanticModel);
            case >= 0 when context.Invocation.ArgumentList.Arguments.Count >
                           index - (context.Method.IsExtensionMethod ? 1 : 0):

                if (context.Method.IsExtensionMethod)
                    index--;

                var expression = context.Invocation.ArgumentList.Arguments[index].Expression;

                var type = ModelExtensions.GetTypeInfo(context.SemanticModel, expression).Type;

                if (type is null && expression is InvocationExpressionSyntax invocation)
                {
                    var symbolInfo = context.SemanticModel.GetSymbolInfo(invocation.Expression);

                    logger.Log(
                        $"Symbol info: {symbolInfo.Symbol}, {symbolInfo.CandidateReason}, {symbolInfo.CandidateSymbols.Length}");

                    if (symbolInfo.Symbol is IMethodSymbol methodSymbol)
                        type = methodSymbol.ReturnType;
                }

                if (type is null)
                    logger.Warn($"Couldn't resolve type for expression {expression} (index {index})");

                return type;
            default:
                logger.Warn(
                    $"Cannot pull parameter: not enough arguments for index {index} ({context.Invocation.ArgumentList.Arguments.Count} arguments total)"
                );
                return null;
        }
    }
}
