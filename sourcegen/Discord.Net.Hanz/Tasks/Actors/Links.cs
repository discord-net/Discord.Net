using System.Collections.Immutable;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;

namespace Discord.Net.Hanz.Tasks.Actors;

public class Links : IGenerationCombineTask<Links.TargetCollection>
{
    public sealed class TargetCollection : IEquatable<TargetCollection>
    {
        public HashSet<GenerationTarget> Targets { get; }

        public TargetCollection(HashSet<GenerationTarget> targets)
        {
            Targets = targets;
        }

        public bool Equals(TargetCollection other)
            => Targets.SequenceEqual(other.Targets);

        public override int GetHashCode()
            => Targets.GetHashCode();
    }

    public abstract class GenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel Semantic { get; } = semantic;
        public INamedTypeSymbol Symbol { get; } = symbol;
        public TypeDeclarationSyntax Syntax { get; } = syntax;

        public virtual bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Symbol.Equals(other.Symbol, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GenerationTarget) obj);
        }

        public override int GetHashCode()
        {
            return SymbolEqualityComparer.Default.GetHashCode(Symbol);
        }

        public static bool operator ==(GenerationTarget? left, GenerationTarget? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GenerationTarget? left, GenerationTarget? right)
        {
            return !Equals(left, right);
        }
    }

    public sealed class BackLinkPropertiesToTypeGenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        HashSet<IPropertySymbol> properties
    ) : GenerationTarget(semantic, symbol, syntax)
    {
        public HashSet<IPropertySymbol> Properties { get; } = properties;
    }

    public sealed class VertexAppliedMethodGenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        IMethodSymbol methodSymbol,
        MethodDeclarationSyntax methodSyntax,
        List<AttributeData> attributes,
        INamedTypeSymbol entity,
        ITypeSymbol id,
        INamedTypeSymbol model
    ) : GenerationTarget(semantic, symbol, syntax)
    {
        public IMethodSymbol MethodSymbol { get; } = methodSymbol;
        public MethodDeclarationSyntax MethodSyntax { get; } = methodSyntax;
        public List<AttributeData> Attributes { get; } = attributes;
        public INamedTypeSymbol Entity { get; } = entity;
        public ITypeSymbol Id { get; } = id;
        public INamedTypeSymbol Model { get; } = model;

        public override bool Equals(GenerationTarget? other)
            => base.Equals(other) &&
               other is VertexAppliedMethodGenerationTarget otherVertex &&
               SymbolEqualityComparer.Default.Equals(MethodSymbol, otherVertex.MethodSymbol) &&
               Attributes.SequenceEqual(otherVertex.Attributes);

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^
                   (Attributes.GetHashCode() * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(MethodSymbol) * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Entity) * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Id) * 397) ^
                   SymbolEqualityComparer.Default.GetHashCode(Model);
        }
    }

    public sealed class BackLinkMethodGenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        IMethodSymbol methodSymbol,
        MethodDeclarationSyntax methodSyntax,
        Dictionary<INamedTypeSymbol, string> backlinkTargets,
        INamedTypeSymbol entity,
        ITypeSymbol id,
        INamedTypeSymbol model
    ) : GenerationTarget(semantic, symbol, syntax)
    {
        public IMethodSymbol MethodSymbol { get; } = methodSymbol;
        public MethodDeclarationSyntax MethodSyntax { get; } = methodSyntax;
        public Dictionary<INamedTypeSymbol, string> BacklinkTargets { get; } = backlinkTargets;
        public INamedTypeSymbol Entity { get; } = entity;
        public ITypeSymbol Id { get; } = id;
        public INamedTypeSymbol Model { get; } = model;

        public override bool Equals(GenerationTarget? other)
            => base.Equals(other) &&
               other is BackLinkMethodGenerationTarget otherBackLink &&
               SymbolEqualityComparer.Default.Equals(MethodSymbol, otherBackLink.MethodSymbol) &&
               BacklinkTargets.Count == otherBackLink.BacklinkTargets.Count &&
               BacklinkTargets.All(x =>
                   otherBackLink.BacklinkTargets.ContainsKey(x.Key) &&
                   otherBackLink.BacklinkTargets[x.Key] == x.Value);

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^
                   (BacklinkTargets.GetHashCode() * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(MethodSymbol) * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Entity) * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Id) * 397) ^
                   SymbolEqualityComparer.Default.GetHashCode(Model);
        }
    }

    public sealed class BackLinkableTypeGenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol symbol,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol? linkType = null,
        INamedTypeSymbol? actorType = null
    ) : GenerationTarget(semantic, symbol, syntax)
    {
        public INamedTypeSymbol? LinkType { get; } = linkType;
        public INamedTypeSymbol? ActorType { get; set; } = actorType;

        public override bool Equals(GenerationTarget? other)
            => base.Equals(other) &&
               other is BackLinkableTypeGenerationTarget otherBackLink &&
               SymbolEqualityComparer.Default.Equals(LinkType, otherBackLink.LinkType) &&
               SymbolEqualityComparer.Default.Equals(ActorType, otherBackLink.ActorType);

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(LinkType) * 397) ^
                   SymbolEqualityComparer.Default.GetHashCode(ActorType);
        }
    }

    public sealed class ActorGenerationTarget(
        SemanticModel semantic,
        INamedTypeSymbol actor,
        INamedTypeSymbol entity,
        ITypeSymbol id,
        INamedTypeSymbol model,
        TypeDeclarationSyntax syntax
    ) : GenerationTarget(semantic, actor, syntax)
    {
        public INamedTypeSymbol Entity { get; } = entity;
        public ITypeSymbol Id { get; } = id;
        public INamedTypeSymbol Model { get; } = model;

        public override bool Equals(GenerationTarget? other)
            => base.Equals(other) && other is ActorGenerationTarget otherActor &&
               SymbolEqualityComparer.Default.Equals(Entity, otherActor.Entity) &&
               SymbolEqualityComparer.Default.Equals(Id, otherActor.Id) &&
               SymbolEqualityComparer.Default.Equals(Model, otherActor.Model);

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Entity) * 397) ^
                   (SymbolEqualityComparer.Default.GetHashCode(Id) * 397) ^
                   SymbolEqualityComparer.Default.GetHashCode(Model);
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax or MethodDeclarationSyntax;
    }

    public TargetCollection? GetTargetForGeneration(GeneratorSyntaxContext context, Logger logger,
        CancellationToken token = default)
    {
        switch (context.Node)
        {
            case TypeDeclarationSyntax typeSyntax:
                if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, typeSyntax, token) is not INamedTypeSymbol
                    symbol) return null;

                var set = new HashSet<GenerationTarget>();

                if (GetBackLinkableTypeGenerationTarget(context, typeSyntax, symbol) is { } backlink)
                    set.Add(backlink);

                if (GetActorGenerationTarget(context, typeSyntax, symbol) is { } actor)
                    set.Add(actor);

                if (GetBackLinkPropertyToTypeGenerationTarget(context, typeSyntax, symbol) is { } backlinkProperties)
                    set.Add(backlinkProperties);

                return set.Count > 0
                    ? new(set)
                    : null;
            case MethodDeclarationSyntax {Parent: TypeDeclarationSyntax typeSyntax} methodSyntax:

                if (
                    ModelExtensions.GetDeclaredSymbol(
                        context.SemanticModel,
                        methodSyntax,
                        token
                    )
                    is not IMethodSymbol method
                ) return null;

                set = new HashSet<GenerationTarget>();

                if (GetBackLinkMethodGenerationTarget(context, methodSyntax, typeSyntax, method) is { } backlinkMethod)
                    set.Add(backlinkMethod);

                if (GetVertexMethodGenerationTarget(context, methodSyntax, typeSyntax, method) is { } vertexMethod)
                    set.Add(vertexMethod);

                return set.Count > 0
                    ? new(set)
                    : null;

            default:
                return null;
        }
    }

    private static AttributeData[] GetBackLinkAttributes(ITypeSymbol symbol)
        => symbol
            .GetAttributes()
            .Where(x =>
                x.AttributeClass?
                    .ToDisplayString() == "Discord.BackLinkAttribute" &&
                !x.AttributeClass.IsGenericType
            )
            .ToArray();

    private static BackLinkPropertiesToTypeGenerationTarget? GetBackLinkPropertyToTypeGenerationTarget(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeSyntax,
        INamedTypeSymbol symbol)
    {
        var attributes = GetBackLinkAttributes(symbol);

        if (attributes.Length == 0) return null;

        var properties = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

        var typeProperties = Hierarchy.GetHierarchy(symbol)
            .SelectMany(x => x.Type.GetMembers())
            .OfType<IPropertySymbol>()
            .Distinct<IPropertySymbol>(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var attribute in attributes)
        {
            if (attribute.ConstructorArguments.FirstOrDefault().Value is not string name)
                continue;

            var property = typeProperties.FirstOrDefault(x => x.Name == name);

            if (property is not null)
                properties.Add(property);
        }

        if (properties.Count == 0) return null;

        return new BackLinkPropertiesToTypeGenerationTarget(
            context.SemanticModel,
            symbol,
            typeSyntax,
            properties
        );
    }

    private static VertexAppliedMethodGenerationTarget? GetVertexMethodGenerationTarget(
        GeneratorSyntaxContext context,
        MethodDeclarationSyntax methodSyntax,
        TypeDeclarationSyntax typeSyntax,
        IMethodSymbol symbol)
    {
        if (symbol.Parameters.Length == 0) return null;

        if (!symbol.IsStatic) return null;

        var vertexAttributes = symbol.GetAttributes()
            .Where(x => x.AttributeClass?.Name == "OnVertexAttribute")
            .ToList();

        if (vertexAttributes.Count == 0) return null;

        if (EntityTraits.GetCoreActorInterface(symbol.ContainingType) is not { } actorInterface) return null;

        // skip entities
        if (EntityTraits.GetEntityModelOfInterface(symbol.ContainingType) is not null) return null;

        if (actorInterface.TypeArguments[1] is not INamedTypeSymbol entity) return null;

        if (EntityTraits.GetEntityModelOfInterface(entity) is not { } entityOf) return null;

        if (entityOf.TypeArguments[0] is not INamedTypeSymbol model) return null;

        return new(
            context.SemanticModel,
            symbol.ContainingType,
            typeSyntax,
            symbol,
            methodSyntax,
            vertexAttributes,
            entity,
            actorInterface.TypeArguments[0],
            model
        );
    }

    private static BackLinkMethodGenerationTarget? GetBackLinkMethodGenerationTarget(
        GeneratorSyntaxContext context,
        MethodDeclarationSyntax methodSyntax,
        TypeDeclarationSyntax typeSyntax,
        IMethodSymbol symbol)
    {
        if (symbol.Parameters.Length == 0) return null;

        if (!symbol.IsStatic) return null;

        var backlinkAttributes = symbol.GetAttributes()
            .Where(x => x.AttributeClass?.Name == "BackLinkAttribute")
            .ToDictionary(
                INamedTypeSymbol (x) => (INamedTypeSymbol) x.AttributeClass!.TypeArguments[0],
                x => x.ConstructorArguments.FirstOrDefault().Value as string ?? symbol.Name,
                SymbolEqualityComparer.Default
            );

        if (backlinkAttributes.Count == 0) return null;

        if (EntityTraits.GetCoreActorInterface(symbol.ContainingType) is not { } actorInterface) return null;

        // skip entities
        if (EntityTraits.GetEntityModelOfInterface(symbol.ContainingType) is not null) return null;

        if (actorInterface.TypeArguments[1] is not INamedTypeSymbol entity) return null;

        if (EntityTraits.GetEntityModelOfInterface(entity) is not { } entityOf) return null;

        if (entityOf.TypeArguments[0] is not INamedTypeSymbol model) return null;

        return new(
            context.SemanticModel,
            symbol.ContainingType,
            typeSyntax,
            symbol,
            methodSyntax,
            backlinkAttributes,
            entity,
            actorInterface.TypeArguments[0],
            model
        );
    }

    public static bool HasBackLinkableAttribute(ITypeSymbol symbol)
        => symbol
            .GetAttributes()
            .Any(x => x
                .AttributeClass?
                .ToDisplayString() == "Discord.BackLinkableAttribute"
            );

    private static INamedTypeSymbol? GetActorType(ITypeSymbol symbol)
    {
        return Hierarchy.GetHierarchy(symbol)
            .FirstOrDefault(x =>
                x.Type.ToDisplayString().StartsWith("Discord.Rest.RestActor<") ||
                x.Type.ToDisplayString().StartsWith("Discord.Rest.IRestActor<") ||
                x.Type.ToDisplayString().StartsWith("Discord.Rest.IRestTrait<") ||
                x.Type.ToDisplayString().StartsWith("Discord.IActor<") ||
                x.Type.ToDisplayString().StartsWith("Discord.IActorTrait<")
            ).Type;
    }

    private static BackLinkableTypeGenerationTarget? GetBackLinkableTypeGenerationTarget(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeSyntax,
        INamedTypeSymbol symbol)
    {
        if (!HasBackLinkableAttribute(symbol)) return null;

        if (typeSyntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1) return null;

        var linkType =
            symbol
                .AllInterfaces
                .FirstOrDefault(x => x.Name is "ILink")
            ??
            symbol
                .AllInterfaces
                .FirstOrDefault(x =>
                    x.Name is "Indexable" or "Enumerable" or "Defined" or "Paged"
                );

        if (linkType is not null)
            return new(context.SemanticModel, symbol, typeSyntax, linkType);

        var actorType = GetActorType(symbol);

        if (actorType is null)
            return null;

        return new(context.SemanticModel, symbol, typeSyntax, actorType: actorType);
    }

    private static ActorGenerationTarget? GetActorGenerationTarget(
        GeneratorSyntaxContext context,
        TypeDeclarationSyntax typeSyntax,
        INamedTypeSymbol symbol)
    {
        var actorInterface = GetActorType(symbol);

        if (actorInterface is null) return null;

        // skip entities
        if (EntityTraits.GetEntityModelOfInterface(symbol) is not null) return null;

        if (actorInterface.TypeArguments[1] is not INamedTypeSymbol entity) return null;

        if (EntityTraits.GetEntityModelOfInterface(entity) is not { } entityOf) return null;

        if (entityOf.TypeArguments[0] is not INamedTypeSymbol model) return null;

        return new ActorGenerationTarget(
            context.SemanticModel,
            symbol,
            entity,
            actorInterface.TypeArguments[0],
            model,
            typeSyntax
        );
    }

    public void Execute(SourceProductionContext context, ImmutableArray<TargetCollection?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var flattened = targets
            .SelectMany(x => x?.Targets ?? [])
            .ToList();

        if (flattened.Count == 0) return;

        var actors = flattened.OfType<ActorGenerationTarget>().ToList();
        var backlinkTypes = flattened.OfType<BackLinkableTypeGenerationTarget>().ToList();
        var backlinkMethods = flattened.OfType<BackLinkMethodGenerationTarget>().ToList();
        var vertexMethods = flattened.OfType<VertexAppliedMethodGenerationTarget>().ToList();
        var backlinkProperties = flattened.OfType<BackLinkPropertiesToTypeGenerationTarget>().ToList();

        try
        {
            if (backlinkTypes.Count > 0)
                ExecuteBackLinkTypes(context, backlinkTypes, logger);

            if (actors.Count > 0)
                ExecuteActors(context, actors, logger);

            if (backlinkMethods.Count > 0)
                ExecuteBackLinkMethods(context, backlinkMethods, logger);

            if (vertexMethods.Count > 0)
                ExecuteVertexMethods(context, vertexMethods, logger);

            if (backlinkProperties.Count > 0)
                ExecuteBackLinkToProperties(context, backlinkProperties, logger);
        }
        catch (Exception x)
        {
            logger.Log(LogLevel.Error, x.ToString());
            throw;
        }
    }

    private static void ExecuteBackLinkToProperties(
        SourceProductionContext context,
        List<BackLinkPropertiesToTypeGenerationTarget> targets,
        Logger logger)
    {
        foreach (var group in targets
                     .GroupBy(
                         x => x.Symbol,
                         (IEqualityComparer<INamedTypeSymbol>) SymbolEqualityComparer.Default)
                )
        {
            TypeDeclarationSyntax? syntax = null;

            foreach (var target in group)
            {
                syntax ??= SyntaxUtils.CreateSourceGenClone(target.Syntax);

                var targetLogger = logger.WithSemanticContext(target.Semantic).GetSubLogger("BackLinkProperties");

                targetLogger.Log($"Processing {target.Symbol}");

                foreach (var property in target.Properties)
                {
                    targetLogger.Log($" - {property.Type} {property.Name} ({property.ContainingType})");

                    if (property.ContainingType.Equals(target.Symbol, SymbolEqualityComparer.Default))
                        continue;

                    if (property.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is not
                        PropertyDeclarationSyntax propertySyntax)
                        continue;

                    if (propertySyntax.Modifiers.IndexOf(SyntaxKind.NewKeyword) != -1)
                    {
                        propertySyntax = propertySyntax.WithModifiers(
                            propertySyntax.Modifiers.RemoveAt(propertySyntax.Modifiers.IndexOf(SyntaxKind.NewKeyword))
                        );
                    }

                    syntax = syntax
                        .AddMembers(
                            propertySyntax
                                .WithType(
                                    SyntaxFactory.ParseTypeName($"{property.Type}.BackLink<{target.Symbol}>")
                                )
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword)),
                            propertySyntax
                                .WithType(
                                    SyntaxFactory.ParseTypeName($"{property.Type}")
                                )
                                .WithExplicitInterfaceSpecifier(
                                    SyntaxFactory.ExplicitInterfaceSpecifier(
                                        SyntaxFactory.IdentifierName(property.ContainingType.ToDisplayString())
                                    )
                                )
                                .WithAccessorList(null)
                                .WithInitializer(null)
                                .WithExpressionBody(
                                    SyntaxFactory.ArrowExpressionClause(
                                        SyntaxFactory.IdentifierName(property.Name)
                                    )
                                )
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        );
                }
            }

            if (syntax is null) continue;

            context.AddSource(
                $"BackLinkProperties/{group.Key.ToFullMetadataName()}",
                $$"""
                  {{group.First().Syntax.GetFormattedUsingDirectives()}}

                  namespace {{group.Key.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static void ExecuteVertexMethods(
        SourceProductionContext context,
        List<VertexAppliedMethodGenerationTarget> targets,
        Logger logger)
    {
        foreach (var group in targets
                     .GroupBy(
                         x => x.Symbol,
                         (IEqualityComparer<INamedTypeSymbol>) SymbolEqualityComparer.Default)
                )
        {
            var syntax = SyntaxFactory.ClassDeclaration(
                [],
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                ),
                SyntaxFactory.Identifier($"{GetFriendlyName(group.Key)}VertexExtensions"),
                null,
                null,
                [],
                []
            );

            foreach (var target in group)
            {
                var targetLogger = logger.WithSemanticContext(target.Semantic).GetSubLogger("VertexMethods");

                targetLogger.Log($"Processing {target.MethodSymbol}");

                if (target.MethodSyntax is {ExpressionBody: null, Body: null})
                {
                    targetLogger.Warn($"{target.MethodSymbol}: No method body");
                    continue;
                }

                var source = target.MethodSymbol.Parameters[0];

                foreach (var vertexAttribute in target.Attributes)
                {
                    if (vertexAttribute.AttributeClass?.TypeArguments.FirstOrDefault() is not { } constraint)
                        continue;

                    if (target.Semantic.Compilation.HasImplicitConversion(constraint, source.Type)) continue;

                    targetLogger.Warn(
                        $"{target.Symbol}: Unable to declare backlink method on {constraint}, not assignable to {source}");
                    goto end;
                }

                var carriedModifiers = SyntaxFactory.TokenList();

                if (target.MethodSyntax.Modifiers.IndexOf(SyntaxKind.AsyncKeyword) != -1)
                    carriedModifiers = carriedModifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

                var methodBody = (SyntaxNode?) target.MethodSyntax.ExpressionBody ?? target.MethodSyntax.Body!;

                foreach (var attribute in target.Attributes)
                {
                    var methodName = attribute.ConstructorArguments.FirstOrDefault().Value as string ??
                                     target.MethodSymbol.Name;

                    var targetType = attribute.AttributeClass?.TypeArguments.FirstOrDefault() ?? source.Type;

                    var parameters = new List<string>()
                        {$"this {targetType} {source.Name}"};

                    foreach (var parameter in target.MethodSyntax.ParameterList.Parameters.Skip(1))
                        parameters.Add(parameter.ToString());

                    syntax = syntax.AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public static {{target.MethodSymbol.ReturnType}} {{methodName}}(
                                  {{string.Join(", ", parameters)}}
                              )
                              {{methodBody}}
                              """
                        )!.AddModifiers(carriedModifiers.ToArray())
                    );
                }

                end: ;
            }

            context.AddSource(
                $"VertexMethods/{group.Key.ToFullMetadataName()}",
                $$"""
                  {{group.First().MethodSyntax.GetFormattedUsingDirectives()}}

                  namespace {{group.Key.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static void ExecuteBackLinkMethods(
        SourceProductionContext context,
        List<BackLinkMethodGenerationTarget> targets,
        Logger logger
    )
    {
        foreach (var group in targets
                     .GroupBy(
                         x => x.Symbol,
                         (IEqualityComparer<INamedTypeSymbol>) SymbolEqualityComparer.Default)
                )
        {
            var syntax = SyntaxFactory.ClassDeclaration(
                [],
                SyntaxFactory.TokenList(
                    SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword)
                ),
                SyntaxFactory.Identifier($"{GetFriendlyName(group.Key)}BacklinkExtensions"),
                null,
                null,
                [],
                []
            );

            foreach (var target in group)
            {
                var targetLogger = logger.WithSemanticContext(target.Semantic).GetSubLogger("BackLinkMethods");

                targetLogger.Log($"Processing {target.MethodSymbol}");

                if (target.MethodSyntax is {ExpressionBody: null, Body: null})
                {
                    targetLogger.Warn($"{target.MethodSymbol}: No method body");
                    continue;
                }

                var source = target.MethodSymbol.Parameters[0];

                foreach (var backlinkTarget in target.BacklinkTargets)
                {
                    if (target.Semantic.Compilation.HasImplicitConversion(backlinkTarget.Key, source.Type)) continue;

                    targetLogger.Warn(
                        $"{target.Symbol}: Unable to declare backlink method on {backlinkTarget}, not assignable to {source}");
                    goto end;
                }

                var carriedModifiers = SyntaxFactory.TokenList();

                if (target.MethodSyntax.Modifiers.IndexOf(SyntaxKind.AsyncKeyword) != -1)
                    carriedModifiers = carriedModifiers.Add(SyntaxFactory.Token(SyntaxKind.AsyncKeyword));

                foreach (var backlinkTarget in target.BacklinkTargets)
                {
                    var backlinkType =
                        $"IBackLink<{backlinkTarget.Key}, {target.Symbol}, {target.Id}, {target.Entity}, {target.Model}>";

                    var parameters = new List<string>()
                        {$"this {backlinkType} __link__"};

                    foreach (var parameter in target.MethodSyntax.ParameterList.Parameters.Skip(1))
                        parameters.Add(parameter.ToString());

                    var methodBody = (SyntaxNode?) target.MethodSyntax.ExpressionBody ?? target.MethodSyntax.Body!;

                    methodBody = methodBody.ReplaceNodes(
                        methodBody.DescendantNodes()
                            .OfType<IdentifierNameSyntax>()
                            .Where(x =>
                                x.Identifier.ValueText == target.MethodSymbol.Parameters[0].Name &&
                                target.Semantic.GetOperation(x) is IParameterReferenceOperation
                            ),
                        (node, _) => SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("__link__"),
                            SyntaxFactory.IdentifierName("Source")
                        )
                    );

                    syntax = syntax.AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public static {{target.MethodSymbol.ReturnType}} {{backlinkTarget.Value}}(
                                  {{string.Join(", ", parameters)}}
                              )
                              {{methodBody}}
                              """
                        )!.AddModifiers(carriedModifiers.ToArray())
                    );
                }

                end: ;
            }

            context.AddSource(
                $"BackLinkMethods/{group.Key.ToFullMetadataName()}",
                $$"""
                  {{group.First().MethodSyntax.GetFormattedUsingDirectives()}}

                  namespace {{group.Key.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static void ExecuteBackLinkTypes(
        SourceProductionContext context,
        List<BackLinkableTypeGenerationTarget> targets,
        Logger logger)
    {
        var generated = new HashSet<string>();

        foreach (var target in targets)
        {
            var targetLogger = logger.WithSemanticContext(target.Semantic).GetSubLogger("BackLinkTypes");

            targetLogger.Log($"Processing {target.Symbol}");

            var syntax = SyntaxUtils.CreateSourceGenClone(target.Syntax);

            var typeArguments = target switch
            {
                {LinkType: not null} => target.LinkType.TypeArguments.Select(x => x.Name).ToList(),
                {ActorType: not null} =>
                [
                    target.Symbol.Name,
                    target.ActorType.TypeArguments[0].Name,
                    target.ActorType.TypeArguments[1].Name,
                    EntityTraits.GetEntityModelOfInterface(target.ActorType.TypeArguments[1])!.TypeArguments[0].Name
                ],
                _ => []
            };

            var isInheritingFriendlyLink = false;

            if (typeArguments.Count == 0 && target.LinkType is not null)
            {
                var linkTypeName = target.LinkType.ToString();

                if (linkTypeName.Contains("Link"))
                    linkTypeName = linkTypeName
                        .Split(['.'], StringSplitOptions.RemoveEmptyEntries)
                        .First(x => x.Contains("Link"));

                if (
                    GetActorFromFriendlyName(linkTypeName, target.Semantic.Compilation) is { } actor &&
                    GetActorType(actor) is { } actorInterface)
                {
                    typeArguments =
                    [
                        actor.Name,
                        actorInterface.TypeArguments[0].Name,
                        actorInterface.TypeArguments[1].Name,
                        EntityTraits.GetEntityModelOfInterface(actorInterface.TypeArguments[1])!.TypeArguments[0].Name
                    ];

                    targetLogger.Log($"Resolved {actor} : {actorInterface}");
                    isInheritingFriendlyLink = true;
                }
            }

            if (typeArguments.Count == 0)
            {
                targetLogger.Warn($"Unable to determine type arguments for backlink to {target.Symbol}");
                continue;
            }

            var formattedTypeArguments = string.Join(
                ", ",
                typeArguments
            );

            MemberDeclarationSyntax? backlinkSyntax = null;

            switch (target.Symbol.TypeKind)
            {
                case TypeKind.Interface:
                    var backlinkBaseType = target.ActorType is not null
                        ? $"Discord.IBackLinkedActor<TSource, BackLink<TSource>, {formattedTypeArguments}>"
                        : $"Discord.IBackLink<TSource, {formattedTypeArguments}>";

                    List<string> members = [];
                    List<string> bases = [target.Symbol.ToDisplayString(), backlinkBaseType];

                    var hierarchy = Hierarchy.GetHierarchy(target.Symbol)
                        .Select(x => x.Type)
                        .Where(HasBackLinkableAttribute)
                        .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                        .ToArray();

                    var parentBackLinks = target.ActorType is not null
                        ? hierarchy
                            .Where(x => GetActorType(x) is not null)
                            .Select(KeyValuePair<INamedTypeSymbol, string>? (x) =>
                            {
                                var actorType = GetActorType(x);
                                if (actorType is null) return null;

                                var modelType = EntityTraits.GetEntityModelOfInterface(actorType.TypeArguments[1])
                                    ?.TypeArguments[0];
                                if (modelType is null) return null;

                                return new(
                                    x,
                                    $"{x}, {actorType.TypeArguments[0]}, {actorType.TypeArguments[1]}, {modelType}"
                                );
                            })
                            .Where(x => x.HasValue)
                            .ToDictionary(
                                x => x.Value.Key,
                                x => x.Value.Value,
                                SymbolEqualityComparer.Default
                            )
                        : [];


                    if (parentBackLinks.Count > 0)
                    {
                        members.AddRange([
                            "new TSource Source { get; }",
                            $"TSource IBackLink<TSource, {formattedTypeArguments}>.Source => Source;"
                        ]);
                    }

                    if (target.LinkType is not null && target.Symbol.Name is not "ILinkType")
                    {
                        bases.Add($"ILinkType<{formattedTypeArguments}>.BackLink<TSource>");

                        foreach (var extra in hierarchy
                                     .Where(x => x.Name is not "ILinkType" and not "ISpecifiedLinkType"))
                        {
                            targetLogger.Log($"{target.Symbol}: extra backlink {extra}");
                            bases.Add($"{extra}.BackLink<TSource>");
                        }
                    }

                    foreach (var parentBackLinkable in parentBackLinks)
                    {
                        bases.Add($"{parentBackLinkable.Key}.BackLink<TSource>");
                        members.Add(
                            $"TSource IBackLink<TSource, {parentBackLinkable.Value}>.Source => Source;"
                        );
                    }

                    backlinkSyntax = SyntaxFactory.ParseMemberDeclaration(
                        $"""
                         public interface BackLink<out TSource> : {string.Join(", ", bases)}
                             where TSource : class, IPathable
                         {(
                             members.Count > 0
                                 ? $"{{ {string.Join("\n", members)} }}"
                                 : ";"
                         )}
                         """
                    );

                    if (backlinkSyntax is null)
                        break;

                    if (
                        isInheritingFriendlyLink ||
                        hierarchy.Length > 0 ||
                        target.Symbol.AllInterfaces.Any(x => x.Name is "ISpecifiedLinkType")
                    ) backlinkSyntax = backlinkSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

                    break;
                case TypeKind.Class:
                    List<string> backlinkInterfaces =
                    [
                        $"ILinkType<{formattedTypeArguments}>.BackLink<TSource>"
                    ];

                    var baseInterfaceSymbols = TypeUtils.GetBaseTypesAndThis(target.Symbol)
                        .SelectMany(x => x.AllInterfaces)
                        .Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default)
                        .Where(HasBackLinkableAttribute)
                        .Where(x =>
                            x.Name is not "ISpecifiedLinkType" &&
                            x.AllInterfaces.Any(x => x.Name is "ILink")
                        )
                        .ToArray();

                    targetLogger.Log($" - {baseInterfaceSymbols.Length} base backlinks");

                    foreach (
                        var baseInterface
                        in baseInterfaceSymbols)
                    {
                        backlinkInterfaces.Add($"{baseInterface}.BackLink<TSource>");
                        targetLogger.Log($" += {baseInterface}.BackLink<TSource>");
                    }

                    var constructors = new List<string>();

                    foreach (var constructor in target.Symbol.Constructors)
                    {
                        var ctorParams = (string[])
                            ["TSource source", ..constructor.Parameters.Select(MemberUtils.ToSyntaxForm)];

                        constructors.Add(
                            $$"""
                              {{MemberUtils.CreateAccessors(constructor.DeclaredAccessibility)}} BackLink(
                                  {{string.Join(", ", ctorParams)}}
                              ) : base({{string.Join(", ", constructor.Parameters.Select(x => x.Name))}})
                              {
                                  Source = source;
                              }
                              """
                        );
                    }

                    backlinkSyntax = SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                            public sealed class BackLink<TSource> : 
                                {{target.Symbol.ToDisplayString()}}
                                {{(
                                    backlinkInterfaces.Count > 0
                                        ? $", {string.Join(
                                            ", ",
                                            backlinkInterfaces
                                        )}"
                                        : string.Empty
                                )}}
                                where TSource : class, IPathable
                            {
                                internal TSource Source { get; }
                                
                                {{
                                    string.Join(
                                        "\n",
                                        constructors
                                    )
                                }}
                                
                                {{
                                    string.Join(
                                        "\n",
                                        baseInterfaceSymbols
                                            .Select(x => string.Join(
                                                ", ",
                                                x.AllInterfaces
                                                    .First(x => x.Name == "ILink")
                                                    .TypeArguments
                                                    .Select(x => x.Name)
                                            ))
                                            .Prepend(formattedTypeArguments)
                                            .Select(x =>
                                                $"TSource IBackLink<TSource, {x}>.Source => Source;"
                                            )
                                            .Distinct()
                                    )
                                }}
                            }
                          """
                    );

                    if (backlinkSyntax is not null &&
                        TypeUtils.GetBaseTypes(target.Symbol).Any(HasBackLinkableAttribute))
                        backlinkSyntax = backlinkSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

                    break;
            }

            if (backlinkSyntax is null)
                continue;

            syntax = syntax.AddMembers(
                backlinkSyntax
            );

            var container = target.Symbol.ContainingType;

            while (container is not null)
            {
                if (
                    container.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax()
                    is not TypeDeclarationSyntax containerSyntax
                ) break;

                syntax = SyntaxUtils.CreateSourceGenClone(containerSyntax).AddMembers(syntax);

                container = container.ContainingType;
            }

            if (!generated.Add(target.Symbol.ToFullMetadataName()))
            {
                targetLogger.Warn($"Failed to generate syntax for {target.Symbol}, already generated");
                continue;
            }

            context.AddSource(
                $"BackLinks/{target.Symbol.ToFullMetadataName()}",
                $$"""
                  {{target.Syntax.GetFormattedUsingDirectives()}}

                  namespace {{target.Symbol.ContainingNamespace}};

                  {{syntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static void ExecuteActors(
        SourceProductionContext context,
        List<ActorGenerationTarget> targets,
        Logger logger)
    {
        var aliases = new Dictionary<string, string>();

        foreach (var target in targets)
        {
            if (target is null) continue;

            var targetLogger = logger.WithSemanticContext(target.Semantic);

            if (target.Symbol.IsGenericType)
            {
                targetLogger.Warn($"{target.Symbol}: skipping generic");
                continue;
            }

            var impl = target.Semantic.Compilation.Assembly.Name.Split('.').Last();

            var name = GetFriendlyName(target.Symbol);

            var linkType = impl switch
            {
                "Core" => "Discord.ILinkType",
                "Rest" => "Discord.Rest.RestLinkType",
                _ => null
            };

            if (linkType is null)
                continue;

            aliases[$"{name}Link"] = $"{linkType}<{target.Symbol}, {target.Id}, {target.Entity}, {target.Model}>";
        }

        if (aliases.Count == 0) return;

        context.AddSource(
            "Links/Globals",
            string.Join(
                "\n",
                aliases.Select(x => $"global using {x.Key} = {x.Value};")
            )
        );

        return;

        static string FormatPaged(
            string type,
            ITypeSymbol pagingParams,
            ITypeSymbol? pagingEntity,
            ActorGenerationTarget target)
        {
            var pagedModel = type.Contains("Rest") && pagingParams.AllInterfaces
                    .FirstOrDefault(x => x is {Name: "IPagingParams", IsGenericType: true})
                is { } generic
                ? $", {generic.TypeArguments[1]}"
                : string.Empty;

            switch (type)
            {
                case not null when type.Contains("Indexable"):
                    var pagingModel =
                        pagingEntity is not null &&
                        type.Contains("Rest") &&
                        EntityTraits.GetEntityModelOfInterface(pagingEntity) is { } entityOf
                            ? $"{entityOf.TypeArguments[0].ToDisplayString()}, "
                            : string.Empty;

                    return $"{type}<" +
                           $"{target.Symbol.ToDisplayString()}, " +
                           $"{target.Id.ToDisplayString()}, " +
                           $"{target.Entity.ToDisplayString()}, " +
                           $"{target.Model.ToDisplayString()}, " +
                           (
                               pagingEntity is not null && type.Contains("Indexable")
                                   ? $"{pagingEntity.ToDisplayString()}, "
                                   : string.Empty
                           ) +
                           pagingModel +
                           $"{pagingParams.ToDisplayString()}" +
                           pagedModel +
                           $">";
                default:
                    return $"{type}<" +
                           $"{target.Symbol.ToDisplayString()}, " +
                           $"{target.Id.ToDisplayString()}, " +
                           $"{target.Entity.ToDisplayString()}, " +
                           $"{target.Model.ToDisplayString()}, " +
                           $"{pagingParams.ToDisplayString()}" +
                           pagedModel +
                           $">";
            }
        }

        static string? Format(string type, ActorGenerationTarget target)
        {
            switch (type)
            {
                case "Discord.Rest.RestEnumerableLink":
                case "Discord.Rest.RestEnumerableIndexableLink":
                case "Discord.Rest.RestDefinedEnumerableIndexableLink":
                    var coreEntity = Hierarchy.GetHierarchy(target.Entity)
                        .FirstOrDefault(x =>
                            x.Type.TypeKind is TypeKind.Interface &&
                            x.Type.AllInterfaces.Any(x => x.Name == "IEntityOf")
                        ).Type;

                    if (coreEntity is null)
                        return null;

                    return $"{type}<" +
                           $"{target.Symbol.ToDisplayString()}, " +
                           $"{target.Id.ToDisplayString()}, " +
                           $"{target.Entity.ToDisplayString()}, " +
                           $"{coreEntity.ToDisplayString()}, " +
                           $"{target.Model.ToDisplayString()}>";
                default:
                    return $"{type}<" +
                           $"{target.Symbol.ToDisplayString()}, " +
                           $"{target.Id.ToDisplayString()}, " +
                           $"{target.Entity.ToDisplayString()}, " +
                           $"{target.Model.ToDisplayString()}>";
            }
        }
    }

    public static string GetFriendlyName(ITypeSymbol symbol)
    {
        if (symbol.TypeKind is TypeKind.Interface)
            return symbol.Name.Remove(0, 1).Replace("Actor", string.Empty);

        return symbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }

    public static INamedTypeSymbol? GetActorFromFriendlyName(string name, Compilation compilation)
    {
        return
            compilation.GetTypeByMetadataName($"Discord.Gateway.Gateway{name.Replace("Link", string.Empty)}Actor")
            ??
            compilation.GetTypeByMetadataName($"Discord.Rest.Rest{name.Replace("Link", string.Empty)}Actor")
            ??
            compilation.GetTypeByMetadataName($"Discord.I{name.Replace("Link", string.Empty)}Actor");
    }
}