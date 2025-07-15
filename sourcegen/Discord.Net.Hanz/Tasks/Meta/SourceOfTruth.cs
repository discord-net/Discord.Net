using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Discord.Net.Hanz.Utils.Bakery;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Meta;

public sealed class SourceOfTruth : GenerationTask
{
    public abstract record Target(
        TypeRef ContainingType,
        ImmutableEquatableArray<string> Namespaces
    );

    public readonly record struct TargetOverride<T>(
        TypeRef ContainingType,
        T Target
    );

    public sealed record MethodTarget(
        TypeRef ContainingType,
        ImmutableEquatableArray<string> Namespaces,
        MethodSpec Method,
        ImmutableEquatableArray<TargetOverride<MethodSpec>> OverrideTargets
    ) : Target(ContainingType, Namespaces);

    public sealed record PropertyTarget(
        TypeRef ContainingType,
        ImmutableEquatableArray<string> Namespaces,
        PropertySpec Property,
        ImmutableEquatableArray<TargetOverride<PropertySpec>> OverrideTargets
    ) : Target(ContainingType, Namespaces);

    public SourceOfTruth(IncrementalGeneratorInitializationContext context, ILogger logger) : base(context, logger)
    {
        context.RegisterSourceOutput(
            context
                .SyntaxProvider
                .ForAttributeWithMetadataName(
                    "Discord.SourceOfTruthAttribute",
                    (node, _) => node is MemberDeclarationSyntax,
                    Map
                )
                .WhereNotNull()
                .GroupBy(x => x.ContainingType)
                .Select(CreateSourceSpec)
        );
    }

    private static SourceSpec CreateSourceSpec(TypeRef containingType, ImmutableArray<Target> targets)
    {
        var typeSpec = TypeSpec.From(containingType).AddModifiers("partial");

        foreach (var target in targets)
        {
            switch (target)
            {
                case MethodTarget methodTarget:
                    AddMethod(ref typeSpec, methodTarget);
                    break;
                case PropertyTarget propertyTarget:
                    AddProperty(ref typeSpec, propertyTarget);
                    break;
            }
        }

        return new SourceSpec(
            $"SourceOfTruth/{containingType.MetadataName}",
            containingType.Namespace,
            new(targets.SelectMany(x => x.Namespaces)),
            Types: new([typeSpec])
        );
    }

    private static void AddMethod(ref TypeSpec spec, MethodTarget target)
    {
        spec = spec.AddMethods(
            target.OverrideTargets.Select(FormatMethodSpec)
        );

        MethodSpec FormatMethodSpec(TargetOverride<MethodSpec> targetOverride)
        {
            var invocation = target.Method.ToInvocationString();
            var modifiers = targetOverride
                .Target
                .Modifiers
                .Intersect(["static"])
                .ToImmutableEquatableArray();

            if (target.Method.Modifiers.Contains("async") || targetOverride.Target.Modifiers.Contains("async"))
            {
                invocation = $"await {invocation}";
                modifiers = modifiers.Add("async");
            }

            var spec = targetOverride.Target with
            {
                ExplicitInterfaceImplementation = targetOverride.ContainingType.ReferenceName,
                Modifiers = modifiers,
                Expression = invocation
            };

            return spec;
        }
    }

    private static void AddProperty(ref TypeSpec spec, PropertyTarget target)
    {
        spec = spec.AddProperties(
            target.OverrideTargets.Select(FormatPropertySpec)
        );

        PropertySpec FormatPropertySpec(TargetOverride<PropertySpec> targetOverride)
        {
            var spec = targetOverride.Target with
            {
                ExplicitInterfaceImplementation = targetOverride.ContainingType.ReferenceName,
                Modifiers = targetOverride.Target.Modifiers.Intersect(["static"]).ToImmutableEquatableArray(),
                Accessibility = Accessibility.NotApplicable
            };

            if (spec is {HasGetter: true, HasSetter: true})
            {
                return spec with
                {
                    Getter = target.Property.Name,
                    Setter = $"{target.Property.Name} = value;"
                };
            }

            return spec with
            {
                Expression = target.Property.Name
            };
        }
    }

