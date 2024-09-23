using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Tasks.Traits;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

public sealed class LinksV2 :
    ISyntaxGenerationCombineTask<LinksV2.GenerationTarget>
{
    public enum AssemblyTarget
    {
        Core,
        Rest
    }

    public static readonly string[] AllowedAssemblies =
    [
        "Discord.Net.V4.Core",
        "Discord.Net.V4.Rest"
    ];

    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        TypeDeclarationSyntax syntax,
        INamedTypeSymbol entity,
        INamedTypeSymbol actor,
        INamedTypeSymbol model,
        ITypeSymbol id,
        AssemblyTarget assembly
    ) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public TypeDeclarationSyntax Syntax { get; } = syntax;
        public INamedTypeSymbol Entity { get; } = entity;
        public INamedTypeSymbol Actor { get; } = actor;
        public INamedTypeSymbol Model { get; } = model;
        public ITypeSymbol Id { get; } = id;

        public AssemblyTarget Assembly { get; } = assembly;

        public bool Equals(GenerationTarget other)
        {
            return Actor.GetAttributes().SequenceEqual(other.Actor.GetAttributes());
        }

        public INamedTypeSymbol GetCoreActor()
        {
            if (Assembly is AssemblyTarget.Core) return Actor;

            return Hierarchy.GetHierarchy(Actor, false)
                .First(x =>
                    x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                    &&
                    x.Type.AllInterfaces.Any(y => y is {Name: "IActor", TypeArguments.Length: 2})
                ).Type;
        }

        public INamedTypeSymbol GetCoreEntity()
        {
            if (Assembly is AssemblyTarget.Core) return Entity;

            return Hierarchy.GetHierarchy(Entity, false)
                .First(x =>
                    x.Type.ContainingAssembly.Name == "Discord.Net.V4.Core"
                    &&
                    x.Type.AllInterfaces.Any(y => y is {Name: "IEntity"})
                ).Type;
        }
    }

    public bool IsValid(SyntaxNode node, CancellationToken token = default)
    {
        return node is TypeDeclarationSyntax;
    }

    public GenerationTarget? GetTargetForGeneration(
        GeneratorSyntaxContext context,
        Logger logger,
        CancellationToken token = default)
    {
        if (!AllowedAssemblies.Contains(context.SemanticModel.Compilation.Assembly.Name)) return null;

        var assembly = context.SemanticModel.Compilation.Assembly.Name switch
        {
            "Discord.Net.V4.Core" => AssemblyTarget.Core,
            "Discord.Net.V4.Rest" => AssemblyTarget.Rest,
            _ => throw new NotSupportedException()
        };

        if (context.Node is not TypeDeclarationSyntax syntax)
            return null;

        if (ModelExtensions.GetDeclaredSymbol(context.SemanticModel, syntax) is not INamedTypeSymbol symbol)
            return null;

        var actorType = assembly switch
        {
            AssemblyTarget.Core => "IActor",
            AssemblyTarget.Rest => "IRestActor",
            _ => throw new NotSupportedException()
        };

        var actorInterface = Hierarchy.GetHierarchy(symbol)
            .Select(x => x.Type)
            .FirstOrDefault(x => x.Name == actorType && x is {TypeArguments.Length: 2});

        if (actorInterface is null)
            return null;

        // don't apply to entities
        if (
            actorInterface.TypeArguments[1].Equals(symbol, SymbolEqualityComparer.Default) ||
            actorInterface.TypeArguments[1] is not INamedTypeSymbol entity ||
            symbol.AllInterfaces.Contains(entity))
            return null;

        var entityOfInterface = Hierarchy.GetHierarchy(actorInterface.TypeArguments[1])
            .Select(x => x.Type)
            .FirstOrDefault(x => x is {Name: "IEntityOf", TypeArguments.Length: 1});

        if (entityOfInterface?.TypeArguments.FirstOrDefault() is not INamedTypeSymbol model)
            return null;

        if (syntax.Modifiers.IndexOf(SyntaxKind.PartialKeyword) == -1)
        {
            logger.Warn($"{symbol} is not partial, skipping");
            return null;
        }

        return new GenerationTarget(
            context.SemanticModel,
            syntax,
            entity,
            symbol,
            model,
            actorInterface.TypeArguments[0],
            assembly
        );
    }

    private static TypeDeclarationSyntax ProvideDefaultImplementation(
        TypeDeclarationSyntax syntax,
        GenerationTarget target,
        Logger logger)
    {
        return Sanitize(syntax);

        static TypeDeclarationSyntax Sanitize(TypeDeclarationSyntax syntax)
        {
            var types = syntax.Members
                .OfType<TypeDeclarationSyntax>()
                .Select(Sanitize);

            var ctors = syntax.Members.OfType<ConstructorDeclarationSyntax>();

            return syntax
                .WithMembers(
                    SyntaxFactory.List(
                        types.OfType<MemberDeclarationSyntax>().Concat(ctors)
                    )
                )
                .WithAttributeLists([])
                .WithModifiers(
                    SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)
                    )
                );
        }
    }

    private static TypeDeclarationSyntax ProvideBaseListsAndImplementations(
        TypeDeclarationSyntax syntax,
        GenerationTarget target,
        INamedTypeSymbol linkTypeForTarget,
        Logger logger)
    {
        switch (target.Assembly)
        {
            case AssemblyTarget.Core:
                return (
                        (TypeDeclarationSyntax) syntax.AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.ParseTypeName(
                                    $"{linkTypeForTarget}.{
                                        string.Join(
                                            ".",
                                            syntax.AncestorsAndSelf()
                                                .OfType<TypeDeclarationSyntax>()
                                                .TakeWhile(x =>
                                                    x is not {Identifier.ValueText: "ILinkType"}
                                                )
                                                .Select(x =>
                                                    $"{x.Identifier}{
                                                        (x.TypeParameterList?.Parameters.Count > 0
                                                            ? $"{x.TypeParameterList.WithParameters(
                                                                SyntaxFactory.SeparatedList(
                                                                    x.TypeParameterList.Parameters
                                                                        .Select(x => x
                                                                            .WithVarianceKeyword(default)
                                                                        )
                                                                )
                                                            )}"
                                                            : string.Empty)
                                                    }")
                                                .Reverse()
                                        )
                                    }"
                                )
                            )
                        )
                    )
                    .WithMembers(
                        SyntaxFactory.List<MemberDeclarationSyntax>(
                            syntax.Members.OfType<TypeDeclarationSyntax>()
                                .Select(x =>
                                    {
                                        ApplyTargetedBases(ref x, linkTypeForTarget, target);
                                        return x;
                                    }
                                )
                        )
                    );
            case var _ when syntax is ClassDeclarationSyntax classSyntax:
                var path = syntax.AncestorsAndSelf()
                    .OfType<TypeDeclarationSyntax>()
                    .TakeWhile(x => x.Identifier.ValueText != linkTypeForTarget.Name)
                    .Reverse()
                    .ToArray();

                logger.Log($"{target.Actor}: {syntax.Identifier} > {path.Length}");

                foreach (var item in syntax.AncestorsAndSelf().OfType<TypeDeclarationSyntax>())
                {
                    logger.Log($" - {item.Identifier}");
                }

                var coreActor = target.GetCoreActor();
                var coreEntity = target.GetCoreEntity();

                var coreLinkTypeForTarget = target.SemanticModel.Compilation
                    .GetTypeByMetadataName("Discord.ILinkType`4")!
                    .Construct(coreActor, target.Id, coreEntity, target.Model);

                var coreLinkType = coreLinkTypeForTarget;
                var linkType = linkTypeForTarget;

                foreach (var part in path)
                {
                    var partType = linkType.GetTypeMembers()
                        .FirstOrDefault(x =>
                            x.Name == part.Identifier.ValueText
                            &&
                            x.TypeParameters.Length == (part.TypeParameterList?.Parameters.Count ?? 0)
                        );

                    var corePartType = coreLinkType.GetTypeMembers()
                        .FirstOrDefault(x =>
                            x.Name == part.Identifier.ValueText
                            &&
                            (
                                partType?.AllInterfaces.Any(y =>
                                    y.Name == x.Name
                                    &&
                                    y.TypeParameters.Length == x.TypeParameters.Length
                                )
                                ?? false
                            )
                        );

                    if (partType is null || corePartType is null)
                    {
                        logger.Warn($"{target.Actor}: {part.Identifier.ValueText} not found in {linkType}:");
                        logger.Warn($"{target.Actor}: Asmb type: {linkType} | {partType}");
                        logger.Warn($"{target.Actor}: Core type: {coreLinkType} | {corePartType}");

                        foreach (var member in linkType.GetTypeMembers())
                        {
                            logger.Warn($" - {member}");
                        }

                        foreach (var member in coreLinkType.GetTypeMembers())
                        {
                            logger.Warn($" - {member}");
                        }

                        return syntax;
                    }

                    logger.Log($"{target.Actor} :>: {linkType} -> {partType}");
                    logger.Log($"{target.GetCoreActor()} :>: {coreLinkType} -> {corePartType}");

                    linkType = partType;
                    coreLinkType = corePartType;
                }

                var baseList = SyntaxFactory.BaseList(
                    SyntaxFactory.SeparatedList((BaseTypeSyntax[])
                    [
                        classSyntax.ParameterList is not null
                            ? SyntaxFactory.PrimaryConstructorBaseType(
                                SyntaxFactory.ParseTypeName(
                                    linkType.ToDisplayString()
                                ),
                                SyntaxFactory.ArgumentList(
                                    SyntaxFactory.SeparatedList(
                                        classSyntax.ParameterList.Parameters.Select(x =>
                                            SyntaxFactory.Argument(
                                                SyntaxFactory.IdentifierName(x.Identifier))
                                        )
                                    )
                                )
                            )
                            : SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.ParseTypeName(
                                    linkType.ToDisplayString()
                                )
                            ),
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                $"{coreActor}{coreLinkType.ToDisplayString().Replace(coreLinkTypeForTarget.ToDisplayString(),
                                    string.Empty)}"
                            )
                        )
                    ])
                );

                logger.Log($"{target.Actor} -> {linkType} <> {baseList}");
                logger.Log($"{target.Actor} -> {coreLinkType} <> {baseList}");
                logger.Log(
                    $"{target.Actor} -> CORE -> {coreActor}{coreLinkType.ToDisplayString().Replace(coreLinkTypeForTarget.ToDisplayString(), string.Empty)}");

                var coreMembersToImplement =
                    (
                        coreLinkType.AllInterfaces
                            .Where(x => x.ToDisplayString().StartsWith(coreLinkTypeForTarget.ToDisplayString()))
                            .Prepend(coreLinkType)
                    )
                    .SelectMany(x => x.GetMembers())
                    .Where(x => x is not ITypeSymbol)
                    .Where(x => x is not IMethodSymbol method || method.MethodKind is MethodKind.Ordinary)
                    .Distinct(SymbolEqualityComparer.Default)
                    .Where(x => x.IsAbstract)
                    .ToArray();

                foreach (var member in coreMembersToImplement)
                {
                    logger.Log($" += {member.ContainingType}.{member.Name}: {member.Kind}");
                }

                var ctors = classSyntax
                    .Members
                    .OfType<ConstructorDeclarationSyntax>()
                    .ToArray();

                return syntax
                    .RemoveNodes(ctors, SyntaxRemoveOptions.KeepNoTrivia)!
                    .WithBaseList(
                        baseList
                    )
                    .AddMembers([
                        SyntaxFactory.ParseMemberDeclaration(
                            $"""
                             {coreActor} IActorProvider<{coreActor}, ulong>.GetActor(ulong id) 
                             => (this as IActorProvider<{coreActor}, ulong>).GetActor(client, id);
                             """
                        )!,
                        SyntaxFactory.ParseMemberDeclaration(
                            $"""
                             {coreEntity} IEntityProvider<{coreEntity}, {target.Model}>.CreateEntity({target.Model} model)
                             => (this as IEntityProvider<{coreEntity}, {target.Model}>).CreateEntity(model);
                             """
                        )!,
                        ..coreMembersToImplement
                            .Select(x =>
                            {
                                return x switch
                                {
                                    IMethodSymbol method => SyntaxFactory.ParseMemberDeclaration(
                                        $"{method.ReturnType} {method.ContainingType}.{method.Name}({
                                            string.Join(", ", method.Parameters.Select(x =>
                                                $"{x.Type} {x.Name}"
                                            ))
                                        }) => (this as {method.ContainingType}).{method.Name}({
                                            string.Join(", ", method.Parameters.Select(x => x.Name))
                                        });"
                                    ),
                                    IPropertySymbol property => SyntaxFactory.ParseMemberDeclaration(
                                        $"{property.Type} {property.ContainingType}.{property.Name} => (this as {property.ContainingType}).{property.Name};"
                                    ),
                                    _ => null
                                };
                            })
                            .OfType<MemberDeclarationSyntax>(),
                        ..ctors.Select(x =>
                            SyntaxFactory.ConstructorDeclaration(
                                [],
                                x.Modifiers,
                                x.Identifier,
                                x.ParameterList,
                                SyntaxFactory.ConstructorInitializer(
                                    SyntaxKind.BaseConstructorInitializer,
                                    SyntaxFactory.ArgumentList(
                                        SyntaxFactory.SeparatedList(
                                            x.ParameterList.Parameters.Select(x =>
                                                SyntaxFactory.Argument(SyntaxFactory.IdentifierName(x.Identifier))
                                            )
                                        )
                                    )
                                ),
                                SyntaxFactory.Block()
                            )
                        )
                    ]);
            default: return syntax;
        }
    }

    public void Execute(SourceProductionContext context, ImmutableArray<GenerationTarget?> targets, Logger logger)
    {
        if (targets.Length == 0) return;

        var results = new Dictionary<GenerationTarget, TypeDeclarationSyntax>();

        try
        {
            foreach (var target in targets)
            {
                if (target is null) continue;

                var targetLogger = logger.WithSemanticContext(target.SemanticModel);

                targetLogger.Log($"Processing {target.Actor}");

                if (target.SemanticModel.Compilation.Assembly.Name is "Discord.Net.V4.Core")
                    LinkMethods.Apply(context, target, targetLogger);

                var targetCtors = target.Syntax.Members
                    .OfType<ConstructorDeclarationSyntax>()
                    .ToArray<MemberDeclarationSyntax>();

                // add the default link types
                var syntax = SyntaxUtils
                    .CreateSourceGenClone(target.Syntax)
                    .WithParameterList(target.Syntax.ParameterList)
                    .AddMembers(
                        targetCtors
                    );

                AddBackLink(ref syntax, target, targetLogger);

                var linkTypeForTarget = target.SemanticModel.Compilation
                    .GetTypeByMetadataName(target.Assembly switch
                    {
                        AssemblyTarget.Core => "Discord.ILinkType`4",
                        AssemblyTarget.Rest => "Discord.Rest.RestLinkTypeV2`4",
                        var u => throw new InvalidOperationException($"Unsupported assembly {u}")
                    })
                    ?.Construct(target.Actor, target.Id, target.Entity, target.Model);


                if (linkTypeForTarget is null)
                {
                    targetLogger.Warn($"Failed to create link type for target {target.Actor}");
                    continue;
                }

                var linkTypeSyntax = linkTypeForTarget.DeclaringSyntaxReferences.Length == 0
                    ? null
                    : SyntaxUtils.CombineMembers(
                        linkTypeForTarget.DeclaringSyntaxReferences
                            .Select(x => x.GetSyntax())
                            .OfType<TypeDeclarationSyntax>()
                    );

                if (linkTypeSyntax is null)
                {
                    targetLogger.Warn($"Failed to create link type syntax for target {target.Actor}");
                    continue;
                }

                var linkTypes = linkTypeSyntax
                    .Members
                    .OfType<TypeDeclarationSyntax>()
                    .Where(x => x.Identifier.ValueText is not "BackLink")
                    .Select(x => ProvideDefaultImplementation(x, target, targetLogger))
                    .Select(x =>
                    {
                        x = ProvideBaseListsAndImplementations(x, target, linkTypeForTarget, targetLogger);

                        if (target.Assembly is not AssemblyTarget.Core)
                        {
                            x = x.ReplaceNodes(
                                x.DescendantNodes().OfType<TypeDeclarationSyntax>(),
                                (_, node) =>
                                    ProvideBaseListsAndImplementations(
                                        node,
                                        target,
                                        linkTypeForTarget,
                                        targetLogger
                                    )
                            );
                        }

                        return x;
                    })
                    .Select(x => x
                        .ReplaceNodes(
                            x.DescendantNodes().OfType<IdentifierNameSyntax>(),
                            (node, x) =>
                            {
                                switch (node.Identifier.Value)
                                {
                                    case "TActor":
                                        return SyntaxFactory.IdentifierName(target.Actor.ToDisplayString());
                                    case "TId":
                                        return SyntaxFactory.IdentifierName(target.Id.ToDisplayString());
                                    case "TEntity":
                                        return SyntaxFactory.IdentifierName(target.Entity.ToDisplayString());
                                    case "TModel":
                                        return SyntaxFactory.IdentifierName(target.Model.ToDisplayString());
                                }

                                return node;
                            }
                        )
                    )
                    .ToList();

                foreach (var linkType in linkTypes)
                {
                    targetLogger.Log(
                        $"{linkType.Identifier}: {string.Join(", ", linkType.AncestorsAndSelf().OfType<TypeDeclarationSyntax>().Select(x => x.Identifier.ValueText))}");
                }

                if (targets.Any(x => x is not null && Hierarchy.Implements(target.Actor, x.Actor)))
                {
                    linkTypes = linkTypes
                        .Select(x => x.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword)))
                        .ToList();
                }

                var pagedAttribute = target.GetCoreEntity()
                    .GetAttributes()
                    .Where(x => x.AttributeClass?.Name == "PagedFetchableOfManyAttribute")
                    .ToArray();

                if (pagedAttribute.Length > 0)
                {
                    foreach (var attribute in pagedAttribute)
                    {
                        var pageLinkType = linkTypes
                            .FirstOrDefault(x =>
                                x.Identifier.ValueText == "Paged" &&
                                x.TypeParameterList?.Parameters.Count == attribute.AttributeClass?.TypeArguments.Length
                            );

                        if (pageLinkType is null)
                        {
                            targetLogger.Warn($"Failed to create paged link type for target {target.Actor}");
                            continue;
                        }

                        var sb = new StringBuilder("Paged");

                        if (attribute.AttributeClass!.TypeArguments.Length >= 1)
                        {
                            var identifier = attribute.AttributeClass!
                                .TypeArguments[0]
                                .Name
                                .Replace("Page", string.Empty)
                                .Replace("Params", string.Empty);

                            sb.Append($"{identifier}");
                        }

                        if (attribute.AttributeClass!.TypeArguments.Length >= 2)
                        {
                            var identifier = GetFriendlyName(
                                attribute.AttributeClass!
                                    .TypeArguments[1]
                            );

                            sb.Append($"As{identifier}");
                        }

                        pageLinkType = pageLinkType
                            .WithBaseList(
                                SyntaxFactory.BaseList(
                                    SyntaxFactory.SeparatedList((BaseTypeSyntax[])
                                    [
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{pageLinkType.Identifier}{
                                                    (pageLinkType.TypeParameterList?.Parameters.Count > 0
                                                        ? $"{pageLinkType.TypeParameterList.WithParameters(
                                                            SyntaxFactory.SeparatedList(
                                                                pageLinkType.TypeParameterList.Parameters
                                                                    .Select(x => x
                                                                        .WithVarianceKeyword(default)
                                                                    )
                                                            )
                                                        )}"
                                                        : string.Empty)
                                                }"
                                            )
                                        )
                                    ])
                                )
                            );


                        pageLinkType = pageLinkType
                            .ReplaceNodes(
                                pageLinkType
                                    .DescendantNodes()
                                    .OfType<IdentifierNameSyntax>(),
                                (node, _) =>
                                {
                                    return node.Identifier.ValueText switch
                                    {
                                        "TParams" => SyntaxFactory.IdentifierName(
                                            attribute.AttributeClass!.TypeArguments[0]
                                                .ToDisplayString()),
                                        "TPaged" => SyntaxFactory.IdentifierName(attribute.AttributeClass!
                                            .TypeArguments[1]
                                            .ToDisplayString()),
                                        _ => node
                                    };
                                }
                            )
                            .WithConstraintClauses([])
                            .WithTypeParameterList(null)
                            .WithIdentifier(
                                SyntaxFactory.Identifier($"{sb}")
                            );

                        if (target.Assembly is not AssemblyTarget.Core)
                        {
                            foreach (
                                var ctor
                                in pageLinkType.Members.OfType<ConstructorDeclarationSyntax>()
                                    .Select(x => x.ParameterList)
                                    .Prepend(pageLinkType.ParameterList)
                            )
                            {
                                if (ctor is null) continue;

                                var ctorParams = string.Join(
                                    ", ",
                                    ctor.Parameters
                                        .Select(x => x.Identifier.ValueText)
                                );

                                pageLinkType = pageLinkType
                                    .AddMembers(
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $$"""
                                                public {{pageLinkType.Identifier}}{{ctor}} : base({{ctorParams}})
                                                {}
                                              """
                                        )!
                                    );
                            }

                            pageLinkType = pageLinkType.WithParameterList(null);
                        }

                        linkTypes.Add(pageLinkType);
                    }
                }


                if (
                    target.Syntax is InterfaceDeclarationSyntax targetInterface &&
                    TryGetFetchableRouteType(target, out var route) && route is IMethodSymbol methodSymbol)
                {
                    var extraParameters =
                        new Dictionary<IParameterSymbol, ParameterSyntax>(SymbolEqualityComparer.Default);

                    EntityTraits.ParseRouteArguments(
                        methodSymbol,
                        new EntityTraits.GenerationTarget(
                            target.SemanticModel,
                            targetInterface,
                            target.Actor,
                            []
                        ),
                        targetLogger,
                        extra =>
                        {
                            if (!extra.IsOptional)
                                return null;

                            if (extra.DeclaringSyntaxReferences.Length == 0)
                                return null;

                            if (extra.DeclaringSyntaxReferences[0].GetSyntax() is not ParameterSyntax extraSyntax ||
                                extraSyntax.Default is null)
                                return null;

                            extraParameters.Add(extra, extraSyntax);
                            return SyntaxFactory.Argument(SyntaxFactory.IdentifierName(extraSyntax.Identifier));
                        }
                    );

                    if (extraParameters.Count > 0)
                    {
                        var enumerableLink = linkTypes
                            .FirstOrDefault(x => x.Identifier.ValueText == "Enumerable");

                        if (enumerableLink is null || !linkTypes.Remove(enumerableLink))
                        {
                            targetLogger.Warn($"Failed to create enumerable link type for target {target.Actor}");
                            continue;
                        }

                        enumerableLink = enumerableLink
                            .AddMembers(
                                SyntaxFactory.ParseMemberDeclaration(
                                    $"new ITask<IReadOnlyCollection<{target.Entity}>> AllAsync({string.Join(", ", extraParameters.Values)}, RequestOptions? options = null, CancellationToken token = default);"
                                )!,
                                SyntaxFactory.ParseMemberDeclaration(
                                    $"ITask<IReadOnlyCollection<{target.Entity}>> {linkTypeForTarget}.Enumerable.AllAsync(RequestOptions? options, CancellationToken token) => AllAsync(options: options, token: token);"
                                )!
                            );

                        linkTypes.Add(enumerableLink);
                    }
                }

                syntax = syntax
                    .AddMembers(
                        linkTypes
                            .Select(x =>
                            {
                                AddBackLink(ref x, target, targetLogger);
                                return x;
                            })
                            .OfType<MemberDeclarationSyntax>()
                            .ToArray()
                    );

                syntax = syntax
                    .RemoveNodes(
                        syntax.DescendantNodes()
                            .Where(x => targetCtors.Any(y => y.IsEquivalentTo(x))),
                        SyntaxRemoveOptions.KeepNoTrivia
                    )!
                    .WithParameterList(null);

                ApplyHierarchicalRoot(ref syntax, target, targets, targetLogger);
                LinkExtensions.Process(ref syntax, target, targets, targetLogger);

                results[target] = syntax;
            }

            if (results.Count > 0)
            {
                LinkCoreInheritance.Process(results, logger);
            }

            foreach (var result in results)
            {
                var target = result.Key;
                var syntax = result.Value;

                var targetLogger = logger.WithSemanticContext(target.SemanticModel);

                // TODO: we need to overload stuff to prevent ambiguous references.
                if (target.Assembly is AssemblyTarget.Core && false)
                {
                    var directParents = targets
                        .Where(x =>
                            x is not null &&
                            target.Actor.Interfaces.Contains(x.Actor)
                        )
                        .ToArray();

                    if (directParents.Length > 0)
                    {
                        syntax = syntax
                            .ReplaceNodes(
                                syntax.DescendantNodes()
                                    .OfType<TypeDeclarationSyntax>(),
                                (old, node) =>
                                {
                                    var path = old.AncestorsAndSelf()
                                        .OfType<TypeDeclarationSyntax>()
                                        .TakeWhile(x => x.Identifier.ValueText != syntax.Identifier.ValueText)
                                        .Reverse()
                                        .Select(ToReferenceName)
                                        .ToArray();

                                    foreach (var parent in directParents)
                                    {
                                        if (parent is null) continue;

                                        if (!results.TryGetValue(parent, out var parentSyntax))
                                            continue;

                                        var i = 0;
                                        var parentNode = parentSyntax;
                                        for (; i != path.Length; i++)
                                        {
                                            parentNode = parentNode
                                                .Members
                                                .OfType<TypeDeclarationSyntax>()
                                                .FirstOrDefault(x => ToReferenceName(x) == path[i]);

                                            if (parentNode is null) break;
                                        }

                                        if (i != path.Length) continue;

                                        var pathStr = string.Join(".", path);

                                        var baseName = $"{parent.Actor}.{pathStr}";

                                        if (node.BaseList?.Types.Any(x => x.ToString() == baseName) ?? false)
                                            continue;

                                        node = (TypeDeclarationSyntax) node
                                            .AddBaseListTypes(
                                                SyntaxFactory.SimpleBaseType(
                                                    SyntaxFactory.ParseTypeName(
                                                        baseName
                                                    )
                                                )
                                            );

                                        targetLogger.Log($"{target.Actor} --> {pathStr} += {parent.Actor}");
                                    }

                                    return node;
                                }
                            );
                    }
                }

                var ancestors = results
                    .Where(x =>
                        Hierarchy.Implements(target.Actor, x.Key.Actor)
                    );

                foreach (var ancestor in ancestors)
                {
                    targetLogger.Log($"{target.Actor}: Anscestor check on {ancestor.Key.Actor}");
                    ApplyNewWhereNeeded(ref syntax, ancestor.Value, targetLogger);
                }

                try
                {
                    context.AddSource(
                        $"Links/{target.Actor.ToFullMetadataName()}",
                        $$"""
                          {{target.Syntax.GetFormattedUsingDirectives("MorseCode.ITask")}}

                          namespace {{target.Actor.ContainingNamespace}};

                          #pragma warning disable CS0108
                          #pragma warning disable CS0109
                          {{syntax.NormalizeWhitespace()}}
                          #pragma warning restore CS0108
                          #pragma warning restore CS0109
                          """
                    );
                }
                catch (Exception x)
                {
                    targetLogger.Warn($"{target.Actor}: {x}");
                }
            }
        }
        catch (Exception x)
        {
            if (targets.Length == 0) return;

            logger.WithSemanticContext(targets.FirstOrDefault(x => x is not null)!.SemanticModel)
                .Warn($"Failed to generate links: {x}");
            return;
        }

        var aliases = targets
            .OfType<GenerationTarget>()
            .Select(x =>
                $"""
                 global using {GetFriendlyName(x.Actor)}Link = Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 global using {GetFriendlyName(x.Actor)}LinkType = Discord.ILinkType<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}>;
                 """
            )
            .ToArray();

        if (aliases.Length > 0)
        {
            context.AddSource("Links/Aliases", string.Join(Environment.NewLine, aliases));
        }
    }

    private static void ApplyNewWhereNeeded<T>(
        ref T source,
        TypeDeclarationSyntax toCheckAgainst,
        Logger logger,
        int depth = 0
    ) where T : TypeDeclarationSyntax
    {
        for (var i = 0; i != source.Members.Count; i++)
        {
            var member = source.Members[i];

            if (member is not T type)
                continue;

            var companion = toCheckAgainst
                .Members
                .FirstOrDefault(x =>
                    x is T target &&
                    target.Identifier.ValueText == type.Identifier.ValueText
                ) as T;

            if (companion is null) continue;

            logger.Log($"{"".PadLeft(depth * 2)} - {type.Identifier} <> {companion.Identifier}");

            if (type.Modifiers.IndexOf(SyntaxKind.NewKeyword) == -1)
                type = (T) type.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

            ApplyNewWhereNeeded(ref type, companion, logger, depth + 1);

            source = (T) source.WithMembers(
                source.Members.RemoveAt(i).Insert(i, type)
            );
        }
    }

    private static string ToReferenceName<T>(T syntax)
        where T : TypeDeclarationSyntax
    {
        return $"{syntax.Identifier}{
            (syntax.TypeParameterList?.Parameters.Count > 0
                ? $"{syntax.TypeParameterList.WithParameters(
                    SyntaxFactory.SeparatedList(
                        syntax.TypeParameterList.Parameters
                            .Select(x => x
                                .WithVarianceKeyword(default)
                            )
                    )
                )}"
                : string.Empty)
        }";
    }

    private static void ApplyTargetedBases<T>(
        ref T syntax,
        INamedTypeSymbol linkType,
        GenerationTarget target
    )
        where T : TypeDeclarationSyntax
    {
        if (syntax.Identifier.ValueText is not "Indexable" and not "Enumerable" and not "Defined" and not "Paged")
            return;

        var path = syntax.AncestorsAndSelf()
            .OfType<T>()
            .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
            .ToArray();

        switch (path.Length)
        {
            case <= 1: break;
            default:
                syntax = (T) syntax.WithBaseList(
                    SyntaxFactory.BaseList(
                        SyntaxFactory.SeparatedList(
                            [
                                SyntaxFactory.SimpleBaseType(
                                    SyntaxFactory.ParseTypeName(
                                        $"{linkType}.{string.Join(".", path.Select(ToReferenceName).Reverse())}"
                                    )
                                ),
                                ..path.SelectMany(x =>
                                    (BaseTypeSyntax[])
                                    [
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{target.Actor}.{ToReferenceName(x)}"
                                            )
                                        ),
                                        SyntaxFactory.SimpleBaseType(
                                            SyntaxFactory.ParseTypeName(
                                                $"{linkType}.{ToReferenceName(x)}"
                                            )
                                        )
                                    ]
                                )
                            ]
                        )
                    )
                );
                break;
        }

        for (var i = 0; i < syntax.Members.Count; i++)
        {
            var member = syntax.Members[i];

            if (member is not T child)
                continue;

            ApplyTargetedBases(ref child, linkType, target);

            syntax = (T) syntax.WithMembers(
                syntax.Members.RemoveAt(i).Insert(i, child)
            );
        }
    }

    private static void ApplyHierarchicalRoot<T>(
        ref T syntax,
        GenerationTarget target,
        ImmutableArray<GenerationTarget?> targets,
        Logger logger
    )
        where T : TypeDeclarationSyntax
    {
        var hierarchyAttribute =
            target.GetCoreActor()
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "LinkHierarchicalRootAttribute");

        if (hierarchyAttribute is null) return;

        var types = hierarchyAttribute.NamedArguments
            .FirstOrDefault(x => x.Key == "Types")
            .Value;

        if (types.Kind is TypedConstantKind.Array)
        {
            foreach (var t in types.Values)
            {
                logger.Log($"- {t.Kind} : {t.Value?.GetType()}");
            }
        }

        var children = types.Kind is not TypedConstantKind.Error
            ? (
                types.Kind switch
                {
                    TypedConstantKind.Array => types.Values.Select(x => (INamedTypeSymbol) x.Value!),
                    _ => (INamedTypeSymbol[]) types.Value!
                }
            )
            .Select(x => targets
                .FirstOrDefault(y =>
                    y is not null
                    &&
                    y.GetCoreActor().Equals(x, SymbolEqualityComparer.Default)
                )
            )
            .OfType<GenerationTarget>()
            .ToArray()
            : targets
                .Where(x => x is not null && Hierarchy.Implements(x.GetCoreActor(), target.GetCoreActor()))
                .OfType<GenerationTarget>()
                .ToArray();

        logger.Log($"{target.Actor}: {children.Length} hierarchical link targets");

        if (children.Length == 0) return;

        foreach (var child in children)
        {
            logger.Log($" - {child.Actor}");
        }
        
        if (target.Assembly is AssemblyTarget.Core)
        {
            syntax = (T) syntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      public interface Hierarchy
                      {
                          {{
                              string.Join(
                                  Environment.NewLine,
                                  children
                                      .Select(x =>
                                      {
                                          var name = string.Join(
                                              string.Empty,
                                              ToNameParts(GetFriendlyName(x.Actor))
                                                  .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                          );

                                          return SyntaxFactory.ParseMemberDeclaration(
                                              $"Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}> {name} {{ get; }}"
                                          )!;
                                      })
                              )
                          }}
                      }  
                      """
                )!
            );
        }

        syntax = syntax
            .ReplaceNodes(
                syntax.DescendantNodes().OfType<T>(),
                (old, node) =>
                {
                    if (node.Identifier.ValueText == "Hierarchy")
                        return node;

                    if (node.Identifier.ValueText is "BackLink") return node;

                    var anscestors = old.AncestorsAndSelf()
                        .OfType<T>()
                        .ToList();

                    var path = string.Join(
                        ".",
                        anscestors
                            .TakeWhile(x => x.Identifier.ValueText != target.Actor.Name)
                            .Select(x =>
                                $"{x.Identifier}{
                                    (x.TypeParameterList?.Parameters.Count > 0
                                        ? $"{x.TypeParameterList.WithParameters(
                                            SyntaxFactory.SeparatedList(
                                                x.TypeParameterList.Parameters
                                                    .Select(x => x
                                                        .WithVarianceKeyword(default)
                                                    )
                                            )
                                        )}"
                                        : string.Empty)
                                }"
                            )
                            .Reverse()
                    );

                    logger.Log($"{target.Actor} += {children.Length} children to {path}");
                    foreach (var ancestor in anscestors)
                    {
                        logger.Log($" - {ancestor.Identifier}");
                    }

                    var props = children.Select(x =>
                    {
                        var name = string.Join(
                            string.Empty,
                            ToNameParts(GetFriendlyName(x.Actor))
                                .Except(ToNameParts(GetFriendlyName(target.Actor)))
                        );

                        var result = new StringBuilder()
                            .AppendLine(
                                old is ClassDeclarationSyntax
                                    ? $"public virtual {x.Actor}.{path} {name} {{ get; }}"
                                    : $"new {x.Actor}.{path} {name} {{ get; }}"
                            );

                        if (target.Assembly is not AssemblyTarget.Core && anscestors.Count >= 1)
                        {
                            result.AppendLine(
                                $"{x.GetCoreActor()}.{path} {target.GetCoreActor()}.{path}.Hierarchy.{name} => {name};"
                            );
                        }
                        else
                        {
                            result.AppendLine(
                                $"Discord.ILink<{x.Actor}, {x.Id}, {x.Entity}, {x.Model}> {(target.Assembly is AssemblyTarget.Core ? target.Actor : target.GetCoreActor())}.Hierarchy.{name} => {name};"
                            );
                        }

                        return result.ToString();
                    });

                    var hierarchy = (T) SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          public {{(old is ClassDeclarationSyntax ? "class" : "interface")}} Hierarchy : {{path}}
                          {
                              {{
                                  string.Join(
                                      Environment.NewLine,
                                      props
                                  )
                              }}
                          }
                          """
                    )!;

                    hierarchy = (T) hierarchy
                        .AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(
                                SyntaxFactory.ParseTypeName(
                                    target.Assembly is AssemblyTarget.Core
                                        ? $"{target.Actor}.Hierarchy"
                                        : $"{target.GetCoreActor()}.{path}.Hierarchy"
                                )
                            )
                        );

                    var ctors = old.Members
                        .OfType<ConstructorDeclarationSyntax>()
                        .Select(x => x.ParameterList)
                        .Prepend(old.ParameterList)
                        .ToArray();

                    if (old is ClassDeclarationSyntax)
                    {
                        foreach
                        (
                            var ctor
                            in ctors
                        )
                        {
                            if (ctor is null) continue;

                            hierarchy = (T) hierarchy.AddMembers(
                                SyntaxFactory.ParseMemberDeclaration(
                                    $$"""
                                      public Hierarchy{{ctor
                                          .AddParameters(children
                                              .Select(x => {
                                                  var name = string.Join(
                                                      string.Empty,
                                                      ToNameParts(GetFriendlyName(x.Actor))
                                                          .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                                  );

                                                  name = $"{char.ToLower(name[0])}{name.Substring(1)}";

                                                  return SyntaxFactory.Parameter(
                                                      [],
                                                      [],
                                                      SyntaxFactory.ParseTypeName(
                                                          $"{x.Actor}.{path}"
                                                      ),
                                                      SyntaxFactory.Identifier(name),
                                                      null
                                                  );
                                              })
                                              .ToArray()
                                          )
                                          .NormalizeWhitespace()
                                      }} : base({{string.Join(", ", ctor.Parameters.Select(x => x.Identifier))}})
                                      {
                                         {{
                                             string.Join(
                                                 Environment.NewLine,
                                                 children.Select(x => {
                                                     var name = string.Join(
                                                         string.Empty,
                                                         ToNameParts(GetFriendlyName(x.Actor))
                                                             .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                                     );

                                                     return $"{name} = {char.ToLower(name[0])}{name.Substring(1)};";
                                                 })
                                             )
                                         }}
                                      }
                                      """
                                )!
                            );
                        }
                    }

                    AddBackLink(ref hierarchy, target, logger, false, false, transformer: backlink =>
                    {
                        backlink = (T) backlink
                            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                            .WithSemicolonToken(default)
                            .AddMembers(
                                children.SelectMany(x =>
                                {
                                    var name = string.Join(
                                        string.Empty,
                                        ToNameParts(GetFriendlyName(x.Actor))
                                            .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                    );

                                    if (target.Assembly is AssemblyTarget.Core)
                                    {
                                        return (MemberDeclarationSyntax[])
                                        [
                                            SyntaxFactory.ParseMemberDeclaration(
                                                $"new {x.Actor}.{path}.BackLink<TSource> {name} {{ get; }}"
                                            )!,
                                            SyntaxFactory.ParseMemberDeclaration(
                                                $"{x.Actor}.{path} {target.Actor}.{path}.Hierarchy.{name} => {name};"
                                            )!
                                        ];
                                    }

                                    return (MemberDeclarationSyntax[])
                                    [
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"public override {x.Actor}.{path}.BackLink<TSource> {name} {{ get; }}"
                                        )!,
                                        SyntaxFactory.ParseMemberDeclaration(
                                            $"{x.GetCoreActor()}.{path}.BackLink<TSource> {target.GetCoreActor()}.{path}.Hierarchy.BackLink<TSource>.{name} => {name};"
                                        )!
                                    ];
                                }).ToArray()
                            );

                        if (target.Assembly is AssemblyTarget.Core) return backlink;

                        backlink = backlink
                            .RemoveNodes(
                                backlink.Members.OfType<ConstructorDeclarationSyntax>(),
                                SyntaxRemoveOptions.KeepNoTrivia
                            )!;

                        foreach (var ctor in ctors)
                        {
                            if (ctor is null) continue;

                            backlink = (T) backlink
                                .AddMembers(
                                    SyntaxFactory.ParseMemberDeclaration(
                                        $$"""
                                          public BackLink{{ctor
                                              .WithParameters(
                                                  SyntaxFactory.SeparatedList([
                                                      SyntaxFactory.Parameter(
                                                          [],
                                                          [],
                                                          SyntaxFactory.ParseTypeName("TSource"),
                                                          SyntaxFactory.Identifier("source"),
                                                          null
                                                      ),
                                                      ..ctor.Parameters,
                                                      ..children
                                                          .Select(x => {
                                                              var name = string.Join(
                                                                  string.Empty,
                                                                  ToNameParts(GetFriendlyName(x.Actor))
                                                                      .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                                              );

                                                              name = $"{char.ToLower(name[0])}{name.Substring(1)}";

                                                              return SyntaxFactory.Parameter(
                                                                  [],
                                                                  [],
                                                                  SyntaxFactory.ParseTypeName(
                                                                      $"{x.Actor}.{path}.BackLink<TSource>"
                                                                  ),
                                                                  SyntaxFactory.Identifier(name),
                                                                  null
                                                              );
                                                          })
                                                  ])
                                              )
                                              .NormalizeWhitespace()
                                          }} : base({{
                                              string.Join(
                                                  ", ",
                                                  ctor.Parameters
                                                      .Select(x => x.Identifier.ValueText)
                                                      .Concat(children.Select(x => {
                                                          var name = string.Join(
                                                              string.Empty,
                                                              ToNameParts(GetFriendlyName(x.Actor))
                                                                  .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                                          );

                                                          return $"{char.ToLower(name[0])}{name.Substring(1)}";
                                                      }))
                                              )
                                          }})
                                          {
                                              Source = source;
                                             {{
                                                 string.Join(
                                                     Environment.NewLine,
                                                     children.Select(x => {
                                                         var name = string.Join(
                                                             string.Empty,
                                                             ToNameParts(GetFriendlyName(x.Actor))
                                                                 .Except(ToNameParts(GetFriendlyName(target.Actor)))
                                                         );

                                                         return $"{name} = {char.ToLower(name[0])}{name.Substring(1)};";
                                                     })
                                                 )
                                             }}
                                          }
                                          """
                                    )!
                                );
                        }

                        return backlink;
                    });

                    return node
                        .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                        .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                        .WithSemicolonToken(default)
                        .AddMembers(
                            hierarchy
                        );
                }
            );
    }

    public static void AddBackLink<T>(
        ref T syntax,
        GenerationTarget filler,
        Logger logger,
        bool searchChildren = true,
        bool applyBaseBackLinks = true,
        Func<T, T>? transformer = null)
        where T : TypeDeclarationSyntax
    {
        if (syntax.Identifier.ValueText == "BackLink")
        {
            logger.Log($"{filler.Actor}: Skipping {syntax.Identifier} (backlink)");
            return;
        }

        var typeMembers = syntax.Members.OfType<T>().ToArray();

        if (typeMembers.Any(x => x.Identifier.ValueText == "BackLink"))
        {
            logger.Log($"{filler.Actor}: Skipping {syntax.Identifier} (has backlink)");
            return;
        }

        var path = syntax.AncestorsAndSelf()
            .OfType<T>()
            .TakeWhile(x => x.Identifier.ValueText != filler.Actor.Name)
            .Reverse()
            .ToArray();

        var isClass = syntax is ClassDeclarationSyntax;

        var baseIdent = new StringBuilder(syntax.Identifier.ValueText);

        if (syntax.TypeParameterList?.Parameters.Count > 0)
        {
            baseIdent
                .Append('<')
                .Append(
                    string.Join(
                        ", ",
                        syntax.TypeParameterList.Parameters.Select(x => x.Identifier)
                    )
                )
                .Append('>');
        }

        if (syntax is ClassDeclarationSyntax {ParameterList: not null})
        {
            baseIdent
                .Append('(')
                .Append(
                    string.Join(
                        ", ",
                        syntax.ParameterList!.Parameters.Select(x => x.Identifier.ValueText)
                    )
                )
                .Append(')');
        }

        var backlink =
            (T) SyntaxFactory
                .ParseMemberDeclaration(
                    $$"""
                        public {{(isClass ? "class" : "interface")}} BackLink<{{(!isClass ? "out " : string.Empty)}}TSource> : 
                            {{baseIdent}},
                            Discord.ILinkType<{{string.Join(", ", filler.Actor, filler.Id, filler.Entity, filler.Model)}}>.BackLink<TSource>,
                            Discord.IBackLink<TSource, {{string.Join(", ", filler.Actor, filler.Id, filler.Entity, filler.Model)}}>
                            where TSource : class, IPathable;
                      """
                )!;


        if (isClass)
        {
            // add core backlink 
            var coreBase = syntax.BaseList?.Types
                .FirstOrDefault(x => x
                    .Type
                    .ToString()
                    .StartsWith(filler.GetCoreActor().ToDisplayString())
                );

            backlink = (T) backlink
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"TSource IBackLink<TSource, {filler.GetCoreActor()}, {filler.Id}, {filler.GetCoreEntity()}, {filler.Model}>.Source => Source;"
                    )!
                )
                .AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName(
                            coreBase is not null
                                ? $"{coreBase}.BackLink<TSource>"
                                : $"{filler.GetCoreActor()}.BackLink<TSource>"
                        )
                    )
                );

            // add actor provider if path is empty
            if (path.Length == 0)
            {
                backlink = (T) backlink
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $"{filler.Actor} IActorProvider<{filler.Actor}, {filler.Id}>.GetActor({filler.Id} id) => this; "
                        )!,
                        SyntaxFactory.ParseMemberDeclaration(
                            $"{filler.GetCoreActor()} IActorProvider<{filler.GetCoreActor()}, {filler.Id}>.GetActor({filler.Id} id) => this; "
                        )!
                    );
            }

            backlink = (T) backlink
                .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"internal TSource Source {{ get; }}{(
                            syntax.ParameterList is not null ? " = source;" : string.Empty
                        )}"
                    )!,
                    SyntaxFactory.ParseMemberDeclaration(
                        $"TSource IBackLink<TSource, {filler.Actor}, {filler.Id}, {filler.Entity}, {filler.Model}>.Source => Source;"
                    )!
                );

            if (syntax.ParameterList is not null)
            {
                backlink = (T) backlink
                    .WithParameterList(
                        SyntaxFactory.ParameterList(
                            syntax.ParameterList.Parameters
                                .Insert(0, SyntaxFactory.Parameter(
                                    [],
                                    [],
                                    SyntaxFactory.ParseTypeName("TSource"),
                                    SyntaxFactory.Identifier("source"),
                                    null
                                ))
                        )
                    );
            }

            var ctors = syntax
                .Members
                .OfType<ConstructorDeclarationSyntax>()
                .ToArray();

            logger.Log(
                $"{filler.Actor}: {syntax.Identifier} ctors: {ctors.Length} (default ctor?: {syntax.ParameterList is not null})");

            foreach (var ctor in ctors)
            {
                var ctorParams = SyntaxFactory.ParameterList(
                    ctor.ParameterList.Parameters.Insert(
                        0,
                        SyntaxFactory.Parameter(
                            [],
                            [],
                            SyntaxFactory.ParseTypeName("TSource"),
                            SyntaxFactory.Identifier("source"),
                            null
                        )
                    )
                );

                var baseParams = SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(
                        ctor.ParameterList.Parameters
                            .Select(x => SyntaxFactory.Argument(
                                SyntaxFactory.IdentifierName(x.Identifier)
                            ))
                    )
                );

                backlink = (T) backlink
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              internal BackLink{{ctorParams.NormalizeWhitespace()}} : base{{baseParams.NormalizeWhitespace()}}
                              {
                                Source = source;
                              }
                              """
                        )!
                    );
            }
        }

        if (applyBaseBackLinks && syntax.BaseList?.Types.Count > 0)
        {
            backlink = (T) backlink.AddBaseListTypes(
                syntax.BaseList.Types
                    .Where(x => x.ToString().StartsWith($"{filler.Actor}"))
                    .Select(x =>
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName($"{x.Type}.BackLink<TSource>")
                        )
                    )
                    .ToArray<BaseTypeSyntax>()
            );
        }

        if (transformer is not null)
            backlink = transformer(backlink);

        syntax = (T) syntax
            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
            .AddMembers(
                backlink
            );

        logger.Log($"{filler.Actor}: += {syntax.Identifier} backlink {baseIdent}");

        if (!searchChildren) return;

        for (var i = 0; i < typeMembers.Length; i++)
        {
            if (syntax.Members[i] is not TypeDeclarationSyntax iface)
                continue;

            AddBackLink(ref iface, filler, logger);

            syntax = (T) syntax.WithMembers(syntax.Members.RemoveAt(i).Insert(i, iface));
        }
    }

    private static string[] ToNameParts(string str)
    {
        var sb = new StringBuilder();

        for (var i = 0; i < str.Length; i++)
        {
            var ch = str[i];
            if (char.IsUpper(ch) && i > 0)
                sb.Append(' ');

            sb.Append(ch);
        }

        return sb.ToString().Split(' ');
    }

    private static bool TryGetFetchableRouteType(GenerationTarget target, out ISymbol route)
    {
        route = null!;

        var fetchableAttribute = target.Entity
            .GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.Name == "FetchableOfManyAttribute");

        if (fetchableAttribute is null)
            return false;

        if (EntityTraits.GetNameOfArgument(fetchableAttribute) is not MemberAccessExpressionSyntax routeMemberAccess)
            return false;

        return (
            route = EntityTraits.GetRouteSymbol(
                routeMemberAccess,
                target.SemanticModel.Compilation.GetSemanticModel(routeMemberAccess.SyntaxTree)
            )!
        ) is not null;
    }

    public static string GetFriendlyName(ITypeSymbol symbol)
    {
        if (symbol.TypeKind is TypeKind.Interface)
            return symbol.Name.Remove(0, 1).Replace("Actor", string.Empty);

        return symbol.Name.Replace("Actor", string.Empty).Replace("Gateway", string.Empty)
            .Replace("Rest", string.Empty);
    }
}