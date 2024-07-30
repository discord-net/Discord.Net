using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public class InterfaceProxy : IGenerationTask<InterfaceProxy.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> properties) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> Properties { get; } = properties;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax) &&
                   Properties.Count == other.Properties.Count &&
                   Properties.All(x =>
                       Properties.ContainsKey(x.Key) &&
                       Properties[x.Key].Count == x.Value.Count &&
                       Properties[x.Key].All(y => x.Value.Any(z => z.IsEquivalentTo(y)))
                   );
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (ClassDeclarationSyntax.GetHashCode() * 397) ^ Properties.GetHashCode();
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token)
    {
        if (node is not ClassDeclarationSyntax classDeclarationSyntax)
            return false;

        return classDeclarationSyntax.Members.Any(x => x is PropertyDeclarationSyntax {AttributeLists.Count: > 0});
    }

    public GenerationTarget? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token)
    {
        if (context.Node is not ClassDeclarationSyntax target) return null;

        var dict = ComputeProxiedTypeProperties(target, context.SemanticModel);

        if (dict.Count > 0)
        {
            return new GenerationTarget(
                context.SemanticModel,
                target,
                dict
            );
        }

        return null;
    }

    public static bool WillHaveProxiedMemberFor(
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol,
        SemanticModel model,
        ISymbol member)
    {
        return GetProxiedInterfaceMembers(syntax, symbol, model).Contains(member);
    }

    public static ImmutableHashSet<ISymbol> GetProxiedInterfaceMembers(ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol, SemanticModel semanticModel)
    {
        var proxiedProperties = ComputeProxiedTypeProperties(syntax, semanticModel);
        return proxiedProperties
            .SelectMany(kvp => GetTargetTypesToProxy(kvp.Value, semanticModel, symbol.Interfaces))
            .SelectMany(x => x.GetMembers().Where(IsValidMember))
            .ToImmutableHashSet(SymbolEqualityComparer.Default);
    }

    private static Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> ComputeProxiedTypeProperties(
        ClassDeclarationSyntax type,
        SemanticModel semanticModel)
    {
        var result = new Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>>();

        foreach (var member in type.Members)
        {
            if (member is not PropertyDeclarationSyntax property) continue;

            foreach (var attribute in property.AttributeLists.SelectMany(x => x.Attributes))
            {
                if (ModelExtensions.GetSymbolInfo(semanticModel, attribute).Symbol is not IMethodSymbol
                    attributeSymbol) continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Discord.ProxyInterfaceAttribute") continue;

                result.Add(
                    property,
                    attribute.ArgumentList?.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList() ?? []
                );
            }
        }

        return result;
    }

    private static IEnumerable<INamedTypeSymbol> GetTargetTypesToProxy(
        List<TypeOfExpressionSyntax> types,
        SemanticModel semanticModel,
        IEnumerable<INamedTypeSymbol> defaultCase
    )
    {
        if (types.Count == 0)
            return defaultCase;

        return types
            .Select(x =>
            {
                var genericNode = x.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();

                return ModelExtensions.GetTypeInfo(semanticModel, genericNode ?? x.Type).Type;
            })
            .Where(x => x is not null)!
            .SelectMany<ITypeSymbol, ITypeSymbol>(x => [x, ..x!.AllInterfaces])
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>();
    }

    private static HashSet<INamedTypeSymbol> CorrectTargetList(
        INamedTypeSymbol targetType,
        HashSet<INamedTypeSymbol> currentTypesTemplateTargets,
        IEnumerable<INamedTypeSymbol> targetInterfaces,
        Logger logger)
    {
        var iteratedTargetInterfaces = targetInterfaces.ToArray();

        foreach (var targets in iteratedTargetInterfaces)
        {
            logger.Log($" - Target: {targets}");
        }

        foreach (var template in currentTypesTemplateTargets)
        {
            logger.Log($" - Template: {template}");
        }

        var correctedTargetInterfaces = ExtendInterfaceDefaults
            .CorrectTemplateExtensionInterfaces(
                iteratedTargetInterfaces
                    .Concat(currentTypesTemplateTargets)
                    .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
            );

        correctedTargetInterfaces.IntersectWith(iteratedTargetInterfaces);

        // finally, only include interfaces present on the type itself
        correctedTargetInterfaces.IntersectWith(targetType.AllInterfaces);

        foreach (var targets in correctedTargetInterfaces)
        {
            logger.Log($" - Corrected: {targets}");
        }

        return correctedTargetInterfaces;
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target, Logger logger)
    {
        if (target is null) return;

        if (ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax) is not
            INamedTypeSymbol targetSymbol)
            return;

        var templateTargets = ExtendInterfaceDefaults.GetTemplateExtensionInterfaces(targetSymbol);

        var syntax = SyntaxUtils.CreateSourceGenClone(target.ClassDeclarationSyntax);

        var addedMembers = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

        foreach (var kvp in target.Properties)
        {
            var property = target.SemanticModel.GetDeclaredSymbol(kvp.Key);

            if (property is null)
                continue;

            var extendedMembers =
                ExtendInterfaceDefaults.GetTargetInterfaces(property.Type, logger) is { } extendedTarget
                    ? ExtendInterfaceDefaults.GetTargetMembersForImplementation(
                        extendedTarget,
                        property.Type
                    ).ToImmutableHashSet(SymbolEqualityComparer.Default)
                    : ImmutableHashSet<ISymbol>.Empty;

            logger.Log($"{targetSymbol}: Processing {property.Type} {property.Name}");

            var correctedTargetInterfaces = CorrectTargetList(
                targetSymbol,
                templateTargets,
                GetTargetTypesToProxy(
                        kvp.Value,
                        target.SemanticModel,
                        property.Type.AllInterfaces
                    )
                    .Concat(extendedTarget.OfType<INamedTypeSymbol>()),
                logger
            );

            foreach (var targetInterface in correctedTargetInterfaces)
            {
                if (
                    AddProxiedMembers(
                        ref syntax,
                        targetInterface,
                        targetSymbol,
                        property,
                        extendedMembers,
                        target.SemanticModel,
                        logger,
                        shouldAdd: member => addedMembers.Add(member)
                    ) == 0
                ) continue;
            }

            var rootProxiedInterfaceTypeSyntax = SyntaxFactory.GenericName(
                SyntaxFactory.Identifier("Discord.IProxied"),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList([
                        SyntaxFactory.ParseTypeName(property.Type.ToDisplayString())
                    ])
                )
            );
            syntax = syntax.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    rootProxiedInterfaceTypeSyntax
                )
            );

            syntax = syntax.AddMembers([
                SyntaxFactory.PropertyDeclaration(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName(property.Type.ToDisplayString()),
                    SyntaxFactory.ExplicitInterfaceSpecifier(
                        rootProxiedInterfaceTypeSyntax
                    ),
                    SyntaxFactory.Identifier("ProxiedValue"),
                    null,
                    SyntaxFactory.ArrowExpressionClause(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.ThisExpression(),
                            SyntaxFactory.IdentifierName(property.Name)
                        )
                    ),
                    null,
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                )
            ]);
        }

        context.AddSource(
            $"InterfaceProxy/{target.ClassDeclarationSyntax.Identifier}_Proxied.g.cs",
            $$"""
              namespace {{ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax)!.ContainingNamespace}};

              #nullable enable

              {{syntax.NormalizeWhitespace()}}

              #nullable restore
              """
        );
    }

    private static bool IsValidMember(ISymbol symbol)
    {
        return symbol switch
        {
            IPropertySymbol {IsStatic: false, ExplicitInterfaceImplementations.Length: 0} => true,
            IMethodSymbol
            {
                IsStatic: false, MethodKind: MethodKind.Ordinary or MethodKind.ExplicitInterfaceImplementation
            } => true,
            _ => false
        };
    }

    private static int AddProxiedMembers(
        ref ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol toProxy,
        INamedTypeSymbol targetSymbol,
        IPropertySymbol proxiedTo,
        ImmutableHashSet<ISymbol> extendedMembers,
        SemanticModel semanticModel,
        Logger logger,
        Action<ISymbol>? onAdd = null,
        Predicate<ISymbol>? shouldAdd = null)
    {
        var count = 0;

        logger.Log($"{targetSymbol}: Proxying {proxiedTo} as {toProxy}");

        var castedSyntax = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AsExpression,
                SyntaxFactory.IdentifierName(proxiedTo.Name),
                SyntaxFactory.ParseTypeName(toProxy.ToDisplayString())
            )
        );

        var willHaveFetchMethods = RestLoadable.WillHaveFetchMethods(targetSymbol);

        var implementedMembers = TypeUtils.GetBaseTypesAndThis(proxiedTo.Type)
            .SelectMany(x => x.GetMembers())
            .Distinct(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var member in toProxy
                     .GetMembers()
                     .Where(IsValidMember))
        {
            if (!shouldAdd?.Invoke(member) ?? false)
                continue;

            var targetExplicitImplementation = targetSymbol.FindImplementationForInterfaceMember(member);
            var proxiedTypeExplicitImplementation = proxiedTo.Type.FindImplementationForInterfaceMember(member);

            var proxyHasUniqueImplementation = (!proxiedTypeExplicitImplementation?.ContainingType.Equals(
                member.ContainingType,
                SymbolEqualityComparer.Default
            ) ?? false) || extendedMembers.Contains(member);

            var targetHasUniqueImplementation = !targetExplicitImplementation?.ContainingType.Equals(
                member.ContainingType,
                SymbolEqualityComparer.Default
            ) ?? false;

            if (targetHasUniqueImplementation && !proxyHasUniqueImplementation)
            {
                logger.Warn($"{targetSymbol}: Skipping {member}, unique implementation");
                continue;
            }

            if (
                member.IsVirtual &&
                !targetHasUniqueImplementation &&
                !proxyHasUniqueImplementation
            )
            {
                logger.Warn(
                    $"{targetSymbol}: Skipping {member}, implemented in {targetExplicitImplementation} and {proxiedTypeExplicitImplementation}"
                );
                continue;
            }

            if (targetHasUniqueImplementation && member.IsAbstract)
            {
                logger.Log($"{targetSymbol}: Skipping {member}, abstract implementation in target");
                continue;
            }

            if (willHaveFetchMethods &&
                member is IMethodSymbol memberMethod &&
                MemberUtils.GetMemberName(memberMethod, x => x.ExplicitInterfaceImplementations) == "FetchAsync")
            {
                // only proxy if the result of fetch async isn't assignable to this but assignable to the proxies
                var restEntity = RestLoadable.GetRestEntity(targetSymbol, semanticModel);

                if (restEntity is null || memberMethod.ReturnType is not INamedTypeSymbol
                    {
                        Name: "ValueTask"
                    } returnType)
                    continue;

                if (semanticModel.Compilation.HasImplicitConversion(restEntity, returnType.TypeArguments[0]))
                    continue;
            }

            var targetImplementation = implementedMembers.Concat(extendedMembers)
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    (
                        (
                            x is IMethodSymbol fromMethod &&
                            member is IMethodSymbol toMethod &&
                            TypeUtils.TypeIsOfProvidedOrDescendant(fromMethod.ReturnType, toMethod.ReturnType,
                                semanticModel)
                        )
                        ||
                        (
                            x is IPropertySymbol fromProp &&
                            member is IPropertySymbol toProp &&
                            TypeUtils.TypeIsOfProvidedOrDescendant(fromProp.Type, toProp.Type, semanticModel)
                        )
                    )
                );

            if (
                targetImplementation is null &&
                proxyHasUniqueImplementation &&
                (
                    (
                        proxiedTypeExplicitImplementation?
                            .ContainingType
                            .Equals(proxiedTo.Type, SymbolEqualityComparer.Default)
                        ?? false
                    )
                    ||
                    extendedMembers.Contains(member)
                )
            )
            {
                logger.Log(
                    $"{targetSymbol}: Choosing unique target implementation {proxiedTypeExplicitImplementation}");
                targetImplementation = proxiedTypeExplicitImplementation;
            }

            if (willHaveFetchMethods &&
                targetImplementation is IMethodSymbol targetMethodImplementation &&
                MemberUtils.GetMemberName(targetMethodImplementation, x => x.ExplicitInterfaceImplementations) ==
                "FetchAsync")
            {
                logger.Log($"{targetSymbol}: Skipping FetchAsync {proxiedTypeExplicitImplementation}");
                continue;
            }

            var canProxyToSelfImplementation =
                !proxyHasUniqueImplementation &&
                targetImplementation is not null &&
                MemberUtils.CanOverride(targetImplementation, member, semanticModel.Compilation);

            var selfHasTargetImplementation = TypeUtils
                .GetBaseTypesAndThis(targetSymbol)
                .Any(typeSymbol =>
                    typeSymbol
                        .GetMembers()
                        .Any(typeSymbolMember => MemberUtils
                            .Conflicts(typeSymbolMember, member)
                        )
                    ||
                    (
                        !typeSymbol.Equals(targetSymbol, SymbolEqualityComparer.Default) &&
                        typeSymbol.DeclaringSyntaxReferences.Length > 0 &&
                        typeSymbol.DeclaringSyntaxReferences[0].GetSyntax() is ClassDeclarationSyntax
                            baseDeclarationSyntax &&
                        ComputeProxiedTypeProperties(
                                baseDeclarationSyntax,
                                semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax
                                    .SyntaxTree)
                            )
                            .Any(baseProxiedProperties =>
                            {
                                var baseSemantic =
                                    semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax
                                        .SyntaxTree);

                                var baseProperty =
                                    baseSemantic.GetDeclaredSymbol(baseProxiedProperties.Key);

                                if (baseProperty is null)
                                {
                                    logger.Warn(
                                        $"Couldn't find base semantic property {baseProxiedProperties.Key.Identifier} for {typeSymbol} ({targetSymbol})");
                                    return false;
                                }

                                return GetTargetTypesToProxy(
                                        baseProxiedProperties.Value,
                                        semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax
                                            .SyntaxTree),
                                        baseProperty.Type.AllInterfaces
                                    )
                                    .Any(baseTargetProxyType => baseTargetProxyType
                                        .GetMembers()
                                        .Any(baseTargetMember =>
                                            MemberUtils.Conflicts(baseTargetMember, member)
                                        )
                                    );
                            })
                    )
                );

            var shouldImplementMember =
                (
                    !selfHasTargetImplementation ||
                    (targetImplementation?.IsOverride ?? false)
                )
                &&
                (
                    targetImplementation is null ||
                    !MemberUtils.IsExplicitInterfaceImplementation(targetImplementation)
                )
                &&
                member.ContainingType
                    .GetAttributes()
                    .All(x => x.AttributeClass?.ToDisplayString() != "Discord.NoExposureAttribute");

            var canImplementExplicitSelector = MemberUtils.CanImplementMemberExplicitly(member);

            logger.Log(
                $"{targetSymbol}: Proxying {member}\n" +
                $" - Adding own member?: {shouldImplementMember}\n" +
                $" - Proxy has unique implementation?: {proxyHasUniqueImplementation}\n" +
                $" - Self has unique implementation?: {targetHasUniqueImplementation}\n" +
                $" - Self has target implementation?: {selfHasTargetImplementation}\n" +
                $" - Can proxy to self implementation?: {canProxyToSelfImplementation}\n" +
                $" - Target implementation?: {targetImplementation}\n" +
                $" - Can implement explicitly?: {canImplementExplicitSelector}"
            );

            switch (member)
            {
                case IPropertySymbol property:
                    if (property.ExplicitInterfaceImplementations.Length > 0 || property.IsStatic)
                        break;

                    if (targetImplementation is IPropertySymbol targetProperty && shouldImplementMember)
                    {
                        var returnType =
                            proxiedTo.Type is INamedTypeSymbol namedProxyToType
                                ? ExtendInterfaceDefaults.GetTargetTypeForImplementation(
                                    targetProperty,
                                    targetProperty.Type,
                                    namedProxyToType,
                                    semanticModel,
                                    logger
                                )
                                : targetProperty.Type;

                        var propertySyntax = SyntaxFactory.PropertyDeclaration(
                            [],
                            CreateAccessors(targetProperty.DeclaredAccessibility)
                                .AddRange(CreateModifiers(targetProperty)),
                            SyntaxFactory.IdentifierName(returnType.ToDisplayString()),
                            null,
                            SyntaxFactory.Identifier(property.Name),
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.IdentifierName(proxiedTo.Name),
                                    SyntaxFactory.IdentifierName(property.Name)
                                )
                            ),
                            null,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        );

                        if (classDeclarationSyntax.Members.All(x =>
                                x.WithLeadingTrivia(null).ToString() != propertySyntax.ToString()))
                        {
                            classDeclarationSyntax = classDeclarationSyntax
                                .AddMembers(
                                    propertySyntax
                                        .WithLeadingTrivia(
                                            SyntaxFactory.Comment($"// {property} through {targetImplementation}")
                                        )
                                );
                        }
                    }

                    if (canImplementExplicitSelector)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.PropertyDeclaration(
                                    [],
                                    [],
                                    SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                                    SyntaxFactory.ExplicitInterfaceSpecifier(
                                        SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
                                    ),
                                    SyntaxFactory.Identifier(property.Name),
                                    null,
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            canProxyToSelfImplementation
                                                ? SyntaxFactory.ThisExpression()
                                                : castedSyntax,
                                            SyntaxFactory.IdentifierName(property.Name)
                                        )
                                    ),
                                    null,
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                )
                                .WithLeadingTrivia(
                                    SyntaxFactory.Comment($"// {property}")
                                )
                        );
                    }

                    if (
                        (targetImplementation is not IPropertySymbol || !shouldImplementMember) &&
                        !canImplementExplicitSelector
                    ) break;

                    onAdd?.Invoke(property);
                    count++;
                    break;

                case IMethodSymbol method:
                    if (method.IsStatic || method.MethodKind is not MethodKind.Ordinary)
                        break;

                    if (targetImplementation is IMethodSymbol targetMethod && shouldImplementMember)
                    {
                        var returnType =
                            proxiedTo.Type is INamedTypeSymbol namedProxyToType
                                ? ExtendInterfaceDefaults.GetTargetTypeForImplementation(
                                    targetMethod,
                                    targetMethod.ReturnType,
                                    namedProxyToType,
                                    semanticModel,
                                    logger
                                )
                                : targetMethod.ReturnType;

                        var methodSyntax = SyntaxFactory.MethodDeclaration(
                            [],
                            CreateAccessors(targetMethod.DeclaredAccessibility)
                                .AddRange(CreateModifiers(targetMethod)),
                            SyntaxFactory.IdentifierName(returnType.ToDisplayString()),
                            null,
                            SyntaxFactory.Identifier(method.Name),
                            method.TypeParameters.Length > 0
                                ? SyntaxFactory.TypeParameterList(
                                    SyntaxFactory.SeparatedList(
                                        method.TypeParameters.Select(x =>
                                            SyntaxFactory.TypeParameter(x.ToDisplayString())
                                        )
                                    )
                                )
                                : null,
                            SyntaxFactory.ParameterList(
                                SyntaxFactory.SeparatedList(
                                    method.Parameters.Select(x =>
                                        SyntaxFactory.Parameter(
                                            new SyntaxList<AttributeListSyntax>(),
                                            new SyntaxTokenList(),
                                            SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                                            SyntaxFactory.Identifier(x.Name),
                                            null
                                        )
                                    )
                                )
                            ),
                            [],
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(proxiedTo.Name),
                                        SyntaxFactory.IdentifierName(method.Name)
                                    ),
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList(
                                            method.Parameters.Select(x =>
                                                SyntaxFactory.Argument(
                                                    null,
                                                    SyntaxFactory.Token(SyntaxKind.None),
                                                    SyntaxFactory.IdentifierName(x.Name)
                                                )
                                            )
                                        )
                                    )
                                )
                            ),
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        );

                        if (classDeclarationSyntax.Members.All(x =>
                                x.WithLeadingTrivia(null).ToString() != methodSyntax.ToString()))
                        {
                            classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                                methodSyntax
                                    .WithLeadingTrivia(
                                        SyntaxFactory.Comment($"// {method} through {targetImplementation}")
                                    )
                            );
                        }
                    }

                    if (canImplementExplicitSelector)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.MethodDeclaration(
                                    [],
                                    [],
                                    SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                                    SyntaxFactory.ExplicitInterfaceSpecifier(
                                        SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
                                    ),
                                    SyntaxFactory.Identifier(method.Name),
                                    method.TypeParameters.Length > 0
                                        ? SyntaxFactory.TypeParameterList(
                                            SyntaxFactory.SeparatedList(
                                                method.TypeParameters.Select(x =>
                                                    SyntaxFactory.TypeParameter(x.ToDisplayString())
                                                )
                                            )
                                        )
                                        : null,
                                    SyntaxFactory.ParameterList(
                                        SyntaxFactory.SeparatedList(
                                            method.Parameters.Select(x =>
                                                SyntaxFactory.Parameter(
                                                    new SyntaxList<AttributeListSyntax>(),
                                                    new SyntaxTokenList(),
                                                    SyntaxFactory.IdentifierName(x.Type.ToDisplayString()),
                                                    SyntaxFactory.Identifier(x.Name),
                                                    null
                                                )
                                            )
                                        )
                                    ),
                                    [],
                                    null,
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.InvocationExpression(
                                            SyntaxFactory.MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                canProxyToSelfImplementation
                                                    ? SyntaxFactory.ThisExpression()
                                                    : castedSyntax,
                                                SyntaxFactory.IdentifierName(method.Name)
                                            ),
                                            SyntaxFactory.ArgumentList(
                                                SyntaxFactory.SeparatedList(
                                                    method.Parameters.Select(x =>
                                                        SyntaxFactory.Argument(
                                                            null,
                                                            SyntaxFactory.Token(SyntaxKind.None),
                                                            SyntaxFactory.IdentifierName(x.Name)
                                                        )
                                                    )
                                                )
                                            )
                                        )
                                    ),
                                    SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                                )
                                .WithLeadingTrivia(
                                    SyntaxFactory.Comment($"// {method}")
                                )
                        );
                    }

                    if (
                        (targetImplementation is not IMethodSymbol || !shouldImplementMember) &&
                        !canImplementExplicitSelector
                    ) break;

                    onAdd?.Invoke(method);
                    count++;
                    break;
            }
        }

        return count;
    }

    private static IEnumerable<SyntaxToken> CreateModifiers(ISymbol symbol)
    {
        if (symbol is {IsVirtual: true, ContainingType: not {TypeKind: TypeKind.Interface}})
            yield return SyntaxFactory.Token(SyntaxKind.VirtualKeyword);

        if (symbol is {IsAbstract: true, ContainingType: not {TypeKind: TypeKind.Interface}})
            yield return SyntaxFactory.Token(SyntaxKind.AbstractKeyword);

        if (symbol.IsSealed)
            yield return SyntaxFactory.Token(SyntaxKind.SealedKeyword);

        if (symbol.IsOverride)
            yield return SyntaxFactory.Token(SyntaxKind.OverrideKeyword);
    }

    private static SyntaxTokenList CreateAccessors(Accessibility accessibility)
    {
        return SyntaxFactory.TokenList(accessibility switch
        {
            Accessibility.Private => [SyntaxFactory.Token(SyntaxKind.PrivateKeyword)],
            Accessibility.Protected => [SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)],
            Accessibility.ProtectedOrInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
            ],
            Accessibility.Internal => [SyntaxFactory.Token(SyntaxKind.InternalKeyword)],
            Accessibility.ProtectedAndInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.InternalKeyword)
            ],
            Accessibility.Public => [SyntaxFactory.Token(SyntaxKind.PublicKeyword)],
            _ => []
        });
    }
}
