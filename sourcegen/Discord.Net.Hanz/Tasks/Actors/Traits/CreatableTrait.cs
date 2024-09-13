using System.Collections.Immutable;
using System.Linq.Expressions;
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
        AttributeData[] traitAttributes,
        SourceProductionContext context,
        Logger logger)
    {
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

        var generatedRoutes = new HashSet<IMethodSymbol>(SymbolEqualityComparer.Default);

        var methods = new List<MemberDeclarationSyntax>();

        foreach (var traitAttribute in traitAttributes)
        {
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

            if (generatedRoutes.Add(routeMethod))
            {
                switch (traitAttribute.AttributeClass!.Name)
                {
                    case "CreatableAttribute":
                        ProcessCreatable(ref syntax, target, creatableTarget, logger);
                        break;
                    case "ActorCreatableAttribute":
                        ProcessCreatableActor(ref syntax, target, creatableTarget, logger);
                        break;
                }
            }

            var linkTargets = traitAttribute.ConstructorArguments.Last().Values;

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
                var backlinkTargets = GetBackLinkTargets(
                    traitAttribute, 
                    target.SemanticModel.Compilation, 
                    logger
                );

                if (backlinkTargets is null)
                {
                    logger.Warn($"Unable to resolve backlink targets for {target.InterfaceSymbol}");
                    return;
                }

                foreach (var linkTarget in backlinkTargets)
                {
                    linkTarget.Formatted =
                        $"Discord.IBackLink<{linkTarget.LinkType}, {target.InterfaceSymbol}, {idType}, {entityType}, {modelType}>";

                    if (CreateLinkExtensionMethod(target, creatableTarget, linkTarget, traitAttribute, logger)
                        is not { } method)
                    {
                        logger.Warn($"Failed to create link extension method for {target.InterfaceSymbol}");
                        continue;
                    }

                    methods.Add(method);
                }
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

    private static List<BackLinkTarget>? GetBackLinkTargets(
        AttributeData traitAttribute,
        Compilation compilation,
        Logger logger)
    {
        var argIndex = traitAttribute.ConstructorArguments.Length - 1;

        if (traitAttribute.ApplicationSyntaxReference?.GetSyntax() is not AttributeSyntax attributeSyntax)
        {
            logger.Warn("Cannot resolve attribute syntax for backlink targets");
            return null;
        }

        if (attributeSyntax.ArgumentList is null)
            return null;

        if (attributeSyntax.ArgumentList.Arguments.Count <= argIndex)
            return [];

        var targets = attributeSyntax.ArgumentList.Arguments[argIndex].Expression switch
        {
            CollectionExpressionSyntax collection => collection.Elements
                .OfType<ExpressionElementSyntax>()
                .Select(x => x.Expression),
            ArrayCreationExpressionSyntax array => array.Initializer?.Expressions,
            _ => attributeSyntax.ArgumentList.Arguments.Skip(argIndex)
                .TakeWhile(x => x is {NameEquals: null, NameColon: null})
                .Select(x => x.Expression)
        };

        if (targets is null) return null;

        var results = new List<BackLinkTarget>();

        foreach (var target in targets)
        {
            if (target is not InvocationExpressionSyntax invocation)
                return null;

            var backlinkTarget = ResolveNameOfExpression(invocation, compilation, logger);

            if (backlinkTarget is null)
            {
                logger.Warn("No target info found for backlink target");
                return null;
            }

            results.Add(backlinkTarget);
        }

        return results;
    }

    private static BackLinkTarget? ResolveNameOfExpression(
        InvocationExpressionSyntax invocation,
        Compilation compilation,
        Logger logger)
    {
        if (invocation is not
            {
                Expression: IdentifierNameSyntax
                {
                    Identifier.ValueText: "nameof"
                },
                ArgumentList.Arguments.Count: 1
            })
            return null;

        if (invocation.ArgumentList.Arguments.Count != 1)
            return null;

        var typeName = invocation.ArgumentList.Arguments[0].Expression.ToString().Split(['.'], StringSplitOptions.RemoveEmptyEntries).Last();

        var candidates = compilation
            .GetSymbolsWithName(x => x.EndsWith(typeName))
            .OfType<INamedTypeSymbol>()
            .ToArray();

        if (candidates.Length == 0)
        {
            logger.Warn("No candidates found for backlink target");
            return null;
        }

        var candidate = candidates[0];

        return new BackLinkTarget(candidate);
    }

    private sealed class BackLinkTarget(ITypeSymbol linkType)
    {
        public ITypeSymbol LinkType { get; } = linkType;
        public string? Formatted { get; set; }
    }

    private static MemberDeclarationSyntax? CreateLinkExtensionMethod(
        EntityTraits.GenerationTarget target,
        Target creatableTarget,
        BackLinkTarget? backlinkTarget,
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

        if (backlinkTarget?.Formatted is not null)
        {
            linkType = $"{backlinkTarget.Formatted}";
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

        var methodName = "CreateAsync";

        if (
            traitAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == "MethodName")
                .Value.Value is string name
        ) methodName = name;

        return SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static async Task<{{extensionReturnType}}> {{methodName}}(
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