    private Target? Map(GeneratorAttributeSyntaxContext context, CancellationToken token)
    {
        if (context.TargetNode is not MemberDeclarationSyntax syntax)
        {
            Logger.Warn($"{context.TargetSymbol}: Not a member declaration");
            return null;
        }

        if (syntax.Parent is not TypeDeclarationSyntax typeDeclaration)
        {
            Logger.Warn($"{context.TargetSymbol}: parent is not a type declaration");
            return null;
        }

        if (typeDeclaration.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            Logger.Warn($"{context.TargetSymbol}: parent is not partial");
            return null;
        }
        
        Logger.Log($"Mapping {context.TargetSymbol}...");
        
        return context.TargetSymbol switch
        {
            IMethodSymbol {ExplicitInterfaceImplementations.Length: 0, MethodKind: MethodKind.Ordinary} method
                => MapMethod(method, context.SemanticModel, syntax, token),
            IPropertySymbol {ExplicitInterfaceImplementations.Length: 0} property
                => MapProperty(property, context.SemanticModel, syntax, token, Logger),
            _ => null
        };
    }

    private PropertyTarget? MapProperty(
        IPropertySymbol symbol, 
        SemanticModel model,
        MemberDeclarationSyntax syntax,
        CancellationToken token,
        ILogger logger)
    {
        if (symbol.ContainingType is not { } containingType)
        {
            Logger.Warn($"{symbol}: containing type is null");
            return null;
        }

        if (containingType.AllInterfaces.Length == 0)
        {
            Logger.Warn($"{symbol}: no interfaces");
            return null;
        }

        var overrideSpecs = new List<TargetOverride<PropertySpec>>();

        foreach (var interfaceSymbol in containingType.AllInterfaces)
        foreach (var interfaceProperty in interfaceSymbol.GetMembers().OfType<IPropertySymbol>())
        {
            token.ThrowIfCancellationRequested();

            if (interfaceProperty.Name != symbol.Name)
                continue;

            if (interfaceProperty.ExplicitInterfaceImplementations.Length > 0)
                continue;

            if (interfaceProperty.IsSealed || !(interfaceProperty.IsAbstract || interfaceProperty.IsVirtual))
                continue;

            if (!CanOverride(interfaceProperty))
            {
                continue;
            }

            overrideSpecs.Add(new TargetOverride<PropertySpec>(
                    new TypeRef(interfaceSymbol),
                    PropertySpec.From(interfaceProperty)
                )
            );
        }

        if (overrideSpecs.Count == 0)
        {
            Logger.Warn($"{symbol}: no override targets");
        }
        else
        {
            Logger.Log($"{symbol}: {overrideSpecs.Count} override targets");
        }
        
        return overrideSpecs.Count > 0
            ? new PropertyTarget(
                new TypeRef(symbol.ContainingType),
                syntax.GetUsingDirectivesSyntax()
                    .Select(x => x.Name?.ToString())
                    .Where(x => x is not null)!
                    .ToImmutableEquatableArray<string>(),
                PropertySpec.From(symbol),
                overrideSpecs.ToImmutableEquatableArray()
            )
            : null;

        bool CanOverride(IPropertySymbol target)
        {
            if (symbol.Type is ITypeParameterSymbol typeParameter)
            {
                foreach (var constraint in typeParameter.ConstraintTypes)
                {
                    if (
                        !target.Type.Equals(constraint, SymbolEqualityComparer.Default) &&
                        !model.Compilation.HasImplicitConversion(constraint, target.Type)
                    )
                    {
                        logger.Warn($"{symbol} cannot override {target}: constraint {constraint} is unassignable");
                        return false;
                    }
                }

                if (typeParameter.HasValueTypeConstraint && !target.Type.IsValueType)
                {
                    logger.Warn($"{symbol} cannot override {target}: not a value type");
                    return false;
                }

                if (typeParameter.HasReferenceTypeConstraint && target.Type.IsValueType)
                {
                    logger.Warn($"{symbol} cannot override {target}: value type");
                    return false;
                }

                return true;
            }

            if (target.Type.Equals(symbol.Type, SymbolEqualityComparer.Default))
                return true;

            if (model.Compilation.HasImplicitConversion(symbol.Type, target.Type))
                return true;

            logger.Warn($"{symbol}: {symbol.Type} isn't convertable to {target.Type}");
            
            return false;
        }
    }

