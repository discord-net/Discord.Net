using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Actors;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Traits;

public static class CreatableTrait
{
    private readonly struct Target(
        INamedTypeSymbol actorInterface,
        ITypeSymbol entityType,
        ITypeSymbol modelType,
        ITypeSymbol? apiParamsType,
        ITypeSymbol idType,
        INamedTypeSymbol? propertiesType,
        IMethodSymbol route,
        MemberAccessExpressionSyntax routeMemberAccess,
        bool isActorCreatable)
    {
        public INamedTypeSymbol ActorInterface { get; } = actorInterface;
        public ITypeSymbol EntityType { get; } = entityType;
        public ITypeSymbol ModelType { get; } = modelType;
        public ITypeSymbol? ApiParamsType { get; } = apiParamsType;
        public ITypeSymbol IdType { get; } = idType;
        public INamedTypeSymbol? PropertiesType { get; } = propertiesType;
        public IMethodSymbol Route { get; } = route;
        public MemberAccessExpressionSyntax RouteMemberAccess { get; } = routeMemberAccess;
        public bool IsActorCreatable { get; } = isActorCreatable;
    }

    public static void Process(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        AttributeData traitAttribute,
        SourceProductionContext context,
        Logger logger)
    {
        if (traitAttribute.ConstructorArguments.Length is < 2 or > 3)
        {
            logger.Warn($"{traitAttribute.ConstructorArguments.Length} attribute arguments specified");
            return;
        }

        var actorInterface = EntityTraits.GetCoreActorInterface(target.InterfaceSymbol);

        if (actorInterface is null)
        {
            logger.Warn($"Cannot find actor interface for {target.InterfaceSymbol}");
            return;
        }

        var entityType = actorInterface.TypeArguments[1];

        var modelType = EntityTraits.GetEntityModelOfInterface(entityType)?.TypeArguments[0];

        if (modelType is null)
        {
            logger.Warn($"Cannot find model interface for {target.InterfaceSymbol}");
            return;
        }

        if (EntityTraits.GetNameOfArgument(traitAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
        {
            logger.Warn($"Cannot find route nameof for creatable {target.InterfaceSymbol}");
            return;
        }

        var isActorCreatable = traitAttribute.AttributeClass?.Name is "ActorCreatableAttribute";
        var isCanonicalCreatable = traitAttribute.AttributeClass?.TypeArguments.Length == 0;

        logger.Log(
            $"{target.InterfaceSymbol}: {traitAttribute}: actor: {isActorCreatable}, canonical: {isCanonicalCreatable}");

        var hasRouteGenericParameters = !isCanonicalCreatable;

        var routeGenerics = traitAttribute.NamedArguments
            .FirstOrDefault(x => x.Key == "RouteGenerics")
            .Value is {Kind: TypedConstantKind.Array} specified
            ? specified.Values
            : ImmutableArray<TypedConstant>.Empty;

        var route = EntityTraits.GetRouteSymbol(
            routeMemberAccess,
            target.SemanticModel,
            routeGenerics.Length
        );

        if (route is not IMethodSymbol {ReturnType: INamedTypeSymbol returnType} routeMethod)
        {
            logger.Warn($"Cannot find route for creatable {target.InterfaceSymbol}");
            return;
        }

        if (hasRouteGenericParameters && routeGenerics.Length > 0)
        {
            var genericNames = routeGenerics
                .Select(x => x.Value!.ToString());

            routeMemberAccess = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                routeMemberAccess.Expression,
                SyntaxFactory.GenericName(
                    routeMemberAccess.Name.Identifier,
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList(
                            genericNames
                                .Select(x => SyntaxFactory.ParseTypeName(x))
                        )
                    )
                )
            );
        }

        var apiParams = isCanonicalCreatable ? null : returnType.TypeArguments[0];
        var idType = actorInterface.TypeArguments[0];
        var properties = isCanonicalCreatable
            ? null
            : (INamedTypeSymbol) traitAttribute.AttributeClass!.TypeArguments[0];

        var creatableTarget = new Target(
            actorInterface,
            entityType,
            modelType,
            apiParams,
            idType,
            properties,
            routeMethod,
            routeMemberAccess,
            isActorCreatable
        );

        switch (traitAttribute.AttributeClass!.Name)
        {
            case "CreatableAttribute":
                ProcessCreatable(ref syntax, target, creatableTarget, logger);
                break;
            case "ActorCreatableAttribute":
                ProcessCreatableActor(ref syntax, target, creatableTarget, logger);
                break;
        }

        var linkTargets = traitAttribute.ConstructorArguments.Last().Values;

        var methods = new List<MemberDeclarationSyntax>();

        if (linkTargets.Length == 0)
        {
            if (CreateLinkExtensionMethod(target, creatableTarget, null, traitAttribute, logger) is not { } method)
            {
                logger.Warn($"Failed to create link extension method for {target.InterfaceSymbol}");
                return;
            }

            methods.Add(method);
        }
        else
        {
            foreach (var linkTarget in linkTargets)
            {
                if (linkTarget.Value is not INamedTypeSymbol linkTargetType)
                {
                    logger.Warn($"Expected a type for link target entry, got '{linkTarget.Value}'");
                    continue;
                }

                if (CreateLinkExtensionMethod(target, creatableTarget, linkTargetType, traitAttribute, logger) is not
                    { } method)
                {
                    logger.Warn($"Failed to create link extension method for {target.InterfaceSymbol}");
                    continue;
                }

                methods.Add(method);
            }
        }

        if (methods.Count == 0)
            return;

        context.AddSource(
            $"Traits/Creatable{target.InterfaceSymbol.ToFullMetadataName()}",
            $$"""
              {{target.InterfaceDeclarationSyntax.GetFormattedUsingDirectives()}}

              namespace {{target.InterfaceSymbol.ContainingNamespace}};

              public static class Creatable{{target.InterfaceSymbol.Name}}Extensions
              {
                  {{
                      string.Join(
                          "\n        ",
                          methods.Select(x => x.NormalizeWhitespace().ToString())
                      )
                  }}
              }
              """
        );
    }

