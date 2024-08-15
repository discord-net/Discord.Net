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
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(GatewayActorInterface);
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
        if (targets.Length == 0) return;

        var nonNullTargets = targets.OfType<GenerationTarget>().ToList();

        if (nonNullTargets.Count == 0) return;

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

            if (
                target.GatewayEntitySymbol.DeclaringSyntaxReferences
                    .FirstOrDefault()
                    ?.GetSyntax()
                is not ClassDeclarationSyntax entitySyntax
            ) continue;

            var targetLogger = logger.WithSemanticContext(target.SemanticModel);

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);
            entitySyntax = SyntaxUtils.CreateSourceGenClone(entitySyntax);

            ImplementLoadable(ref syntax, target, nonNullTargets, targetLogger);

            ImplementProxyForEntity(ref entitySyntax, target, nonNullTargets);

            context.AddSource(
                $"Loadables/{GetSimpleName(target.ClassSymbol)}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives("System.Collections.Immutable", "Discord.Gateway.State")}}

                  namespace {{target.ClassSymbol.ContainingNamespace}};

                  {{entitySyntax.NormalizeWhitespace()}}

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }

        var storesSyntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)
            ),
            SyntaxFactory.Identifier("Stores"),
            null,
            null,
            [],
            []
        );

        var brokersSyntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.InternalKeyword),
                SyntaxFactory.Token(SyntaxKind.StaticKeyword)
            ),
            SyntaxFactory.Identifier("Brokers"),
            null,
            null,
            [],
            []
        );

        PopulateStoresMap(ref storesSyntax, nonNullTargets);
        PopulateBrokersMap(ref brokersSyntax, nonNullTargets);

        context.AddSource(
            "Brokers/BrokersMap",
            $$"""
              using Discord;
              using Discord.Gateway;
              using Discord.Gateway.State;

              namespace Discord.Gateway;

              {{brokersSyntax.NormalizeWhitespace()}}
              """
        );

        context.AddSource(
            "Stores/StoreMap",
            $$"""
              using Discord;
              using Discord.Gateway;
              using Discord.Gateway.State;

              namespace Discord.Gateway;

              {{storesSyntax.NormalizeWhitespace()}}
              """
        );
    }

    private static void ImplementProxyForEntity(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        List<GenerationTarget> targets)
    {
        var simpleName = GetSimpleName(target.ClassSymbol);

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        $"IContextConstructable<{target.GatewayEntitySymbol}, {target.ModelType}, IGatewayConstructionContext, DiscordGatewayClient>"
                    )
                ),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        $"IStoreInfoProvider<{target.IdType}, {target.ModelType}>"
                    )
                ),
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        $"IBrokerInfoProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>"
                    )
                )
            )
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    "internal override CachePathable CachePath => Actor.CachePath;"
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static ValueTask<IStoreInfo<{{target.IdType}}, {{target.ModelType}}>> IStoreInfoProvider<{{target.IdType}}, {{target.ModelType}}>.GetStoreInfoAsync(DiscordGatewayClient client, IPathable? path, CancellationToken token)
                          => Stores.{{simpleName}}.GetStoreInfoAsync(client, path, token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> IStoreProvider<{{target.IdType}}, {{target.ModelType}}>.GetStoreAsync(DiscordGatewayClient client, IPathable? path, CancellationToken token)
                          => Stores.{{simpleName}}.GetStoreAsync(client, path, token);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static IBrokerInfo<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}> IBrokerInfoProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.GetBrokerInfo(DiscordGatewayClient client)
                          => Brokers.{{simpleName}}.GetBrokerInfo(client);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.GetBroker(DiscordGatewayClient client)
                          => Brokers.{{simpleName}}.GetBroker(client);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>.GetBroker(DiscordGatewayClient client)
                          => Brokers.{{simpleName}}.GetBroker(client);
                      """
                )!,
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      static IReadOnlyDictionary<Type, IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>.GetBrokerHierarchy(DiscordGatewayClient client)
                          => Brokers.{{simpleName}}.GetBrokerHierarchy(client);
                      """
                )!
            );

        var cacheableInterface = target.GatewayEntitySymbol.Interfaces
            .FirstOrDefault(x => x.Name is "ICacheableEntity");

        if (cacheableInterface is not null)
        {
            syntax = syntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      CachePathable {{cacheableInterface}}.CachePath => CachePath;
                      """
                )!
            );
        }
    }

    private static void GetTargetHierarchy(
        GenerationTarget target,
        List<GenerationTarget> targets,
        out GenerationTarget[] children,
        out List<ITypeSymbol> bases,
        out GenerationTarget[] parents)
    {
        children = targets
            .Where(x =>
                TypeUtils
                    .GetBaseTypes(x.ClassSymbol)
                    .Contains(target.ClassSymbol, SymbolEqualityComparer.Default)
            )
            .ToArray();

        var basesLocal = bases = TypeUtils
            .GetBaseTypes(target.ClassSymbol)
            .ToList();

        parents = targets
            .Where(x =>
                basesLocal.Contains(x.ClassSymbol, SymbolEqualityComparer.Default)
            )
            .OrderBy(x => basesLocal
                .FindIndex(y =>
                    y.Equals(x.ClassSymbol, SymbolEqualityComparer.Default)
                )
            )
            .ToArray();
    }

    private static IEnumerable<string> GetCachePathEntries(ITypeSymbol type) =>
        type.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => IsGatewayActor(x.Type))
            .Select(x => $"{x.Name}.Identity | {x.Name}");

    [SuppressMessage("ReSharper", "ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract")]
    private static bool IsGatewayActor(ITypeSymbol type)
        => TypeUtils.GetBaseTypesAndThis(type)
            .Any(x => x.ToDisplayString().StartsWith("Discord.Gateway.GatewayActor"));

    private static void ImplementLoadable(
        ref ClassDeclarationSyntax syntax,
        GenerationTarget target,
        List<GenerationTarget> targets,
        Logger logger)
    {
        if (!TryCreateRestEntityConstruction(target.RestEntitySymbol, target.ModelType, logger,
                out var restEntityConstruction))
        {
            logger.Warn($"{target.ClassSymbol}: failed to find a way to construct {target.RestEntitySymbol}");
            return;
        }

        var simpleName = GetSimpleName(target.ClassSymbol);

        GetTargetHierarchy(
            target,
            targets,
            out var children,
            out var bases,
            out var parents
        );

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
        )!;

        var cachePathProperty = SyntaxFactory.ParseMemberDeclaration(
            $"internal override CachePathable CachePath => _cachePath ??= new() {{ {string.Join(", ", cachePathEntries)} }};"
        )!;

        var rootParent = parents.LastOrDefault();
        var internalTarget = rootParent ?? target;

        var internalMethodModifier = target.ClassSymbol.IsSealed && rootParent is null ? "private" : "protected";

        var getHandleInternalMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              {{internalMethodModifier}} async ValueTask<IEntityHandle<{{internalTarget.IdType}}, {{internalTarget.GatewayEntitySymbol}}>?> GetHandleInternalAsync(CancellationToken token = default)
              {
                  var store = await Stores.{{simpleName}}.GetStoreInfoAsync(this, token);
                  var broker = Brokers.{{simpleName}}.GetBroker(Client);
                  
                  return await broker.GetAsync(CachePath, Identity, store, this, token)
                  {{(
                      internalTarget != target
                          ? $" as IEntityHandle<{internalTarget.IdType}, {internalTarget.GatewayEntitySymbol}>"
                          : string.Empty
                  )}};
              }
              """
        )!;

        var getHandleMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<IEntityHandle<{{target.IdType}}, {{target.GatewayEntitySymbol}}>?> GetHandleAsync(CancellationToken token = default)
                  => await GetHandleInternalAsync(token){{(
                      internalTarget != target
                          ? $"as IEntityHandle<{target.IdType}, {target.GatewayEntitySymbol}>"
                          : string.Empty
                  )}};
              """
        )!;

        var getMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public async ValueTask<{{target.GatewayEntitySymbol}}?> GetAsync(CancellationToken token = default)
                  => (await GetHandleInternalAsync(token)).ConsumeAsReference(){{(
                      internalTarget != target
                          ? $" as {target.GatewayEntitySymbol}"
                          : string.Empty
                  )}};
              """
        )!;

        var fetchLoadableMethod = GetLoadableMethods(
                target.ClassSymbol,
                target.GatewayEntitySymbol,
                target.SemanticModel
            )
            .FirstOrDefault(x => x.Name is "FetchAsync");

        MemberDeclarationSyntax? fetchMethod = null;
        MemberDeclarationSyntax? fetchInternalMethod = null;


        if (fetchLoadableMethod is not null)
        {
            var fetchParameterMapping = MemberUtils
                .CreateParameterList(fetchLoadableMethod, requestOptions: "Discord.Gateway.GatewayRequestOptions")
                .NormalizeWhitespace();

            var nonDefaultParameters =
                fetchLoadableMethod?.Parameters.Length > 2
                    ? $", {string.Join(", ", fetchLoadableMethod.Parameters.Take(fetchLoadableMethod.Parameters.Length - 2).Select(x => x.Name))}"
                    : string.Empty;

            var fetchArgumentMapping = string.Join(
                ", ",
                fetchParameterMapping.Parameters.Select(x => $"{x.Identifier}: {x.Identifier}")
            );

            fetchInternalMethod = SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  {{internalMethodModifier}} async ValueTask<{{internalTarget.RestEntitySymbol}}?> FetchInternalAsync{{fetchParameterMapping}}
                  {
                      {{target.ModelType.WithNullableAnnotation(NullableAnnotation.Annotated)}} model = await Client.RestApiClient.ExecuteAsync(
                          {{target.CoreActor}}.FetchRoute(this, Id{{nonDefaultParameters}}),
                          options ?? Client.DefaultRequestOptions,
                          token
                      );
                      
                      if (model is null)
                          return null;
                          
                      if (options?.UpdateCache ?? false)
                      {
                          var store = await Stores.{{simpleName}}.GetStoreInfoAsync(this, token);
                          var broker = Brokers.{{simpleName}}.GetBroker(Client);
                          await broker.UpdateAsync(model, store, token);
                      }
                      
                      return {{restEntityConstruction}};
                  }
                  """
            )!;

            fetchMethod = SyntaxFactory.ParseMemberDeclaration(
                $$"""
                  public async ValueTask<{{target.RestEntitySymbol}}?> FetchAsync{{fetchParameterMapping}}
                      => await FetchInternalAsync({{fetchArgumentMapping}}) as {{target.RestEntitySymbol}};
                  """
            )!;
        }


        if (rootParent is not null)
        {
            fetchMethod = fetchMethod?.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            getMethod = getMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            getHandleMethod = getHandleMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));
            fetchInternalMethod = fetchInternalMethod?.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
            getHandleInternalMethod =
                getHandleInternalMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.OverrideKeyword));
        }
        else if (children.Length > 0)
        {
            fetchInternalMethod = fetchInternalMethod?.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));
            getHandleInternalMethod =
                getHandleInternalMethod.AddModifiers(SyntaxFactory.Token(SyntaxKind.VirtualKeyword));
        }

        syntax = syntax.AddMembers(
            cachePathField,
            cachePathProperty,
            getHandleInternalMethod,
            getHandleMethod,
            getMethod
        );

        if (fetchMethod is not null && fetchInternalMethod is not null)
            syntax = syntax.AddMembers(fetchMethod, fetchInternalMethod);
    }

    private static bool TryCreateRestEntityConstruction(
        INamedTypeSymbol restEntity,
        ITypeSymbol model,
        Logger logger,
        out string entityConstruction)
    {
        foreach (var methodSymbol in restEntity.GetMembers("Construct").OfType<IMethodSymbol>())
        {
            if (!methodSymbol.Parameters.Last().Type.Equals(model, SymbolEqualityComparer.Default))
                continue;

            if (methodSymbol.Parameters.Length == 2)
            {
                entityConstruction = $"{restEntity}.Construct(Client.Rest, model)";
                return true;
            }

            if (methodSymbol.Parameters.Length != 3)
            {
                logger.Log($"{restEntity}: Unknown Construct method {methodSymbol}");
                continue;
            }

            if (methodSymbol.Parameters[1].Type is not INamedTypeSymbol contextType)
            {
                logger.Log($"{restEntity}: Unknown context type {methodSymbol.Parameters[1]}");
                continue;
            }

            var isNullable = methodSymbol.Parameters[1].NullableAnnotation is NullableAnnotation.Annotated;

            switch (contextType.Name)
            {
                case "IIdentifiable" when TryGetCoreInterfaceOfEntity(contextType.TypeArguments[1], out var coreEntity):
                    var pathAccess = isNullable ? "Optionally" : "Require";

                    var context = $"(this as IPathable)!.{pathAccess}<{contextType.TypeArguments[0]}, {coreEntity}>()";

                    context = isNullable
                        ? $"{context} is {{}} context ? {contextType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)}.Of(context) : null"
                        : $"{contextType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)}.Of({context})";

                    entityConstruction =
                        $"{restEntity}.Construct(Client.Rest, {context}, model)";


                    return true;
                case "Context":
                    foreach (var constructor in contextType.Constructors)
                    {
                        var mappedParameters = new List<string?>(
                            constructor.Parameters.Select(x =>
                                x.HasExplicitDefaultValue
                                    ? SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue).ToString()
                                    : x.NullableAnnotation is NullableAnnotation.Annotated
                                        ? "null"
                                        : null
                            )
                        );

                        for (var i = 0; i < constructor.Parameters.Length; i++)
                        {
                            var parameter = constructor.Parameters[i];
                            if (parameter.Type is not INamedTypeSymbol {Name: "IIdentifiable"} identityType)
                                continue;

                            if (!TryGetCoreInterfaceOfEntity(identityType.TypeArguments[1], out var coreEntity))
                                continue;

                            isNullable = parameter.NullableAnnotation is NullableAnnotation.Annotated;
                            pathAccess = isNullable ? "Optionally" : "Require";

                            mappedParameters[i] =
                                $"(this as IPathable)!.{pathAccess}<{identityType.TypeArguments[0]}, {coreEntity}>()";

                            if (isNullable)
                                mappedParameters[i] =
                                    $"{mappedParameters[i]} is {{}} {parameter.Name} ? {identityType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)}.Of({parameter.Name}) : null";
                            else
                                mappedParameters[i] =
                                    $"{identityType.WithNullableAnnotation(NullableAnnotation.NotAnnotated)}.Of({mappedParameters[i]})";
                        }

                        var isValid = !constructor.Parameters
                            .Where((t, i) => !t.IsOptional && mappedParameters[i] is null)
                            .Any();

                        if (!isValid)
                        {
                            logger.Log($"Invalid constructor for context {constructor}");
                            continue;
                        }

                        entityConstruction =
                            $"{restEntity}.Construct(Client.Rest, new {contextType}({string.Join(", ", mappedParameters.Where(x => x is not null))}), model)";
                        return true;
                    }

                    break;
                default:
                    logger.Log($"Unknown context type '{contextType.Name}'");
                    break;
            }
        }

        entityConstruction = null!;
        return false;
    }

    private static bool TryGetCoreInterfaceOfEntity(ITypeSymbol entity, out ITypeSymbol coreEntity)
    {
        var actor = Hierarchy.GetHierarchy(entity)
            .FirstOrDefault(x => x.Type.ToDisplayString().StartsWith("Discord.IActor"))
            .Type;

        return (coreEntity = actor?.TypeArguments[1]!) is not null;
    }

    public static IEnumerable<IMethodSymbol> GetLoadableMethods(
        ITypeSymbol type,
        ITypeSymbol entityType,
        SemanticModel model)
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
            .Where(x =>
                model.Compilation.HasImplicitConversion(
                    entityType,
                    x.ReturnType is INamedTypeSymbol {Name: "Task" or "ValueTask"} asyncResult
                        ? asyncResult.TypeArguments[0]
                        : x.ReturnType
                )
            )
            .OrderByDescending(x => x.Parameters.Length);
    }


    private static IPropertySymbol? GetStoreRoot(GenerationTarget target, List<GenerationTarget> all)
    {
        return target.ClassSymbol.GetMembers()
            .OfType<IPropertySymbol>()
            .FirstOrDefault(x => x
                    .GetAttributes()
                    .Any(x =>
                        x.AttributeClass?.ToDisplayString() ==
                        "Discord.Gateway.StoreRootAttribute"
                    ) && all.Any(y => y.ClassSymbol.Equals(x.Type, SymbolEqualityComparer.Default))
            );
    }

    private sealed class StoreInfo(
        ClassDeclarationSyntax syntax,
        GenerationTarget target,
        GenerationTarget[] children,
        GenerationTarget[] parents,
        GenerationTarget? parentStoreTarget = null)
    {
        public readonly GenerationTarget? ParentStoreTarget = parentStoreTarget;
        public readonly GenerationTarget Target = target;
        public readonly GenerationTarget[] Children = children;
        public readonly GenerationTarget[] Parents = parents;

        public ClassDeclarationSyntax Syntax = syntax;
    }

    private static void PopulateStoresMap(
        ref ClassDeclarationSyntax syntax,
        List<GenerationTarget> targets)
    {
        var stores = new Dictionary<GenerationTarget, StoreInfo>();

        foreach (var target in targets)
        {
            var children = targets
                .Where(x =>
                    TypeUtils
                        .GetBaseTypes(x.ClassSymbol)
                        .Contains(target.ClassSymbol, SymbolEqualityComparer.Default)
                )
                .OrderByDescending(x => TypeUtils
                    .GetBaseTypes(x.ClassSymbol)
                    .TakeWhile(x => 
                        !x.Equals(target.ClassSymbol, SymbolEqualityComparer.Default)
                    )
                    .Count()
                )
                .ToArray();

            var bases = TypeUtils
                .GetBaseTypes(target.ClassSymbol)
                .ToList();

            var parents = targets
                .Where(x =>
                    bases.Contains(x.ClassSymbol, SymbolEqualityComparer.Default)
                )
                .OrderBy(x => bases
                    .FindIndex(y =>
                        y.Equals(x.ClassSymbol, SymbolEqualityComparer.Default)
                    )
                )
                .ToArray();

            var root =
                GetStoreRoot(target, targets)
                ??
                parents
                    .Select(x =>
                        GetStoreRoot(x, targets)
                    )
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault();

            var rootTarget = root is null
                ? null
                : targets.First(x =>
                    x.ClassSymbol.Equals(root.Type, SymbolEqualityComparer.Default)
                );

            var name = target.ClassSymbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty);
            var targetSyntax = CreateInternalClassSyntax(name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.SealedKeyword));

            if (root is not null && rootTarget is not null)
            {
                var maximum = root.ContainingType;
                var containingRootIsUs = maximum.Equals(target.ClassSymbol, SymbolEqualityComparer.Default);
                var maximumTarget = containingRootIsUs
                    ? target
                    : parents.First(x => x.ClassSymbol.Equals(maximum, SymbolEqualityComparer.Default));

                var candidates =
                    maximum.Equals(target.ClassSymbol, SymbolEqualityComparer.Default)
                        ? []
                        : parents
                            .TakeWhile(x =>
                                !x.ClassSymbol.Equals(maximum, SymbolEqualityComparer.Default)
                            )
                            .ToArray();

                targetSyntax = targetSyntax
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public static async ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> GetStoreAsync(
                                  DiscordGatewayClient client,
                                  IEntityModelStore<{{rootTarget.IdType}}, {{rootTarget.ModelType}}> parent,
                                  {{rootTarget.IdType}} parentId,
                                  CancellationToken token = default)
                              {
                                  {{(
                                      containingRootIsUs
                                          ? $"""
                                             return await parent.GetSubStoreAsync<{target.IdType}, {target.ModelType}>(parentId, token);
                                             """
                                          : $$"""
                                              if(client.StateController.CanUseStoreType<{{target.GatewayEntitySymbol}}>())
                                                  return await parent.GetSubStoreAsync<{{target.IdType}}, {{target.ModelType}}>(parentId, token);
                                              """
                                  )}}
                              
                                  {{
                                      string.Join(
                                          "\n",
                                          candidates.Select(x =>
                                              $$"""
                                                if(client.StateController.CanUseStoreType<{{x.GatewayEntitySymbol}}>())
                                                    return (await parent.GetSubStoreAsync<{{x.IdType}}, {{x.ModelType}}>(parentId, token)).CastDown(Template.Of<{{target.ModelType}}>());
                                                """
                                          )
                                      )
                                  }}
                                  
                                  {{(
                                      !containingRootIsUs
                                          ? $$"""
                                              return (await parent.GetSubStoreAsync<{{maximumTarget.IdType}}, {{maximumTarget.ModelType}}>(parentId, token)).CastDown(Template.Of<{{target.ModelType}}>());
                                              """
                                          : string.Empty
                                  )}}
                              }   
                              """
                        )!
                    );
            }
            else
            {
                targetSyntax = targetSyntax.AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static async ValueTask<IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>> GetStoreAsync(
                                DiscordGatewayClient client,
                                CancellationToken token = default)
                          {
                              return await client.CacheProvider.GetStoreAsync<{{target.IdType}}, {{target.ModelType}}>(token);
                          }
                          """
                    )!
                );
            }

            var childStores = children
                .Where(x =>
                    x.ModelType.Name is not "ISelfUserModel" &&
                    (
                        GetStoreRoot(x, targets)?.Equals(root, SymbolEqualityComparer.Default)
                        ?? root is not null
                    )
                )
                .ToArray();

            if (childStores.Length > 0)
            {
                targetSyntax = targetSyntax
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public static async ValueTask<IReadOnlyDictionary<Type, IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>>> GetStoreHierarchyAsync(
                                  DiscordGatewayClient client,
                                  {{(
                                      rootTarget is not null
                                          ? $$"""
                                              IEntityModelStore<{{rootTarget.IdType}}, {{rootTarget.ModelType}}> parent,
                                              {{rootTarget.IdType}} parentId,
                                              """
                                          : string.Empty
                                  )}}
                                  CancellationToken token = default)
                              {
                                  var map = new Dictionary<Type, IEntityModelStore<{{target.IdType}}, {{target.ModelType}}>>();
                                  
                                  {{
                                      string.Join(
                                          "\n",
                                          childStores.Select(x =>
                                              $$"""
                                                if(client.StateController.CanUseStoreType<{{x.GatewayEntitySymbol}}>())
                                                    map.Add(
                                                        typeof({{x.ModelType}}), 
                                                        {{(
                                                            rootTarget is not null
                                                                ? $"(await parent.GetSubStoreAsync<{x.IdType}, {x.ModelType}>(parentId, token)).CastUp(Template.Of<{target.ModelType}>())"
                                                                : $"(await client.CacheProvider.GetStoreAsync<{x.IdType}, {x.ModelType}>(token)).CastUp(Template.Of<{target.ModelType}>())"
                                                        )}}
                                                    );
                                                """
                                          )
                                      )
                                  }}
                                  
                                  return map.AsReadOnly();
                              }
                              """
                        )!
                    );
            }

            stores.Add(target, new StoreInfo(targetSyntax, target, childStores, parents, rootTarget));
        }

        foreach (var storeInfo in stores)
        {
            var parents = new List<string>();

            var parent = storeInfo.Value.ParentStoreTarget;
            var hasParent = parent is not null;

            var parentsPath = new List<StoreInfo>();

            while (parent is not null)
            {
                parentsPath.Add(stores[parent]);
                parent = stores[parent].ParentStoreTarget;
            }

            parentsPath.Reverse();
            string? lastParentName = null;

            foreach (var parentEntry in parentsPath)
            {
                var isOurParent = parentEntry.Target == storeInfo.Value.ParentStoreTarget;
                var parentName = isOurParent ? "parent" : ModelToStoreVariableName(parentEntry.Target.ModelType);
                var parentSimpleName = GetSimpleName(parentEntry.Target.ClassSymbol);

                var getCall =
                    $$"""
                      await {{parentSimpleName}}.GetStoreAsync(
                          client,
                          {{(
                              lastParentName is not null
                                  ? $"{lastParentName}, {lastParentName}Id,"
                                  : string.Empty
                          )}}
                          token
                      );
                      """;

                parents.Add(
                    $$"""
                      var {{parentName}} = {{getCall}}
                      var {{parentName}}Id = path.Require<{{parentEntry.Target.IdType}}, {{parentEntry.Target.CoreEntity}}>();
                      """
                );

                lastParentName = parentName;
            }

            if (hasParent)
            {
                parent = storeInfo.Value.ParentStoreTarget!;

                storeInfo.Value.Syntax = storeInfo.Value.Syntax
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public static async ValueTask<({{parent.IdType}} ParentId, IEntityModelStore<{{parent.IdType}}, {{parent.ModelType}}> ParentStore)> GetParentStoreAsync(
                                  DiscordGatewayClient client,
                                  IPathable path,
                                  CancellationToken token = default)
                              {
                                  {{
                                      string.Join("\n", parents)
                                  }}
                                  
                                  return (parentId, parent);
                              }
                              """
                        )!
                    );
            }

            storeInfo.Value.Syntax = storeInfo.Value.Syntax
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            $"IStoreInfoProvider<{storeInfo.Key.IdType}, {storeInfo.Key.ModelType}>"
                        )
                    ),
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            $"IStoreProvider<{storeInfo.Key.IdType}, {storeInfo.Key.ModelType}>"
                        )
                    )
                )
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static ValueTask<IStoreInfo<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>> GetStoreInfoAsync({{storeInfo.Key.ClassSymbol}} actor, CancellationToken token = default)
                              => GetStoreInfoAsync(actor.Client, actor.CachePath, token);
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static async ValueTask<IStoreInfo<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>> GetStoreInfoAsync(
                              DiscordGatewayClient client,
                              IPathable? path = null,
                              CancellationToken token = default)  
                          {
                              {{(
                                  hasParent
                                      ? "var (parentId, parent) = await GetParentStoreAsync(client, path ?? CachePathable.Empty, token);"
                                      : string.Empty
                              )}}
                              
                              var store = await GetStoreAsync(
                                  client, 
                                  {{(
                                      hasParent ? "parent, parentId," : string.Empty
                                  )}}
                                  token
                              );
                              
                              return IStoreInfo<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>.Create(
                                  store,
                                  {{(
                                      storeInfo.Value.Children.Length > 0
                                          ? $$"""
                                              await GetStoreHierarchyAsync(
                                                  client,
                                                  {{(
                                                      hasParent ? "parent, parentId," : string.Empty
                                                  )}}
                                                  token
                                              )
                                              """
                                          : $"System.Collections.Immutable.ImmutableDictionary<Type, IEntityModelStore<{storeInfo.Key.IdType}, {storeInfo.Key.ModelType}>>.Empty"
                                  )}}  
                              );
                          }
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $"static ValueTask<IStoreInfo<{storeInfo.Key.IdType}, {storeInfo.Key.ModelType}>> IStoreInfoProvider<{storeInfo.Key.IdType}, {storeInfo.Key.ModelType}>.GetStoreInfoAsync(DiscordGatewayClient client, IPathable? path, CancellationToken token) => GetStoreInfoAsync(client, path, token);"
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static async ValueTask<IEntityModelStore<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>> GetStoreAsync(DiscordGatewayClient client, IPathable? path = null, CancellationToken token = default)
                          {
                              {{(
                                  hasParent
                                      ? "var (parentId, parent) = await GetParentStoreAsync(client, path ?? CachePathable.Empty, token);"
                                      : string.Empty
                              )}}
                                
                              return await GetStoreAsync(
                                  client, 
                                  {{(
                                      hasParent ? "parent, parentId," : string.Empty
                                  )}}
                                  token
                              );
                          }
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          static ValueTask<IEntityModelStore<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>> IStoreProvider<{{storeInfo.Key.IdType}}, {{storeInfo.Key.ModelType}}>.GetStoreAsync(DiscordGatewayClient client, IPathable? path, CancellationToken token)
                              => GetStoreAsync(client, path, token);
                          """
                    )!
                );
        }

        foreach (var info in stores)
        {
            syntax = syntax.AddMembers(
                info.Value.Syntax
            );
        }
    }

    private static string ModelToStoreVariableName(ITypeSymbol model)
    {
        var name = model.Name.Remove(0, 1);
        return $"{char.ToLowerInvariant(name[0])}{name.Substring(1)}Store";
    }

    private static void PopulateBrokersMap(
        ref ClassDeclarationSyntax syntax,
        List<GenerationTarget> targets)
    {
        foreach (var target in targets)
        {
            var children = targets
                .Where(x =>
                    TypeUtils
                        .GetBaseTypes(x.ClassSymbol)
                        .Contains(target.ClassSymbol, SymbolEqualityComparer.Default)
                )
                .OrderByDescending(x => TypeUtils
                    .GetBaseTypes(x.ClassSymbol)
                    .TakeWhile(x => 
                        !x.Equals(target.ClassSymbol, SymbolEqualityComparer.Default)
                    )
                    .Count()
                )
                .ToArray();

            var name = GetSimpleName(target.ClassSymbol);

            var targetSyntax = CreateInternalClassSyntax(name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.SealedKeyword))
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            $"IBrokerInfoProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>"
                        )
                    ),
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            $"IBrokerProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>"
                        )
                    ),
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            $"IBrokerProvider<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>"
                        )
                    )
                )
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"""
                         public static async ValueTask<ConfiguredBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>> GetConfiguredBrokerAsync(DiscordGatewayClient client, IPathable? path = null, CancellationToken token = default)    
                             => new(await Stores.{name}.GetStoreInfoAsync(client, path, token), GetBroker(client));
                         """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $"""
                         public static IEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}> GetBroker(DiscordGatewayClient client)    
                             => client.StateController.GetBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ClassSymbol}, {target.ModelType}>();
                         """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static IBrokerInfo<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}> GetBrokerInfo(DiscordGatewayClient client)
                              => IBrokerInfo<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.Create(GetBroker(client), GetBrokerHierarchy(client));
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public static IReadOnlyDictionary<Type, IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>> GetBrokerHierarchy(DiscordGatewayClient client)    
                          {
                             return
                             {{(
                                 children.Length > 0
                                     ? $$"""
                                         new Dictionary<Type, IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>>()
                                         {
                                             {{
                                                 string.Join(
                                                     ",\n",
                                                     children.Select(x =>
                                                         $$"""
                                                           { typeof({{x.ModelType}}), (IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>){{GetSimpleName(x.ClassSymbol)}}.GetBroker(client) }
                                                           """
                                                     )
                                                 )
                                             }}
                                         }.AsReadOnly()
                                         """
                                     : $"System.Collections.Immutable.ImmutableDictionary<Type, IManageableEntityBroker<{target.IdType}, {target.GatewayEntitySymbol}, {target.ModelType}>>.Empty"
                             )}};
                          }
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          static IBrokerInfo<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}> IBrokerInfoProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.GetBrokerInfo(DiscordGatewayClient client)
                              => GetBrokerInfo(client);
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          static IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>.GetBroker(DiscordGatewayClient client)
                              => GetBroker(client);
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          static IEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ClassSymbol}}, {{target.ModelType}}>.GetBroker(DiscordGatewayClient client)
                              => GetBroker(client);
                          """
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          static IReadOnlyDictionary<Type, IManageableEntityBroker<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>> IBrokerProvider<{{target.IdType}}, {{target.GatewayEntitySymbol}}, {{target.ModelType}}>.GetBrokerHierarchy(DiscordGatewayClient client)
                              => GetBrokerHierarchy(client);
                          """
                    )!
                );

            syntax = syntax
                .AddMembers(
                    targetSyntax
                );
        }
    }

    private static string GetSimpleName(ITypeSymbol symbol)
    {
        return symbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty);
    }

    private static ClassDeclarationSyntax CreateInternalClassSyntax(string name)
    {
        return SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.InternalKeyword)
            ),
            SyntaxFactory.Identifier(name),
            null,
            null,
            [],
            []
        );
    }
}