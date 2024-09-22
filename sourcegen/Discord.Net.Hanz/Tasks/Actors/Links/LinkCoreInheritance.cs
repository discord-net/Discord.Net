using System.Collections.Immutable;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks.Actors;

public class LinkCoreInheritance
{
    public static void Process(Dictionary<LinksV2.GenerationTarget, TypeDeclarationSyntax> results, Logger logger)
    {
        if (results.Count == 0) return;

        // check for core
        var first = results.First().Key;
        if (first.Assembly is not LinksV2.AssemblyTarget.Core) return;

        var newTrees = results
            .Select(x =>
            {
                var tree = x.Key.Syntax.SyntaxTree.GetCompilationUnitRoot()
                    .WithUsings(SyntaxFactory.List(x.Key.Syntax.GetUsingDirectivesSyntax()))
                    .WithMembers(SyntaxFactory.List<MemberDeclarationSyntax>([
                        SyntaxFactory
                            .FileScopedNamespaceDeclaration(
                                SyntaxFactory.IdentifierName(x.Key.Actor.ContainingNamespace.ToDisplayString())
                            )
                            .WithMembers(
                                SyntaxFactory.List<MemberDeclarationSyntax>([
                                    x.Value
                                ])
                            )
                    ]))
                    .SyntaxTree;

                return tree.WithRootAndOptions(tree.GetRoot(), x.Key.Syntax.SyntaxTree.Options);
            })
            .ToArray();

        var newComp = first.SemanticModel.Compilation
            .AddSyntaxTrees(newTrees);

        // foreach (var tree in newTrees)
        // {
        //     var semantic = newComp.GetSemanticModel(tree);
        //     var targetLogger = logger.WithSemanticContext(semantic);
        //
        //     var symbol = semantic.GetDeclaredSymbol(
        //         tree.GetRoot().DescendantNodes().OfType<TypeDeclarationSyntax>().First()
        //     );
        //
        //     if (symbol is null) continue;
        //
        //     targetLogger.Log($" -|- {symbol.ToDisplayString()} : {symbol!.GetTypeMembers().Length}");
        // }

        foreach (var result in results.ToArray())
        {
            var targetLogger = logger.WithSemanticContext(result.Key.SemanticModel);

            var actor = newComp.GetTypeByMetadataName(result.Key.Actor.ToFullMetadataName());

            targetLogger.Log($"{result.Key.Actor} -> {actor}");

            if (actor is null) continue;

            var syntax = result.Value;
            ProcessTarget(result.Key, results, ref syntax, actor, newComp, targetLogger);
            results[result.Key] = syntax;
        }
    }

