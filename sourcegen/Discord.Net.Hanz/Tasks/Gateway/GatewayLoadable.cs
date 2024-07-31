using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks.Gateway;

public sealed class GatewayLoadable : IGenerationCombineTask<GatewayLoadable.GenerationContext>
{
    public sealed class GenerationContext(
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
    ) : IEquatable<GenerationContext>
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

        public bool Equals(GenerationContext? other)
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
            ReferenceEquals(this, obj) || obj is GenerationContext other && Equals(other);

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

        public static bool operator ==(GenerationContext? left, GenerationContext? right) => Equals(left, right);

        public static bool operator !=(GenerationContext? left, GenerationContext? right) => !Equals(left, right);
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

    public GenerationContext? GetTargetForGeneration(
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

        if (hierarchy.All(x => !LoadableTrait.IsLoadable(x.Type)))
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: not loadable");
            return null;
        }

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

        //logger.Log($"{classSymbol}: Got rest entity type {restEntity} ({actorType})");

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

        return new GenerationContext(
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

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationContext?> targets, Logger logger)
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

            targetLogger.Log($"{target.ClassSymbol}: Rest entity: {target.RestEntitySymbol}");

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var root = TypeUtils.GetBaseTypes(target.ClassSymbol).LastOrDefault(bases.Contains);
            var rootTarget = root is not null
                ? targets.First(x => x is not null && x.ClassSymbol.Equals(root, SymbolEqualityComparer.Default))
                : null;
            var hasChild = bases.Contains(target.ClassSymbol);

            if (!CreateLoadableSources(
                    ref syntax,
                    target,
                    targetLogger))
                continue;

            if (!CreateGatewayLoadable(
                    ref syntax,
                    target,
                    root is not null,
                    hasChild,
                    rootTarget?.GatewayEntitySymbol,
                    rootTarget?.RestEntitySymbol,
                    targetLogger)
               ) continue;

            CreateOverloadsAsync(ref syntax, target, targetLogger);


