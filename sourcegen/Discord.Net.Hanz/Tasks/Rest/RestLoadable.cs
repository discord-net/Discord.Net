using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

namespace Discord.Net.Hanz.Tasks;

public class RestLoadable : IGenerationCombineTask<RestLoadable.GenerationContext>
{
    public sealed class GenerationContext(
        SemanticModel semanticModel,
        ClassDeclarationSyntax syntax,
        INamedTypeSymbol classSymbol,
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
        public INamedTypeSymbol ClassSymbol { get; } = classSymbol;
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
                   CoreActor.Equals(other.CoreActor, SymbolEqualityComparer.Default) &&
                   IdType.Equals(other.IdType, SymbolEqualityComparer.Default) &&
                   CoreEntity.Equals(other.CoreEntity, SymbolEqualityComparer.Default) &&
                   ModelType.Equals(other.ModelType, SymbolEqualityComparer.Default) &&
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
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(CoreActor);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(CoreEntity);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(ModelType);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(RestEntitySymbol);
                hashCode = (hashCode * 397) ^ SymbolEqualityComparer.Default.GetHashCode(IdentityType);
                return hashCode;
            }
        }

        public static bool operator ==(GenerationContext? left, GenerationContext? right) => Equals(left, right);

        public static bool operator !=(GenerationContext? left, GenerationContext? right) => !Equals(left, right);
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
        => node is ClassDeclarationSyntax cls && cls.Identifier.ValueText.EndsWith("Actor");

    public static bool WillHaveFetchMethods(INamedTypeSymbol type)
    {
        if (!type.Name.StartsWith("Rest") || !type.Name.EndsWith("Actor"))
            return false;

        var hierarchy = Hierarchy.GetHierarchy(type);

        if (hierarchy.All(x => !LoadableTrait.IsLoadable(x.Type)))
            return false;

        return true;
    }

    public static INamedTypeSymbol? GetRestEntity(INamedTypeSymbol restActor, SemanticModel model)
    {
        return model.Compilation
            .GetSymbolsWithName(
                restActor.Name.Replace("Actor", string.Empty),
                SymbolFilter.Type
            )
            .FirstOrDefault() as INamedTypeSymbol;
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

        var modelType = Hierarchy.GetHierarchy(coreEntityType)
            .FirstOrDefault(x =>
                x.Type.ToDisplayString().StartsWith("Discord.IEntityOf")
            )
            .Type?.TypeArguments[0];

        if (modelType is null)
        {
            logger.Warn(
                $"Ignoring {classDeclarationSyntax.Identifier}: cannot resolve model type for {coreEntityType}");
            return null;
        }

        if (GetRestEntity(classSymbol, context.SemanticModel) is not { } restEntityType)
        {
            logger.Warn($"Ignoring {classDeclarationSyntax.Identifier}: cannot resolve rest entity type");
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
            classSymbol,
            restEntityType,
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

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var shouldOverride = TypeUtils.GetBaseTypes(target.ClassSymbol).Any(bases.Contains);
            var shouldBeVirtual = bases.Contains(target.ClassSymbol);

            var bestMatch = FindBestFetchMethod(target.CoreActor);

            if (!AddCachedValueFieldProperty(
                    ref syntax,
                    target.SemanticModel,
                    target.IdentityType,
                    target.RestEntitySymbol,
                    shouldBeVirtual,
                    shouldOverride,
                    targetLogger
                ))
            {
                continue;
            }

            if (!AddFetchMethod(
                    ref syntax,
                    target.RestEntitySymbol,
                    target.CoreActor,
                    bestMatch,
                    shouldOverride,
                    targetLogger
                ))
            {
                continue;
            }

            if (!AddFetchMethodOverrides(
                    ref syntax,
                    target.SemanticModel,
                    target.CoreActor,
                    target.RestEntitySymbol,
                    logger
                ))
            {
                continue;
            }

            context.AddSource(
                $"RestLoadables/{target.ClassSymbol.Name}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.ClassSymbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static IMethodSymbol? FindBestFetchMethod(INamedTypeSymbol coreActor)
    {
        return coreActor
            .GetMembers()
            .OfType<IMethodSymbol>()
            .Where(x => x.Name == "FetchAsync")
            .OrderByDescending(x => x.Parameters.Length)
            .FirstOrDefault();
    }

    public static IEnumerable<IMethodSymbol> GetFetchMethods(ITypeSymbol type)
    {
        return Hierarchy.GetHierarchy(type)
            .Select(x => x.Type)
            .Prepend(type)
            .SelectMany(x => x.GetMembers().OfType<IMethodSymbol>())
            .Where(x =>
                MemberUtils.GetMemberName(
                    x,
                    x => x.ExplicitInterfaceImplementations
                ) == "FetchAsync"
            )
            .OrderByDescending(x => x.Parameters.Length);
    }

    private static bool AddFetchMethodOverrides(
        ref ClassDeclarationSyntax syntax,
        SemanticModel semanticModel,
        INamedTypeSymbol coreActor,
        INamedTypeSymbol restEntity,
        Logger logger
    )
    {
        var addedOverloads = new HashSet<string>();

        foreach (var member in GetFetchMethods(coreActor))
        {
            if (!addedOverloads.Add(member.ContainingType.ToString()))
                continue;

            if (
                member.ReturnType is INamedTypeSymbol {Name: "ValueTask"} valueTask &&
                !semanticModel.Compilation.HasImplicitConversion(restEntity, valueTask.TypeArguments[0])
            ) continue;

            var parameterList = CreateParameterList(member, false);

            var memberSyntax = SyntaxFactory.ParseMemberDeclaration(
                $"async {member.ReturnType} {member.ContainingType}.FetchAsync{parameterList.NormalizeWhitespace()} => await this.FetchAsync({
                    string.Join(", ", parameterList.Parameters.Select(x => $"{x.Identifier.ValueText}: {x.Identifier.ValueText}"))
                });"
            );

            if (memberSyntax is null)
            {
                logger.Warn($"Failed to declare fetch override for {member}");
                return false;
            }

            syntax = syntax.AddMembers(memberSyntax);
        }

        return true;
    }

    private static bool AddFetchMethod(
        ref ClassDeclarationSyntax syntax,
        INamedTypeSymbol restEntityType,
        INamedTypeSymbol coreActor,
        IMethodSymbol? bestMatch,
        bool shouldOverride,
        Logger logger
    )
    {
        var modifier = shouldOverride ? " new" : string.Empty;

        var parameters = CreateParameterList(bestMatch);

        var extraArgs = parameters.Parameters.Count > 2
            ? ", " + string.Join(
                ", ",
                parameters.Parameters
                    .Take(parameters.Parameters.Count - 2)
                    .Select(x => x.Identifier.ValueText))
            : string.Empty;

        var method = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public{{modifier}} async ValueTask<{{restEntityType}}?> FetchAsync{{parameters.NormalizeWhitespace()}}
              {
                  if ((options?.AllowCached ?? true) && CachedValue is not null)
                      return CachedValue;

                  return _cachedValue = await {{coreActor}}.FetchInternalAsync(
                      Client,
                      this,
                      {{coreActor}}.FetchRoute(this, Id{{extraArgs}}),
                      options,
                      token
                  ) as {{restEntityType}};
              }
              """
        );

        if (method is null)
        {
            logger.Warn("Failed to declare fetch method");
            return false;
        }

        syntax = syntax.AddMembers(method);

        return true;
    }

    private static ParameterListSyntax CreateParameterList(IMethodSymbol? method, bool withDefaults = true)
    {
        return method is null
            ? SyntaxFactory.ParseParameterList(
                "(RequestOptions? options = null, CancellationToken token = default)"
            )
            : SyntaxFactory.ParameterList(
                SyntaxFactory.SeparatedList(
                    method.Parameters.Select(x =>
                        SyntaxFactory.Parameter(
                            [],
                            [],
                            SyntaxFactory.ParseTypeName(x.Type.ToDisplayString()),
                            SyntaxFactory.Identifier(x.Name),
                            x.HasExplicitDefaultValue && withDefaults
                                ? SyntaxFactory.EqualsValueClause(
                                    SyntaxUtils.CreateLiteral(x.Type, x.ExplicitDefaultValue)
                                )
                                : null
                        )
                    )
                )
            );
    }

    private static bool TryGetIdentityEntityType(ITypeSymbol identityType, out INamedTypeSymbol? entityType)
    {
        entityType = null;

        if (identityType is not INamedTypeSymbol namedIdentityType)
            return false;

        if (!namedIdentityType.ToDisplayString().StartsWith("Discord.IIdentifiable"))
            return false;

        if (namedIdentityType.TypeArguments.Length < 3)
            return false;

        return (entityType = namedIdentityType.TypeArguments[1] as INamedTypeSymbol) is not null;
    }

    private static bool AddCachedValueFieldProperty(
        ref ClassDeclarationSyntax syntax,
        SemanticModel semanticModel,
        ITypeSymbol identityType,
        INamedTypeSymbol restEntityType,
        bool shouldBeVirtual,
        bool shouldOverride,
        Logger logger)
    {
        var modifier = shouldOverride ? " override" : shouldBeVirtual ? " virtual" : string.Empty;

        var identityAccess = " ??= this.Identity.Entity";

        if (
            TryGetIdentityEntityType(identityType, out var identityEntityType) &&
            !identityEntityType!.Equals(restEntityType, SymbolEqualityComparer.Default)
        )
        {
            if (semanticModel.Compilation.HasImplicitConversion(restEntityType, identityEntityType))
                identityAccess += $" as {restEntityType}";
            else
                identityAccess = string.Empty;
        }

        var property = SyntaxFactory.ParseMemberDeclaration(
            $"public{modifier} {restEntityType.ToDisplayString()}? CachedValue => _cachedValue{identityAccess};"
        );
        var field = SyntaxFactory.ParseMemberDeclaration(
            $"private {restEntityType.ToDisplayString()}? _cachedValue;"
        );

        if (property is null)
        {
            logger.Warn("Failed to declare cached value property");
            return false;
        }

        if (field is null)
        {
            logger.Warn("Failed to declare cached value field");
            return false;
        }

        syntax = syntax.AddMembers(property, field);

        return true;
    }
}