    private static void ProcessTarget(
        LinksV2.GenerationTarget target,
        Dictionary<LinksV2.GenerationTarget, TypeDeclarationSyntax> targets,
        ref TypeDeclarationSyntax syntax,
        INamedTypeSymbol parsedSymbol,
        Compilation compilation,
        Logger logger)
    {
        var directAncestors = targets
            .Where(x => parsedSymbol.Interfaces.Any(y => y.ToDisplayString() == x.Key.Actor.ToDisplayString()))
            .ToArray();

        if (directAncestors.Length == 0) return;

        logger.Log($"{parsedSymbol}: {directAncestors.Length} ancestors");

        // var reimplementedMembers = new Dictionary<
        //     INamedTypeSymbol,
        //     Dictionary<
        //         INamedTypeSymbol,
        //         HashSet<ISymbol>
        //     >
        // >(SymbolEqualityComparer.Default);

        var allTypeMembers = GetFullTypeMembersList(parsedSymbol)
            .ToImmutableHashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var ancestor in directAncestors)
        {
            var parsedAnscestor = compilation.GetTypeByMetadataName(ancestor.Key.Actor.ToFullMetadataName());

            if (parsedAnscestor is null) continue;

            logger.Log($"{parsedSymbol}: Processing {parsedAnscestor}");

            foreach (var typeMember in parsedAnscestor.GetTypeMembers())
            {
                logger.Log($" - {typeMember.ToDisplayString()}");
            }

            foreach (var type in allTypeMembers)
            {
                var path = TypeUtils.SelfAndContaingTypes(type)
                    .OfType<INamedTypeSymbol>()
                    .TakeWhile(x => !x.Equals(parsedSymbol, SymbolEqualityComparer.Default))
                    .Reverse()
                    .ToArray();

                var pathStr = string.Join(
                    ".",
                    path.Select(x => x.Name)
                );

                var anscestorPart = parsedAnscestor;
                var syntaxPart = syntax;

                foreach (var part in path)
                {
                    var lastValidAncestor = anscestorPart;
                    anscestorPart = anscestorPart.GetTypeMembers()
                        .FirstOrDefault(x =>
                            x.Name == part.Name &&
                            x.TypeArguments.Length == part.TypeArguments.Length
                        );

                    if (anscestorPart is null)
                    {
                        logger.Warn($"{parsedSymbol}: anscestor {parsedAnscestor} has no {part.Name} part");

                        foreach (var ancestorMember in lastValidAncestor.GetTypeMembers())
                        {
                            logger.Warn($" - {ancestorMember}");
                        }

                        goto end_anscestor;
                    }

                    syntaxPart = syntaxPart.Members.OfType<TypeDeclarationSyntax>()
                        .FirstOrDefault(x =>
                            x.Identifier.ValueText == part.Name &&
                            (x.TypeParameterList?.Parameters.Count ?? 0) == part.TypeArguments.Length
                        );

                    if (syntaxPart is null)
                    {
                        logger.Warn($"{parsedSymbol}: syntax has no {part}");
                        goto end_anscestor;
                    }
                }

                logger.Log($"{parsedSymbol}.{pathStr}: += {anscestorPart}");

                // add base
                var newPartSyntax = (TypeDeclarationSyntax) syntaxPart
                    .AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                anscestorPart.ToDisplayString()
                            )
                        )
                    );

                var implemented = new HashSet<string>();
                // create overrides
                var abstractMembers = GetAbstractMembers(type, target)
                    .ToArray();

                var anscestorAbstractMembers = GetAbstractMembers(anscestorPart, ancestor.Key)
                    .ToArray();

                foreach (var member in abstractMembers)
                {
                    if (implemented.Contains(member.ToDisplayString()))
                    {
                        continue;
                    }

                    // foreach (var baseLinkType in type.AllInterfaces.Where(allTypeMembers.Contains).Prepend(type))
                    // {
                    //     if
                    //     (
                    //         reimplementedMembers.TryGetValue(baseLinkType, out var impls)
                    //         &&
                    //         impls.TryGetValue(member.ContainingType, out var reimpls)
                    //         && reimpls.Contains(member)
                    //     )
                    //     {
                    //         logger.Log($"{parsedSymbol}.{pathStr}: Skipping {member.Name}: already impl'd");
                    //         goto end_member;
                    //     }
                    // }

                    var conflicting = anscestorAbstractMembers.FirstOrDefault(
                        x =>
                            MemberUtils.Conflicts(member, x) &&
                            !member.ContainingType.Equals(x.ContainingType, SymbolEqualityComparer.Default)
                    );

                    if (conflicting is null) continue;

                    logger.Log($"{parsedSymbol}: {ancestor.Key.Actor} -> {member} conflicts with {conflicting}");

                    // check if its generic
                    var memberType = MemberUtils.GetMemberType(member);
                    var conflictingType = MemberUtils.GetMemberType(conflicting);

                    if (memberType is null || conflictingType is null)
                    {
                        logger.Warn($"{parsedSymbol}: no member types: {memberType} | {conflictingType}");
                        goto end_anscestor;
                    }

                    // var ourVariant = type.AllInterfaces.FirstOrDefault(
                    //     x => x.Equals(member.ContainingType, SymbolEqualityComparer.Default)
                    // );

                    var canOverride =
                        (
                            // the generic parameter would resolve to the same value.
                            memberType is ITypeParameterSymbol &&
                            conflictingType is ITypeParameterSymbol &&
                            type.IsGenericType
                        )
                        ||
                        MemberUtils.CanOverride(member, conflicting, compilation)
                        ||
                        (
                            member is IMethodSymbol a &&
                            conflicting is IMethodSymbol b &&
                            compilation.GetTypeByMetadataName(a.ReturnType.ToDisplayString()) is ITypeSymbol aT &&
                            compilation.GetTypeByMetadataName(b.ReturnType.ToDisplayString()) is ITypeSymbol bT &&
                            MemberUtils.CanOverrideMethod(
                                aT,
                                MemberUtils.GetMemberName(a),
                                a.Parameters.ToList(),
                                bT,
                                MemberUtils.GetMemberName(b),
                                b.Parameters.ToList(),
                                compilation
                            )
                        );

                    if (canOverride)
                    {
                        logger.Log(" - overridable");
                        var memberSyntax =
                            member.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as MemberDeclarationSyntax;

                        if (memberSyntax is null)
                        {
                            logger.Warn(" - no syntax");
                            continue;
                        }

                        if (memberSyntax.Modifiers.IndexOf(SyntaxKind.NewKeyword) == -1)
                            memberSyntax = memberSyntax.AddModifiers(SyntaxFactory.Token(SyntaxKind.NewKeyword));

                        if (TypeUtils.SelfAndContaingTypes(member.ContainingType)
                            .Any(v => v is INamedTypeSymbol {IsGenericType: true}))
                        {
                            var generics = TypeUtils.SelfAndContaingTypes(member.ContainingType)
                                .OfType<INamedTypeSymbol>()
                                .Where(x => x.IsGenericType)
                                .SelectMany(t => t.TypeParameters.Select((x, i) =>
                                        (
                                            Name: x.Name,
                                            Filler: t.TypeArguments[i]
                                        )
                                    )
                                )
                                .ToDictionary(x => x.Name, x => x.Filler);

                            memberSyntax = memberSyntax.ReplaceNodes(
                                memberSyntax.DescendantNodes().OfType<IdentifierNameSyntax>(),
                                (old, node) =>
                                {
                                    if (generics.TryGetValue(node.Identifier.ValueText, out var generic))
                                        return node.WithIdentifier(
                                            SyntaxFactory.Identifier(
                                                generic.ToDisplayString()
                                            )
                                        );

                                    return node;
                                }
                            );
                        }

                        memberSyntax = memberSyntax.NormalizeWhitespace();

                        var ourOverload = FormatOverload(memberSyntax, member, memberType);
                        var theirOverload = FormatOverload(memberSyntax, conflicting, conflictingType);

                        if (ourOverload is null || theirOverload is null)
                        {
                            logger.Warn(" - no overload syntax generated");
                            goto end_member;
                        }

                        newPartSyntax = newPartSyntax
                            .WithOpenBraceToken(SyntaxFactory.Token(SyntaxKind.OpenBraceToken))
                            .WithCloseBraceToken(SyntaxFactory.Token(SyntaxKind.CloseBraceToken))
                            .WithSemicolonToken(default)
                            .AddMembers(
                                memberSyntax,
                                ourOverload,
                                theirOverload
                            );

                        // if (!reimplementedMembers.TryGetValue(type, out var impls))
                        //     reimplementedMembers[type] = impls = new(SymbolEqualityComparer.Default);
                        //
                        // if (!impls.TryGetValue(member.ContainingType, out var reimpls))
                        //     impls[member.ContainingType] = reimpls = new(SymbolEqualityComparer.Default);
                        //
                        // reimpls.Add(member);
                        implemented.Add(member.ToDisplayString());
                    }
                    else
                    {
                        var member2 = compilation.GetTypeByMetadataName(memberType.ToDisplayString());
                        var conflicting2 = compilation.GetTypeByMetadataName(conflictingType.ToDisplayString());

                        logger.Log($" - return:");
                        logger.Log($"   - ours: {memberType} | {member2}");
                        logger.Log($"   - theirs: {conflictingType} | {conflicting2}");
                        logger.Log($"   - result: {compilation.HasImplicitConversion(memberType, conflictingType)}");
                        logger.Log($"   - result2: {compilation.HasImplicitConversion(
                            member2,
                            conflicting2
                        )}");

                        if (member2 is not null)
                        {
                            foreach (var iface in member2.AllInterfaces)
                            {
                                logger.Log($"     - {iface}");
                            }
                        }
                    }

                    end_member: ;
                }

                if (!newPartSyntax.IsEquivalentTo(syntaxPart))
                {
                    syntax = syntax.ReplaceNode(syntaxPart, newPartSyntax.NormalizeWhitespace());
                }
            }

            end_anscestor: ;
        }
    }

    private static MemberDeclarationSyntax? FormatOverload(
        MemberDeclarationSyntax templateSyntax,
        ISymbol member,
        ITypeSymbol type)
    {
        switch (templateSyntax)
        {
            case MethodDeclarationSyntax methodSyntax:
                return methodSyntax
                    .WithModifiers([])
                    .WithReturnType(SyntaxFactory.ParseTypeName(type.ToDisplayString()))
                    .WithExplicitInterfaceSpecifier(
                        SyntaxFactory.ExplicitInterfaceSpecifier(
                            SyntaxFactory.IdentifierName(
                                member.ContainingType.ToDisplayString()
                            )
                        )
                    )
                    .WithParameterList(
                        methodSyntax.ParameterList.WithParameters(
                            SyntaxFactory.SeparatedList(
                                methodSyntax.ParameterList.Parameters
                                    .Select(x => x.WithDefault(null))
                            )
                        )
                    )
                    .WithBody(null)
                    .WithExpressionBody(
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.ParseExpression(
                                $"{MemberUtils.GetMemberName(member)}({
                                    string.Join(", ", methodSyntax.ParameterList.Parameters.Select(x => x.Identifier))
                                })"
                            )
                        )
                    )
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            case PropertyDeclarationSyntax propertySyntax:
                return propertySyntax
                    .WithModifiers([])
                    .WithType(SyntaxFactory.ParseTypeName(type.ToDisplayString()))
                    .WithInitializer(null)
                    .WithAccessorList(null)
                    .WithExplicitInterfaceSpecifier(
                        SyntaxFactory.ExplicitInterfaceSpecifier(
                            SyntaxFactory.IdentifierName(
                                member.ContainingType.ToDisplayString()
                            )
                        )
                    )
                    .WithExpressionBody(
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.IdentifierName(MemberUtils.GetMemberName(member))
                        )
                    )
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            case IndexerDeclarationSyntax indexerSyntax:
                return indexerSyntax
                    .WithModifiers([])
                    .WithType(SyntaxFactory.ParseTypeName(type.ToDisplayString()))
                    .WithAccessorList(null)
                    .WithExplicitInterfaceSpecifier(
                        SyntaxFactory.ExplicitInterfaceSpecifier(
                            SyntaxFactory.IdentifierName(
                                member.ContainingType.ToDisplayString()
                            )
                        )
                    )
                    .WithExpressionBody(
                        SyntaxFactory.ArrowExpressionClause(
                            SyntaxFactory.ParseExpression(
                                $"this[{
                                    string.Join(", ", indexerSyntax.ParameterList.Parameters.Select(x => x.Identifier))
                                }]"
                            )
                        )
                    )
                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            default: return null;
        }
    }

    private static IEnumerable<ISymbol> GetAbstractMembers(INamedTypeSymbol type, LinksV2.GenerationTarget target)
    {
        return type.AllInterfaces
            .Prepend(type)
            .Where(x =>
                !target.Actor.AllInterfaces.Any(y => y.ToDisplayString() == x.ToDisplayString())
            )
            .SelectMany(x => x.GetMembers())
            .Where(x => x switch
            {
                IMethodSymbol method => method is
                    {IsAbstract: true, MethodKind: MethodKind.Ordinary, ExplicitInterfaceImplementations.Length: 0},
                IPropertySymbol property => property is {IsSealed: false, ExplicitInterfaceImplementations.Length: 0},
                _ => false
            })
            .Distinct(SymbolEqualityComparer.Default);
    }

    private static IEnumerable<INamedTypeSymbol> GetFullTypeMembersList(INamedTypeSymbol symbol)
    {
        foreach (var member in symbol.GetTypeMembers())
        {
            yield return member;

            foreach (var child in GetFullTypeMembersList(member))
                yield return child;
        }
    }
}