            context.AddSource(
                $"GatewayLoadable/{target.ClassSymbol.Name}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.ClassSymbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
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

    private void CreateOverloadsAsync(
        ref ClassDeclarationSyntax syntax,
        GenerationContext target,
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

    private readonly struct StoreRootPath(
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

    private static bool TryGetStoreRootMap(ITypeSymbol type, out List<StoreRootPath> map)
    {
        map = new();

        var checkingType = type;

        while (checkingType is not null && !checkingType.IsAbstract)
        {
            if(TryGetStoreRoot(checkingType, out var root) && TryGetBrokerTypes(root.Type, out var id, out var entity, out var model))
                map.Add(new(checkingType, root, id, entity, model));

            checkingType = checkingType.BaseType;
        }

        map.Reverse();

        return map.Count > 0;
    }

    private static string FormatRootChain(List<StoreRootPath> map)
    {
        if (map.Count < 1)
            throw new InvalidOperationException("store root map must contain atleast 1 element");

        var sb = new StringBuilder();

        sb.Append(map[0].Format());

        for (var i = 1; i < map.Count; i++)
        {
            var prev = map[i - 1];
            var path = map[i];

            //.Chain<{storeRootId}, {storeRootModel}, {idType}, {modelType}>({storeRoot.Name}.Id)
            sb.Append(
                $".Chain<{prev.IdType}, {prev.ModelType}, {path.IdType}, {path.ModelType}>({prev.Property.Name}.Id)");
        }

        return sb.ToString();
    }

    private bool CreateLoadableSources(
        ref ClassDeclarationSyntax syntax,
        GenerationContext target,
        Logger logger)
    {
        var idType = target.IdType.ToDisplayString();
        var entityType = target.GatewayEntitySymbol.ToDisplayString();
        var modelType = target.ModelType.ToDisplayString();
        var actorType = target.ClassSymbol.ToDisplayString();

        var interfaceBrokerOverload = SyntaxFactory.ParseMemberDeclaration(
            $"async ValueTask<IEntityBroker<{idType}, {entityType}, {modelType}>> IBrokerProvider<{idType}, {entityType}, {modelType}>.GetBrokerAsync(CancellationToken token) => (await GetOrCreateBrokerAsync(token)).Value;"
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
            logger.Warn($"{target.ClassSymbol}: failed to create broker overload");
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
            $"private Discord.Gateway.IRefCounted<Discord.Gateway.State.IEntityBroker<{idType}, {entityType}, {modelType}>>? _broker;"
        );

        if (broker is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create broker field");
            return false;
        }

        var disposeMethod = SyntaxFactory.ParseMemberDeclaration(
            """
            private ValueTask DisposeLoadableResources()
            {
                _broker?.Dispose();
                return ValueTask.CompletedTask;
            }
            """
        );

        if (disposeMethod is null)
        {
            logger.Warn($"{target.ClassSymbol}: failed to create dispose method");
            return false;
        }

        Func<ITypeSymbol, ITypeSymbol, string> storeInitializer = (idType, modelType) =>
        {
            var store = $"await Client.CacheProvider.GetStoreAsync<{idType}, {modelType}>(token)";

            if (!target.ModelType.Equals(modelType, SymbolEqualityComparer.Default))
                store = $"({store}).Cast(Template.Of<{target.ModelType}>())";

            return store;
        };

        if (
            TryGetStoreRootMap(target.ClassSymbol, out var storeRootMap)
        )
        {
            logger.Log($"{target.ClassSymbol}: Store root found: {storeRootMap.Count} roots");

            storeInitializer = (idType, modelType) =>
            {
                var formatted = FormatRootChain(storeRootMap);
                var lastRoot = storeRootMap.Last();

                var store =
                    $"await {formatted}.Chain<{lastRoot.IdType}, {lastRoot.ModelType}, {idType}, {modelType}>({lastRoot.Property.Name}.Id)";

                if (!target.ModelType.Equals(modelType, SymbolEqualityComparer.Default))
                    store = $"({store}).Cast(Template.Of<{target.ModelType}>())";

                return store;
            };
        }
        else
        {
            logger.Log($"{target.ClassSymbol}: Flat store found: {target.GatewayEntitySymbol}");
        }

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
                    $"return _store = {storeInitializer(cacheable.TypeArguments[1], cacheable.TypeArguments[2])};");
                break;
            }

            storeBuilder
                .AppendLine(
                    $"if (Client.StateController.CanUseStoreType<{cacheable.TypeArguments[0].ToDisplayString()}>())")
                .AppendLine(
                    $"return _store = {storeInitializer(cacheable!.TypeArguments[1], cacheable.TypeArguments[2])};");
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
              private async ValueTask<Discord.Gateway.IRefCounted<Discord.Gateway.State.IEntityBroker<{{idType}}, {{entityType}}, {{modelType}}>>> GetOrCreateBrokerAsync(
                  CancellationToken token)
              {
                  if (_broker is not null)
                      return _broker;
                  await StateSemaphore.WaitAsync(token);
                  try
                  {
                      if (_broker is not null)
                          return _broker;
                      var broker = await Client.StateController.GetBrokerAsync<{{idType}}, {{entityType}}, {{actorType}}, {{modelType}}>(token);
                      RegisterDisposeTask(DisposeLoadableResources);
                      return _broker = broker;
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
                    SyntaxFactory.ParseTypeName($"IBrokerProvider<{idType}, {entityType}, {modelType}>")
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
                disposeMethod,
                getOrCreateStore,
                getOrCreateBroker
            );

        return true;
    }

    private bool CreateGatewayLoadable(
        ref ClassDeclarationSyntax syntax,
        GenerationContext target,
        bool hasParent,
        bool hasChild,
        INamedTypeSymbol? rootEntityType,
        INamedTypeSymbol? rootRestEntityType,
        Logger logger
    )
    {
        var idType = target.IdType.ToDisplayString();
        var entityType = target.GatewayEntitySymbol.ToDisplayString();
        var modelType = target.ModelType.ToDisplayString();
        var coreEntityType = target.CoreEntity.ToDisplayString();
        var restEntityType = target.RestEntitySymbol.ToDisplayString();
        var coreActorType = target.CoreActor.ToDisplayString();

        var logicalEntityType = rootEntityType?.ToDisplayString() ?? entityType;
        var logicalRestEntityType = rootRestEntityType?.ToDisplayString() ?? restEntityType;


        var getHandleInternal = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              protected async ValueTask<Discord.Gateway.IEntityHandle<{{idType}}, {{logicalEntityType}}>?> GetHandleInternalAsync(CancellationToken token = default)
              {
                  var store = await GetOrCreateStoreAsync(token);
                  var broker = await GetOrCreateBrokerAsync(token);
                  return await broker.Value.GetAsync(this, Identity, store, token);
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
              protected async ValueTask<{{logicalEntityType}}?> GetInternalAsync(CancellationToken token = default)
              {
                  var store = await GetOrCreateStoreAsync(token);
                  var broker = await GetOrCreateBrokerAsync(token);
                  return await broker.Value.GetImplicitAsync(this, Identity, store, token);
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

        var fetchInternalMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              protected async Task<{{logicalRestEntityType}}?> FetchInternalAsync(Discord.Gateway.GatewayRequestOptions? options = null, System.Threading.CancellationToken token = default)
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
                      var store = await GetOrCreateStoreAsync(token);
                      var broker = await GetOrCreateBrokerAsync(token);
                      await broker.Value.UpdateAsync(model, store, token);
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
            getHandle,
            getHandleInternal,
            getMethod,
            getMethodInternal,
            getOrFetch,
            fetchMethod,
            fetchInternalMethod
        );

        return true;
    }

    // private IEntityModelStore<ulong, IUserModel>? _store;
    // private IRefCounted<IEntityBroker<ulong, GatewayUser, IUserModel>>? _broker;
    //
    // private ValueTask DisposeLoadableResources()
    // {
    //     _broker?.Dispose();
    //
    //     return ValueTask.CompletedTask;
    // }
    //
    // private async ValueTask<IEntityModelStore<ulong, IUserModel>> GetOrCreateStoreAsync(CancellationToken token)
    // {
    //     if (_store is not null)
    //         return _store;
    //
    //     await StateSemaphore.WaitAsync(token);
    //
    //     try
    //     {
    //         if (_store is not null)
    //             return _store;
    //
    //         return _store = await GetStoreAsync(token);
    //     }
    //     finally
    //     {
    //         StateSemaphore.Release();
    //     }
    // }
    //
    // private async ValueTask<IRefCounted<IEntityBroker<ulong, GatewayUser, IUserModel>>> GetOrCreateBrokerAsync(
    //     CancellationToken token)
    // {
    //     if (_broker is not null)
    //         return _broker;
    //
    //     await StateSemaphore.WaitAsync(token);
    //
    //     try
    //     {
    //         if (_broker is not null)
    //             return _broker;
    //
    //         var broker = await Client.StateController.GetBrokerAsync<ulong, GatewayUser, IUserModel>(token);
    //         RegisterDisposeTask(DisposeLoadableResources);
    //         return broker;
    //     }
    //     finally
    //     {
    //         StateSemaphore.Release();
    //     }
    // }
    //
    // public async ValueTask<IEntityHandle<ulong, GatewayUser>?> GetHandleAsync(CancellationToken token = default)
    // {
    //     var store = await GetOrCreateStoreAsync(token);
    //     var broker = await GetOrCreateBrokerAsync(token);
    //
    //     return await broker.Value.GetAsync(this, Identity, store, token);
    // }
    //
    // public async ValueTask<GatewayUser?> GetAsync(CancellationToken token = default)
    // {
    //     var handle = await GetHandleAsync(token);
    //
    //     if (handle is null)
    //         return null;
    //
    //     return await handle.ConsumeAsync();
    // }
    //
    // public async ValueTask<IUser?> GetOrFetchAsync(GatewayRequestOptions? options = null, CancellationToken token = default)
    // {
    //     var cached = await GetAsync(token);
    //     if (cached is not null)
    //         return cached;
    //
    //     return await FetchAsync(options, token);
    // }
    //
    // public async ValueTask<Discord.Rest.RestUser?> FetchAsync(Discord.Gateway.GatewayRequestOptions? options = null, System.Threading.CancellationToken token = default)
    // {
    //     var result =
    //         await Discord.IUserActor.FetchInternalAsync(Client, this, Discord.IUserActor.FetchRoute(this, Id), options,
    //             token) as Discord.Rest.RestUser;
    //
    //     if (result is not null && (options?.UpdateCache ?? false))
    //     {
    //         var store = await GetOrCreateStoreAsync(token);
    //         await store.AddOrUpdateAsync(result.GetModel(), token);
    //     }
    //
    //     return result;
    // }
    //
    // async System.Threading.Tasks.ValueTask<Discord.IUser?> Discord.IUserActor.FetchAsync(Discord.RequestOptions? options, System.Threading.CancellationToken token) => await this.FetchAsync(options: GatewayRequestOptions.FromRestOptions(options), token: token);
    // async System.Threading.Tasks.ValueTask<Discord.IUser?> Discord.ILoadable<Discord.IUserActor, ulong, Discord.IUser, Discord.Models.IUserModel>.FetchAsync(Discord.RequestOptions? options, System.Threading.CancellationToken token) => await this.FetchAsync(options: GatewayRequestOptions.FromRestOptions(options), token: token);
    // async ValueTask<IUser?> ILoadable<IUserActor, ulong, IUser, IUserModel>.GetAsync(CancellationToken token) => await GetAsync(token);
    //
    // ValueTask<IUser?> ILoadable<IUserActor, ulong, IUser, IUserModel>.GetOrFetchAsync(Discord.RequestOptions? options,
    //     System.Threading.CancellationToken token) => GetOrFetchAsync(GatewayRequestOptions.FromRestOptions(options), token);
}