    private static MethodTarget? MapMethod(
        IMethodSymbol symbol,
        SemanticModel model,
        MemberDeclarationSyntax syntax,
        CancellationToken token)
    {
        if (symbol.ContainingType is not { } containingType)
            return null;

        if (containingType.AllInterfaces.Length == 0)
            return null;

        var overrideSpecs = new List<TargetOverride<MethodSpec>>();

        foreach (var interfaceSymbol in containingType.AllInterfaces)
        foreach (var interfaceMethod in interfaceSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            token.ThrowIfCancellationRequested();

            if (interfaceMethod.MethodKind != symbol.MethodKind)
                continue;

            if (interfaceMethod.Name != symbol.Name)
                continue;

            if (interfaceMethod.ExplicitInterfaceImplementations.Length > 0)
                continue;

            if (interfaceMethod.IsSealed || !(interfaceMethod.IsAbstract || interfaceMethod.IsVirtual))
                continue;

            if (!CanOverrideMethod(symbol, interfaceMethod, model.Compilation, out var isAsync))
                continue;

            var spec = MethodSpec.From(interfaceMethod);

            if (isAsync)
                spec = spec with
                {
                    Modifiers = spec.Modifiers.Add("async")
                };

            overrideSpecs.Add(new TargetOverride<MethodSpec>(
                    new TypeRef(interfaceSymbol),
                    spec
                )
            );
        }

        return overrideSpecs.Count > 0
            ? new MethodTarget(
                new TypeRef(symbol.ContainingType),
                syntax.GetUsingDirectivesSyntax()
                    .Select(x => x.Name?.ToString())
                    .Where(x => x is not null)
                    .ToImmutableEquatableArray(),
                MethodSpec.From(symbol),
                overrideSpecs.ToImmutableEquatableArray()
            )
            : null;
    }

    private static bool CanOverrideMethod(
        IMethodSymbol target,
        IMethodSymbol baseMethod,
        Compilation compilation,
        out bool isAsync)
    {
        isAsync = false;

        return
            CanUseReturnType(ref isAsync) &&
            ParametersMatch();

        bool ParametersMatch()
        {
            if (target.Parameters.Length != baseMethod.Parameters.Length)
                return false;

            for (var i = 0; i < target.Parameters.Length; i++)
            {
                var parameter = target.Parameters[i];

                if (!TypeIsAssignable(parameter.Type, baseMethod.Parameters[i].Type))
                    return false;
            }

            return true;
        }

        bool CanUseReturnType(ref bool isAsync)
        {
            if (TypeIsAssignable(target.ReturnType, baseMethod.ReturnType))
                return true;

            if (
                target.ReturnType is INamedTypeSymbol {Name: "ValueTask" or "Task", TypeArguments.Length: 1} targetAsync 
                &&
                baseMethod.ReturnType is INamedTypeSymbol {Name: "ValueTask" or "Task", TypeArguments.Length: 1} baseAsync
            )
            {
                return isAsync = TypeIsAssignable(targetAsync.TypeArguments[0], baseAsync.TypeArguments[0]);
            }

            return false;
        }

        bool TypeIsAssignable(ITypeSymbol a, ITypeSymbol b)
        {
            return a.Equals(b, SymbolEqualityComparer.Default) || compilation.HasImplicitConversion(a, b);
        }
    }
    
}