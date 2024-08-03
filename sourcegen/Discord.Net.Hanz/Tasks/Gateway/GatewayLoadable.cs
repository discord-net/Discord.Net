using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Discord.Net.Hanz.Tasks.Gateway;

public sealed class GatewayLoadable : IGenerationCombineTask<GatewayLoadable.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol gatewayActorInterface,
        INamedTypeSymbol classSymbol,
        INamedTypeSymbol gatewayEntitySymbol,
        INamedTypeSymbol restEntitySymbol,
        INamedTypeSymbol coreActor,
        ITypeSymbol idType,
        ITypeSymbol coreEntity,
        ITypeSymbol modelType,
        ITypeSymbol identityType
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol GatewayActorInterface { get; } = gatewayActorInterface;
        public INamedTypeSymbol ClassSymbol { get; } = classSymbol;
        public INamedTypeSymbol GatewayEntitySymbol { get; } = gatewayEntitySymbol;
        public INamedTypeSymbol RestEntitySymbol { get; } = restEntitySymbol;

        public INamedTypeSymbol CoreActor { get; } = coreActor;
        public ITypeSymbol IdType { get; } = idType;
        public ITypeSymbol CoreEntity { get; } = coreEntity;
        public ITypeSymbol ModelType { get; } = modelType;
        public ITypeSymbol IdentityType { get; } = identityType;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassSymbol.Equals(other.ClassSymbol, SymbolEqualityComparer.Default) &&
                   GatewayActorInterface.Equals(other.GatewayActorInterface, SymbolEqualityComparer.Default) &&
                   CoreActor.Equals(other.CoreActor, SymbolEqualityComparer.Default) &&
                   IdType.Equals(other.IdType, SymbolEqualityComparer.Default) &&
                   CoreEntity.Equals(other.CoreEntity, SymbolEqualityComparer.Default) &&
                   ModelType.Equals(other.ModelType, SymbolEqualityComparer.Default) &&
                   GatewayEntitySymbol.Equals(other.GatewayEntitySymbol, SymbolEqualityComparer.Default) &&
                   RestEntitySymbol.Equals(other.RestEntitySymbol, SymbolEqualityComparer.Default) &&
                   IdentityType.Equals(other.IdentityType, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is GenerationTarget other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SymbolEqualityComparer.Default.GetHashCode(ClassSymbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(gatewayActorInterface);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(CoreActor);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(CoreEntity);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ModelType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(GatewayEntitySymbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(RestEntitySymbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdentityType);
                return hashCode;
            }
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right) => Equals(left, right);

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax cls &&
           cls.Identifier.ValueText.EndsWith("Actor") &&
           cls.Identifier.ValueText.StartsWith("Gateway");

    public static bool WillHaveFetchMethods(ITypeSymbol type)
    {
        if (!type.Name.StartsWith("Gateway") || !type.Name.EndsWith("Actor"))
            return false;

        var hierarchy = Hierarchy.GetHierarchy(type);

        if (hierarchy.All(x => !LoadableTrait.IsLoadable(x.Type)))
            return false;

        return true;
    }

    public static INamedTypeSymbol? GetGatewayEntity(INamedTypeSymbol restActor, SemanticModel model)
    {
        return model.Compilation
            .GetSymbolsWithName(
                restActor.Name.Replace("Actor", string.Empty),
                SymbolFilter.Type
            )
            .FirstOrDefault() as INamedTypeSymbol;
    }

    public static bool TryGetBrokerTypes(
        ITypeSymbol type,
        out ITypeSymbol idType,
        out ITypeSymbol entityType,
        out ITypeSymbol modelType)
    {
        idType = null!;
        entityType = null!;
        modelType = null!;

        if (!WillHaveFetchMethods(type)) return false;

        var hierarchy = Hierarchy.GetHierarchy(type);

        if (hierarchy.All(x => !LoadableTrait.IsLoadable(x.Type)))
            return false;

        var gatewayCacheableActor = hierarchy.FirstOrDefault(x =>
            x.Type.ToDisplayString().StartsWith("Discord.Gateway.IGatewayCachedActor<")
        ).Type;

        if (gatewayCacheableActor is null || gatewayCacheableActor.TypeArguments.Length != 4)
            return false;

        idType = gatewayCacheableActor.TypeArguments[0];
        entityType = gatewayCacheableActor.TypeArguments[1];
        modelType = gatewayCacheableActor.TypeArguments[3];

        return true;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclarationSyntax)
            return null;

        logger = logger.WithSemanticContext(context.SemanticModel);

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, classDeclarationSyntax) is not INamedTypeSymbol
            classSymbol)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: not an INamedTypeSymbol");
            return null;
        }

        var hierarchy = Hierarchy.GetHierarchy(classSymbol);

        // if (hierarchy.All(x => !LoadableTrait.IsLoadable(x.Type)))
        // {
        //     logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: not loadable");
        //     return null;
        // }

        var gatewayCacheableActor = hierarchy.FirstOrDefault(x =>
            x.Type.ToDisplayString().StartsWith("Discord.Gateway.IGatewayCachedActor<")
        ).Type;

        if (gatewayCacheableActor is null)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: no gateway cacheable actor");
            return null;
        }

        var coreActor = classSymbol.Interfaces.FirstOrDefault(IsActor);

        if (coreActor is null)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: no first-specified core actor");
            return null;
        }

        var actorType = GetActorInterface(coreActor);

        if (actorType is null)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: actor type is null");
            return null;
        }

        var idType = actorType.TypeArguments[0];
        var coreEntityType = actorType.TypeArguments[1];
        var entityType = gatewayCacheableActor.TypeArguments[1];
        var modelType = gatewayCacheableActor.TypeArguments[3];

        var restEntity =
            context.SemanticModel.Compilation.GetTypeByMetadataName(
                $"Discord.Rest.Rest{coreEntityType.Name.Remove(0, 1)}");

        if (restEntity is null)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: cannot resolve rest entity type");
            return null;
        }

        if (entityType is not INamedTypeSymbol entityNamedType)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: cannot resolve gateway entity type");
            return null;
        }

        var identityProperty = TypeUtils.GetBaseTypesAndThis(classSymbol)
            .SelectMany(x => x.GetMembers().OfType<IPropertySymbol>())
            .FirstOrDefault(x => x.Name == "Identity");

        if (identityProperty is null)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: cannot resolve identity type");
            return null;
        }

        return new GenerationTarget(
            context.SemanticModel,
            classDeclarationSyntax,
            gatewayCacheableActor,
            classSymbol,
            entityNamedType,
            restEntity,
            coreActor,
            idType,
            coreEntityType,
            modelType,
            identityProperty.Type
        );
    }

    private static bool IsActor(INamedTypeSymbol type)
        => GetActorInterface(type) is not null;

    private static INamedTypeSymbol? GetActorInterface(INamedTypeSymbol type)
        => type.Interfaces.FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.IActor"));

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        foreach (var target in Hierarchy
                     .OrderByHierarchy(
                         targets,
                         x => x.ClassSymbol,
                         out var map,
                         out var bases)
                )
        {
            if (target is null)
                continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var targetBases = TypeUtils.GetBaseTypes(target.ClassSymbol).ToArray();

            var root = targetBases.LastOrDefault(bases.Contains);
            var rootTarget = root is not null
                ? targets.First(x => x is not null && x.ClassSymbol.Equals(root, SymbolEqualityComparer.Default))
                : null;
            var hasChild = bases.Contains(target.ClassSymbol);

            var children = map
                .Where(x => TypeUtils.GetBaseTypes(x.Key)
                    .Contains(target.ClassSymbol, SymbolEqualityComparer.Default)
                )
                .Select(x => x.Value)
                .ToArray();

            var parents = map
                .Where(x =>
                    targetBases.Contains(x.Key, SymbolEqualityComparer.Default)
                )
                .Select(x => x.Value)
                .ToArray();

            CreateLookupTables(
                ref syntax,
                target,
                children,
                parents,
                targetLogger
            );

            if (!CreateLoadableSources(
                    ref syntax,
                    target,
                    children,
                    targetLogger))
                continue;

            if (!CreateModelHierarchyMap(
                    ref syntax,
                    target,
                    children,
                    parents,
                    targetLogger)
               ) continue;

            if (!CreateGatewayLoadable(
                    ref syntax,
                    target,
                    root is not null,
                    hasChild,
                    rootTarget?.GatewayEntitySymbol,
                    targetLogger)
               ) continue;

            if (
                LoadableTrait.IsLoadable(target.CoreActor) &&
                !CreateLoadableTraitMethods(
                    ref syntax,
                    target,
                    root is not null,
                    hasChild,
                    rootTarget?.RestEntitySymbol,
                    targetLogger
                )
            ) continue;

            CreateOverloadsAsync(ref syntax, target, targetLogger);

            context.AddSource(
                $"GatewayLoadable/{target.ClassSymbol.Name}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives("System.Collections.Immutable")}}

                  namespace {{target.ClassSymbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static string GetStoreForTarget(GenerationTarget target, bool hasStoreRoots)
    {
        return hasStoreRoots
            ? $"await parentStore.GetSubStoreAsync<{target.IdType}, {target.ModelType}>(parentId)"
            : $"await client.CacheProvider.GetStoreAsync<{target.IdType}, {target.ModelType}>";
    }

    private static ITypeSymbol GetCoreEntityType(ITypeSymbol entity)
    {
        if (entity.TypeKind is TypeKind.Interface)
            return entity;

        return Hierarchy.GetHierarchy(entity)
            .First(x =>
                x.Type.TypeKind is TypeKind.Interface &&
                x.Type.AllInterfaces.Any(x =>
                    x.ToDisplayString().StartsWith("Discord.IEntity")
                )
            ).Type;
    }

    private static string ToStoreInfo(string store, bool hasStoreRoots, ITypeSymbol actor)
    {
        return hasStoreRoots
            ? $"({store}).ToInfo(client, parentStore, parentId, Template.Of<{actor}>());"
            : $"({store}).ToInfo(client, Template.Of<{actor}>());";
    }

    private static void CreateLookupTables(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        IEnumerable<GenerationTarget> children,
        IEnumerable<GenerationTarget> parents,
        Logger logger)
    {
        var hasStoreRoot = TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap);
        var parentTargets = parents as GenerationTarget[] ?? parents.ToArray();
        var parentsWithSameRoot = parentTargets
            .Where(x =>
                !hasStoreRoot
                ||
                (
                    TryGetStoreRootMap(x.ClassSymbol, out var parentStoreRootMap) &&
                    parentStoreRootMap.Count == storeRootMap.Count
                )
            )
            .Reverse()
            .ToArray();

        CorrectTargetList(ref children, hasStoreRoot, storeRootMap);

        var stringBuilder = new StringBuilder();

        var getStores = new StringBuilder();

        for (var i = -1; i < parentsWithSameRoot.Length; i++)
        {
            var parent = i == -1 ? target : parentsWithSameRoot[i];

            if (i < parentsWithSameRoot.Length - 1)
                getStores.AppendLine(
                    $"if (client.StateController.CanUseStoreType<{parent.GatewayEntitySymbol}>())"
                );

            var storeForParent = $"{GetStoreForTarget(parent, hasStoreRoot)}";

            if (!parent.ModelType.Equals(target.ModelType, SymbolEqualityComparer.Default))
                storeForParent = $"({storeForParent}).CastDown(Template.Of<{target.ModelType}>())";

            getStores.AppendLine(
                $"return {storeForParent};"
            );
        }

        if (hasStoreRoot)
        {
            stringBuilder
                .AppendLine(
                    $"var parentId = path.Require<{GetCoreEntityType(storeRootMap.Last().EntityType)}>();"
                )
                .AppendLine(
                    $"var parentStore = await {storeRootMap.Last().Property.Type}.GetStoreAsync(client, path, token);"
                );

            syntax = syntax
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          private static async ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> GetSubStoreAsync(DiscordGatewayClient client, IEntityModelStore<{{storeRootMap.Last().IdType}}, {{storeRootMap.Last().ModelType}}> parentStore, {{storeRootMap.Last().IdType}} parentId, CancellationToken token = default)
                          {
                              {{getStores}}
                          }
                          """
                    )!
                );
        }

        var getStoreStatic = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static async ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> GetStoreAsync(DiscordGatewayClient client, IPathable path, CancellationToken token = default)
              {
                  {{stringBuilder}}
                  {{(
                      hasStoreRoot
                          ? "return await GetSubStoreAsync(client, parentStore, parentId, token);"
                          : getStores
                  )}}
              }
              """
        )!;

        if (parentTargets.Length > 0)
            getStoreStatic = getStoreStatic.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        var getStoreInfoStatic = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static async ValueTask<IStoreInfo<{{target.IdType}}, {{target.ModelType}}>> GetStoreInfoAsync(DiscordGatewayClient client, IPathable path, CancellationToken token = default)
              {
                  {{stringBuilder}}

                  return {{
                      ToStoreInfo(
                          hasStoreRoot
                              ? "await GetSubStoreAsync(client, parentStore, parentId, token)"
                              : "await GetStoreAsync(client, path, token)",
                          hasStoreRoot,
                          target.ClassSymbol
                      )
                  }}
              }
              """
        )!;

        var getStoreInfo = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal async ValueTask<IStoreInfo<{{target.IdType}}, {{target.ModelType}}>> GetStoreInfoAsync(CancellationToken token = default)
                  => _storeInfo ??= await {{target.ClassSymbol}}.GetStoreInfoAsync(Client, this, token);
              """
        )!;

        if (parentTargets.Length > 0)
        {
            getStoreInfoStatic = getStoreInfoStatic.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            getStoreInfo = getStoreInfo.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
        }

        stringBuilder.Clear();

        var childrenTargets = children as GenerationTarget[] ?? children.ToArray();

        var storeDelegateType = $"StoreProviderInfo<{target.IdType}, {target.ModelType}>";
        var storeHierarchyType = $"IReadOnlyDictionary<Type, {storeDelegateType}>";


        foreach (var entry in childrenTargets)
        {
            stringBuilder.AppendLine($"#region {entry.ClassSymbol}");

            var bases = TypeUtils.GetBaseTypesAndThis(entry.ClassSymbol);

            var entryTargets = childrenTargets
                .Where(x =>
                    bases.Contains(x.ClassSymbol, SymbolEqualityComparer.Default)
                )
                .OrderByDescending(x => TypeUtils.GetBaseTypes(x.ClassSymbol).Count())
                .ToArray();

            for (var i = 0; i < entryTargets.Length; i++)
            {
                var element = entryTargets[i];

                if (i > 0)
                    stringBuilder.Append("else ");

                stringBuilder
                    .AppendLine(
                        $"if (client.StateController.CanUseStoreType<{element.GatewayEntitySymbol}>())"
                    );

                stringBuilder.AppendLine(
                    $"map[typeof({entry.ModelType})] = new(typeof({element.ModelType}), async (client, token) => ({
                        CreateStoreInitialization(
                            target,
                            element.IdType,
                            element.ModelType,
                            hasStoreRoot,
                            storeRootMap,
                            "parentStore",
                            "parentId",
                            true,
                            false,
                            true
                        )
                    }).CastUp(Template.Of<{target.ModelType}>()));"
                );
            }

            stringBuilder
                .AppendLine("else")
                .AppendLine(
                    $"map[typeof({entry.ModelType})] = new(typeof({target.ModelType}), async (client, token) => ({
                        CreateStoreInitialization(
                            target,
                            target.IdType,
                            target.ModelType,
                            hasStoreRoot,
                            storeRootMap,
                            "parentStore",
                            "parentId",
                            true,
                            false,
                            true
                        )
                    }));"
                );

            stringBuilder.AppendLine("#endregion");
        }

        var rootStoreParam = hasStoreRoot
            ? $", IEntityModelStore<{storeRootMap.Last().IdType}, {storeRootMap.Last().ModelType}> parentStore, {storeRootMap.Last().IdType} parentId"
            : string.Empty;

        var getStoreHierarchyMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static {{storeHierarchyType}} GetStoreHierarchy(DiscordGatewayClient client{{rootStoreParam}})
              {
                  var map = new Dictionary<Type, {{storeDelegateType}}>();

                  {{stringBuilder}}

                  return map;
              }
              """
        )!;

        if (parentsWithSameRoot.Length > 0)
            getStoreHierarchyMethod = getStoreHierarchyMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        var brokerDelegateType =
            $"BrokerProviderDelegate<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>";

        var brokerHierarchyType =
            $"IReadOnlyCollection<{brokerDelegateType}>";

        var getBrokerHierarchyMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static {{brokerHierarchyType}} GetBrokerHierarchy()
              {
                  if (_brokerHierarchy is not null)
                      return _brokerHierarchy;

                  return _brokerHierarchy = new List<{{brokerDelegateType}}>()
                  {
                      {{
                          string.Join(
                              ",\n",
                              childrenTargets
                                  .Prepend(target)
                                  .Select(element =>
                                      $"async (client, token) => (await client.StateController.GetBrokerAsync<{element.IdType}, {element.GatewayEntitySymbol}, {element.ClassSymbol}, {element.ModelType}>(token) as IManageableEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>)!"
                                  )
                          )
                      }}
                  }.ToImmutableList();
              }
              """
        )!;

        if (parentTargets.Length > 0)
            getBrokerHierarchyMethod =
                getBrokerHierarchyMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        if (hasStoreRoot)
        {
            var parent = storeRootMap.Last();
            var ifaceName =
                $"ISubStoreProvider<{parent.IdType}, {parent.ModelType}, {target.IdType}, {target.ModelType}>";
            syntax = syntax
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"static {storeHierarchyType} {ifaceName}.GetStoreHierarchy(DiscordGatewayClient client, IEntityModelStore<{parent.IdType}, {parent.ModelType}> parentStore, {parent.IdType} parentId) => GetStoreHierarchy(client, parentStore, parentId);"
                    )!
                )
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(ifaceName)
                    )
                );
        }
        else
        {
            var ifaceName = $"IRootStoreProvider<{target.IdType}, {target.ModelType}>";
            syntax = syntax
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"static {storeHierarchyType} {ifaceName}.GetStoreHierarchy(DiscordGatewayClient client) => GetStoreHierarchy(client);"
                    )!
                )
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(ifaceName)
                    )
                );
        }

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        $"IStoreInfoProvider<{target.IdType}, {target.ModelType}>"
                    )
                )
            )
            .AddMembers(
                getStoreHierarchyMethod,
                getStoreStatic,
                getStoreInfoStatic,
                SyntaxFactory.ParseMemberDeclaration(
                    $"private IStoreInfo<{target.IdType}, {target.ModelType}>? _storeInfo;"
                )!,
                getStoreInfo,
                SyntaxFactory.ParseMemberDeclaration(
                    $"static ValueTask<IEntityModelStore<{target.IdType}, {target.ModelType}>> IStoreProvider<{target.IdType}, {target.ModelType}>.GetStoreAsync(DiscordGatewayClient client, IPathable path, CancellationToken token) => GetStoreAsync(client, path, token);"
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $"static ValueTask<IStoreInfo<{target.IdType}, {target.ModelType}>> IStoreInfoProvider<{target.IdType}, {target.ModelType}>.GetStoreInfoAsync(DiscordGatewayClient client, IPathable path, CancellationToken token) => GetStoreInfoAsync(client, path, token);"
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $"ValueTask<IStoreInfo<{target.IdType}, {target.ModelType}>> IStoreInfoProvider<{target.IdType}, {target.ModelType}>.GetStoreInfoAsync(CancellationToken token) => GetStoreInfoAsync(token);"
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $"private static {brokerHierarchyType}? _brokerHierarchy;"
                )!,
                getBrokerHierarchyMethod,
                SyntaxFactory.ParseMemberDeclaration(
                    $"static {brokerHierarchyType} IBrokerProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>.GetBrokerHierarchy() => GetBrokerHierarchy();"
                )!
            );
    }

    public static IEnumerable<IMethodSymbol> GetLoadableMethods(ITypeSymbol type)
    {
        return Hierarchy.GetHierarchy(type)
            .Select(x => x.Type)
            .Prepend(type)
            .SelectMany(x => x.GetMembers().OfType<IMethodSymbol>())
            .Where(x =>
                MemberUtils.GetMemberName(
                    x,
                    x => x.ExplicitInterfaceImplementations
                ) is "FetchAsync" or "GetAsync" or "GetOrFetchAsync"
            )
            .OrderByDescending(x => x.Parameters.Length);
    }

    private static string ChainSubStore(
        ITypeSymbol parentIdType,
        ITypeSymbol parentModelType,
        ITypeSymbol idType,
        ITypeSymbol modelType,
        string rootStoreId,
        bool useStoreMethod = false
    )
    {
        return useStoreMethod
            ? $".GetSubStoreAsync<{idType}, {modelType}>({rootStoreId}, token)"
            : $".Chain<{parentIdType}, {parentModelType}, {idType}, {modelType}>({rootStoreId})";
    }

    private static string CreateStoreInitialization(
        GenerationTarget target,
        ITypeSymbol idType,
        ITypeSymbol modelType,
        bool hasStoreRootMap,
        List<StoreRootPath> storeRootMap,
        string? rootStore = null,
        string? rootStoreId = null,
        bool useStoreMethod = false,
        bool doCast = true,
        bool isStatic = false)
    {
        string store;
        var client = isStatic ? "client" : "Client";

        if (hasStoreRootMap)
        {
            var formatted = FormatRootChain(
                storeRootMap,
                rootStore,
                rootStoreId,
                useStoreMethod
            );

            var lastRoot = storeRootMap.Last();

            store =
                $"await {formatted}{
                    ChainSubStore(
                        lastRoot.IdType,
                        lastRoot.ModelType,
                        idType,
                        modelType,
                        rootStoreId ?? $"{lastRoot.Property.Name}.Id",
                        useStoreMethod
                    )
                }";

            if (doCast && !target.ModelType.Equals(modelType, SymbolEqualityComparer.Default))
                store = $"({store}).CastDown(Template.Of<{target.ModelType}>())";

            return store;
        }

        store = $"await {client}.CacheProvider.GetStoreAsync<{idType}, {modelType}>(token)";

        if (doCast && !target.ModelType.Equals(modelType, SymbolEqualityComparer.Default))
            store = $"({store}).CastDown(Template.Of<{target.ModelType}>())";

        return store;
    }

    private static void CorrectTargetList(
        ref IEnumerable<GenerationTarget> targets,
        bool hasStoreRootMap,
        List<StoreRootPath> storeRootMap)
    {
        targets = targets
            .Where(x =>
            {
                if (x.ModelType.ToDisplayString() == "Discord.Models.ISelfUserModel")
                    return false;

                var hasStoreMap = TryGetStoreRootMap(x.ClassSymbol, out var map);

                return
                    (!hasStoreMap && !hasStoreRootMap)
                    ||
                    map.Count == storeRootMap.Count;
            })
            .OrderByDescending(x => TypeUtils.GetBaseTypes(x.ClassSymbol).Count());
    }

    private static bool CreateModelHierarchyMap(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        IEnumerable<GenerationTarget> children,
        IEnumerable<GenerationTarget> parents,
        Logger logger)
    {
        var hasStoreRootMap = TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap);

        var parentTargets = parents as GenerationTarget[] ?? parents.ToArray();

        CorrectTargetList(ref children, hasStoreRootMap, storeRootMap);

        var hierarchy = children.Append(target).ToArray();

        var root = hierarchy.Last();

        hierarchy = hierarchy.Take(hierarchy.Length - 1).ToArray();

        var stringBuilder = new StringBuilder();

        foreach (var element in hierarchy)
        {
            stringBuilder
                .AppendLine($"if (modelType == typeof({element.ModelType}))")
                .AppendLine(
                    $"return (await client.StateController.GetBrokerAsync<{target.IdType}, {element.GatewayEntitySymbol}, {element.ClassSymbol}, {element.ModelType}>(token) as IManageableEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>)!;");
        }

        stringBuilder
            .AppendLine(
                $"return (await client.StateController.GetBrokerAsync<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>(token) as IManageableEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>)!;"
            );


        var getBrokerFromModel = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static async ValueTask<IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>> GetBrokerForModelAsync(DiscordGatewayClient client, Type modelType, CancellationToken token)
              {
                  {{stringBuilder}}
              }
              """
        );

        if (getBrokerFromModel is null)
        {
            logger.Warn($"{target.ClassSymbol}: Failed to create GetBrokerFromModel method");
            return false;
        }

        if (parentTargets.Any())
            getBrokerFromModel = getBrokerFromModel.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        stringBuilder.Clear();

        stringBuilder
            .AppendLine("switch(model)")
            .AppendLine("{");

        foreach (var element in hierarchy)
        {
            stringBuilder
                .AppendLine($"case {element.ModelType}:")
                .AppendLine(
                    $"return (await GetStoreForModelAsync<{element.ModelType}>(token))?.CastUp(Template.Of<{root.ModelType}>());");
        }

        stringBuilder
            .AppendLine("default:")
            .AppendLine($"return await GetOrCreateStoreAsync(token);")
            .AppendLine("}");

        var fetchMap = hasStoreRootMap
            ? $"GetStoreHierarchy(Client, await {storeRootMap.Last().Format()}, {storeRootMap.Last().Property.Name}.Id)"
            : "GetStoreHierarchy(Client)";

        var dangerousGetStoreForModel = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private async ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>?> GetStoreForModelAsync(Type type, CancellationToken token = default)
              {
                  var map = _storeHierarchyMap ??= {{target.ClassSymbol}}.{{fetchMap}};

                  if(map.TryGetValue(type, out var storeInfo))
                      return await storeInfo.StoreDelegate(Client, token);

                  return null;
              }
              """
        );

        if (dangerousGetStoreForModel is null)
        {
            logger.Warn($"{target.ClassSymbol}: Failed to create GetStoreForModel method");
            return false;
        }

        var getStoreForModel = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private async ValueTask<IEntityModelStore<{{target.IdType}}, TSubModel>?> GetStoreForModelAsync<TSubModel>(CancellationToken token = default)
                  where TSubModel : class, {{target.ModelType}}, IEntityModel<{{target.IdType}}>
              {
                  var map = _storeHierarchyMap ??= {{target.ClassSymbol}}.{{fetchMap}};

                  if(map.TryGetValue(typeof(TSubModel), out var storeInfo))
                      return (await storeInfo.StoreDelegate(Client, token)).CastDown(Template.Of<TSubModel>());

                  return null;
              }
              """
        );

        if (getStoreForModel is null)
        {
            logger.Warn($"{target.ClassSymbol}: Failed to create GetStoreForModelAsync method");
            return false;
        }

        var interfaceStoreForModelOverload = SyntaxFactory.ParseMemberDeclaration(
            $"ValueTask<IEntityModelStore<{target.IdType}, TSubModel>?> IStoreProvider<{target.IdType}, {target.ModelType}>.GetStoreForModelAsync<TSubModel>(CancellationToken token) => GetStoreForModelAsync<TSubModel>(token);"
        );

        if (interfaceStoreForModelOverload is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create store model overload");
            return false;
        }

        var interfaceDangerousStoreForModelOverload = SyntaxFactory.ParseMemberDeclaration(
            $"ValueTask<IEntityModelStore<{target.IdType}, {target.ModelType}>?> IStoreProvider<{target.IdType}, {target.ModelType}>.GetStoreForModelAsync(Type type, CancellationToken token) => GetStoreForModelAsync(type, token);"
        );

        if (interfaceDangerousStoreForModelOverload is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create dangerous store model overload");
            return false;
        }

        var interfaceGetBrokerFromModelOverload = SyntaxFactory.ParseMemberDeclaration(
            $"static ValueTask<IManageableEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>> IBrokerProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>.GetBrokerForModelAsync(DiscordGatewayClient client, Type modelType, CancellationToken token) => GetBrokerForModelAsync(client, modelType, token);"
        );

        if (interfaceGetBrokerFromModelOverload is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create broker from model overload");
            return false;
        }

        var getBrokerStatic = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              internal static ValueTask<IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>> GetBrokerAsync(DiscordGatewayClient client, CancellationToken token = default)
                  => client.StateController.GetBrokerAsync<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>(token);
              """
        )!;

        var interfaceGetPartialBrokerStatic = SyntaxFactory.ParseMemberDeclaration(
            $$"""
            static async ValueTask<IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>.GetBrokerAsync(DiscordGatewayClient client, CancellationToken token)
                => await GetBrokerAsync(client, token);
            """
        )!;

        var interfaceGetBrokerStatic = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              static ValueTask<IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.GetBrokerAsync(DiscordGatewayClient client, CancellationToken token)
                  => GetBrokerAsync(client, token);
              """
        )!;

        if (parentTargets.Length > 0)
        {
            getBrokerStatic = getBrokerStatic.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
        }


        syntax = syntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $"private IReadOnlyDictionary<Type, StoreProviderInfo<{target.IdType}, {target.ModelType}>>? _storeHierarchyMap;"
            )!,
            getStoreForModel,
            dangerousGetStoreForModel,
            getBrokerFromModel,
            getBrokerStatic,
            interfaceGetPartialBrokerStatic,
            interfaceGetBrokerStatic,
            interfaceGetBrokerFromModelOverload,
            interfaceDangerousStoreForModelOverload,
            interfaceStoreForModelOverload
        );

        return true;
    }

    private static void CreateOverloadsAsync(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        Logger logger)
    {
        var addedOverloads = new HashSet<string>();

        foreach (var method in GetLoadableMethods(target.ClassSymbol))
        {
            var methodPureName = MemberUtils.GetMemberName(method);

            if (!addedOverloads.Add($"{method.ContainingType.ToDisplayString()}#{methodPureName}"))
                continue;

            var parameterList = MemberUtils.CreateParameterList(method, false).NormalizeWhitespace();

            var invocationArguments = string.Join(
                ", ",
                parameterList.Parameters.Select(x =>
                {
                    if (x.Type!.ToString().Contains("RequestOptions"))
                        return $"{x.Identifier}: GatewayRequestOptions.FromRestOptions({x.Identifier})";
                    return $"{x.Identifier}: {x.Identifier}";
                })
            );

            var methodSyntax = SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  async {{method.ReturnType.ToDisplayString()}} {{method.ContainingType}}.{{methodPureName}}{{parameterList}}
                      => await {{methodPureName}}({{invocationArguments}});
                  """
            );

            if (methodSyntax is null)
            {
                logger.Warn($"{target.ClassSymbol}: failed to create member overload for {method}");
                continue;
            }

            syntax = syntax.AddMembers(methodSyntax);
        }
    }

    private static bool TryGetStoreRoot(ITypeSymbol type, out IPropertySymbol root)
    {
        root = type.GetMembers()
            .OfType<IPropertySymbol>()
            .FirstOrDefault(x => x
                .GetAttributes()
                .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.Gateway.StoreRootAttribute")
            )!;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        return root is not null;
    }

    public readonly struct StoreRootPath(
        ITypeSymbol actor,
        IPropertySymbol property,
        ITypeSymbol idType,
        ITypeSymbol entityType,
        ITypeSymbol modelType)
    {
        public readonly ITypeSymbol Actor = actor;
        public readonly IPropertySymbol Property = property;
        public readonly ITypeSymbol IdType = idType;
        public readonly ITypeSymbol EntityType = entityType;
        public readonly ITypeSymbol ModelType = modelType;

        public string Format()
        {
            return $"(this.{Property.Name} as IStoreProvider<{IdType}, {ModelType}>).GetStoreAsync(token)";
        }
    }

    public static bool TryGetStoreRootMap(ITypeSymbol type, out List<StoreRootPath> map)
    {
        map = new();

        var checkingType = type;

        while (checkingType is not null && !checkingType.IsAbstract)
        {
            if (TryGetStoreRoot(checkingType, out var root) &&
                TryGetBrokerTypes(root.Type, out var id, out var entity, out var model))
                map.Add(new(checkingType, root, id, entity, model));

            checkingType = checkingType.BaseType;
        }

        map.Reverse();

        return map.Count > 0;
    }

    private static string FormatRootChain(
        List<StoreRootPath> map,
        string? rootStore = null,
        string? rootStoreId = null,
        bool useStoreMethod = false)
    {
        if (map.Count < 1)
            throw new InvalidOperationException("store root map must contain at least 1 element");

        var sb = new StringBuilder();

        sb.Append(rootStore ?? map[0].Format());

        for (var i = 1; i < map.Count; i++)
        {
            var prev = map[i - 1];
            var path = map[i];

            sb.Append(
                ChainSubStore(
                    prev.IdType,
                    prev.ModelType,
                    path.IdType,
                    path.ModelType,
                    rootStoreId ?? $"{prev.Property.Name}.Id",
                    useStoreMethod
                )
            );
        }

        return sb.ToString();
    }

    private bool CreateLoadableSources(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        IEnumerable<GenerationTarget> children,
        Logger logger)
    {
        var idType = target.IdType.ToDisplayString();
        var entityType = target.GatewayEntitySymbol.ToDisplayString();
        var modelType = target.ModelType.ToDisplayString();
        var actorType = target.ClassSymbol.ToDisplayString();

        var interfaceBrokerOverload = SyntaxFactory.ParseMemberDeclaration(
            $"async ValueTask<IEntityBroker<{idType}, {entityType}, {actorType}, {modelType}>> IBrokerProvider<{idType}, {entityType}, {actorType}, {modelType}>.GetBrokerAsync(CancellationToken token) => await GetOrCreateBrokerAsync(token);"
        );

        if (interfaceBrokerOverload is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create broker overload");
            return false;
        }

        var interfaceStoreOverload = SyntaxFactory.ParseMemberDeclaration(
            $"ValueTask<IEntityModelStore<{idType}, {modelType}>> IStoreProvider<{idType}, {modelType}>.GetStoreAsync(CancellationToken token) => GetOrCreateStoreAsync(token);"
        );

        if (interfaceStoreOverload is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create store overload");
            return false;
        }

        var store = SyntaxFactory.ParseMemberDeclaration(
            $"private Discord.Gateway.IEntityModelStore<{idType}, {modelType}>? _store;"
        );

        if (store is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create store field");
            return false;
        }

        var broker = SyntaxFactory.ParseMemberDeclaration(
            $"private Discord.Gateway.State.IEntityBroker<{idType}, {entityType}, {actorType}, {modelType}>? _broker;"
        );

        if (broker is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create broker field");
            return false;
        }

        var hasStoreRootMap = TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap);

        var storeBuilder = new StringBuilder();

        foreach
            (var (type, actor) in TypeUtils
                .GetBaseTypesAndThis(target.GatewayEntitySymbol)
                .Zip(
                    TypeUtils.GetBaseTypesAndThis(target.ClassSymbol),
                    (x, y) => (x, y)
                )
            )
        {
            if (!GatewayCacheableEntity.TryGetCacheableType(type, out var cacheable))
                continue;

            // only thing exempt here is self user
            if (cacheable!.TypeArguments[2].ToDisplayString() == "Discord.Models.ISelfUserModel")
                continue;

            if ((type.BaseType?.IsAbstract ?? true) || TryGetStoreRoot(actor, out _))
            {
                storeBuilder.AppendLine(
                    $"return _store = {CreateStoreInitialization(target, cacheable.TypeArguments[1], cacheable.TypeArguments[2], hasStoreRootMap, storeRootMap)};");
                break;
            }

            storeBuilder
                .AppendLine(
                    $"if (Client.StateController.CanUseStoreType<{cacheable.TypeArguments[0].ToDisplayString()}>())")
                .AppendLine(
                    $"return _store = {CreateStoreInitialization(target, cacheable.TypeArguments[1], cacheable.TypeArguments[2], hasStoreRootMap, storeRootMap)};");
        }

        var getOrCreateStore = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private async ValueTask<Discord.Gateway.IEntityModelStore<{{idType}}, {{modelType}}>> GetOrCreateStoreAsync(CancellationToken token)
              {
                  if (_store is not null)
                      return _store;
                  await StateSemaphore.WaitAsync(token);
                  try
                  {
                      if (_store is not null)
                          return _store;

                      {{storeBuilder}}
                  }
                  finally
                  {
                      StateSemaphore.Release();
                  }
              }
              """
        );

        if (getOrCreateStore is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create GetOrCreateStore method");
            return false;
        }

        var getOrCreateBroker = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private async ValueTask<Discord.Gateway.State.IEntityBroker<{{idType}}, {{entityType}}, {{actorType}}, {{modelType}}>> GetOrCreateBrokerAsync(
                  CancellationToken token)
              {
                  if (_broker is not null)
                      return _broker;
                  await StateSemaphore.WaitAsync(token);
                  try
                  {
                      if (_broker is not null)
                          return _broker;
                      return _broker = await Client.StateController.GetBrokerAsync<{{idType}}, {{entityType}}, {{actorType}}, {{modelType}}>(token);
                  }
                  finally
                  {
                      StateSemaphore.Release();
                  }
              }
              """
        );

        if (getOrCreateBroker is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create GetOrCreateBroker method");
            return false;
        }

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName($"IBrokerProvider<{idType}, {entityType}, {actorType}, {modelType}>")
                ),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName($"IStoreProvider<{idType}, {modelType}>")
                )
            )
            .AddMembers(
                interfaceBrokerOverload,
                interfaceStoreOverload,
                store,
                broker,
                getOrCreateStore,
                getOrCreateBroker
            );

        return true;
    }

    private static bool CreateLoadableTraitMethods(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        bool hasParent,
        bool hasChild,
        INamedTypeSymbol? rootRestEntityType,
        Logger logger
    )
    {
        var hasStoreRoot = TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap);

        var coreEntityType = target.CoreEntity.ToDisplayString();
        var restEntityType = target.RestEntitySymbol.ToDisplayString();
        var coreActorType = target.CoreActor.ToDisplayString();

        var logicalRestEntityType = rootRestEntityType?.ToDisplayString() ?? restEntityType;

        var internalModifier = target.ClassSymbol.IsSealed && !hasParent && !hasChild ? "private" : "protected";

        var getOrFetch = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<{{coreEntityType}}?> GetOrFetchAsync(GatewayRequestOptions? options = null, CancellationToken token = default)
              {
                  var cached = await GetAsync(token);
                  if (cached is not null)
                      return cached;
                  return await FetchAsync(options, token);
              }
              """
        );

        if (getOrFetch is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create GetOrFetch method");
            return false;
        }

        if (hasParent)
            getOrFetch = getOrFetch.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        var storeAccessor = hasStoreRoot
            ? $"(await GetOrCreateStoreAsync(token)).ToInfo(Client, await ({storeRootMap.Last().Property.Name} as IStoreProvider<{storeRootMap.Last().IdType}, {storeRootMap.Last().ModelType}>).GetStoreAsync(token), {storeRootMap.Last().Property.Name}.Id, Template.Of<{target.ClassSymbol}>())"
            : $"(await GetOrCreateStoreAsync(token)).ToInfo(Client, Template.Of<{target.ClassSymbol}>())";

        var fetchInternalMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              {{internalModifier}} async Task<{{logicalRestEntityType}}?> FetchInternalAsync(Discord.Gateway.GatewayRequestOptions? options = null, System.Threading.CancellationToken token = default)
              {
                  var model = await Client.RestApiClient.ExecuteAsync(
                      {{coreActorType}}.FetchRoute(this, Id),
                      options ?? Client.DefaultRequestOptions,
                      token
                  );
                  if (model is null)
                      return null;
                  if (options?.UpdateCache ?? false)
                  {
                      var store = {{storeAccessor}};
                      var broker = await GetOrCreateBrokerAsync(token);
                      await broker.UpdateAsync(model, store, token);
                  }
                  return {{restEntityType}}.Construct(Client.Rest, model);
              }
              """
        );

        if (fetchInternalMethod is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create fetchInternal method");
            return false;
        }

        if (hasParent)
            fetchInternalMethod = fetchInternalMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        else if (hasChild)
            fetchInternalMethod = fetchInternalMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

        var fetchMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<{{restEntityType}}?> FetchAsync(Discord.Gateway.GatewayRequestOptions? options = null, System.Threading.CancellationToken token = default)
                  => await FetchInternalAsync(options, token) as {{restEntityType}};
              """
        );

        if (fetchMethod is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create fetch method");
            return false;
        }

        if (hasParent)
            fetchMethod = fetchMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        syntax = syntax.AddMembers(
            getOrFetch,
            fetchMethod,
            fetchInternalMethod
        );

        return true;
    }

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    private static bool TryGetGatewayActor(ITypeSymbol type, out INamedTypeSymbol actor)
    {
        actor = (TypeUtils.GetBaseTypesAndThis(type)
                .FirstOrDefault(x => x.ToDisplayString().StartsWith("Discord.Gateway.GatewayActor"))
            as INamedTypeSymbol)!;

        return actor is not null;
    }

    private static IEnumerable<string> GetCachePathEntries(ITypeSymbol type) =>
        type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => TryGetGatewayActor(x.Type, out _))
            .Select(x => $"{x.Name}.Identity | {x.Name}");

    private bool CreateGatewayLoadable(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        bool hasParent,
        bool hasChild,
        INamedTypeSymbol? rootEntityType,
        Logger logger
    )
    {
        var hasStoreRoot = TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap);

        var idType = target.IdType.ToDisplayString();
        var entityType = target.GatewayEntitySymbol.ToDisplayString();
        var logicalEntityType = rootEntityType?.ToDisplayString() ?? entityType;

        var internalModifier = target.ClassSymbol.IsSealed && !hasParent && !hasChild ? "private" : "protected";

        var cachePathMap = TypeUtils
            .GetBaseTypesAndThis(target.ClassSymbol)
            .Where(x => !x.IsAbstract)
            .ToDictionary<
                INamedTypeSymbol,
                INamedTypeSymbol,
                IEnumerable<string>
            >(
                x => x,
                GetCachePathEntries,
                SymbolEqualityComparer.Default
            );

        var cachePathEntries = cachePathMap
            .SelectMany(x => x.Value)
            .Prepend("Identity | this")
            .ToArray();

        var cachePathField = SyntaxFactory.ParseMemberDeclaration(
            "private CachePathable? _cachePath;"
        );

        var cachePathProperty = SyntaxFactory.ParseMemberDeclaration(
            $"internal CachePathable CachePath => _cachePath ??= new() {{ {string.Join(", ", cachePathEntries)} }};"
        );

        if (cachePathProperty is null || cachePathField is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create cache paths");
            return false;
        }

        var hasCachePathParent =
            cachePathMap.Skip(1).Sum(x => x.Value.Count()) > 0 ||
            cachePathMap.Skip(1).Any(x => TryGetGatewayActor(x.Key, out _));

        if (hasCachePathParent)
            cachePathProperty = cachePathProperty.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        else if (hasChild)
            cachePathProperty = cachePathProperty.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

        syntax = syntax.AddMembers(
            cachePathField,
            cachePathProperty
        );

        var cachePathAccess = cachePathEntries.Length == 0 ? "CachePathable.Default" : "CachePath";

        var storeAccessor = hasStoreRoot
            ? $"(await GetOrCreateStoreAsync(token)).ToInfo(Client, await ({storeRootMap.Last().Property.Name} as IStoreProvider<{storeRootMap.Last().IdType}, {storeRootMap.Last().ModelType}>).GetStoreAsync(token), {storeRootMap.Last().Property.Name}.Id, Template.Of<{storeRootMap.Last().Actor}>());"
            : $"(await GetOrCreateStoreAsync(token)).ToInfo(Client, Template.Of<{target.ClassSymbol}>());";

        var getHandleInternal = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              {{internalModifier}} async ValueTask<Discord.Gateway.IEntityHandle<{{idType}}, {{logicalEntityType}}>?> GetHandleInternalAsync(CancellationToken token = default)
              {
                  var store = {{storeAccessor}}
                  var broker = await GetOrCreateBrokerAsync(token);
                  return await broker.GetAsync({{cachePathAccess}}, Identity, store, this, token);
              }
              """
        );

        if (getHandleInternal is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create GetHandleInternal method");
            return false;
        }

        if (hasParent)
            getHandleInternal = getHandleInternal.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        else if (hasChild)
            getHandleInternal = getHandleInternal.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

        var getHandle = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<Discord.Gateway.IEntityHandle<{{idType}}, {{entityType}}>?> GetHandleAsync(CancellationToken token = default)
                  => await GetHandleInternalAsync(token) as IEntityHandle<{{idType}}, {{entityType}}>;
              """
        );

        if (getHandle is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create GetHandle method");
            return false;
        }

        if (hasParent)
            getHandle = getHandle.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        var getMethodInternal = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              {{internalModifier}} async ValueTask<{{logicalEntityType}}?> GetInternalAsync(CancellationToken token = default)
              {
                  var store = {{storeAccessor}}
                  var broker = await GetOrCreateBrokerAsync(token);
                  return await broker.GetImplicitAsync({{cachePathAccess}}, Identity, store, this, token);
              }
              """
        );

        if (getMethodInternal is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create getMethodInternal method");
            return false;
        }

        if (hasParent)
            getMethodInternal = getMethodInternal.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        else if (hasChild)
            getMethodInternal = getMethodInternal.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));

        var getMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<{{entityType}}?> GetAsync(CancellationToken token = default)
                  => await GetInternalAsync(token) as {{entityType}};
              """
        );

        if (getMethod is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create get method");
            return false;
        }

        if (hasParent)
            getMethod = getMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

        syntax = syntax.AddMembers(
            getHandle,
            getHandleInternal,
            getMethod,
            getMethodInternal
        );

        return true;
    }
}
