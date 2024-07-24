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

    public static void Execute(
        SourceProductionContext context,
        ((ImmutableArray<JsonModelTarget?>, Compilation), (string?, string)) input,
        Logger logger)
    {
        var ((potentialTargets, compilation), (parentAssembly, rootNameSpace)) = input;

        //var generatedText = new HashSet<SourceText>();

        var modelInterfaces =
            new Dictionary<ITypeSymbol, HashSet<Hierarchy.SortedHierarchySymbol>>(SymbolEqualityComparer.Default);

        var contextAttributes = new HashSet<string>();
        var modelTypesForContext = new HashSet<ITypeSymbol>(SymbolEqualityComparer.Default);

        foreach (var target in potentialTargets)
        {
            if (target is null) continue;

            if (!IsSpeculativeJsonModel(target.TypeSymbol))
            {
                logger.Log($"{target.TypeSymbol} is not a json target");
                continue;
            }

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

//             var source = SourceText.From(
//                 $$"""
//                   namespace {{target.TypeSymbol.ContainingNamespace}};
//
//                   {{string.Join("\n", attributes)}}
//                   public sealed partial class {{target.TypeSymbol.Name}}JsonContext : System.Text.Json.Serialization.JsonSerializerContext;
//                   """,
//                 Encoding.UTF8
//             );
//
//             if (!generatedText.Add(source))
//             {
//                 logger.Warn($"Skipping {hintName}: already generated source text");
//                 continue;
//             }
//
//             context.AddSource(
//                 $"Json/{hintName}",
//                 source
//             );
//
//             modelTypesForContext.Add(target.TypeSymbol);
        }

        if (modelTypesForContext.Count == 0)
            return;

        foreach (var modelInterface in modelInterfaces)
        {
            logger.Log($"{modelInterface.Key}:");

            foreach (var value in modelInterface.Value)
            {
                logger.Log($"- {value.Distance} : {value.Type}");
            }
        }

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

        // if (!AddLookupTables(ref resolverSyntax, modelTypesForContext, lowestDistanceInterfaceMap))
        // {
        //     logger.Warn("Failed to add lookup tables");
        //     return;
        // }
        //
        // if (!AddJsonTypeInfoResolvers(ref resolverSyntax))
        // {
        //     logger.Warn("Failed to add context resolvers map");
        //     return;
        // }

        var options = GenerateOptions(modelTypesForContext, lowestDistanceInterfaceMap, out var extra);

        contextAttributes.UnionWith(extra.Select(x =>
            $"[System.Text.Json.Serialization.JsonSerializable(typeof({x}))]"
        ));

        var generated = SourceText.From(
            $$"""
              using System.Text.Json;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;

              namespace {{rootNameSpace ?? parentAssembly}};

              {{options}}
              {{string.Join("\n", contextAttributes)}}
              {{resolverSyntax.NormalizeWhitespace()}}
              """,
            Encoding.UTF8
        );

        context.AddSource(
            "Json/ModelJsonContext",
            generated
        );

        RunSTJSourceGenerator(context, compilation, generated, logger);
    }

    public const string ModelToContextTable = "ModelToContextInfo";
    public const string InterfaceToContextTable = "InterfaceToContextInfo";
    public const string EnumerableModelToContextTable = "EnumerableModelToContextInfo";
    public const string EnumerableInterfaceToContextTable = "EnumerableInterfaceToContextInfo";

    private static bool AddLookupTables(
        ref ClassDeclarationSyntax syntax,
        HashSet<ITypeSymbol> models,
        Dictionary<ITypeSymbol, (int Distance, ITypeSymbol Symbol)> map)
    {
//         var entrySyntax = SyntaxFactory.ParseMemberDeclaration(
//             """
//             public sealed class ContextInfo(JsonSerializerContext context, Func<JsonSerializerOptions, JsonSerializerContext> factory)
//             {
//                 public readonly JsonSerializerContext Default = context;
//                 public readonly Func<JsonSerializerOptions, JsonSerializerContext> Factory = factory;
//
//                 private readonly Dictionary<JsonSerializerOptions, JsonSerializerContext> _contexts = new();
//
//                 private readonly object _syncRoot = new();
//
//                 public JsonSerializerContext GetContextForOptions(JsonSerializerOptions options)
//                 {
//                     if(Default.Options == options)
//                         return Default;
//
//                     lock(_syncRoot)
//                     {
//                         if(_contexts.TryGetValue(options, out var result))
//                             return result;
//
//                         return _contexts[options] = Factory(options);
//                     }
//                 }
//             }
//             """
//         );

        var mainTable = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static readonly Dictionary<Type, Func<ModelJsonContext, JsonTypeInfo>> {{ModelToContextTable}} = new()
              {
                  {{
                      string.Join(
                          ",\n",
                          models.Select(x =>
                              $"{{ typeof({x.ToDisplayString()}), (self) => self.{x.Name} }}"
                          )
                      )
                  }}
              };
              """
        );

        var interfaceTable = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static readonly Dictionary<Type, Func<ModelJsonContext, JsonTypeInfo>> {{InterfaceToContextTable}} = new()
              {
                  {{
                      string.Join(
                          ",\n",
                          map.Select(x =>
                              $"{{ typeof({x.Key}), (self) => self.{x.Value.Symbol.Name}}}"
                          )
                      )
                  }}
              };
              """
        );

        var enumerableModelTable = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static readonly Dictionary<Type, Func<ModelJsonContext, JsonTypeInfo>> {{EnumerableModelToContextTable}} = new()
              {
                  {{
                      string.Join(
                          ",\n",
                          models.Select(x =>
                              $"{{ typeof(IEnumerable<{x}>), (self) => self.IEnumerable{x.Name}}}"
                          )
                      )
                  }}
              };
              """
        );

        var enumerableInterfaceTable = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public static readonly Dictionary<Type, Func<ModelJsonContext, JsonTypeInfo>> {{EnumerableInterfaceToContextTable}} = new()
              {
                  {{
                      string.Join(
                          ",\n",
                          map.Select(x =>
                              $"{{ typeof(IEnumerable<{x.Key}>), (self) => self.IEnumerable{x.Value.Symbol}}}"
                          )
                      )
                  }}
              };
              """
        );

        if (mainTable is null || interfaceTable is null || enumerableInterfaceTable is null ||
            enumerableModelTable is null)
            return false;

        syntax = syntax.AddMembers(
            mainTable,
            interfaceTable,
            enumerableModelTable,
            enumerableInterfaceTable
        );

        return true;
    }

    private static bool AddJsonTypeInfoResolvers(
        ref ClassDeclarationSyntax syntax)
    {
        const string resolverType = "System.Text.Json.Serialization.Metadata.JsonTypeInfoResolver";
        const string resolverTypeInterface = "System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver";

        var interfaceResolver = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private sealed class InterfaceResolver : System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver
              {
                  public static readonly InterfaceResolver Instance = new();

                  public System.Text.Json.Serialization.Metadata.JsonTypeInfo? GetTypeInfo(Type type, System.Text.Json.JsonSerializerOptions options)
                  {
                      if(!type.IsInterface)
                          return null;

                      if({{InterfaceToContextTable}}.TryGetValue(type, out var info))
                          return info.GetContextForOptions(options).GetTypeInfo(type);

                      if({{EnumerableInterfaceToContextTable}}.TryGetValue(type, out info))
                          return info.GetContextForOptions(options).GetTypeInfo(type);

                      if({{EnumerableModelToContextTable}}.TryGetValue(type, out info))
                          return info.GetContextForOptions(options).GetTypeInfo(type);

                      return null;
                  }
              }
              """
        );

        var modelResolver = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private sealed class ModelResolver : System.Text.Json.Serialization.Metadata.IJsonTypeInfoResolver
              {
                  public static readonly ModelResolver Instance = new();

                  public System.Text.Json.Serialization.Metadata.JsonTypeInfo? GetTypeInfo(Type type, System.Text.Json.JsonSerializerOptions options)
                  {
                      if({{ModelToContextTable}}.TryGetValue(type, out var info))
                          return info.GetContextForOptions(options).GetTypeInfo(type);

                      return null;
                  }
              }
              """
        );

        if (interfaceResolver is null || modelResolver is null)
            return false;

        var mainResolver = SyntaxFactory.ParseMemberDeclaration(
            $"public static readonly {resolverTypeInterface} Resolver = {resolverType}.Combine(InterfaceResolver.Instance, ModelResolver.Instance);"
        );

        if (mainResolver is null)
            return false;

        syntax = syntax.AddMembers(interfaceResolver, modelResolver, mainResolver);

        return true;
    }

    private static string? GenerateOptions(
        IEnumerable<ITypeSymbol> symbols,
        Dictionary<ITypeSymbol, (int Distance, ITypeSymbol Symbol)> interfaceMaps,
        out HashSet<string> additionalSerializables)
    {
        var converters = new HashSet<string>()
        {
            "Discord.Converters.SnowflakeConverter",
            "Discord.Converters.BigIntegerConverter"
        };

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
            options.Add($"Converters = [{string.Join(", ", converters.Select(x => $"typeof({x})"))}]");
        }

        return options.Count > 0
            ? $"[System.Text.Json.Serialization.JsonSourceGenerationOptions({string.Join(", ", options)})]"
            : null;
    }

    private static IEnumerable<Hierarchy.SortedHierarchySymbol> GetModelInterfaces(ITypeSymbol symbol)
    {
        return Hierarchy.GetHierarchy(symbol)
            .Where(x =>
                !x.Type.ToDisplayString().StartsWith("Discord.Models.IEntityModel") &&
                x.Type
                    .AllInterfaces
                    .Any(x => x.ToDisplayString() == "Discord.Models.IEntityModel")
            );
    }

    private static void RunSTJSourceGenerator(
        SourceProductionContext context,
        Compilation compilation,
        SourceText toRunAgainst,
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
        compilation = compilation.AddSyntaxTrees(syntaxTree);

        var assembly = AppDomain.CurrentDomain.GetAssemblies()
            .FirstOrDefault(a => a.FullName.Contains("System.Text.Json.SourceGeneration"));

        var stjSourceGenerator = assembly?.GetType("System.Text.Json.SourceGeneration.JsonSourceGenerator");

        if (stjSourceGenerator is null)
        {
            logger.Log(LogLevel.Error, "Unable to find System.Text.Json generator");
            return;
        }

        var jsonGenerator = ((IIncrementalGenerator)Activator.CreateInstance(stjSourceGenerator)).AsSourceGenerator();

        var driverResult = CSharpGeneratorDriver
            .Create(jsonGenerator)
            .RunGenerators(compilation)
            .GetRunResult();

        foreach (var result in driverResult.Results)
        {
            foreach (var source in result.GeneratedSources)
            {
                context.AddSource("GeneratedSTJ/" + source.HintName, source.SourceText);
            }
        }
    }

    private static bool IsSpeculativeJsonModel(ITypeSymbol type)
    {
        return TypeUtils.GetBaseTypesAndThis(type).Any(HasJsonProperties);
    }

    private static bool HasJsonProperties(ITypeSymbol type)
    {
        return type.GetMembers()
            .OfType<IPropertySymbol>()
            .Any(x => x
                .GetAttributes()
                .Any(x =>
                    x.AttributeClass?.ToDisplayString() == "System.Text.Json.Serialization.JsonPropertyNameAttribute"
                )
            );
    }
}
