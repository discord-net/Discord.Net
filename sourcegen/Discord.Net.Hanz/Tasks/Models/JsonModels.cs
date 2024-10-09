using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace Discord.Net.Hanz.Tasks;

public static class JsonModels
{
    public sealed class JsonModelTarget(
        ClassDeclarationSyntax syntax,
        ITypeSymbol typeSymbol,
        SemanticModel semanticModel) : IEquatable<JsonModelTarget>
    {
        public ClassDeclarationSyntax Syntax { get; } = syntax;
        public ITypeSymbol TypeSymbol { get; } = typeSymbol;
        public SemanticModel SemanticModel { get; } = semanticModel;

        public bool Equals(JsonModelTarget? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return TypeSymbol.Equals(other.TypeSymbol, SymbolEqualityComparer.Default);
        }

        public override bool Equals(object? obj) =>
            ReferenceEquals(this, obj) || obj is JsonModelTarget other && Equals(other);

        public override int GetHashCode() => SymbolEqualityComparer.Default.GetHashCode(TypeSymbol);

        public static bool operator ==(JsonModelTarget? left, JsonModelTarget? right) => Equals(left, right);

        public static bool operator !=(JsonModelTarget? left, JsonModelTarget? right) => !Equals(left, right);
    }

    public static JsonModelTarget? GetTarget(GeneratorSyntaxContext context)
    {
        if (context.Node is not ClassDeclarationSyntax syntax) return null;

        if (context.SemanticModel.GetDeclaredSymbol(syntax) is not ITypeSymbol typeSymbol) return null;

        return new JsonModelTarget(syntax, typeSymbol, context.SemanticModel);
    }

    public sealed class Context(
        ClassDeclarationSyntax resolver,
        HashSet<string> contextAttributes,
        HashSet<ITypeSymbol> modelTypesForContext,
        JsonModelTarget[] targets
    )
    {
        public ClassDeclarationSyntax Resolver = resolver;
        public HashSet<string> ContextAttributes { get; } = contextAttributes;
        public HashSet<ITypeSymbol> ModelTypesForContext { get; } = modelTypesForContext;
        public HashSet<string> AdditionalConverters { get; } = new();
        public JsonModelTarget[] Targets { get; } = targets;

        public HashSet<string> NoContextTypeInfos = new();

        public HashSet<ITypeSymbol> RequestedNoConverterTypeInfos = new(SymbolEqualityComparer.Default);

        public Dictionary<string, SourceText> DynamicSources = new();
    }

    public static string GetJsonPropertyName(IPropertySymbol property)
    {
        var nameAttribute = property.GetAttributes()
            .FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == "System.Text.Json.Serialization.JsonPropertyNameAttribute"
            );

        if (nameAttribute is null)
            return property.Name;

