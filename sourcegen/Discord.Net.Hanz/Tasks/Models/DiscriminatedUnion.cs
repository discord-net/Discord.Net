using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Discord.Net.Hanz.Tasks;

public sealed class DiscriminatedUnion
{
    public const string UnionPropertyAttribute = "Discord.DiscriminatedUnionAttribute";
    public const string UnionEntryAttribute = "Discord.DiscriminatedUnionEntryAttribute";
    public const string UnionTypeRootAttribute = "Discord.DiscriminatedUnionRootTypeAttribute";
    public const string UnionTypeAttribute = "Discord.DiscriminatedUnionTypeAttribute";

    public static void Process(
        ITypeSymbol symbol,
        SemanticModel semanticModel,
        JsonModels.Context jsonContext,
        SourceProductionContext context,
        Logger logger
    )
    {
        if (symbol.TypeKind is not TypeKind.Class)
            return;

        if (TryGetUnionTypeRoot(symbol, out var propertyName))
        {
            ProcessUnionTypeRoot(symbol, propertyName, jsonContext, context, logger);
            return;
        }

        if (TryGetUnionProperties(symbol, out var properties))
        {
            ProcessUnionProperties(symbol, properties, jsonContext, context, logger);
        }
    }

    private static void ProcessUnionProperties(
        ITypeSymbol root,
        IEnumerable<DiscriminatedUnionPropertyInfo> properties,
        JsonModels.Context jsonContext,
        SourceProductionContext context,
        Logger logger)
    {
        var typeName = root.ToDisplayString();

        var propertyInfos = properties as DiscriminatedUnionPropertyInfo[] ?? properties.ToArray();
        var propMethods = propertyInfos
            .Select(x => CreateDeserializer(root, x));

        var syntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword)
            ),
            SyntaxFactory.Identifier($"{root.Name}UnionConverter"),
            null,
            SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList<BaseTypeSyntax>((BaseTypeSyntax[])
                [
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName($"JsonConverter<{typeName}>")
                    )
                ])
            ),
            [],
            []
        );

        jsonContext.RequestedNoConverterTypeInfos.Add(root);

        if (!JsonModels.AddGetTypeInfoToConverter(ref syntax, root))
            return;

        var read = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override {{typeName}}? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
              {
                  using var jsonDoc = JsonDocument.ParseValue(ref reader);
                  var element = jsonDoc.RootElement;
              
                  var value = element.Deserialize(GetTypeInfoWithoutConverter(options));
              
                  if(value is null) return null;
              
                  {{
                      string.Join(
                          "\n",
                          propertyInfos.Select(x =>
                              $$"""
                                if (element.TryGetProperty("{{JsonModels.GetJsonPropertyName(x.Property)}}", out var json{{x.Property.Name}}))
                                {
                                    value.{{x.Property.Name}} = {{(
                                        x.IsOptionalType
                                            ? $"Optional.Some(Deserialize{x.Property.Name}(value, json{x.Property.Name}, options){(x.UnionRootType.NullableAnnotation is not NullableAnnotation.Annotated ? "!" : string.Empty)})"
                                            : $"Deserialize{x.Property.Name}(value, json{x.Property.Name}, options)"
                                    )}};
                                }
                                """
                          )
                      )
                  }}
              
                  return value;
              }
              """
        );

        var write = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override void Write(Utf8JsonWriter writer, {{typeName}} value, JsonSerializerOptions options)
              {
                  var jsonNode = JsonSerializer.SerializeToNode(value, GetTypeInfoWithoutConverter(options))
                      as System.Text.Json.Nodes.JsonObject;
              
                  if(jsonNode is null) return;
              
                  {{
                      string.Join(
                          "\n",
                          propertyInfos.Select(x =>
                              $$"""
                                jsonNode.Add(
                                    "{{JsonModels.GetJsonPropertyName(x.Property)}}",
                                    JsonSerializer.SerializeToNode(
                                        value.{{x.Property.Name}},
                                        options
                                    )
                                );
                                """
                          )
                      )
                  }}
              
                  jsonNode.WriteTo(writer, options);
              }
              """
        );

        if (read is null || write is null)
            return;

        if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.{root.Name}UnionConverter"))
            return;

        syntax = syntax
            .AddMembers(read, write)
            .AddMembers(propMethods.ToArray());

        var filename = $"GeneratedConverters/{root.Name}Union";

        if (jsonContext.DynamicSources.ContainsKey(filename)) return;

        jsonContext.DynamicSources[filename] = SourceText.From(
            $$"""
              using Discord.Models;
              using Discord.Models.Json;
              using System.Text.Json;
              using System.Text.Json.Nodes;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Converters;

              {{syntax.NormalizeWhitespace()}}
              """,
            Encoding.UTF8
        );
    }

    private static MemberDeclarationSyntax CreateDeserializer(
        ITypeSymbol root,
        DiscriminatedUnionPropertyInfo info)
    {
        var propType = info.UnionRootType.WithNullableAnnotation(
            info.UnionRootType.IsReferenceType
                ? NullableAnnotation.NotAnnotated
                : info.UnionRootType.NullableAnnotation
        ).ToDisplayString();

        var propTypeWithNullable = info.UnionRootType.ToDisplayString();

        if (info.UnionRootType is {IsReferenceType: true, NullableAnnotation: not NullableAnnotation.Annotated})
            propTypeWithNullable += "?";

        var nullCase = info.Property.Type.IsReferenceType ||
                       info.Property.Type.NullableAnnotation is NullableAnnotation.Annotated
            ? $$"""
                if (delimiter is null)
                {
                    return {{(
                        info.Entries.FirstOrDefault(x => x.Value.Length == 0).Key is { } nullCaseType
                            ? $"element.Deserialize<{nullCaseType.ToDisplayString()}>(options)"
                            : "null"
                    )}};
                }
                """
            : string.Empty;

        return SyntaxFactory.ParseMemberDeclaration(
            $$"""
              private static {{propTypeWithNullable}} Deserialize{{info.Property.Name}}(
                  {{root.ToDisplayString()}} obj,
                  JsonElement element,
                  JsonSerializerOptions options)
              {
                  var delimiter = obj.{{info.Delimiter.Name}};
              
                  {{nullCase}}
              
                  return element.Deserialize(
                      delimiter switch
                      {
                          {{
                              string.Join(
                                  "\n",
                                  info.Entries
                                      .Where(x => x.Value is not null)
                                      .Select(x =>
                                          $$"""
                                            {{
                                                string.Join(
                                                    " or ",
                                                    x.Value.Select(val => SyntaxUtils.CreateLiteral(x.Key, val))
                                                )
                                            }} => typeof({{x.Key.ToDisplayString()}}),
                                            """
                                      )
                              )
                          }}
                          _ => typeof({{propType}})
                      },
                      options
                  ) as {{propType}};
              }
              """
        )!;
    }

    private static void ProcessUnionTypeRoot(
        ITypeSymbol root,
        string? propertyName,
        JsonModels.Context jsonContext,
        SourceProductionContext context,
        Logger logger)
    {
        var unionTypes = GetUnionTypes(jsonContext.Targets, root, propertyName);

        if (unionTypes.Count == 0)
        {
            logger.Warn($"Union root {root} has no entry types!");
            return;
        }

        var converters = propertyName is not null
            ?
            [
                (
                    Syntax: CreateRootConverter(
                        root,
                        propertyName,
                        unionTypes,
                        logger,
                        out var unionProperty
                    ),
                    Property: unionProperty
                )
            ]
            : unionTypes.GroupBy(x => x.Property)
                .Select(x =>
                    (
                        Syntax: CreateRootConverter(
                            root,
                            x.Key,
                            x.ToList(),
                            logger,
                            out var unionProperty
                        ),
                        Property: unionProperty
                    )
                );

        foreach (var converter in converters)
        {
            if (converter.Syntax is null || converter.Property is null)
            {
                logger.Warn($"Failed to create a union converter for {root}");
                continue;
            }

            jsonContext.RequestedNoConverterTypeInfos.Add(root);

            if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.{converter.Syntax.Identifier.ValueText}_{converter.Property.Name}"))
                return;

            var filename = $"GeneratedConverters/{root.Name}_{converter.Property.Name}Union";

            if (jsonContext.DynamicSources.ContainsKey(filename)) return;

            jsonContext.DynamicSources[filename] = SourceText.From(
                $$"""
                  using Discord.Models;
                  using Discord.Models.Json;
                  using System.Text.Json;
                  using System.Text.Json.Nodes;
                  using System.Text.Json.Serialization;
                  using System.Text.Json.Serialization.Metadata;

                  namespace Discord.Converters;

                  {{converter.Syntax.NormalizeWhitespace()}}
                  """,
                Encoding.UTF8
            );

            // we want to add converters for every type within the hierarchy tree from 'symbol' to each member
            foreach (var extraBase in unionTypes
                         .SelectMany(x =>
                             TypeUtils.GetBaseTypes(x.Type)
                         )
                         .Where(x =>
                             !x.Equals(root, SymbolEqualityComparer.Default) &&
                             unionTypes.All(y =>
                                 !y.Type.Equals(x, SymbolEqualityComparer.Default)
                             ) &&
                             TypeUtils.GetBaseTypes(x).Contains(root, SymbolEqualityComparer.Default)
                         ))
            {
                var extraConverter = CreateRootConverter(
                    extraBase,
                    converter.Property.Name,
                    unionTypes.Where(x => TypeUtils
                        .GetBaseTypes(x.Type)
                        .Contains(extraBase, SymbolEqualityComparer.Default)
                    ).ToList(),
                    logger,
                    out _,
                    converter.Property
                );

                if (extraConverter is null)
                {
                    logger.Warn($"{root}: Failed to create a converter for {extraBase}");
                    continue;
                }

                if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.{extraConverter.Identifier.ValueText}"))
                    continue;

                jsonContext.RequestedNoConverterTypeInfos.Add(extraBase);

                filename = $"GeneratedConverters/{extraBase.Name}_{converter.Property.Name}Union";

                if (jsonContext.DynamicSources.ContainsKey(filename)) continue;

                jsonContext.DynamicSources[filename] = SourceText.From(
                    $$"""
                      using Discord.Models;
                      using Discord.Models.Json;
                      using System.Text.Json;
                      using System.Text.Json.Nodes;
                      using System.Text.Json.Serialization;
                      using System.Text.Json.Serialization.Metadata;

                      namespace Discord.Converters;

                      {{extraConverter.NormalizeWhitespace()}}
                      """,
                    Encoding.UTF8
                );
            }
        }
    }

    private static ClassDeclarationSyntax? CreateRootConverter(
        ITypeSymbol symbol,
        string propertyName,
        List<UnionType> unionTypes,
        Logger logger,
        out IPropertySymbol? unionProperty,
        IPropertySymbol? property = null
    )
    {
        unionProperty = property;

        if (property is null)
        {
            var properties = symbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Concat(unionTypes
                    .SelectMany(x => x
                        .Type
                        .GetMembers()
                        .OfType<IPropertySymbol>()
                    )
                )
                .ToArray();

            unionProperty = property = properties.FirstOrDefault(x => x.Name == propertyName);

            if (property is null)
            {
                logger.Warn($"{symbol}: Failed to find a union property with the name '{propertyName}'");
                logger.Warn($"{symbol}: Candidate properties:");
                foreach (var candidate in properties)
                {
                    logger.Warn($" - {candidate.ContainingType}: {candidate}");
                }

                return null;
            }
        }

        var typeName = symbol.ToDisplayString();

        var syntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword)
            ),
            SyntaxFactory.Identifier($"{symbol.Name}UnionConverter"),
            null,
            SyntaxFactory.BaseList(
                SyntaxFactory.SeparatedList<BaseTypeSyntax>((BaseTypeSyntax[])
                [
                    SyntaxFactory.SimpleBaseType(
                        SyntaxFactory.ParseTypeName($"JsonConverter<{symbol.Name}>")
                    )
                ])
            ),
            [],
            []
        );

        if (!JsonModels.AddGetTypeInfoToConverter(ref syntax, symbol))
            return null;

        var notNullCases = unionTypes.Where(x => x.Value is not null).ToArray();
        var nullCase = unionTypes.FirstOrDefault(x => x.Value is null);
        var notPresentCase = unionTypes.FirstOrDefault(x => x.WhenSpecified == false);
        var presentNoMatchCase = unionTypes.FirstOrDefault(x => x.WhenSpecified == true);

        if (!symbol.IsAbstract && notPresentCase is null)
        {
            notPresentCase = new(symbol, propertyName, null, null);
        }

        if (notNullCases.Length == 0 && nullCase is null && presentNoMatchCase is null && notPresentCase is null)
        {
            logger.Warn($"{symbol}: Invalid union configuration");
            return null;
        }

        var table =
            notNullCases.Length > 0
                ? SyntaxFactory.ParseMemberDeclaration(
                    $$"""
                      private static readonly Dictionary<{{property.Type.ToDisplayString()}}, Type> _lookupTable = new()
                      {
                          {{
                              string.Join(
                                  ",\n",
                                  unionTypes
                                      .Where(x => x.Value is not null)
                                      .Select(x =>
                                          $"{{ {SyntaxUtils.CreateLiteral(property.Type, x.Value)}, typeof({x.Type.ToDisplayString()}) }}"
                                      )
                              )
                          }}
                      };
                      """
                )
                : null;

        var read = SyntaxFactory.ParseMemberDeclaration(
            $$""""
              public override {{typeName}}? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
              {
                  using var jsonDoc = JsonDocument.ParseValue(ref reader);
                  var root = jsonDoc.RootElement;
              
                  if(!root.TryGetProperty("{{JsonModels.GetJsonPropertyName(property)}}", out var delimiterElement))
                      return {{(
                          notPresentCase is not null
                              ? $"root.Deserialize<{notPresentCase.Type.ToDisplayString()}>(options)"
                              : "null"
                      )}};
              
                  {{(
                      table is not null
                          ? $$"""
                              var delimiter = delimiterElement.Deserialize<{{property.Type.ToDisplayString()}}>(options);

                              {{(
                                  property.Type.IsReferenceType || property.Type.NullableAnnotation is NullableAnnotation.Annotated
                                      ? $$"""
                                          if(delimiter is null)
                                          {
                                              return {{(
                                                  nullCase is not null
                                                      ? $"root.Deserialize<{nullCase.Type.ToDisplayString()}>(options)"
                                                      : "null"
                                              )}};
                                          }
                                          """
                                      : string.Empty
                              )}}

                              if (!_lookupTable.TryGetValue(delimiter, out var unionType))
                              return {{(
                                  presentNoMatchCase is not null
                                      ? $"root.Deserialize<{presentNoMatchCase.Type.ToDisplayString()}>(options)"
                                      : "root.Deserialize(GetTypeInfoWithoutConverter(options))"
                              )}};
                              return root.Deserialize(unionType, options) as {{typeName}};
                              """
                          : $"return root.Deserialize<{(presentNoMatchCase ?? notPresentCase!).Type.ToDisplayString()}>(options);"
                  )}}
              }
              """"
        );

        var write = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override void Write(Utf8JsonWriter writer, {{typeName}} value, JsonSerializerOptions options)
                  => JsonSerializer.Serialize(writer, value, GetTypeInfoWithoutConverter(options));
              """
        );

        if (read is null || write is null)
            return null;

        if (table is not null)
            syntax = syntax.AddMembers(table);

        return syntax.AddMembers(read, write);
    }

    private sealed class UnionType(ITypeSymbol type, string property, object? value, bool? whenSpecified)
    {
        public ITypeSymbol Type { get; } = type;
        public string Property { get; } = property;
        public object? Value { get; } = value;
        public bool? WhenSpecified { get; } = whenSpecified;
    }

    private static List<UnionType> GetUnionTypes(
        JsonModels.JsonModelTarget[] targets,
        ITypeSymbol symbol,
        string? property)
    {
        var result = new List<UnionType>();

        var candidates = targets
            .Where(x =>
                TypeUtils.GetBaseTypes(x.TypeSymbol).Contains(symbol, SymbolEqualityComparer.Default)
                ||
                x.TypeSymbol.AllInterfaces.Contains(symbol, SymbolEqualityComparer.Default)
            );

        foreach (var candidate in candidates)
        {
            foreach (var attribute in candidate.TypeSymbol.GetAttributes()
                         .Where(x => x.AttributeClass?.ToDisplayString() == UnionTypeAttribute))
            {
                if (attribute.ConstructorArguments[0].Value is not string typeProp)
                    continue;

                if (property is not null && typeProp != property)
                    continue;

                result.Add(
                    new(
                        candidate.TypeSymbol,
                        typeProp,
                        attribute.ConstructorArguments[1].Value,
                        (bool?) attribute.NamedArguments.FirstOrDefault(x => x.Key == "WhenSpecified").Value.Value
                    )
                );
            }
        }

        return result;
    }

    private static bool TryGetUnionTypeRoot(ITypeSymbol symbol, out string? property)
    {
        property = null!;

        var attribute = symbol.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == UnionTypeRootAttribute);

        if (attribute is null)
            return false;

        property = attribute.ConstructorArguments[0].Value as string;
        return true;
    }

    private static bool TryGetUnionProperties(
        ITypeSymbol symbol,
        out IEnumerable<DiscriminatedUnionPropertyInfo> properties)
    {
        properties = null!;

        var props = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(IsValidUnionProperty)
            .ToArray();

        if (props.Length == 0) return false;

        properties = props.Select(property =>
        {
            if (!TryGetDelimiterProperty(property, out var delimiter))
                throw new InvalidOperationException("No delimiter found");

            return new DiscriminatedUnionPropertyInfo(
                property,
                delimiter,
                property.GetAttributes()
                    .Where(IsUnionEntry)
                    .ToDictionary<AttributeData, ITypeSymbol, object[]>(
                        x => x.AttributeClass!.TypeArguments[0],
                        x => x.ConstructorArguments[0].Values.Select(x => x.Value).ToArray(),
                        SymbolEqualityComparer.Default
                    )
            );
        });

        return true;
    }

    private static ITypeSymbol UnwrapOptional(ITypeSymbol symbol)
    {
        return symbol switch
        {
            INamedTypeSymbol {IsGenericType: true, Name: "Optional"} wrapping
                => UnwrapOptional(wrapping.TypeArguments[0]),
            _ => symbol
        };
    }

    private static bool IsValidUnionProperty(IPropertySymbol property)
    {
        var attributes = property.GetAttributes();
        if (attributes.Length == 0) return false;

        return TryGetDelimiterProperty(property, out _) && attributes.Any(IsUnionEntry);
    }

    private static bool TryGetDelimiterProperty(IPropertySymbol unionProp, out IPropertySymbol delimiter)
    {
        delimiter = null!;

        var attributes = unionProp.GetAttributes();

        if (attributes.Length == 0) return false;

        var delimiterName = attributes.FirstOrDefault(IsUnionProperty)?.ConstructorArguments[0].Value as string;

        if (
            delimiterName is null
            ||
            (
                delimiter = unionProp.ContainingType.GetMembers(delimiterName)
                    .OfType<IPropertySymbol>()
                    .FirstOrDefault()!
            ) is null
        ) return false;

        return true;
    }

    private static bool IsUnionProperty(AttributeData data)
        => data.AttributeClass?.ToDisplayString() == UnionPropertyAttribute;

    private static bool IsUnionEntry(AttributeData data)
        => data.AttributeClass?.ToDisplayString().StartsWith(UnionEntryAttribute) ?? false;

    private sealed class DiscriminatedUnionPropertyInfo
    {
        public IPropertySymbol Property { get; }
        public IPropertySymbol Delimiter { get; }
        public IDictionary<ITypeSymbol, object[]> Entries { get; }

        public bool IsOptionalType { get; }

        public ITypeSymbol UnionRootType { get; }

        public DiscriminatedUnionPropertyInfo(
            IPropertySymbol property,
            IPropertySymbol delimiter,
            IDictionary<ITypeSymbol, object[]> entries)
        {
            Property = property;
            Delimiter = delimiter;
            Entries = entries;

            UnionRootType = UnwrapOptional(property.Type);
            IsOptionalType = !property.Type.Equals(UnionRootType, SymbolEqualityComparer.Default);
        }
    }
}