    private static MemberDeclarationSyntax? CreateLinkExtensionMethod(
        EntityTraits.GenerationTarget target,
        Target creatableTarget,
        INamedTypeSymbol? backlinkTarget,
        AttributeData traitAttribute,
        Logger logger)
    {
        var createProperties =
            creatableTarget.PropertiesType is null
                ? []
                : TypeUtils.GetBaseTypesAndThis(creatableTarget.PropertiesType)
                    .SelectMany(x => x.GetMembers().OfType<IPropertySymbol>())
                    .ToArray();

        var requiredProps = createProperties
            .Where(x => x.IsRequired)
            .ToArray();

        var notRequiredProps = createProperties
            .Where(x => !x.IsRequired)
            .ToArray();

        var linkType =
            $"ILink<{target.InterfaceSymbol}, {creatableTarget.IdType}, {creatableTarget.EntityType}, {creatableTarget.ModelType}>";

        if (backlinkTarget is not null)
        {
            var friendlyName = Links.GetFriendlyName(target.InterfaceSymbol);
            var backlinkProperty = Hierarchy.GetHierarchy(backlinkTarget)
                .Select(x => x.Type)
                .Prepend(backlinkTarget)
                .SelectMany(x => x.GetMembers())
                .OfType<IPropertySymbol>()
                .FirstOrDefault(x =>
                    x.Type.ToString().StartsWith($"{friendlyName}Link")
                );

            if (backlinkProperty is null)
            {
                logger.Warn($"No backlink found from {backlinkTarget} -> {target.InterfaceSymbol}");

                return null;
            }

            linkType = $"{backlinkProperty.Type}";

            if (!linkType.EndsWith($".BackLink<{backlinkTarget.ToDisplayString()}>"))
                linkType = $"{linkType}.BackLink<{backlinkTarget.ToDisplayString()}>";
        }

        var parameters = new List<string>()
        {
            $"this {linkType} link"
        };

        if (requiredProps.Length > 0)
        {
            parameters.AddRange(
                requiredProps.Select(x =>
                    $"{x.Type} {char.ToLowerInvariant(x.Name[0])}{x.Name.Remove(0, 1)}"
                )
            );
        }

        if (creatableTarget.PropertiesType is not null && notRequiredProps.Length > 0)
        {
            parameters.Add($"Action<{creatableTarget.PropertiesType}>? func = null");
        }

        parameters.AddRange([
            "RequestOptions? options = null",
            "CancellationToken token = default"
        ]);

        var ctor = creatableTarget.PropertiesType?.Constructors
            .FirstOrDefault(x => x.Parameters.Length == 1 && x.Parameters[0].Type.Name == "IPathable");

        var extensionReturnType = creatableTarget.IsActorCreatable
            ? target.InterfaceSymbol
            : creatableTarget.EntityType;

        List<string> flattened;

        return SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static async Task<{{extensionReturnType}}> CreateAsync(
                  {{string.Join(",\n".PadRight(10), parameters)}}      
              )
              {
                  {{(
                      creatableTarget.PropertiesType is not null
                          ? $$"""
                              var args = new {{creatableTarget.PropertiesType}}({{(ctor is not null ? "link" : string.Empty)}}){{(
                                  requiredProps.Length > 0
                                      ? $" {{ {string.Join(", ", requiredProps.Select(x => $"{x.Name} = {char.ToLowerInvariant(x.Name[0])}{x.Name.Remove(0, 1)}"))} }}"
                                      : string.Empty
                              )}};
                              {{(notRequiredProps.Length > 0 ? "func?.Invoke(args);" : string.Empty)}}
                              """
                          : string.Empty
                  )}}
              
                  var model = await link.Client.RestApiClient.ExecuteRequiredAsync(
                      {{target.InterfaceSymbol}}.CreateRoute(link{{(
                          creatableTarget.PropertiesType is not null
                              ? $$"""
                                  , args{{(!creatableTarget.IsActorCreatable ? ".ToApiModel()" : string.Empty)}}
                                  """
                              : string.Empty)}}),
                      options ?? link.Client.DefaultRequestOptions,
                      token
                  );
                  
                  {{(
                      creatableTarget.IsActorCreatable
                          ? $"return link.Specifically(model.{
                              string.Join(
                                  ".",
                                  (
                                      flattened = SyntaxUtils
                                          .FlattenMemberAccess(
                                              (MemberAccessExpressionSyntax) EntityTraits.GetNameOfArgument(
                                                  traitAttribute,
                                                  1
                                              )!
                                          )
                                          .ToList()
                                  )
                                  .Skip(
                                      flattened.IndexOf(((INamedTypeSymbol) creatableTarget.Route.ReturnType).TypeArguments[1].Name) + 1
                                  )
                              )
                          });"
                          : "return link.CreateEntity(model);"
                  )}}
              }
              """
        );
    }

    private static void ProcessCreatableActor(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        Target creatableTarget,
        Logger logger
    )
    {
        if (
            creatableTarget.Route.ReturnType is not
            INamedTypeSymbol {IsGenericType: true, TypeArguments.Length: 2} returnType
        ) return;

        var interfaceName =
            $"Discord.IActorCreatable<" +
            $"{target.InterfaceSymbol}, " +
            $"{creatableTarget.IdType}, " +
            $"{creatableTarget.PropertiesType}, " +
            $"{creatableTarget.ApiParamsType}, " +
            $"{returnType.TypeArguments[1]}>";

        var paramsHeuristic = creatableTarget.PropertiesType.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x
                .GetAttributes()
                .Any(x =>
                    x.AttributeClass?.Name == "IdHeuristicAttribute"
                )
            )
            .ToDictionary(
                x => x,
                x => x
                    .GetAttributes()
                    .Where(x => x.AttributeClass?.Name == "IdHeuristicAttribute")
                    .Select(x => x.AttributeClass!.TypeArguments[0])
                    .ToImmutableHashSet(SymbolEqualityComparer.Default),
                (IEqualityComparer<IPropertySymbol>) SymbolEqualityComparer.Default
            );

        var extraParameters = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);
        var routeInvocation = SyntaxFactory.InvocationExpression(
            creatableTarget.RouteMemberAccess,
            EntityTraits.ParseRouteArguments(
                creatableTarget.Route,
                target,
                logger,
                extra =>
                {
                    if (
                        extra.Type.Equals(
                            creatableTarget.ApiParamsType,
                            SymbolEqualityComparer.Default
                        )
                        ||
                        target.SemanticModel.Compilation
                            .ClassifyCommonConversion(
                                creatableTarget.ApiParamsType,
                                extra.Type
                            )
                            .Exists
                    )
                    {
                        return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args.ToApiModel()"));
                    }

                    if (!extra.IsOptional)
                        return null;

                    if (extra.DeclaringSyntaxReferences.Length == 0)
                        return null;

                    if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                        extraSyntax.Default is null)
                        return null;

                    extraParameters.Add(extra, extraSyntax);
                    return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(extraSyntax.Identifier));
                },
                heuristic: (_, type) =>
                {
                    var prop = paramsHeuristic
                        .FirstOrDefault(x => x.Value.Contains(type));

                    if (prop.Key is null)
                        return null;

                    return SyntaxFactory.Argument(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("args"),
                            SyntaxFactory.IdentifierName(prop.Key.Name)
                        )
                    );
                }
            )
        );

        var modifiers = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)
        );

        if (extraParameters.Count == 0)
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword));
        }

        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList([
                SyntaxFactory.Parameter(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName("IPathable"),
                    SyntaxFactory.Identifier("path"),
                    null
                ),
                SyntaxFactory.Parameter(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName(creatableTarget.PropertiesType.ToDisplayString()),
                    SyntaxFactory.Identifier("args"),
                    null
                ),
                ..extraParameters.Select(x =>
                    SyntaxFactory.Parameter(
                        [],
                        [],
                        x.Value.Type,
                        x.Value.Identifier,
                        x.Value.Default
                    )
                )
            ])
        );

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        interfaceName
                    )
                )
            )
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $"IApiInOutRoute<{creatableTarget.ApiParamsType}, {returnType.TypeArguments[1]}> CreateRoute{parameterList.NormalizeWhitespace()} => {routeInvocation.NormalizeWhitespace()};"
                )!.WithModifiers(modifiers),
                SyntaxFactory.ParseMemberDeclaration(
                    $"static IApiInOutRoute<{creatableTarget.ApiParamsType}, {returnType.TypeArguments[1]}> {interfaceName}.CreateRoute(IPathable path, {creatableTarget.PropertiesType} args) => CreateRoute(path, args);"
                )!
            );
    }

    private static void ProcessCreatable(
        ref InterfaceDeclarationSyntax syntax,
        EntityTraits.GenerationTarget target,
        Target creatableTarget,
        Logger logger)
    {
        var interfaceName =
            creatableTarget.PropertiesType is null
                ? "Discord.ICanonicallyCreatable<" +
                  $"{target.InterfaceSymbol}, " +
                  $"{creatableTarget.IdType}" +
                  ">"
                : $"Discord.ICreatable<" +
                  $"{target.InterfaceSymbol}, " +
                  $"{creatableTarget.EntityType}, " +
                  $"{creatableTarget.IdType}, " +
                  $"{creatableTarget.PropertiesType}, " +
                  $"{creatableTarget.ApiParamsType}, " +
                  $"{creatableTarget.ModelType}>";

        var extraParameters = new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);

        var routeInvocation = SyntaxFactory.InvocationExpression(
            creatableTarget.RouteMemberAccess,
            EntityTraits.ParseRouteArguments(creatableTarget.Route, target, logger, extra =>
            {
                if (
                    extra.Type.Equals(
                        creatableTarget.ApiParamsType,
                        SymbolEqualityComparer.Default
                    )
                    ||
                    (
                        creatableTarget.ApiParamsType is not null
                        &&
                        target.SemanticModel.Compilation
                            .ClassifyCommonConversion(
                                creatableTarget.ApiParamsType,
                                extra.Type
                            )
                            .Exists
                    )
                )
                {
                    return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                }

                if (extra.Type.TypeKind is TypeKind.TypeParameter && extra.Type.Name == "TArgs")
                {
                    return SyntaxFactory.Argument(SyntaxFactory.IdentifierName("args"));
                }

                if (!extra.IsOptional)
                    return null;

                if (extra.DeclaringSyntaxReferences.Length == 0)
                    return null;

                if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                    extraSyntax.Default is null)
                    return null;

                extraParameters.Add(extra, extraSyntax);
                return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(extraSyntax.Identifier));
            })
        );

        var modifiers = SyntaxFactory.TokenList(
            SyntaxFactory.Token(SyntaxKind.StaticKeyword)
        );

        if (extraParameters.Count == 0)
        {
            modifiers = modifiers.Add(SyntaxFactory.Token(SyntaxKind.NewKeyword));
        }

        var secondArgName = creatableTarget.ApiParamsType is not null
            ? "args"
            : "id";

        var parameterList = SyntaxFactory.ParameterList(
            SyntaxFactory.SeparatedList([
                SyntaxFactory.Parameter(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName("IPathable"),
                    SyntaxFactory.Identifier("path"),
                    null
                ),
                SyntaxFactory.Parameter(
                    [],
                    [],
                    SyntaxFactory.ParseTypeName((creatableTarget.ApiParamsType ?? creatableTarget.IdType)
                        .ToDisplayString()),
                    SyntaxFactory.Identifier(secondArgName),
                    null
                ),
                ..extraParameters.Select(x =>
                    SyntaxFactory.Parameter(
                        [],
                        [],
                        x.Value.Type,
                        x.Value.Identifier,
                        x.Value.Default
                    )
                )
            ])
        );

        var routeType = creatableTarget.ApiParamsType is not null
            ? $"IApiInOutRoute<{creatableTarget.ApiParamsType}, {creatableTarget.ModelType}>"
            : "IApiRoute";

        var secondArg = creatableTarget.ApiParamsType is not null
            ? $"{creatableTarget.ApiParamsType} args"
            : $"{creatableTarget.IdType} id";

        syntax = syntax
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(
                        interfaceName
                    )
                )
            )
            .AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $"{routeType} CreateRoute{parameterList.NormalizeWhitespace()} => {routeInvocation.NormalizeWhitespace()};"
                )!.WithModifiers(modifiers),
                SyntaxFactory.ParseMemberDeclaration(
                    $"static {routeType} {interfaceName}.CreateRoute(IPathable path, {secondArg}) => CreateRoute(path, {secondArgName});"
                )!
            );
    }

    private static bool IsCreatableTarget(ITypeSymbol symbol)
    {
        if (symbol.GetAttributes()
            .Any(x => x.AttributeClass?.ToDisplayString() == "Discord.CreatableAttribute"))
            return true;

        return symbol.AllInterfaces.Any(x => x.ToDisplayString().StartsWith("Discord.ICreatable"));
    }
}