using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public class InterfaceProxy : IGenerationTask<InterfaceProxy.GenerationTarget>
{
    public sealed class GenerationTarget(
        SemanticModel semanticModel,
        ClassDeclarationSyntax classDeclarationSyntax,
        Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> properties) : IEquatable<GenerationTarget>
    {
        public SemanticModel SemanticModel { get; } = semanticModel;
        public ClassDeclarationSyntax ClassDeclarationSyntax { get; } = classDeclarationSyntax;
        public Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> Properties { get; } = properties;

        public bool Equals(GenerationTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return ClassDeclarationSyntax.IsEquivalentTo(other.ClassDeclarationSyntax) &&
                   Properties.Count == other.Properties.Count &&
                   Properties.All(x =>
                       Properties.ContainsKey(x.Key) &&
                       Properties[x.Key].Count == x.Value.Count &&
                       Properties[x.Key].All(y => x.Value.Any(z => z.IsEquivalentTo(y)))
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

        var dict = ComputeProxiedTypeProperties(target, context.SemanticModel);

        if (dict.Count > 0)
        {
            return new GenerationTarget(
                context.SemanticModel,
                target,
                dict
            );
        }

        return null;
    }

    public static ImmutableHashSet<ISymbol> GetProxiedInterfaceMembers(ClassDeclarationSyntax syntax,
        INamedTypeSymbol symbol, SemanticModel semanticModel)
    {
        var proxiedProperties = ComputeProxiedTypeProperties(syntax, semanticModel);
        return proxiedProperties
            .SelectMany(kvp => GetTargetTypesToProxy(kvp.Value, semanticModel, symbol.Interfaces))
            .SelectMany(x => x.GetMembers().Where(IsValidMember))
            .ToImmutableHashSet(SymbolEqualityComparer.Default);
    }

    private static Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>> ComputeProxiedTypeProperties(
        ClassDeclarationSyntax type,
        SemanticModel semanticModel)
    {
        var result = new Dictionary<PropertyDeclarationSyntax, List<TypeOfExpressionSyntax>>();

        foreach (var member in type.Members)
        {
            if (member is not PropertyDeclarationSyntax property) continue;

            foreach (var attribute in property.AttributeLists.SelectMany(x => x.Attributes))
            {
                if (ModelExtensions.GetSymbolInfo(semanticModel, attribute).Symbol is not IMethodSymbol
                    attributeSymbol) continue;

                if (attributeSymbol.ContainingType.ToDisplayString() != "Discord.ProxyInterfaceAttribute") continue;

                result.Add(
                    property,
                    attribute.ArgumentList?.DescendantNodes().OfType<TypeOfExpressionSyntax>().ToList() ?? []
                );
            }
        }

        return result;
    }

    private static IEnumerable<INamedTypeSymbol> GetTargetTypesToProxy(
        List<TypeOfExpressionSyntax> types,
        SemanticModel semanticModel,
        IEnumerable<INamedTypeSymbol> defaultCase
    )
    {
        if (types.Count == 0)
            return defaultCase;

        return types
            .Select(x =>
            {
                var genericNode = x.DescendantNodes().OfType<GenericNameSyntax>().FirstOrDefault();

                return ModelExtensions.GetTypeInfo(semanticModel, genericNode ?? x.Type).Type;
            })
            .Where(x => x is not null)!
            .SelectMany<ITypeSymbol, ITypeSymbol>(x => [x, ..x!.AllInterfaces])
            .Distinct(SymbolEqualityComparer.Default)
            .OfType<INamedTypeSymbol>();
    }

    public void Execute(SourceProductionContext context, GenerationTarget? target, Logger logger)
    {
        if (target is null) return;

        if (ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax) is not
            INamedTypeSymbol targetSymbol)
            return;

        var syntax = SyntaxUtils.CreateSourceGenClone(target.ClassDeclarationSyntax);

        foreach (var kvp in target.Properties)
        {
            var property = target.SemanticModel.GetDeclaredSymbol(kvp.Key);

            if (property is null)
                continue;

            var targetInterfaces = GetTargetTypesToProxy(
                kvp.Value,
                target.SemanticModel,
                property.Type.AllInterfaces
            );

            foreach (var targetInterface in targetInterfaces)
            {
                if (AddProxiedMembers(ref syntax, targetInterface, targetSymbol, property, target.SemanticModel,
                        logger) == 0)
                    continue;

                var proxiedInterfaceTypeSyntax = SyntaxFactory.GenericName(
                    SyntaxFactory.Identifier("Discord.IProxied"),
                    SyntaxFactory.TypeArgumentList(
                        SyntaxFactory.SeparatedList([
                            SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString())
                        ])
                    )
                );
                syntax = syntax.AddBaseListTypes(
                    SyntaxFactory.SimpleBaseType(
                        proxiedInterfaceTypeSyntax
                    )
                );

                syntax = syntax.AddMembers(
                    SyntaxFactory.PropertyDeclaration(
                        [],
                        [],
                        SyntaxFactory.ParseTypeName(targetInterface.ToDisplayString()),
                        SyntaxFactory.ExplicitInterfaceSpecifier(
                            proxiedInterfaceTypeSyntax
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
                );
            }

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

            context.AddSource(
                $"InterfaceProxy/{target.ClassDeclarationSyntax.Identifier}_Proxied.g.cs",
                $$"""
                  namespace {{ModelExtensions.GetDeclaredSymbol(target.SemanticModel, target.ClassDeclarationSyntax)!.ContainingNamespace}};

                  #nullable enable

                  {{syntax.NormalizeWhitespace()}}

                  #nullable restore
                  """
            );
        }
    }

    private static bool IsValidMember(ISymbol symbol)
    {
        return symbol switch
        {
            IPropertySymbol {IsStatic: false, ExplicitInterfaceImplementations.Length: 0} => true,
            IMethodSymbol {IsStatic: false, MethodKind: MethodKind.Ordinary} => true,
            _ => false
        };
    }

    private static int AddProxiedMembers(
        ref ClassDeclarationSyntax classDeclarationSyntax,
        INamedTypeSymbol toProxy,
        INamedTypeSymbol targetSymbol,
        IPropertySymbol proxiedTo,
        SemanticModel semanticModel,
        Logger logger)
    {
        var count = 0;

        var castedSyntax = SyntaxFactory.ParenthesizedExpression(
            SyntaxFactory.BinaryExpression(
                SyntaxKind.AsExpression,
                SyntaxFactory.IdentifierName(proxiedTo.Name),
                SyntaxFactory.ParseTypeName(toProxy.ToDisplayString())
            )
        );

        var implementedMembers = TypeUtils.GetBaseTypesAndThis(proxiedTo.Type)
            .SelectMany(x => x.GetMembers())
            .Distinct(SymbolEqualityComparer.Default)
            .ToArray();

        foreach (var member in toProxy.GetMembers().Where(IsValidMember))
        {
            if (
                targetSymbol.FindImplementationForInterfaceMember(member) is not null ||
                Hierarchy.GetHierarchy(targetSymbol).Any(x =>
                    x.Type.FindImplementationForInterfaceMember(member) is not null)
            ) continue;

            var targetImplementation = implementedMembers
                .FirstOrDefault(x =>
                    x.Name == member.Name &&
                    (
                        (
                            x is IMethodSymbol fromMethod &&
                            member is IMethodSymbol toMethod &&
                            TypeUtils.TypeIsOfProvidedOrDescendant(fromMethod.ReturnType, toMethod.ReturnType,
                                semanticModel)
                        )
                        ||
                        (
                            x is IPropertySymbol fromProp &&
                            member is IPropertySymbol toProp &&
                            TypeUtils.TypeIsOfProvidedOrDescendant(fromProp.Type, toProp.Type, semanticModel)
                        )
                    )
                );

            var canProxyToSelfImplementation =
                targetImplementation is not null &&
                MemberUtils.CanOverride(targetImplementation, member, semanticModel.Compilation);

            var selfHasTargetImplementation = TypeUtils.GetBaseTypesAndThis(targetSymbol)
                .Any(typeSymbol =>
                    typeSymbol.GetMembers().Any(typeSymbolMember => MemberUtils.Conflicts(typeSymbolMember, member)) ||
                    (
                        !typeSymbol.Equals(targetSymbol, SymbolEqualityComparer.Default) &&
                        typeSymbol.DeclaringSyntaxReferences.Length > 0 &&
                        typeSymbol.DeclaringSyntaxReferences[0].GetSyntax() is ClassDeclarationSyntax
                            baseDeclarationSyntax &&
                        ComputeProxiedTypeProperties(
                                baseDeclarationSyntax,
                                semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax.SyntaxTree)
                            )
                            .Any(baseProxiedProperties =>
                            {
                                var baseSemantic =
                                    semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax.SyntaxTree);

                                var baseProperty = baseSemantic.GetDeclaredSymbol(baseProxiedProperties.Key);

                                if (baseProperty is null)
                                {
                                    logger.Warn(
                                        $"Couldn't find base semantic property {baseProxiedProperties.Key.Identifier} for {typeSymbol} ({targetSymbol})");
                                    return false;
                                }

                                return GetTargetTypesToProxy(
                                        baseProxiedProperties.Value,
                                        semanticModel.Compilation.GetSemanticModel(baseDeclarationSyntax.SyntaxTree),
                                        baseProperty.Type.AllInterfaces
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

            switch (member)
            {
                case IPropertySymbol property:
                    if (property.ExplicitInterfaceImplementations.Length > 0 || property.IsStatic)
                        break;

                    if (targetImplementation is IPropertySymbol targetProperty && !selfHasTargetImplementation)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.PropertyDeclaration(
                                [],
                                CreateAccessors(targetProperty.DeclaredAccessibility),
                                SyntaxFactory.IdentifierName(targetProperty.Type.ToDisplayString()),
                                null,
                                SyntaxFactory.Identifier(property.Name),
                                null,
                                SyntaxFactory.ArrowExpressionClause(
                                    SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        SyntaxFactory.IdentifierName(proxiedTo.Name),
                                        SyntaxFactory.IdentifierName(property.Name)
                                    )
                                ),
                                null,
                                SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                            )
                        );
                    }

                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.PropertyDeclaration(
                            [],
                            [],
                            SyntaxFactory.IdentifierName(property.Type.ToDisplayString()),
                            SyntaxFactory.ExplicitInterfaceSpecifier(
                                SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
                            ),
                            SyntaxFactory.Identifier(property.Name),
                            null,
                            SyntaxFactory.ArrowExpressionClause(
                                SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    canProxyToSelfImplementation
                                        ? SyntaxFactory.ThisExpression()
                                        : castedSyntax,
                                    SyntaxFactory.IdentifierName(property.Name)
                                )
                            ),
                            null,
                            SyntaxFactory.Token(SyntaxKind.SemicolonToken)
                        )
                    );

                    count++;
                    break;

                case IMethodSymbol method:
                    if (method.IsStatic || method.MethodKind is not MethodKind.Ordinary)
                        break;

                    if (targetImplementation is IMethodSymbol targetMethod && !selfHasTargetImplementation)
                    {
                        classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                            SyntaxFactory.MethodDeclaration(
                                [],
                                CreateAccessors(targetMethod.DeclaredAccessibility),
                                SyntaxFactory.IdentifierName(targetMethod.ReturnType.ToDisplayString()),
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
                                            SyntaxFactory.IdentifierName(proxiedTo.Name),
                                            SyntaxFactory.IdentifierName(method.Name)
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
                            )
                        );
                    }

                    classDeclarationSyntax = classDeclarationSyntax.AddMembers(
                        SyntaxFactory.MethodDeclaration(
                            [],
                            [],
                            SyntaxFactory.IdentifierName(method.ReturnType.ToDisplayString()),
                            SyntaxFactory.ExplicitInterfaceSpecifier(
                                SyntaxFactory.IdentifierName(toProxy.ToDisplayString())
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
                                SyntaxFactory.InvocationExpression(
                                    SyntaxFactory.MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        canProxyToSelfImplementation
                                            ? SyntaxFactory.ThisExpression()
                                            : castedSyntax,
                                        SyntaxFactory.IdentifierName(method.Name)
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
                        )
                    );
                    count++;
                    break;
            }
        }

        return count;
    }

    private static SyntaxTokenList CreateAccessors(Accessibility accessibility)
    {
        return SyntaxFactory.TokenList(accessibility switch
        {
            Accessibility.Private => [SyntaxFactory.Token(SyntaxKind.PrivateKeyword)],
            Accessibility.Protected => [SyntaxFactory.Token(SyntaxKind.ProtectedKeyword)],
            Accessibility.ProtectedOrInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
            ],
            Accessibility.Internal => [SyntaxFactory.Token(SyntaxKind.InternalKeyword)],
            Accessibility.ProtectedAndInternal =>
            [
                SyntaxFactory.Token(SyntaxKind.ProtectedKeyword),
                SyntaxFactory.Token(SyntaxKind.InternalKeyword)
            ],
            Accessibility.Public => [SyntaxFactory.Token(SyntaxKind.PublicKeyword)],
            _ => []
        });
    }
}
