using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Discord.Net.Hanz.Tasks;

public static class ExtendedModel
{
    public const string ExtendedAttributeName = "Discord.JsonExtendAttribute";

    public static void Process(
        ITypeSymbol symbol,
        SemanticModel semanticModel,
        JsonModels.Context jsonContext,
        SourceProductionContext context,
        Logger logger
    )
    {
        var extendedMembers = symbol.GetMembers()
            .OfType<IPropertySymbol>()
            .Where(x => x
                .GetAttributes()
                .Any(x => x.AttributeClass?.ToDisplayString() == ExtendedAttributeName)
            )
            .ToArray();

        if (extendedMembers.Length == 0)
            return;

        var converter = CreateConverterSyntax(symbol, extendedMembers, semanticModel);

        if (converter is null)
        {
            logger.Warn($"{symbol}: No converter was created");
            return;
        }

        JsonModels.AddNoConverterMethod(symbol, jsonContext);

        if (!jsonContext.AdditionalConverters.Add($"Extended{symbol.Name}Converter"))
        {
            logger.Warn($"{symbol}: converter already exists");
            return;
        }

        context.AddSource(
            $"GeneratedConverters/Extended{symbol.Name}Converter",
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

        AddProxiesIfNeeded(symbol, extendedMembers, context);
    }

    private static void AddProxiesIfNeeded(
        ITypeSymbol symbol,
        IPropertySymbol[] properties,
        SourceProductionContext context)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        if (symbol.DeclaringSyntaxReferences[0].GetSyntax() is not TypeDeclarationSyntax targetSyntax)
            return;

        if (targetSyntax.Modifiers.All(x => x.RawKind != (ushort)SyntaxKind.PartialKeyword))
            return;

        var proxiedMembers = new Dictionary<IPropertySymbol, List<IPropertySymbol>>(SymbolEqualityComparer.Default);

        foreach (var property in properties)
        {
            var commonInterfaces = symbol.AllInterfaces.Where(x =>
                property.Type.AllInterfaces.Contains(x)
            );

            foreach (var commonInterface in commonInterfaces)
            foreach (var member in commonInterface.GetMembers().OfType<IPropertySymbol>())
            {
                if(!member.IsAbstract) continue;

                if(symbol.FindImplementationForInterfaceMember(member) is not null) continue;

                if (!proxiedMembers.TryGetValue(property, out var members))
                    proxiedMembers[property] = members = new();

                members.Add(member);
            }
        }

        if (proxiedMembers.Count == 0)
            return;

        var syntax = SyntaxUtils.CreateSourceGenClone(targetSyntax);

        foreach (var entry in proxiedMembers)
        foreach (var prop in entry.Value)
        {
            var iface = prop.ContainingType.ToDisplayString();
            var propType = prop.Type.ToDisplayString();

            syntax = syntax.AddMembers(
                SyntaxFactory.ParseMemberDeclaration(
                    $"{propType} {iface}.{prop.Name} => ({entry.Key.Name} as {iface}).{prop.Name};"
                )!
            );
        }

        context.AddSource(
            $"ProxiedModels/{symbol.Name}",
            $$"""
            {{targetSyntax.GetFormattedUsingDirectives()}}

            namespace {{symbol.ContainingNamespace}};

            {{syntax.NormalizeWhitespace()}}
            """
        );
    }

    private static ClassDeclarationSyntax? CreateConverterSyntax(
        ITypeSymbol symbol,
        IPropertySymbol[] extended,
        SemanticModel semanticModel)
    {
        var typeName = symbol.ToDisplayString();

        var syntax = SyntaxFactory.ClassDeclaration(
            [],
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                SyntaxFactory.Token(SyntaxKind.SealedKeyword)
            ),
            SyntaxFactory.Identifier($"Extended{symbol.Name}Converter"),
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

        var read = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override {{typeName}}? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
              {
                  using var jsonDocument = JsonDocument.ParseValue(ref reader);
                  var element = jsonDocument.RootElement;
                  var result = element.Deserialize(GetTypeInfoWithoutConverter(options));

                  if(result is null) return null;

                  {{
                      string.Join(
                          "\n",
                          extended.Select(x =>
                              $$"""
                                result.{{x.Name}} = element.Deserialize<{{x.Type.ToDisplayString()}}>(options)!;
                                """
                          )
                      )
                  }}

                  return result;
              }
              """
        );

        var write = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override void Write(Utf8JsonWriter writer, {{typeName}} value, JsonSerializerOptions options)
              {
                  var node = JsonSerializer.SerializeToNode(value, GetTypeInfoWithoutConverter(options))
                      as System.Text.Json.Nodes.JsonObject;

                  if(node is null) return;

                  {{
                      string.Join(
                          "\n",
                          extended.Select(x =>
                              $$"""
                              var {{x.Name}}Prop = JsonSerializer.SerializeToNode(value.{{x.Name}}, options)
                                  as System.Text.Json.Nodes.JsonObject;

                              if ({{x.Name}}Prop is not null)
                              {
                                  foreach(var prop in {{x.Name}}Prop)
                                      node.Add(prop);
                              }

                              """

                          )
                      )
                  }}

                  node.WriteTo(writer, options);
              }
              """
        );

        if (read is null || write is null)
            return null;

        return syntax.AddMembers(read, write);
    }
}
