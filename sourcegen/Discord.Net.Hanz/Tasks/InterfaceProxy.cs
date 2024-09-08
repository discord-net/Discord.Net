using Discord.Net.Hanz.Tasks.Gateway;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public class InterfaceProxy : ISyntaxGenerationCombineTask<InterfaceProxy.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol symbol,
        Dictionary<IPropertySymbol, List<INamedTypeSymbol>> properties
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public Dictionary<IPropertySymbol, List<INamedTypeSymbol>> Properties { get; } = properties;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax) &&
                   Properties.Count == other.Properties.Count &&
                   Properties.All(x =>
                       Properties.ContainsKey(x.Key) &&
                       Properties[x.Key].Count == x.Value.Count &&
                       Properties[x.Key].SequenceEqual(x.Value, SymbolEqualityComparer.Default)
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

        if (context.SemanticModel.GetDeclaredSymbol(target) is not { } symbol)
            return null;

        var dict = ComputeProxiedTypeProperties(symbol);

        if (dict.Count == 0)
            return null;

        return new GenerationTarget(
            context.SemanticModel,
            target,
            symbol,
            dict
        );
    }

    public static bool WillHaveProxiedMemberFor(
        INamedTypeSymbol symbol,
        ISymbol member)
    {
        return ComputeProxiedTypeProperties(symbol)
            .Any(x =>
                GetTargetTypesToProxy(x.Value, x.Key.Type.AllInterfaces)
                    .Any(x => x
                        .GetMembers()
                        .Where(IsValidMember)
                        .Contains(member, SymbolEqualityComparer.Default)
                    )
            );
    }

    public static HashSet<ProxiedMember> GetProxiedMembers(
        INamedTypeSymbol symbol,
        SemanticModel semanticModel)
    {
        return
        [
            ..ComputeProxiedTypeProperties(symbol)
                .Select(x => (Property: x.Key, Interfaces: GetTargetTypesToProxy(x.Value, x.Key.Type.AllInterfaces)))
                .SelectMany(x =>
                    GetProxiedMembers(symbol, x.Property, x.Interfaces.ToList(), semanticModel)
                )
        ];
    }

    public static bool WillHaveConflictingMemberFor(
        INamedTypeSymbol symbol,
        ISymbol member,
        SemanticModel semanticModel,
        ref HashSet<ProxiedMember>? members)
    {
        members ??= GetProxiedMembers(symbol, semanticModel);
        return members.Any(x => MemberUtils.Conflicts(x.Symbol, member));
    }

    public static Dictionary<IPropertySymbol, List<INamedTypeSymbol>> ComputeProxiedTypeProperties(
        INamedTypeSymbol symbol)
    {
        var result = new Dictionary<IPropertySymbol, List<INamedTypeSymbol>>(SymbolEqualityComparer.Default);

        foreach (var property in symbol.GetMembers().OfType<IPropertySymbol>())
        {
            var attribute = property.GetAttributes()
                .FirstOrDefault(x =>
                    x.AttributeClass?.ToDisplayString() == "Discord.ProxyInterfaceAttribute"
                );

            if (attribute is null) continue;


            var types = attribute.ConstructorArguments
                .FirstOrDefault()
                .Values
                .Select(x => x.Value as INamedTypeSymbol)
                .OfType<INamedTypeSymbol>()
                .ToList();

            if (types.Count == 0 && property.Type is ITypeParameterSymbol typeParameter)
            {
                types.AddRange(typeParameter.ConstraintTypes.OfType<INamedTypeSymbol>());
            }

            result.Add(property, types);
        }

        return result;
    }

    public static ImmutableHashSet<ISymbol> GetProxiedInterfaceMembers(INamedTypeSymbol symbol)
    {
        return ComputeProxiedTypeProperties(symbol)
            .SelectMany(x =>
                GetTargetTypesToProxy(x.Value, x.Key.Type.AllInterfaces)
                    .SelectMany(x => x.GetMembers().Where(IsValidMember))
            )
            .ToImmutableHashSet(SymbolEqualityComparer.Default);
    }

    public static IEnumerable<INamedTypeSymbol> GetTargetTypesToProxy(
        List<INamedTypeSymbol> specifiedTypes,
        IEnumerable<INamedTypeSymbol> defaultCase
    )
    {
        if (specifiedTypes.Count == 0)
            return defaultCase;

        return specifiedTypes
            .SelectMany<ITypeSymbol, ITypeSymbol>(x => [x, ..x!.AllInterfaces])
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>();
    }

    private static void AddImplicitTargetInterfaces(
        INamedTypeSymbol type,
        ref IEnumerable<INamedTypeSymbol> interfaces,
        SemanticModel semanticModel
    )
    {
        var enumerated = interfaces as INamedTypeSymbol[] ?? interfaces.ToArray();

        var cacheableInterface =
            enumerated.FirstOrDefault(x =>
                x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<")
            )
            ??
            type.BaseType?
                .Interfaces
                .FirstOrDefault(x =>
                    x.ToDisplayString().StartsWith("Discord.Gateway.ICacheableEntity<")
                );

        if (cacheableInterface is not null)
        {
            var storeInterface = semanticModel.Compilation.GetTypeByMetadataName("Discord.Gateway.IStoreProvider`2");
            var brokerInterface = semanticModel.Compilation.GetTypeByMetadataName("Discord.Gateway.IBrokerProvider`3");

            if (storeInterface is null || brokerInterface is null) return;

            interfaces = enumerated.Concat([
                storeInterface.Construct(cacheableInterface.TypeArguments[1], cacheableInterface.TypeArguments[2]),
                brokerInterface.Construct(
                    cacheableInterface.TypeArguments[1],
                    cacheableInterface.TypeArguments[0],
                    cacheableInterface.TypeArguments[2]
                )
            ]);
        }
    }

    private static HashSet<INamedTypeSymbol> CorrectTargetList(
        INamedTypeSymbol targetType,
        ITypeSymbol propertyType,
        HashSet<INamedTypeSymbol> currentTypesTemplateTargets,
        IEnumerable<INamedTypeSymbol> targetInterfaces,
        SemanticModel semanticModel,
        Logger? logger = null)
    {
        if (
            GatewayLoadable.TryGetBrokerTypes(propertyType, out var id, out var entity, out var model) &&
            semanticModel.Compilation.GetTypeByMetadataName("Discord.Gateway.IBrokerProvider`3") is { } brokerType &&
            semanticModel.Compilation.GetTypeByMetadataName("Discord.Gateway.IStoreProvider`2") is { } storeType)
        {
            targetInterfaces = targetInterfaces
                .Prepend(brokerType.Construct(id, entity, model))
                .Prepend(storeType.Construct(id, model));
        }

        AddImplicitTargetInterfaces(targetType, ref targetInterfaces, semanticModel);

        var iteratedTargetInterfaces = targetInterfaces.ToArray();

        foreach (var targets in iteratedTargetInterfaces)
        {
            logger?.Log($" - Target: {targets}");
        }

        foreach (var template in currentTypesTemplateTargets)
        {
            logger?.Log($" - Template: {template}");
        }

        var correctedTargetInterfaces = ExtendInterfaceDefaults
            .CorrectTemplateExtensionInterfaces(
                iteratedTargetInterfaces
                    .Concat(currentTypesTemplateTargets)
                    .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
            );

        correctedTargetInterfaces.IntersectWith(iteratedTargetInterfaces);

        // finally, only include interfaces present on the type itself
        var directTargetInterfaces = TypeUtils.AllDirectInterfaces(targetType);
        AddImplicitTargetInterfaces(targetType, ref directTargetInterfaces, semanticModel);

        correctedTargetInterfaces.IntersectWith(directTargetInterfaces);

        foreach (var targets in correctedTargetInterfaces)
        {
            logger?.Log($" - Corrected: {targets}");
        }

        return correctedTargetInterfaces;
    }

    private static void DedupeInstanceMemberConflicts(List<ProxiedMember> members, SemanticModel model, Logger logger)
    {
        var potentials = members
            .Where(x => x is {ShouldImplementOwnMember: true, Symbol: IMethodSymbol})
            .ToList();

        var resolved = new HashSet<ProxiedMember>();

        foreach (var potential in potentials)
        {
            if (resolved.Contains(potential)) continue;

            var conflicting = potentials
                .Where(x =>
                    MemberUtils.Conflicts(x.Symbol, potential.Symbol) &&
                    x != potential
                )
                .ToArray();

            if (conflicting.Length == 0) continue;

            var buckets = new List<ProxiedMember>() {potential};

            foreach (var conflict in conflicting)
            {
                var missCount = 0;

                for (var i = 0; i < buckets.Count; i++)
                {
                    var bucket = buckets[i];

                    var isLower = model.Compilation.HasImplicitConversion(
                        MemberUtils.GetMemberType(conflict.Symbol),
                        MemberUtils.GetMemberType(bucket.Symbol)
                    );
                    var isUpper = model.Compilation.HasImplicitConversion(
                        MemberUtils.GetMemberType(bucket.Symbol),
                        MemberUtils.GetMemberType(conflict.Symbol)
                    );

                    if (!isLower && !isUpper)
                    {
                        missCount++;
                        logger.Log($"Miss #{missCount}/{buckets.Count}");
                    }
                    else if (isUpper)
                    {
                        logger.Log($"Skipping upper-bounded {conflict.Symbol} -> {bucket.Symbol}");
                        break;
                    }
                    else if (isLower)
                    {
                        buckets.RemoveAt(i);
                        buckets.Insert(i, conflict);
                        logger.Log($"Replaced {bucket.Symbol} -> {conflict.Symbol}");
                    }
                }

                if (missCount == buckets.Count)
                {
                    logger.Log($"New bucket: {conflict.Symbol}");
                    buckets.Add(conflict);
                }
            }

            logger.Log($"picked {buckets.Count} from the following conflicting members:");

            foreach (var conflict in (ProxiedMember[]) [potential, ..conflicting])
            {
                resolved.Add(conflict);

                if (buckets.Contains(conflict)) continue;

                logger.Log($" - yeet {conflict.Symbol}");
                members.Remove(conflict);
            }

            foreach (var member in buckets)
            {
                logger.Log($" - kept {member}");
            }
        }
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var generated = new Dictionary<string, (ClassDeclarationSyntax Syntax, GenerationTarget Target)>();

        var prepared =
            new Dictionary<INamedTypeSymbol, (GenerationTarget Target, List<ProxiedMember> Members)>(
                SymbolEqualityComparer
                    .Default);

        foreach
        (
            var target in Hierarchy
                .OrderByHierarchy(
                    targets,
                    x => x.Symbol,
                    out _,
                    out _)
        )
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            var templateTargets = ExtendInterfaceDefaults.GetTemplateExtensionInterfaces(target.Symbol);

            var members = new List<ProxiedMember>();

            foreach (var property in target.Properties)
            {
                var extendedMembers =
                    ExtendInterfaceDefaults.GetTargetInterfaces(
                        property.Key.Type,
                        targetLogger
                    ) is { } extendedTargets
                        ? ExtendInterfaceDefaults.GetTargetMembersForImplementation(
                            extendedTargets,
                            property.Key.Type
                        ).ToImmutableHashSet(SymbolEqualityComparer.Default)
                        : ImmutableHashSet<ISymbol>.Empty;

                members.AddRange(
                    GetProxiedMembers(
                        target.Symbol,
                        property.Key,
                        property.Value,
                        target.SemanticModel,
                        targetLogger,
                        extendedTargets.OfType<INamedTypeSymbol>(),
                        extendedMembers,
                        templateTargets
                    )
                );
            }

            DedupeInstanceMemberConflicts(members, target.SemanticModel, targetLogger);

            if (prepared.TryGetValue(target.Symbol, out var existing))
                existing.Members.AddRange(members);
            else
                prepared.Add(target.Symbol, (target, members));
        }

        foreach (var target in prepared)
        {
            if (generated.ContainsKey(target.Key.ToFullMetadataName()))
                continue;

            var targetLogger = logger.WithSemanticContext(target.Value.Target.SemanticModel);

            var children = prepared
                .Where(x => TypeUtils
                    .GetBaseTypes(x.Key)
                    .Contains(target.Key, SymbolEqualityComparer.Default)
                )
                .ToDictionary<
                    KeyValuePair<INamedTypeSymbol, (GenerationTarget, List<ProxiedMember> Members)>,
                    INamedTypeSymbol,
                    List<ProxiedMember>
                >(
                    x => x.Key,
                    x => x.Value.Members,
                    SymbolEqualityComparer.Default
                );

            var bases = TypeUtils
                .GetBaseTypes(target.Key);

            var parents = prepared
                .Where(x =>
                    bases.Contains(x.Key, SymbolEqualityComparer.Default)
                )
                .ToDictionary<
                    KeyValuePair<INamedTypeSymbol, (GenerationTarget, List<ProxiedMember> Members)>,
                    INamedTypeSymbol,
                    List<ProxiedMember>
                >(
                    x => x.Key,
                    x => x.Value.Members,
                    SymbolEqualityComparer.Default
                );

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Value.Target.ClassDeclarationSyntax);

            var generatedMembers = new HashSet<ISymbol>(SymbolEqualityComparer.Default);

            foreach (var member in target.Value.Members)
            {
                if (!generatedMembers.Add(member.Symbol))
                {
                    targetLogger.Log($"{target.Key}: Skipping {member.Symbol} (already implemented)");
                    continue;
                }

                AddProxiedMember(
                    ref syntax,
                    member,
                    parents,
                    children,
                    target.Value.Target.SemanticModel,
                    targetLogger
                );
            }

            foreach (var entry in target.Value.Target.Properties)
            {
                var property = entry.Key;

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

            generated.Add(target.Key.ToFullMetadataName(), (syntax, target.Value.Target));
        }

        foreach (var item in generated)
        {
            logger.Log($"Generating {item.Key}");

            TypeDeclarationSyntax syntax = item.Value.Syntax;
            
            SyntaxUtils.ApplyNesting(item.Value.Target.Symbol, ref syntax);
            
            context.AddSource(
                $"InterfaceProxy/{item.Key}",
                $$"""
                  {{item.Value.Target.ClassDeclarationSyntax.GetFormattedUsingDirectives()}}

                  namespace {{ModelExtensions.GetDeclaredSymbol(item.Value.Target.SemanticModel, item.Value.Target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
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

    public readonly record struct ProxiedMember
    {
        public readonly ISymbol Symbol;
        public readonly INamedTypeSymbol MemberInterface;
        public readonly INamedTypeSymbol TargetType;
        public readonly IPropertySymbol ProxiedProperty;

        public readonly ISymbol? TargetExplicitImplementation;
        public readonly ISymbol? ProxyExplicitImplementation;

        public readonly bool ProxyHasUniqueImplementation;
        public readonly bool TargetHasUniqueImplementation;

        public readonly ISymbol? TargetImplementation;

        public readonly ITypeSymbol MemberReturnType;

        public readonly bool ShouldImplementOwnMember;
        public readonly bool CanImplementExplicitInterfaceMember;
        public readonly bool CanProxyToSelfImplementation;

        public ProxiedMember(
            ISymbol symbol,
            INamedTypeSymbol memberInterface,
            INamedTypeSymbol targetType,
            IPropertySymbol proxiedProperty,
            ISymbol? targetExplicitImplementation,
            ISymbol? proxyExplicitImplementation,
            bool proxyHasUniqueImplementation,
            bool targetHasUniqueImplementation,
            ISymbol? targetImplementation,
            bool shouldImplementOwnMember,
            bool canImplementExplicitInterfaceMember,
            bool canProxyToSelfImplementation,
            SemanticModel model,
            Logger? logger = null)
        {
            Symbol = symbol;
            MemberInterface = memberInterface;
            TargetType = targetType;
            ProxiedProperty = proxiedProperty;
            TargetExplicitImplementation = targetExplicitImplementation;
            ProxyExplicitImplementation = proxyExplicitImplementation;
            ProxyHasUniqueImplementation = proxyHasUniqueImplementation;
            TargetHasUniqueImplementation = targetHasUniqueImplementation;
            TargetImplementation = targetImplementation;
            ShouldImplementOwnMember = shouldImplementOwnMember;
            CanImplementExplicitInterfaceMember = canImplementExplicitInterfaceMember;
            CanProxyToSelfImplementation = canProxyToSelfImplementation;

            var memberType = MemberUtils.GetMemberType(TargetImplementation) ?? symbol switch
            {
                IPropertySymbol prop => prop.Type,
                IMethodSymbol method => method.ReturnType,
                _ => throw new ArgumentException($"Expected property or method, got {symbol.Kind}")
            };

            MemberReturnType = proxiedProperty.Type is INamedTypeSymbol namedProxyToType
                ? ExtendInterfaceDefaults.GetTargetTypeForImplementation(
                    symbol,
                    memberType,
                    namedProxyToType,
                    model,
                    logger
                )
                : memberType;
        }
    }

    private static IEnumerable<ProxiedMember> GetProxiedMembers(
        INamedTypeSymbol targetType,
        IPropertySymbol proxiedProperty,
        List<INamedTypeSymbol> specifiedTypes,
        SemanticModel semanticModel,
        Logger? logger = null,
        IEnumerable<INamedTypeSymbol>? extendedTargets = null,
        ImmutableHashSet<ISymbol>? extendedMembers = null,
        HashSet<INamedTypeSymbol>? templateTargets = null)
    {
        templateTargets ??= ExtendInterfaceDefaults.GetTemplateExtensionInterfaces(targetType);
        extendedTargets ??= [];
        extendedMembers ??= ImmutableHashSet<ISymbol>.Empty;

        var correctedTargetInterfaces = CorrectTargetList(
            targetType,
            proxiedProperty.Type,
            templateTargets,
            GetTargetTypesToProxy(
                    specifiedTypes,
                    proxiedProperty.Type.AllInterfaces
                )
                .Concat(extendedTargets),
            semanticModel,
            logger
        );

        var willHaveFetchMethods = 
            RestLoadable.WillHaveFetchMethods(targetType) ||
            GatewayLoadable.WillHaveFetchMethods(targetType);

        var implementedMembers = TypeUtils.GetBaseTypesAndThis(proxiedProperty.Type)
            .SelectMany(x => x.GetMembers())
            .Distinct(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var targetInterface in correctedTargetInterfaces)
        {
            foreach
            (
                var member in targetInterface
                    .GetMembers()
                    .Where(IsValidMember)
            )
            {
                var targetExplicitImplementation = targetType.FindImplementationForInterfaceMember(member);
                var proxiedTypeExplicitImplementation =
                    proxiedProperty.Type.FindImplementationForInterfaceMember(member);

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
                    logger?.Warn($"{targetType}: Skipping {member}, unique implementation");
                    continue;
                }

                if (
                    member.IsVirtual &&
                    !targetHasUniqueImplementation &&
                    !proxyHasUniqueImplementation
                )
                {
                    logger?.Warn(
                        $"{targetType}: Skipping {member}, implemented in {targetExplicitImplementation} and {proxiedTypeExplicitImplementation}"
                    );
                    continue;
                }

                if (targetHasUniqueImplementation && member.IsAbstract)
                {
                    logger?.Log($"{targetType}: Skipping {member}, abstract implementation in target");
                    continue;
                }

                if (willHaveFetchMethods &&
                    member is IMethodSymbol memberMethod &&
                    MemberUtils.GetMemberName(memberMethod, x => x.ExplicitInterfaceImplementations) == "FetchAsync")
                {
                    // only proxy if the result of fetch async isn't assignable to this but assignable to the proxies
                    var restEntity = RestLoadable.GetRestEntity(targetType, semanticModel);

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
                                TypeUtils.TypeIsOfProvidedOrDescendant(
                                    fromMethod.ReturnType,
                                    toMethod.ReturnType,
                                    semanticModel
                                ) &&
                                fromMethod.Parameters.Length == toMethod.Parameters.Length &&
                                fromMethod.Parameters.Select((x, i) => (Parameter: x, Index: i))
                                    .All(x => toMethod.Parameters[x.Index].Type.Equals(
                                            x.Parameter.Type,
                                            SymbolEqualityComparer.Default
                                        )
                                    )
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
                                .Equals(proxiedProperty.Type, SymbolEqualityComparer.Default)
                            ?? false
                        )
                        ||
                        extendedMembers.Contains(member)
                    )
                )
                {
                    logger?.Log(
                        $"{targetType}: Choosing unique target implementation {proxiedTypeExplicitImplementation}");
                    targetImplementation = proxiedTypeExplicitImplementation;
                }

                if (willHaveFetchMethods &&
                    targetImplementation is IMethodSymbol targetMethodImplementation &&
                    MemberUtils.GetMemberName(targetMethodImplementation, x => x.ExplicitInterfaceImplementations) ==
                    "FetchAsync")
                {
                    logger?.Log($"{targetType}: Skipping FetchAsync {proxiedTypeExplicitImplementation}");
                    continue;
                }

                var canProxyToSelfImplementation =
                    !proxyHasUniqueImplementation &&
                    targetImplementation is not null &&
                    MemberUtils.CanOverride(
                        targetImplementation,
                        member,
                        semanticModel.Compilation
                    ) &&
                    TypeUtils.GetBaseTypesAndThis(targetType)
                        .Contains(targetImplementation.ContainingType, SymbolEqualityComparer.Default);

                var selfHasTargetImplementation = TypeUtils
                    .GetBaseTypesAndThis(targetType)
                    .Any(typeSymbol =>
                        typeSymbol
                            .GetMembers()
                            .Any(typeSymbolMember => MemberUtils
                                .Conflicts(typeSymbolMember, member)
                            )
                        ||
                        (
                            !typeSymbol.Equals(targetType, SymbolEqualityComparer.Default) &&
                            ComputeProxiedTypeProperties(
                                    typeSymbol
                                )
                                .Any(baseProxiedProperties =>
                                {
                                    var baseProperty = baseProxiedProperties.Key;

                                    if (baseProperty is null)
                                    {
                                        logger?.Warn(
                                            $"Couldn't find base semantic property {baseProxiedProperties.Key} for {typeSymbol} ({targetType})");
                                        return false;
                                    }

                                    return GetTargetTypesToProxy(
                                            baseProxiedProperties.Value,
                                            baseProxiedProperties.Key.Type.AllInterfaces
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

                if (canImplementExplicitSelector && !shouldImplementMember && canProxyToSelfImplementation)
                    continue;

                //canProxyToSelfImplementation |= shouldImplementMember;

                yield return new ProxiedMember(
                    member,
                    targetInterface,
                    targetType,
                    proxiedProperty,
                    targetExplicitImplementation,
                    proxiedTypeExplicitImplementation,
                    proxyHasUniqueImplementation,
                    targetHasUniqueImplementation,
                    targetImplementation,
                    shouldImplementMember,
                    canImplementExplicitSelector,
                    canProxyToSelfImplementation,
                    semanticModel,
                    logger
                );
            }
        }
    }

    private static bool CanOverride(
        ProxiedMember self,
        ProxiedMember target,
        SemanticModel semantic)
    {
        if (!self.ShouldImplementOwnMember || !target.ShouldImplementOwnMember)
            return false;

        if (!MemberUtils.Conflicts(target.Symbol, self.Symbol))
            return false;

        if (self.Symbol is IPropertySymbol && target.Symbol is IPropertySymbol)
        {
            return MemberUtils.CanOverrideProperty(
                self.MemberReturnType, MemberUtils.GetMemberName(self.Symbol),
                target.MemberReturnType, MemberUtils.GetMemberName(target.Symbol),
                semantic.Compilation
            );
        }

        if (self.Symbol is IMethodSymbol methodA && target.Symbol is IMethodSymbol methodB)
        {
            return MemberUtils.CanOverrideMethod(
                self.MemberReturnType, MemberUtils.GetMemberName(self.Symbol), methodA.Parameters,
                target.MemberReturnType, MemberUtils.GetMemberName(target.Symbol), methodB.Parameters,
                semantic.Compilation
            );
        }

        return false;
    }

    private static bool AddProxiedMember(
        ref ClassDeclarationSyntax classDeclarationSyntax,
        ProxiedMember member,
        Dictionary<INamedTypeSymbol, List<ProxiedMember>> parents,
        Dictionary<INamedTypeSymbol, List<ProxiedMember>> children,
        SemanticModel semanticModel,
        Logger logger,
        Action<ISymbol>? onAdd = null
    )
    {
        logger.Log(
            $"{member.TargetType}: Proxying {member.ProxiedProperty} {member.ToString()}"
        );

        var castedSyntax = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AsExpression,
                SyntaxFactory.IdentifierName(member.ProxiedProperty.Name),
                SyntaxFactory.ParseTypeName(member.MemberInterface.ToDisplayString())
            )
        );

        var modifiers = new HashSet<SyntaxToken>(CreateModifiers(member.Symbol));

        if (member.TargetType.IsSealed)
            modifiers.RemoveWhere(x => x.Kind() is SyntaxKind.VirtualKeyword or SyntaxKind.AbstractKeyword);

        if (
            parents.Any(x => x.Value
                .Any(x =>
                    CanOverride(member, x, semanticModel)
                )
            )
        )
        {
            logger.Log($"{member.TargetType}: override {member.Symbol}");
            modifiers.Add(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        }
        else if (
            children.Any(x => x.Value
                .Any(x =>
                    CanOverride(x, member, semanticModel)
                )
            )
        )
        {
            logger.Log($"{member.TargetType}: virtual {member.Symbol}");
            modifiers.Add(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));
        }

        switch (member.Symbol)
        {
            case IPropertySymbol property:
                if (member is {TargetImplementation: IPropertySymbol targetProperty, ShouldImplementOwnMember: true})
                {
                    var propertySyntax = SyntaxFactory.PropertyDeclaration(
                        [],
                        MemberUtils.CreateAccessors(targetProperty.DeclaredAccessibility)
                            .AddRange(modifiers),
                        SyntaxFactory.IdentifierName(member.MemberReturnType.ToDisplayString()),
                        null,
                        SyntaxFactory.Identifier(property.Name),
                        null,
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                SyntaxFactory.IdentifierName(member.ProxiedProperty.Name),
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
                                        SyntaxFactory.Comment($"// {property} through {member.TargetImplementation}")
                                    )
                            );
                    }
                }

                if (member.CanImplementExplicitInterfaceMember)
                {
                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.PropertyDeclaration(
                                [],
                                [],
                                SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                    SyntaxFactory.IdentifierName(member.MemberInterface.ToDisplayString())
                                ),
                                SyntaxFactory.Identifier(property.Name),
                                null,
                                SyntaxFactory.ArrowExpressionClause(
                                    SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        member.CanProxyToSelfImplementation
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
                    (member.TargetImplementation is not IPropertySymbol || !member.ShouldImplementOwnMember) &&
                    !member.CanImplementExplicitInterfaceMember
                ) break;

                onAdd?.Invoke(property);
                return true;
            case IMethodSymbol method:
                if (member is {TargetImplementation: IMethodSymbol targetMethod, ShouldImplementOwnMember: true})
                {
                    var methodSyntax = SyntaxFactory.MethodDeclaration(
                        [],
                        MemberUtils.CreateAccessors(targetMethod.DeclaredAccessibility)
                            .AddRange(modifiers),
                        SyntaxFactory.IdentifierName(member.MemberReturnType.ToDisplayString()),
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
                                    SyntaxFactory.IdentifierName(member.ProxiedProperty.Name),
                                    method.IsGenericMethod
                                        ? SyntaxFactory.GenericName(
                                            SyntaxFactory.Identifier(method.Name),
                                            SyntaxFactory.TypeArgumentList(
                                                SyntaxFactory.SeparatedList<TypeSyntax>(
                                                    method.TypeParameters.Select(x =>
                                                        SyntaxFactory.IdentifierName(x.Name)
                                                    )
                                                )
                                            )
                                        )
                                        : SyntaxFactory.IdentifierName(method.Name)
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
                                    SyntaxFactory.Comment($"// {method} through {member.TargetImplementation}")
                                )
                        );
                    }
                }

                if (member.CanImplementExplicitInterfaceMember)
                {
                    ExpressionSyntax body = SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            member.CanProxyToSelfImplementation
                                ? SyntaxFactory.ThisExpression()
                                : castedSyntax,
                            method.IsGenericMethod
                                ? SyntaxFactory.GenericName(
                                    SyntaxFactory.Identifier(method.Name),
                                    SyntaxFactory.TypeArgumentList(
                                        SyntaxFactory.SeparatedList<TypeSyntax>(
                                            method.TypeParameters.Select(x =>
                                                SyntaxFactory.IdentifierName(x.Name)
                                            )
                                        )
                                    )
                                )
                                : SyntaxFactory.IdentifierName(method.Name)
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
                    );

                    var needsToAwait =
                        !member.MemberReturnType.Equals(method.ReturnType, SymbolEqualityComparer.Default)
                        &&
                        member.MemberReturnType is {Name: "Task" or "ValueTask"};

                    if (needsToAwait)
                        body = SyntaxFactory.AwaitExpression(body);

                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.MethodDeclaration(
                                [],
                                needsToAwait
                                    ? SyntaxFactory.TokenList(
                                        SyntaxFactory.Token(SyntaxKind.AsyncKeyword)
                                    )
                                    : [],
                                SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                                SyntaxFactory.ExplicitInterfaceSpecifier(
                                    SyntaxFactory.IdentifierName(member.MemberInterface.ToDisplayString())
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
                                    body
                                ),
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                            )
                            .WithLeadingTrivia(
                                SyntaxFactory.Comment($"// {method}")
                            )
                    );
                }

                if (
                    (member.TargetImplementation is not IMethodSymbol || !member.ShouldImplementOwnMember) &&
                    !member.CanImplementExplicitInterfaceMember
                ) break;

                onAdd?.Invoke(method);
                return true;
            default:
                return false;
        }

        return false;
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
}