        return (nameAttribute.ConstructorArguments[0].Value as string)!;
    }

    private static IEnumerable<JsonModelTarget> FormatGenericTarget(
        JsonModelTarget target, 
        INamedTypeSymbol named,
        Logger logger)
    {
        var variance = new List<ITypeSymbol[]>();

        foreach (var genericParameter in named.TypeParameters)
        {
            var oneOf = genericParameter.GetAttributes()
                .FirstOrDefault(x => x.AttributeClass?.Name == "OneOfAttribute")
                ?.ConstructorArguments
                .FirstOrDefault()
                .Values.Select(x => (ITypeSymbol) x.Value!)
                .ToArray();

            if (oneOf is null || oneOf.Length == 0) yield break;

            variance.Add(oneOf);
        }

        var indexMaps = variance.Select(x => x.Length).ToArray();

        for (var i = 0; i != indexMaps.Sum(); i++)
        {
            var symbol = named.Construct(
                variance
                    .Select((x, pos) =>
                    {
                        if (i == 0)
                            return x[0];
                        
                        if (pos == 0)
                            return x[i % x.Length];

                        var offset = indexMaps.Take(pos).Sum();
                        if (i < offset || offset == 0)
                            return x[0];

                        return x[(int) Math.Floor((double) i / offset)];
                    })
                    .ToArray()
            );
            
            logger.Log($"{target.TypeSymbol}: {i + 1}/{indexMaps.Sum()} -> {symbol}");

            yield return new JsonModelTarget(target.Syntax, symbol, target.SemanticModel);
        }
    }

    public static void Execute(
        SourceProductionContext context,
        (ImmutableArray<JsonModelTarget?> Left, Compilation Right) input,
        string? projectName,
        string? rootNamespace,
        Logger logger)
    {
        var (potentialTargets, compilation) = input;

        var modelInterfaces =
            new Dictionary<ITypeSymbol, HashSet<Hierarchy.SortedHierarchySymbol>>(SymbolEqualityComparer.Default);

        var contextAttributes = new HashSet<string>();
        var modelTypesForContext = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        var resolverSyntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword),
                SyntaxFactory.Token(SyntaxKind.PartialKeyword)
            ),
            SyntaxFactory.Identifier("ModelJsonContext"),
            null,
            SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList((BaseTypeSyntax[])
                [
                    SyntaxFactory.SimpleBaseType(SyntaxFactory.IdentifierName("JsonSerializerContext"))
                ])
            ),
            [],
            []
        );

        var targets = potentialTargets
            .Where(x => x is not null && IsSpeculativeJsonModel(x.TypeSymbol))
            .Cast<JsonModelTarget>()
            .SelectMany(x =>
            {
                if (x.TypeSymbol is not INamedTypeSymbol {IsGenericType: true} generic)
                    return [x];

                return FormatGenericTarget(x, generic, logger);
            })
            .ToArray();

        var jsonContext = new Context(
            resolverSyntax,
            contextAttributes,
            modelTypesForContext,
            targets
        );

        foreach (var target in targets)
        {
            logger.Log($"Processing model {target.TypeSymbol}");

            HashSet<Hierarchy.SortedHierarchySymbol>? interfaces = null;

            foreach (var modelInterface in GetModelInterfaces(target.TypeSymbol))
            {
                if (interfaces is null && !modelInterfaces.TryGetValue(target.TypeSymbol, out interfaces))
                    modelInterfaces[target.TypeSymbol] = interfaces = new();

                interfaces.Add(modelInterface);
            }

            var attributes = new HashSet<string>()
            {
                $"[System.Text.Json.Serialization.JsonSerializable(typeof({target.TypeSymbol.ToDisplayString()}))]",
                $"[System.Text.Json.Serialization.JsonSerializable(typeof(IEnumerable<{target.TypeSymbol.ToDisplayString()}>))]",
            };

            contextAttributes.UnionWith(attributes);

            modelTypesForContext.Add(target.TypeSymbol);

            ExtendedModel.Process(
                target.TypeSymbol,
                target.SemanticModel,
                jsonContext,
                context,
                logger
            );

            DiscriminatedUnion.Process(
                target.TypeSymbol,
                target.SemanticModel,
                jsonContext,
                context,
                logger
            );

            Partials.ProcessJsonModel(
                target.TypeSymbol,
                target.SemanticModel,
                jsonContext,
                logger
            );
        }

        if (modelTypesForContext.Count == 0)
            return;

        // foreach (var modelInterface in modelInterfaces)
        // {
        //     logger.Log($"{modelInterface.Key}:");
        //
        //     foreach (var value in modelInterface.Value)
        //     {
        //         logger.Log($"- {value.Distance} : {value.Type}");
        //     }
        // }

        var lowestDistanceInterfaceMap =
            new Dictionary<ITypeSymbol, (int Distance, ITypeSymbol Symbol)>(SymbolEqualityComparer.Default);

        foreach (var entry in modelInterfaces)
        {
            foreach (var candidate in entry.Value.Where(x => x.Type.TypeKind is TypeKind.Interface or TypeKind.Struct))
            {
                var hasExisting = lowestDistanceInterfaceMap.TryGetValue(candidate.Type, out var existing);

                if ((hasExisting && existing.Distance > candidate.Distance) || !hasExisting)
                    lowestDistanceInterfaceMap[candidate.Type] = (candidate.Distance, entry.Key);
            }
        }

        foreach (var entry in lowestDistanceInterfaceMap)
        {
            logger.Log($"{entry.Key} -> {entry.Value.Symbol} ({entry.Value.Distance})");
        }

        foreach (var entry in lowestDistanceInterfaceMap)
        {
            contextAttributes.Add(
                $"[System.Text.Json.Serialization.JsonSerializable(typeof({entry.Key.ToDisplayString()}))]"
            );
            contextAttributes.Add(
                $"[System.Text.Json.Serialization.JsonSerializable(typeof(IEnumerable<{entry.Key.ToDisplayString()}>))]"
            );
        }

        if (!AddGatewayMessageTypeInfoFactory(ref jsonContext.Resolver))
        {
            logger.Warn("Failed to add gateway message type info factory");
            return;
        }

        if (!AddUserTypeInfoFactory(ref jsonContext.Resolver))
        {
            logger.Warn("Failed to add user type info factory");
            return;
        }

        logger.Log("Using the following additional converters:");

        foreach (var converter in jsonContext.AdditionalConverters)
        {
            logger.Log($" - {converter}");
        }

        var options = GenerateOptions(
            modelTypesForContext,
            lowestDistanceInterfaceMap,
            jsonContext.AdditionalConverters,
            out var extra
        );

        contextAttributes.UnionWith(extra.Select(x =>
            $"[System.Text.Json.Serialization.JsonSerializable(typeof({x}))] // extra from GenerateOptions"
        ));

        var generated = SourceText.From(
            $$"""
              using System.Text.Json;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;
              using Discord.Models;
              using Discord.Models.Json;

              namespace {{rootNamespace ?? projectName}};

              {{options}}
              {{string.Join("\n", contextAttributes)}}
              {{jsonContext.Resolver.NormalizeWhitespace()}}
              """,
            Encoding.UTF8
        );

        context.AddSource(
            "Json/ModelJsonContext",
            generated
        );

        foreach (var dynamicSource in jsonContext.DynamicSources)
        {
            logger.Log($"Generating dynamic source '{dynamicSource.Key}'");

            context.AddSource(
                dynamicSource.Key,
                dynamicSource.Value
            );
        }

        var stjResult = RunSTJSourceGenerator(
            context,
            compilation,
            generated,
            jsonContext,
            logger
        ).ToArray();

        if (jsonContext.RequestedNoConverterTypeInfos.Count == 0)
            return;

        var getTypeInfosSyntax = SyntaxUtils.CreateSourceGenClone(jsonContext.Resolver);

        foreach (var typeInfo in jsonContext.RequestedNoConverterTypeInfos.ToArray())
        {
            foreach (var result in stjResult.SelectMany(x => x.GeneratedSources))
            {
                var body = GetTypeInfoInit(typeInfo, result.SyntaxTree);

                if (body is null) continue;

                getTypeInfosSyntax = getTypeInfosSyntax
                    .AddMembers(
                        SyntaxFactory.ParseMemberDeclaration(
                            $$"""
                              internal global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::{{typeInfo.ToDisplayString()}}> Create{{typeInfo.Name}}TypeInfoNoConverter(global::System.Text.Json.JsonSerializerOptions options)
                              {
                                  global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::{{typeInfo.ToDisplayString()}}> jsonTypeInfo;
                              
                                  {{
                                      string.Join(
                                          "\n",
                                          body.Statements.Select(x => x.ToString())
                                      )
                                  }}
                              
                                  jsonTypeInfo.OriginatingResolver = this;
                                  return jsonTypeInfo;
                              }
                              """
                        )!
                    );

                jsonContext.RequestedNoConverterTypeInfos.Remove(typeInfo);
            }
        }

        foreach (var remaining in jsonContext.RequestedNoConverterTypeInfos)
        {
            logger.Warn($"Could not resolve the create type info method for {remaining}");
        }

        if (getTypeInfosSyntax.Members.Count > 0)
        {
            context.AddSource(
                "Json/ModelInfos",
                $$"""
                  using System.Text.Json;
                  using System.Text.Json.Serialization;
                  using System.Text.Json.Serialization.Metadata;

                  namespace {{rootNamespace ?? projectName}};

                  {{getTypeInfosSyntax.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static BlockSyntax? GetTypeInfoInit(ITypeSymbol target, SyntaxTree tree)
    {
        var method = tree.GetRoot().DescendantNodes()
            .OfType<MethodDeclarationSyntax>()
            .FirstOrDefault(x => x.Identifier.ValueText == $"Create_{target.Name}");

        if (method is null) return null;

        return method.DescendantNodes()
            .OfType<IfStatementSyntax>()
            .FirstOrDefault()
            ?.ChildNodes()
            .OfType<BlockSyntax>()
            .FirstOrDefault();
    }

    public static bool AddGetTypeInfoToConverter(
        ref ClassDeclarationSyntax converterSyntax,
        ITypeSymbol target
    )
    {
        var typeName = target.ToDisplayString();

        var typeInfoField = SyntaxFactory.ParseMemberDeclaration(
            $"private static JsonTypeInfo<{typeName}>? _typeInfo;"
        );

        var nonTypeInfoMethod = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private static JsonTypeInfo<{{typeName}}> GetTypeInfoWithoutConverter(JsonSerializerOptions options)
              {
                  if(_typeInfo is not null)
                      return _typeInfo;
              
                  if (options.TypeInfoResolver is ModelJsonContext modelJsonContext)
                      return _typeInfo = modelJsonContext.Create{{target.Name}}TypeInfoNoConverter(options);
              
                  return _typeInfo = options.TypeInfoResolverChain
                      .OfType<ModelJsonContext>()
                      .First()
                      .Create{{target.Name}}TypeInfoNoConverter(options);
              }
              """
        );

        if (typeInfoField is null || nonTypeInfoMethod is null)
            return false;

        converterSyntax = converterSyntax.AddMembers(typeInfoField, nonTypeInfoMethod);

        return true;
    }

    private static bool AddGatewayMessageTypeInfoFactory(
        ref ClassDeclarationSyntax syntax)
    {
        var method = SyntaxFactory.ParseMemberDeclaration(
            """
            internal global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Discord.Models.Json.GatewayMessage> CreateGatewayMessageTypeInfoNoConverter(global::System.Text.Json.JsonSerializerOptions options)
            {
                var objectInfo = new global::System.Text.Json.Serialization.Metadata.JsonObjectInfoValues<global::Discord.Models.Json.GatewayMessage>
                {
                    ObjectCreator = () => new global::Discord.Models.Json.GatewayMessage(),
                    ObjectWithParameterizedConstructorCreator = null,
                    PropertyMetadataInitializer = _ => GatewayMessagePropInit(options),
                    ConstructorParameterMetadataInitializer = null,
                    SerializeHandler = GatewayMessageSerializeHandler
                };
            
                var jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateObjectInfo<global::Discord.Models.Json.GatewayMessage>(options, objectInfo);
                jsonTypeInfo.NumberHandling = null;
                jsonTypeInfo.OriginatingResolver = this;
                return jsonTypeInfo;
            }
            """
        );

        if (method is null)
            return false;

        syntax = syntax.AddMembers(method);

        return true;
    }

    private static bool AddUserTypeInfoFactory(
        ref ClassDeclarationSyntax syntax)
    {
        var method = SyntaxFactory.ParseMemberDeclaration(
            """
            internal global::System.Text.Json.Serialization.Metadata.JsonTypeInfo<global::Discord.Models.Json.User> CreateUserTypeInfoNoConverter(global::System.Text.Json.JsonSerializerOptions options)
            {
                var objectInfo = new global::System.Text.Json.Serialization.Metadata.JsonObjectInfoValues<global::Discord.Models.Json.User>
                {
                    ObjectCreator = null,
                    ObjectWithParameterizedConstructorCreator = static args => new global::Discord.Models.Json.User(){ Username = (string)args[0], Discriminator = (string)args[1] },
                    PropertyMetadataInitializer = _ => UserPropInit(options),
                    ConstructorParameterMetadataInitializer = UserCtorParamInit,
                    SerializeHandler = UserSerializeHandler
                };
            
                var jsonTypeInfo = global::System.Text.Json.Serialization.Metadata.JsonMetadataServices.CreateObjectInfo<global::Discord.Models.Json.User>(options, objectInfo);
                jsonTypeInfo.NumberHandling = null;
                jsonTypeInfo.OriginatingResolver = this;
                return jsonTypeInfo;
            }
            """
        );

        if (method is null)
            return false;

        syntax = syntax.AddMembers(method);
        return true;
    }

    private static string? GenerateOptions(
        IEnumerable<ITypeSymbol> symbols,
        Dictionary<ITypeSymbol, (int Distance, ITypeSymbol Symbol)> interfaceMaps,
        HashSet<string> additionalConverters,
        out HashSet<string> additionalSerializables)
    {
        var converters = new HashSet<string>()
        {
            "Discord.Converters.SnowflakeConverter",
            "Discord.Converters.BigIntegerConverter",
            "Discord.Converters.UserConverter",
            "Discord.Converters.GatewayPayloadConverter",
            "Discord.Converters.MillisecondEpocConverter",
            "Discord.Converters.ReactionIdConverter"
        };

        converters.UnionWith(additionalConverters);

        additionalSerializables = new HashSet<string>();

        foreach (var entry in interfaceMaps)
        {
            converters.Add(
                $"Discord.Converters.ModelInterfaceConverter<{entry.Key.ToDisplayString()}, {entry.Value.Symbol.ToDisplayString()}>"
            );
        }

        foreach (var symbol in symbols)
        {
            // add the optional converters
            foreach (var member in symbol
                         .GetMembers()
                         .OfType<IPropertySymbol>()
                         .Where(x => x
                             .Type
                             .ToDisplayString()
                             .StartsWith("Discord.Optional")
                         )
                    )
            {
                var inner = (member.Type as INamedTypeSymbol)!.TypeArguments[0];
                var converterType =
                    $"Discord.Converters.OptionalConverter<{inner.ToDisplayString()}>";

                if (inner.NullableAnnotation is NullableAnnotation.Annotated && inner.IsReferenceType)
                    inner = inner.WithNullableAnnotation(NullableAnnotation.NotAnnotated);

                additionalSerializables.Add(inner.ToDisplayString());
                converters.Add(converterType);
            }
        }

        var options = new HashSet<string>()
        {
            "PropertyNameCaseInsensitive = false",
            "IgnoreReadOnlyProperties = true"
        };

        if (converters.Count > 0)
        {
            options.Add(
                $"Converters = [{Environment.NewLine}    {string.Join($",{Environment.NewLine}", converters.Select(x => $"typeof({x})")).WithNewlinePadding(4)}]"
            );
        }

        return options.Count > 0
            ? $"[System.Text.Json.Serialization.JsonSourceGenerationOptions({Environment.NewLine}    {string.Join($",{Environment.NewLine}", options).WithNewlinePadding(4)}{Environment.NewLine})]"
            : null;
    }

    public static IEnumerable<Hierarchy.SortedHierarchySymbol> GetModelInterfaces(ITypeSymbol symbol)
    {
        return Hierarchy.GetHierarchy(symbol)
            .Where(x =>
                !x.Type.ToDisplayString().StartsWith("Discord.Models.IModel") &&
                !x.Type.ToDisplayString().StartsWith("Discord.Models.IEntityModel") &&
                x.Type
                    .AllInterfaces
                    .Any(x => x.ToDisplayString() == "Discord.Models.IModel")
            );
    }

    private static IEnumerable<GeneratorRunResult> RunSTJSourceGenerator(
        SourceProductionContext context,
        Compilation compilation,
        SourceText toRunAgainst,
        Context jsonContext,
        Logger logger)
    {
        ParseOptions options;
        if (compilation is CSharpCompilation {SyntaxTrees.Length: > 0} csharpCompilation)
        {
            options = csharpCompilation.SyntaxTrees[0].Options;
        }
        else
        {
            options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
        }

        var syntaxTree = SyntaxFactory.ParseSyntaxTree(toRunAgainst, options);
        compilation = compilation.AddSyntaxTrees([
            syntaxTree,
            ..jsonContext.DynamicSources.Values
                .Select(x =>
                    SyntaxFactory.ParseSyntaxTree(x, options)
                )
        ]);

        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.FullName.Contains("System.Text.Json.SourceGeneration"));

        var stjSourceGenerator = assembly?.GetType("System.Text.Json.SourceGeneration.JsonSourceGenerator");

        if (stjSourceGenerator is null)
        {
            logger.Log(LogLevel.Error, "Unable to find System.Text.Json generator");
            yield break;
        }

        var sourceGen = Activator.CreateInstance(stjSourceGenerator);

        var jsonGenerator = sourceGen switch
        {
            IIncrementalGenerator inc => inc.AsSourceGenerator(),
            ISourceGenerator src => src,
            _ => null
        };
        
        if (jsonGenerator is null)
        {
            logger.Warn("STJ sourcegen is null");
            logger.Warn($"{stjSourceGenerator}");
            yield break;
        }
        
        var driverResult = CSharpGeneratorDriver
            .Create(jsonGenerator)
            .RunGenerators(compilation)
            .GetRunResult();

        foreach (var result in driverResult.Results)
        {
            foreach (var source in result.GeneratedSources)
            {
                var sourceText = source.SourceText;

                if (InterceptOptionals(source, jsonContext, out var tree, logger))
                {
                    sourceText = SourceText.From(
                        tree.ToString(),
                        sourceText.Encoding ?? Encoding.UTF8
                    );
                }

                context.AddSource("GeneratedSTJ/" + source.HintName, sourceText);
            }

            yield return result;
        }
    }

    private static bool InterceptOptionals(
        GeneratedSourceResult source,
        Context jsonContext,
        out SyntaxTree syntaxTree,
        Logger logger)
    {
        syntaxTree = source.SyntaxTree;
        SyntaxNode rootNode = syntaxTree.GetRoot();

        rootNode = rootNode.ReplaceNodes(
            rootNode
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(x => x.Identifier.ValueText.EndsWith("PropInit")),
            (node, _) =>
            {
                if (node.Body is null) return node;

                var type = jsonContext.ModelTypesForContext.FirstOrDefault(x =>
                    x.Name == node.Identifier.ValueText.Replace("PropInit", string.Empty)
                );

                if (type is null)
                {
                    return node;
                }

                var optionalMembers = type.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(IsNotIgnoredJsonProperty)
                    .Where(x => x.Type.Name is "Optional")
                    .ToArray();

                if (optionalMembers.Length == 0)
                {
                    return node;
                }

                return node.WithBody(
                    node.Body.ReplaceNodes(
                        node.Body.DescendantNodes()
                            .OfType<ObjectCreationExpressionSyntax>()
                            .Where(x => x.Type is QualifiedNameSyntax
                            {
                                Right.Identifier.ValueText: "JsonPropertyInfoValues"
                            }),
                        (node, _) =>
                        {
                            if (node.Initializer is null)
                                return node;

                            var assignments = node.Initializer.Expressions
                                .OfType<AssignmentExpressionSyntax>()
                                .ToArray();

                            if (assignments.Length == 0)
                                return node;

                            var isOptional = assignments
                                .Any(x =>
                                    x is
                                    {
                                        Left: IdentifierNameSyntax
                                        {
                                            Identifier.ValueText: "PropertyName"
                                        },
                                        Right: LiteralExpressionSyntax
                                        {
                                            RawKind: (int) SyntaxKind.StringLiteralExpression
                                        } literal
                                    }
                                    &&
                                    optionalMembers.Any(y => y.Name == literal.Token.Value as string)
                                );

                            if (!isOptional) return node;

                            if (
                                assignments.FirstOrDefault(x => x is
                                {
                                    Left: IdentifierNameSyntax
                                    {
                                        Identifier.ValueText: "IgnoreCondition"
                                    }
                                }) is { } existing
                            )
                            {
                                node = node.WithInitializer(
                                    node.Initializer.WithExpressions(
                                        node.Initializer.Expressions.Remove(existing)
                                    )
                                );
                            }

                            return node.WithInitializer(
                                node.Initializer!.AddExpressions(
                                    SyntaxFactory.AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        SyntaxFactory.IdentifierName("IgnoreCondition"),
                                        SyntaxFactory.MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            SyntaxFactory.IdentifierName(
                                                "System.Text.Json.Serialization.JsonIgnoreCondition"),
                                            SyntaxFactory.IdentifierName("WhenWritingDefault")
                                        )
                                    )
                                )
                            );
                        }
                    )
                );
            }
        );


        rootNode = rootNode.ReplaceNodes(
            rootNode
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .Where(x => x.Identifier.ValueText.EndsWith("SerializeHandler")),
            (node, _) =>
            {
                if (node.Body is null) return node;

                var type = jsonContext.ModelTypesForContext.FirstOrDefault(x =>
                    x.Name == node.Identifier.ValueText.Replace("SerializeHandler", string.Empty)
                );

                if (type is null)
                {
                    return node;
                }

                var optionalMembers = type.GetMembers()
                    .OfType<IPropertySymbol>()
                    .Where(IsNotIgnoredJsonProperty)
                    .Where(x => x.Type.Name is "Optional")
                    .ToArray();

                if (optionalMembers.Length == 0)
                {
                    return node;
                }

                var newBody = SyntaxFactory.Block();

                for (var i = 0; i < node.Body.Statements.Count; i++)
                {
                    var statement = node.Body.Statements[i];

                    // were looking for:
                    // global::System.Text.Json.JsonSerializer.Serialize(writer, ((global::Discord.Models.Json.SomeModel)value).SomeOptionalProp, OptionalType);
                    if (
                        statement is ExpressionStatementSyntax
                        {
                            Expression: InvocationExpressionSyntax
                            {
                                ArgumentList.Arguments.Count: 3,
                                Expression: MemberAccessExpressionSyntax
                                {
                                    Name.Identifier.ValueText: "Serialize"
                                }
                            } invocationExpressionSyntax
                        }
                        &&
                        invocationExpressionSyntax.ArgumentList.Arguments[1].Expression is MemberAccessExpressionSyntax
                            access
                        &&
                        optionalMembers.Any(x => x.Name == access.Name.Identifier.ValueText)
                        &&
                        node.Body.Statements[i - 1] is ExpressionStatementSyntax
                        {
                            Expression: InvocationExpressionSyntax
                            {
                                Expression: MemberAccessExpressionSyntax
                                {
                                    Name.Identifier.ValueText: "WritePropertyName"
                                }
                            }
                        }
                    )
                    {
                        newBody = newBody
                            .WithStatements(newBody.Statements
                                .RemoveAt(newBody.Statements.Count - 1)
                                .Add(
                                    SyntaxFactory.ParseStatement(
                                        $$"""
                                          if ({{invocationExpressionSyntax.ArgumentList.Arguments[1].Expression}}.IsSpecified)
                                          {
                                             {{node.Body.Statements[i - 1]}}
                                             {{statement}}
                                          }
                                          """
                                    )
                                )
                            );

                        continue;
                    }

                    newBody = newBody.AddStatements(statement);
                }

                return node.WithBody(newBody);
            });

        var isEq = rootNode.IsEquivalentTo(source.SyntaxTree.GetRoot());

        if (!isEq)
            syntaxTree = SyntaxFactory.SyntaxTree(rootNode.NormalizeWhitespace(), syntaxTree.Options,
                encoding: syntaxTree.Encoding);

        return !isEq;
    }

    private static bool IsSpeculativeJsonModel(ITypeSymbol type)
    {
        return TypeUtils.GetBaseTypesAndThis(type).Any(HasJsonProperties);
    }

    private static bool HasJsonProperties(ITypeSymbol type)
    {
        return type.GetMembers()
            .OfType<IPropertySymbol>()
            .Any(IsJsonProperty);
    }

    public static bool IsJsonProperty(IPropertySymbol symbol)
    {
        return symbol
            .GetAttributes()
            .Any(x =>
                x.AttributeClass?.ToDisplayString()
                    is "System.Text.Json.Serialization.JsonPropertyNameAttribute"
                    or ExtendedModel.ExtendedAttributeName
            );
    }

    public static bool IsNotIgnoredJsonProperty(IPropertySymbol symbol)
    {
        return IsJsonProperty(symbol) && symbol
            .GetAttributes()
            .All(x =>
                x.AttributeClass?.ToDisplayString() is not "System.Text.Json.Serialization.JsonIgnoreAttribute"
            );
    }
}