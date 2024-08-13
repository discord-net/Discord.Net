using System.Collections.Immutable;
using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Text;

namespace Discord.Net.Hanz.Tasks;

public class Partials
{
    private static INamedTypeSymbol? _nullableType;

    private static ITypeSymbol GetNullableType(ITypeSymbol symbol, SemanticModel model)
    {
        _nullableType ??= model.Compilation.GetTypeByMetadataName("System.Nullable`1");

        if (_nullableType is null)
            throw new InvalidOperationException("Could not find System.Nullable type");

        if (symbol is {IsValueType: true, Name: "Nullable"} or
            {IsReferenceType: true, NullableAnnotation: NullableAnnotation.Annotated})
            return symbol;

        return symbol switch
        {
            {IsValueType: true, Name: not "Nullable"} => _nullableType.Construct(symbol),
            _ => symbol.WithNullableAnnotation(NullableAnnotation.Annotated)
        };
    }

    public static bool GeneratePartialForm(
        JsonModels.Context context,
        INamedTypeSymbol interfaceSymbol,
        InterfaceDeclarationSyntax interfaceSyntax,
        SemanticModel semanticModel,
        Logger logger)
    {
        var fileName = $"PartialInterfaces/{interfaceSymbol.ToFullMetadataName()}";

        if (context.DynamicSources.ContainsKey(fileName)) return true;

        var syntax = SyntaxUtils.CreateSourceGenClone(interfaceSyntax)
            .WithIdentifier(
                SyntaxFactory.Identifier(interfaceSymbol.Name.Insert(1, "Partial"))
            )
            .AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName($"Discord.Models.IPartial<{interfaceSymbol.ToDisplayString()}>")
                )
            );

        foreach
        (
            var property in interfaceSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x => x.ExplicitInterfaceImplementations.Length == 0)
        )
        {
            if (!TryGetSyntax<PropertyDeclarationSyntax>(property, out var propertySyntax))
                continue;

            var propertyType = property.Type;

            var isRequired = IsRequired(property);

            if (!isRequired && propertyType.NullableAnnotation is not NullableAnnotation.Annotated)
                propertyType = GetNullableType(propertyType, semanticModel);

            syntax = syntax.AddMembers(
                propertySyntax.WithType(
                    SyntaxFactory.ParseTypeName(propertyType.ToDisplayString())
                )
            );

            if (!isRequired)
            {
                syntax = syntax.AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"bool {property.Name}IsSpecified {{ get; }}"
                    )!
                );
            }
        }

        if (syntax.Members.Count == 0) return false;

        foreach
        (
            var property in interfaceSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x =>
                    x.ExplicitInterfaceImplementations.Length > 0
                )
        )
        {
            if (!TryGetSyntax<PropertyDeclarationSyntax>(property, out var propertySyntax))
                continue;

            if (CanUseExplicitInterfaceProperty(ref propertySyntax, property, semanticModel))
                syntax = syntax.AddMembers(propertySyntax);
        }

        foreach (var targetBase in interfaceSymbol.Interfaces)
        {
            var name = targetBase.ToString();

            if (
                HasPartialAttribute(targetBase) &&
                TryGetSyntax<InterfaceDeclarationSyntax>(targetBase, out var baseSyntax) &&
                GeneratePartialForm(context, targetBase, baseSyntax, semanticModel, logger)
            )
            {
                name = name.Insert(1, "Partial");
            }

            syntax = syntax.AddBaseListTypes(
                SyntaxFactory.SimpleBaseType(
                    SyntaxFactory.ParseTypeName(name)
                )
            );
        }

        if (!context.DynamicSources.ContainsKey(fileName))
        {
            context.DynamicSources[fileName] = SourceText.From(
                $"""
                 {interfaceSyntax.GetFormattedUsingDirectives()}

                 namespace {interfaceSymbol.ContainingNamespace};

                 {syntax.NormalizeWhitespace()}
                 """,
                Encoding.UTF8
            );
        }

        return true;
    }

    private static bool CanUseExplicitInterfaceProperty(
        ref PropertyDeclarationSyntax syntax,
        IPropertySymbol symbol,
        SemanticModel semanticModel
    )
    {
        if (syntax.ExpressionBody?.Expression is not IdentifierNameSyntax identifierNameSyntax) return true;

        if (
            symbol.ContainingType
                    .GetMembers(identifierNameSyntax.Identifier.ValueText)
                    .FirstOrDefault()
                is not IPropertySymbol targetProp
            ||
            IsRequired(targetProp)
        ) return true;

        var targetPropActualType = GetNullableType(targetProp.Type, semanticModel);

        if (semanticModel.Compilation.HasImplicitConversion(targetPropActualType, targetProp.Type))
            return false;

        syntax = syntax.WithExpressionBody(
            SyntaxFactory.ArrowExpressionClause(
                SyntaxFactory.BinaryExpression(
                    SyntaxKind.CoalesceExpression,
                    identifierNameSyntax,
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.DefaultLiteralExpression
                    )
                )
            )
        );

        return true;
    }

    private static bool HasPartialAttribute(ITypeSymbol symbol)
        => symbol
            .GetAttributes()
            .Any(x => x
                .AttributeClass
                ?.ToDisplayString() == "Discord.HasPartialVariantAttribute"
            );

    private static IEnumerable<IPropertyReferenceOperation> GetPropertyReferences(
        SyntaxNode node,
        SemanticModel semanticModel
    ) => node.DescendantNodes()
        .OfType<IdentifierNameSyntax>()
        .Select(x => semanticModel.GetOperation(x))
        .OfType<IPropertyReferenceOperation>();

    private static bool ImplementPartialInterfaceProperty(
        ITypeSymbol symbol,
        string interfaceName,
        IPropertySymbol property,
        IPropertySymbol implementation,
        SemanticModel semanticModel,
        List<MemberDeclarationSyntax> addedMembers,
        HashSet<IPropertySymbol> generatedOverloads,
        Logger logger
    )
    {
        var isFromPartial = HasPartialAttribute(property.ContainingType);

        var interfacePropertyType = isFromPartial ? GetNullableType(property.Type, semanticModel) : property.Type;

        var canImplement = implementation.ExplicitInterfaceImplementations.Length == 0;

        if (
            !canImplement &&
            symbol.GetMembers(MemberUtils.GetMemberName(property))
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault(x => x.ExplicitInterfaceImplementations.Length == 0)
                is { } match
        )
        {
            implementation = match;
            canImplement = true;
        }

        if (canImplement)
        {
            // implicitly implemented, we can just point to our prop
            if (IsRequired(implementation))
                return true;

            var optionalAccessor = property.Type switch
            {
                {IsValueType: true, Name: not "Nullable"} => $"{implementation.Name}.ToNullable()",
                _ => $"~{implementation.Name}"
            };

            addedMembers.Add(
                SyntaxFactory.ParseMemberDeclaration(
                    $"{interfacePropertyType} {interfaceName}.{property.Name} => {optionalAccessor};"
                )!
            );

            if (isFromPartial)
                addedMembers.Add(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"bool {interfaceName}.{property.Name}IsSpecified => {implementation.Name}.IsSpecified;"
                    )!
                );

            generatedOverloads.Add(property);

            return true;
        }

        if (
            TryGetSyntax<PropertyDeclarationSyntax>(implementation, out var explicitPropertySyntax) &&
            explicitPropertySyntax.ExpressionBody?.Expression is not null)
        {
            var typeInfo = semanticModel.GetTypeInfo(explicitPropertySyntax.ExpressionBody.Expression);

            var type = typeInfo.ConvertedType ?? typeInfo.Type;

            if (type is null)
                goto failed_condition;

            var propertyReferences = GetPropertyReferences(
                    explicitPropertySyntax.ExpressionBody.Expression,
                    semanticModel
                )
                .ToArray();

            if (propertyReferences.Length == 0) goto failed_condition;

            var dependants = propertyReferences
                .Where(x =>
                    !IsRequired(x.Property)
                )
                .ToDictionary(x => (IdentifierNameSyntax) x.Syntax);

            var checkDependantsForSpecificity =
                dependants.Count == 0
                    ? SyntaxFactory.LiteralExpression(SyntaxKind.TrueLiteralExpression)
                    : dependants
                        .Keys
                        .Select(ExpressionSyntax (x) =>
                            SyntaxFactory.MemberAccessExpression(
                                SyntaxKind.SimpleMemberAccessExpression,
                                x,
                                SyntaxFactory.IdentifierName("IsSpecified")
                            )
                        )
                        .Aggregate((a, b) =>
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.LogicalAndExpression,
                                a,
                                b
                            )
                        );

            if (interfacePropertyType.Equals(type, SymbolEqualityComparer.Default) && !isFromPartial)
            {
                logger.Log($"{symbol}: {property} implementations is reusable, {interfacePropertyType} <> {type}");
                addedMembers.Add(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"{interfacePropertyType} {interfaceName}.{property.Name} => {explicitPropertySyntax.ExpressionBody.Expression};"
                    )!
                );

                if (!IsRequired(property) && isFromPartial)
                {
                    addedMembers.Add(
                        SyntaxFactory.ParseMemberDeclaration(
                            $"bool {interfaceName}.{property.Name}IsSpecified => {checkDependantsForSpecificity};"
                        )!
                    );
                }

                generatedOverloads.Add(property);

                return true;
            }

            var expressionSyntax = explicitPropertySyntax.ExpressionBody
                .ReplaceNodes(
                    explicitPropertySyntax.ExpressionBody
                        .DescendantNodes()
                        .OfType<PrefixUnaryExpressionSyntax>()
                        .Where(x => x.IsKind(SyntaxKind.BitwiseNotExpression)),
                    (node, _) =>
                    {
                        var operandType = semanticModel.GetTypeInfo(node.Operand).Type;
                        if (operandType is null)
                        {
                            logger.Log($"{symbol}: {property} > returning default, type not found");
                            return node;
                        }

                        if (
                            operandType is INamedTypeSymbol {IsValueType: true, Name: "Optional"} named &&
                            named.TypeArguments[0] is {IsValueType: true, Name: not "Nullable"}
                        )
                        {
                            logger.Log($"{symbol}: {property} > returning 'ToNullable' invocation");

                            type = GetNullableType(type, semanticModel);

                            return SyntaxFactory.InvocationExpression(
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    node.Operand,
                                    SyntaxFactory.IdentifierName("ToNullable")
                                )
                            );
                        }

                        logger.Log($"{symbol}: {property} > returning default");
                        return node;
                    }
                );

            if (semanticModel.Compilation.HasImplicitConversion(type, interfacePropertyType))
            {
                if (dependants.Count == 0)
                {
                    logger.Log($"{symbol}: {property} conversion {type} -> {interfacePropertyType} with no dependants");

                    addedMembers.Add(
                        SyntaxFactory.ParseMemberDeclaration(
                            $"{interfacePropertyType} {interfaceName}.{property.Name} => {expressionSyntax};"
                        )!
                    );

                    generatedOverloads.Add(property);

                    return true;
                }

                logger.Log(
                    $"{symbol}: {property} conversion {type} -> {interfacePropertyType} with {dependants.Count} dependants");

                addedMembers.Add(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"{interfacePropertyType} {interfaceName}.{property.Name} => {expressionSyntax.Expression};"
                    )!
                );

                if (!IsRequired(property) && isFromPartial)
                {
                    addedMembers.Add(
                        SyntaxFactory.ParseMemberDeclaration(
                            $"bool {interfaceName}.{property.Name}IsSpecified => {checkDependantsForSpecificity};"
                        )!
                    );
                }

                generatedOverloads.Add(property);

                return true;
            }

            if (
                interfacePropertyType.IsValueType
                &&
                type is INamedTypeSymbol
                {
                    IsValueType: true, Name: "Nullable"
                } nullable
                &&
                semanticModel.Compilation.HasImplicitConversion(nullable.TypeArguments[0], interfacePropertyType)
            )
            {
                logger.Log($"{symbol}: {property} {type} -> {interfacePropertyType} via nullable conversion");

                addedMembers.Add(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"{interfacePropertyType} {interfaceName}.{property.Name} => {expressionSyntax.Expression};"
                    )!
                );

                if (!IsRequired(property) && isFromPartial)
                {
                    addedMembers.Add(
                        SyntaxFactory.ParseMemberDeclaration(
                            $"bool {interfaceName}.{property.Name}IsSpecified => {checkDependantsForSpecificity};"
                        )!
                    );
                }

                generatedOverloads.Add(property);

                return true;
            }
        }

        failed_condition:
        logger.Warn(
            $"{symbol}: Unable to resolve implementation for {property}, skipping partial base {interfaceName}"
        );

        return false;
    }

    public static void ProcessJsonModel(
        ITypeSymbol symbol,
        SemanticModel semanticModel,
        JsonModels.Context jsonContext,
        Logger logger
    )
    {
        if (!HasPartialAttribute(symbol))
            return;

        if (
            symbol.DeclaringSyntaxReferences
                .FirstOrDefault()
                ?.GetSyntax()
            is not TypeDeclarationSyntax typeDeclarationSyntax
        ) return;

        var optionalType = semanticModel.Compilation.GetTypeByMetadataName("Discord.Optional`1");

        if (optionalType is null)
        {
            logger.Warn("Couldn't find optional type");
            return;
        }

        var partialFullName = ToPartialTypeName(symbol);

        var syntax = SyntaxFactory.ClassDeclaration(
            [],
            typeDeclarationSyntax.Modifiers,
            SyntaxFactory.Identifier($"Partial{symbol.Name}"),
            null,
            SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList((BaseTypeSyntax[])
                [
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName($"IPartial<{symbol.ToDisplayString()}>")
                    )
                ])
            ),
            [],
            []
        );

        var optionalProperties = new List<IPropertySymbol>();

        foreach
        (
            var property in symbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(JsonModels.IsJsonProperty)
        )
        {
            if (!TryGetSyntax<PropertyDeclarationSyntax>(property, out var propertySyntax))
                continue;

            var propertyType = property.Type;

            if (!IsRequired(property) && propertyType.Name is not "Optional")
            {
                propertyType = optionalType.Construct(propertyType);
            }

            if (propertyType.Name is "Optional")
                optionalProperties.Add(property);

            var propSyntax = propertySyntax
                .WithType(SyntaxFactory.ParseTypeName(propertyType.ToDisplayString()));

            if (
                propSyntax.Modifiers
                    .FirstOrDefault(x =>
                        x.IsContextualKeyword() &&
                        x.ValueText == "required"
                    )
                is {RawKind: > 0} requiredToken
            )
            {
                propSyntax = propSyntax.WithModifiers(propSyntax.Modifiers.Remove(requiredToken));
            }

            syntax = syntax.AddMembers(
                propSyntax
            );
        }

        if (syntax.Members.Count == 0) return;

        AddApplyToMethod(ref syntax, symbol, optionalProperties);

        var generatedOverloads = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);
        var implementedInterfaces = new HashSet<INamedTypeSymbol>(SymbolEqualityComparer.Default);

        if (symbol.BaseType is {Name: not "Object"})
        {
            if (!HasPartialAttribute(symbol.BaseType))
            {
                logger.Warn($"{symbol} base type '{symbol.BaseType.Name}' is not marked as partial");
                return;
            }

            var baseList = syntax.BaseList ?? SyntaxFactory.BaseList();

            syntax = syntax.WithBaseList(
                baseList.WithTypes(
                    baseList.Types.Insert(
                        0,
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName(
                                ToPartialTypeName(symbol.BaseType)
                            )
                        )
                    )
                )
            );
        }

        foreach (var modelInterface in symbol.AllInterfaces
                     .Where(x => IsModelInterface(x) || IsModelSourceLike(x)))
        {
            if (!TryGetSyntax<InterfaceDeclarationSyntax>(modelInterface, out var modelInterfaceSyntax))
                continue;

            var interfaceName = modelInterface.ToDisplayString();

            if (IsModelSourceLike(modelInterface))
            {
                if (!modelInterface.IsGenericType)
                {
                    if (modelInterface.GetMembers("GetDefinedModels").FirstOrDefault() is not IMethodSymbol method)
                        continue;

                    var links = symbol.GetMembers()
                        .OfType<IPropertySymbol>()
                        .Where(x => x.ExplicitInterfaceImplementations.Length == 0)
                        .Where(IsModelLinkProperty);

                    syntax = syntax.AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              public IEnumerable<IEntityModel> GetDefinedModels()
                              {
                                  {{
                                      string.Join(
                                          "\n",
                                          links
                                              .Select(x =>
                                                  {
                                                      var access = x.Type.Name is "Optional" or "Nullable"
                                                          ? $"{x.Name}.Value"
                                                          : x.Name;

                                                      var yieldValue = IsIterable(x.Type)
                                                          ? $"foreach (var item in {access}) yield return item;"
                                                          : $"yield return {access};";


                                                      if (optionalProperties.Contains(x))
                                                          return $"if ({x.Name}.IsSpecified) {yieldValue}";

                                                      if (
                                                          x.Type
                                                          is
                                                          {
                                                              IsValueType: true,
                                                              Name: "Nullable"}
                                                          or
                                                          {
                                                              IsReferenceType: true,
                                                              NullableAnnotation: NullableAnnotation.Annotated
                                                          }
                                                      ) return $"if ({x.Name} is not null) {yieldValue}";

                                                      return yieldValue;
                                                  }
                                              )
                                      )
                                  }}
                              }
                              """
                        )!
                    );

                    logger.Log($"{symbol}: Implementing {modelInterface} via non-generic model source");
                    implementedInterfaces.Add(modelInterface);
                    continue;
                }

                var interfaceMember = modelInterface.GetMembers().FirstOrDefault();

                if (interfaceMember is null)
                    continue;

                var targetImpl = symbol.FindImplementationForInterfaceMember(interfaceMember);

                if (targetImpl is null) continue;

                var members = new List<MemberDeclarationSyntax>();
                switch (interfaceMember, targetImpl)
                {
                    case (IPropertySymbol interfaceProperty, IPropertySymbol targetProperty):
                        var modelTargetType = modelInterface.TypeArguments[0];
                        if (modelTargetType.NullableAnnotation is not NullableAnnotation.Annotated)
                            modelTargetType = modelTargetType.WithNullableAnnotation(NullableAnnotation.Annotated);

                        interfaceName = modelInterface
                            .ConstructedFrom
                            .Construct(modelTargetType)
                            .ToDisplayString();

                        if (
                            !ImplementPartialInterfaceProperty(
                                symbol,
                                interfaceName,
                                interfaceProperty,
                                targetProperty,
                                semanticModel,
                                members,
                                generatedOverloads,
                                logger
                            )
                        ) continue;
                        break;
                    case (IMethodSymbol interfaceMethod, IMethodSymbol targetMethod):
                        if (!TryGetSyntax<MethodDeclarationSyntax>(targetMethod, out var targetMethodSyntax))
                            continue;

                        var methodBody = (SyntaxNode?) targetMethodSyntax.ExpressionBody ?? targetMethodSyntax.Body;
                        if (methodBody is null) continue;

                        var referencedProperties = GetPropertyReferences(
                            methodBody,
                            semanticModel
                        );

                        var verified = new HashSet<IPropertySymbol>(SymbolEqualityComparer.Default);

                        var statements = new List<string>();

                        foreach (var reference in referencedProperties)
                        {
                            if (!verified.Add(reference.Property)) continue;

                            var access = reference.Property.Type.Name is "Optional" or "Nullable"
                                ? $"{reference.Property.Name}.Value"
                                : reference.Property.Name;

                            var yieldValue = IsIterable(reference.Property.Type)
                                ? $"foreach (var item in {access}) yield return item;"
                                : $"yield return {access};";


                            if (optionalProperties.Contains(reference.Property))
                            {
                                statements.Add($"if ({reference.Property.Name}.IsSpecified) {yieldValue}");
                                continue;
                            }

                            if (
                                reference.Property.Type
                                is
                                {
                                    IsValueType: true,
                                    Name: "Nullable"
                                }
                                or
                                {
                                    IsReferenceType: true,
                                    NullableAnnotation: NullableAnnotation.Annotated
                                }
                            )
                            {
                                statements.Add($"if ({reference.Property.Name} is not null) {yieldValue}");
                                continue;
                            }

                            statements.Add(yieldValue);
                        }

                        var body = statements.Count == 0 ? "return [];" : string.Join("\n", statements);

                        members.Add(
                            SyntaxFactory.ParseMemberDeclaration(
                                $"{interfaceMethod.ReturnType} {modelInterface.ToDisplayString()}.GetModels() {{ {body} }}"
                            )!
                        );
                        break;
                    default: continue;
                }

                syntax = syntax
                    .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName)))
                    .AddMembers(members.ToArray());

                continue;
            }

            if (HasPartialAttribute(modelInterface))
            {
                if (!GeneratePartialForm(jsonContext, modelInterface, modelInterfaceSyntax, semanticModel, logger))
                    continue;

                interfaceName = ToPartialTypeName(modelInterface);
            }

            var addedMembers = new List<MemberDeclarationSyntax>();

            foreach
            (
                var property in modelInterface
                    .GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(x => x.ExplicitInterfaceImplementations.Length == 0)
            )
            {
                if (symbol.FindImplementationForInterfaceMember(property) is not IPropertySymbol implementation)
                    continue;

                if (!implementation.ContainingType.Equals(symbol, SymbolEqualityComparer.Default))
                    continue;

                if (
                    ImplementPartialInterfaceProperty(
                        symbol,
                        interfaceName,
                        property,
                        implementation,
                        semanticModel,
                        addedMembers,
                        generatedOverloads,
                        logger
                    )
                ) continue;

                goto end_iter;
            }

            syntax = syntax
                .AddBaseListTypes(SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(interfaceName)))
                .AddMembers(addedMembers.ToArray());

            if (HasPartialAttribute(modelInterface))
            {
                syntax = syntax.AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $$"""
                          void Discord.Models.IPartial<{{modelInterface}}>.ApplyTo({{modelInterface}} model)
                          {
                              if(model is {{symbol}} ourModel)
                                  ApplyTo(ourModel);
                          }
                          """
                    )!
                );

                jsonContext.AdditionalConverters.Add(
                    $"Discord.Converters.ModelInterfaceConverter<{interfaceName}, {partialFullName}>"
                );

                jsonContext.ContextAttributes.Add(
                    $"[System.Text.Json.Serialization.JsonSerializable(typeof({interfaceName}))]"
                );
                jsonContext.ContextAttributes.Add(
                    $"[System.Text.Json.Serialization.JsonSerializable(typeof(IEnumerable<{interfaceName}>))]"
                );
            }

            logger.Log($"{symbol}: Implementing {modelInterface}");
            implementedInterfaces.Add(modelInterface);

            end_iter: ;
        }

        foreach
        (
            var unimplementedExplicitProperty in symbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(x =>
                    x.ExplicitInterfaceImplementations.Length > 0 &&
                    !generatedOverloads.Contains(x.ExplicitInterfaceImplementations[0]))
        )
        {
            logger.Log(
                $"{symbol}: Unimplemented explicit implementation in source model: {unimplementedExplicitProperty} ({unimplementedExplicitProperty.ExplicitInterfaceImplementations[0].ContainingType})");

            if (
                !implementedInterfaces.Contains(
                    unimplementedExplicitProperty.ExplicitInterfaceImplementations[0].ContainingType
                )
            )
            {
                logger.Log($" - skipped, not implemented by us");
                continue;
            }

            if (!TryGetSyntax<PropertyDeclarationSyntax>(unimplementedExplicitProperty, out var propertySyntax))
            {
                logger.Log($" - skipped, no syntax");
                continue;
            }

            syntax = syntax.AddMembers(
                propertySyntax.WithLeadingTrivia(
                    SyntaxFactory.Comment("// unresolved explicit")
                )
            );
        }

        var filename = $"Partials/{symbol.ToFullMetadataName()}";
        if (!jsonContext.DynamicSources.ContainsKey(filename))
            jsonContext.DynamicSources[filename] = SourceText.From(
                $"""
                 {typeDeclarationSyntax.GetFormattedUsingDirectives()}

                 namespace {symbol.ContainingNamespace};

                 {syntax.NormalizeWhitespace()}
                 """,
                Encoding.UTF8
            );

        jsonContext.ContextAttributes.Add(
            $"[System.Text.Json.Serialization.JsonSerializable(typeof({partialFullName}))]"
        );
        jsonContext.ContextAttributes.Add(
            $"[System.Text.Json.Serialization.JsonSerializable(typeof(IEnumerable<{partialFullName}>))]"
        );
    }

    private static bool IsModelLinkProperty(IPropertySymbol property)
        => SearchIsModelType(property.Type);

    private static bool SearchIsModelType(ITypeSymbol symbol)
    {
        return symbol switch
        {
            INamedTypeSymbol {IsGenericType: true, Name: "Optional" or "IEnumerable"} generic
                => SearchIsModelType(generic.TypeArguments[0]),
            INamedTypeSymbol named => IsModelInterface(named),
            _ => false
        };
    }

    private static string ToPartialTypeName(ITypeSymbol symbol)
    {
        var name = symbol.TypeKind is TypeKind.Interface
            ? $"{symbol.ContainingNamespace}.{symbol.Name.Insert(1, "Partial")}"
            : $"{symbol.ContainingNamespace}.Partial{symbol.Name}";

        if (symbol is INamedTypeSymbol {IsGenericType: true} generic)
            name = $"{name}<{string.Join(", ", generic.TypeArguments.Select(x => x.ToDisplayString()))}>";

        return name;
    }

    private static bool TryGetSyntax<TSyntax>(ISymbol symbol, out TSyntax syntax)
        where TSyntax : SyntaxNode
    {
        return (
            syntax = (symbol.DeclaringSyntaxReferences
                    .FirstOrDefault()
                    ?.GetSyntax()
                as TSyntax)!
        ) is not null;
    }

    private static bool IsModelInterface(INamedTypeSymbol symbol)
        => symbol.Name is "IEntityModel" || symbol.AllInterfaces.Any(x => x.Name is "IEntityModel");

    private static void AddApplyToMethod(
        ref ClassDeclarationSyntax syntax,
        ITypeSymbol model,
        IEnumerable<IPropertySymbol> optionalProperties)
    {
        syntax = syntax.AddMembers(
            SyntaxFactory.ParseMemberDeclaration(
                $$"""
                    public void ApplyTo({{model.ToDisplayString()}} model)
                    {  
                        {{
                            string.Join(
                                "\n",
                                optionalProperties
                                    .Select(x =>
                                        $$"""
                                          if({{x.Name}}.IsSpecified) model.{{x.Name}} = {{x.Name}}.Value;
                                          """
                                    )
                            )
                        }}
                    }
                  """
            )!
        );
    }

    private static bool IsIterable(ITypeSymbol symbol)
    {
        return symbol switch
        {
            {
                SpecialType:
                SpecialType.System_Array or
                SpecialType.System_Collections_IEnumerable or
                SpecialType.System_Collections_Generic_IEnumerable_T or
                SpecialType.System_Collections_Generic_ICollection_T or
                SpecialType.System_Collections_Generic_IList_T or
                SpecialType.System_Collections_Generic_IReadOnlyCollection_T or
                SpecialType.System_Collections_Generic_IReadOnlyList_T
            } => true,
            _ => symbol.AllInterfaces.Any(x => x.Name is "IEnumerable")
        };
    }

    private static bool IsModelSourceLike(INamedTypeSymbol symbol)
        => symbol.Name is "IModelSource" or "IModelSourceOf" or "IModelSourceOfMultiple";

    private static bool IsRequired(IPropertySymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Any(x =>
                x.AttributeClass?.ToDisplayString()
                    is
                    "System.Text.Json.Serialization.JsonRequiredAttribute"
                    or
                    "Discord.PartialIgnoreAttribute"
            );
    }
}