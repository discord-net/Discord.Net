using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
                                if (element.TryGetProperty("{{GetJsonPropertyName(x.Property)}}", out var json{{x.Property.Name}}))
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
                                    "{{GetJsonPropertyName(x.Property)}}",
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

        context.AddSource(
            $"GeneratedConverters/{root.Name}Union",
            $$"""
              using Discord.Models;
              using Discord.Models.Json;
              using System.Text.Json;
              using System.Text.Json.Nodes;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Converters;

              {{syntax.NormalizeWhitespace()}}
              """
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
        string propertyName,
        JsonModels.Context jsonContext,
        SourceProductionContext context,
        Logger logger)
    {
        if (root.GetMembers(propertyName).FirstOrDefault() is not IPropertySymbol property)
            return;

        var unionTypes = GetUnionTypes(jsonContext.Targets, root, propertyName);

        if (unionTypes.Count == 0)
            return;

        var converter = CreateRootConverter(root, property, unionTypes);

        if (converter is null)
            return;

        jsonContext.RequestedNoConverterTypeInfos.Add(root);

        if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.{converter.Identifier.ValueText}"))
            return;

        context.AddSource(
            $"GeneratedConverters/{root.Name}Union",
            $$"""
              using Discord.Models;
              using Discord.Models.Json;
              using System.Text.Json;
              using System.Text.Json.Nodes;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Converters;

              {{converter.NormalizeWhitespace()}}
              """
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
                property,
                unionTypes.Where(x => TypeUtils
                    .GetBaseTypes(x.Type)
                    .Contains(extraBase, SymbolEqualityComparer.Default)
                ).ToList()
            );

            if (extraConverter is null)
                continue;

            if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.{extraConverter.Identifier.ValueText}"))
                continue;

            jsonContext.RequestedNoConverterTypeInfos.Add(extraBase);

            context.AddSource(
                $"GeneratedConverters/{extraBase.Name}Union",
                $$"""
                  using Discord.Models;
                  using Discord.Models.Json;
                  using System.Text.Json;
                  using System.Text.Json.Nodes;
                  using System.Text.Json.Serialization;
                  using System.Text.Json.Serialization.Metadata;

                  namespace Discord.Converters;

                  {{extraConverter.NormalizeWhitespace()}}
                  """
            );
        }
    }

    private static ClassDeclarationSyntax? CreateRootConverter(
        ITypeSymbol symbol,
        IPropertySymbol property,
        List<UnionType> unionTypes)
    {
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

        var table = SyntaxFactory.ParseMemberDeclaration(
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
        );

        var nullCase = unionTypes.FirstOrDefault(x => x.Value is null);

        var read = SyntaxFactory.ParseMemberDeclaration(
            $$""""
              public override {{typeName}}? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
              {
                  using var jsonDoc = JsonDocument.ParseValue(ref reader);
                  var root = jsonDoc.RootElement;

                  if(!root.TryGetProperty("{{GetJsonPropertyName(property)}}", out var delimiterElement))
                      return null;

                  var delimiter = delimiterElement.Deserialize<{{property.Type.ToDisplayString()}}>(options);

                  {{(
                      property.Type.IsReferenceType || property.Type.NullableAnnotation is NullableAnnotation.Annotated
                          ? $$"""
                              if(delimiter is null)
                              {
                                  return {{(
                                      nullCase is not null
                                          ? $"root.Deserialize<{nullCase.Type.ToDisplayString()}>()"
                                          : "null"
                                  )}};
                              }
                              """
                          : string.Empty
                  )}}

                  if (!_lookupTable.TryGetValue(delimiter, out var unionType))
                      return root.Deserialize(GetTypeInfoWithoutConverter(options));

                  return root.Deserialize(unionType, options) as {{typeName}};
              }
              """"
        );

        var write = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override void Write(Utf8JsonWriter writer, {{typeName}} value, JsonSerializerOptions options)
                  => JsonSerializer.Serialize(writer, value, GetTypeInfoWithoutConverter(options));
              """
        );

        if (table is null || read is null || write is null)
            return null;

        return syntax.AddMembers(table, read, write);
    }

    private static string GetJsonPropertyName(IPropertySymbol property)
    {
        var nameAttribute = property.GetAttributes()
            .FirstOrDefault(x =>
                x.AttributeClass?.ToDisplayString() == "System.Text.Json.Serialization.JsonPropertyNameAttribute"
            );

        if (nameAttribute is null)
            return property.Name;

        return (nameAttribute.ConstructorArguments[0].Value as string)!;
    }

    private sealed class UnionType(ITypeSymbol type, object? value)
    {
        public ITypeSymbol Type { get; } = type;
        public object? Value { get; } = value;
    }

    private static List<UnionType> GetUnionTypes(
        JsonModels.JsonModelTarget[] targets,
        ITypeSymbol symbol,
        string property)
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
                if (attribute.ConstructorArguments[0].Value is string typeProperty && typeProperty == property)
                    result.Add(new(candidate.TypeSymbol, attribute.ConstructorArguments[1].Value));
            }
        }

        return result;
    }

    private static bool TryGetUnionTypeRoot(ITypeSymbol symbol, out string property)
    {
        property = null!;

        var attribute = symbol.GetAttributes()
            .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == UnionTypeRootAttribute);

        if (attribute is null)
            return false;

        property = (attribute.ConstructorArguments[0].Value as string)!;
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
