using System.Text;
using Discord.Net.Hanz.Utils;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

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
        {
            return;
        }

        var converter = CreateConverterSyntax(symbol, extendedMembers, semanticModel);

        if (converter is null)
        {
            logger.Warn($"{symbol}: No converter was created");
            return;
        }

        jsonContext.RequestedNoConverterTypeInfos.Add(symbol);

        if (!jsonContext.AdditionalConverters.Add($"Discord.Converters.Extended{symbol.Name}Converter"))
        {
            logger.Warn($"{symbol}: converter already exists");
            return;
        }

        var filename = $"GeneratedConverters/Extended{symbol.Name}Converter";

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

              {{converter.NormalizeWhitespace()}}
              """,
            Encoding.UTF8
        );


        AddProxiesIfNeeded(symbol, extendedMembers, jsonContext);

        logger.Log($"{symbol}: Generated extended converter");
    }

    private static void AddProxiesIfNeeded(
        ITypeSymbol symbol,
        IPropertySymbol[] properties,
        JsonModels.Context context)
    {
        if (symbol.DeclaringSyntaxReferences.Length == 0)
            return;

        if (symbol.DeclaringSyntaxReferences[0].GetSyntax() is not TypeDeclarationSyntax targetSyntax)
            return;

        if (targetSyntax.Modifiers.All(x => x.RawKind != (ushort) SyntaxKind.PartialKeyword))
            return;

        var syntax = SyntaxUtils.CreateSourceGenClone(targetSyntax);

        foreach (var property in properties)
        {
            syntax = syntax
                .WithBaseList(
                    (syntax.BaseList ?? SyntaxFactory.BaseList())
                    .AddTypes(
                        SyntaxFactory.SimpleBaseType(
                            SyntaxFactory.ParseTypeName($"IExtendedModel<{property.Type}>")
                        )
                    )
                )
                .AddMembers(
                    SyntaxFactory.ParseMemberDeclaration(
                        $"{property.Type} IExtendedModel<{property.Type}>.ExtendedValue => {property.Name};"
                    )!
                );
        }

        var proxiedMembers = new Dictionary<IPropertySymbol, List<IPropertySymbol>>(SymbolEqualityComparer.Default);

        foreach (var property in properties)
        {
            var commonInterfaces = symbol.AllInterfaces.Where(x =>
                property.Type.AllInterfaces.Contains(x)
            );

            foreach (var commonInterface in commonInterfaces)
            foreach (var member in commonInterface.GetMembers().OfType<IPropertySymbol>())
            {
                if (!member.IsAbstract) continue;

                if (symbol.FindImplementationForInterfaceMember(member) is not null) continue;

                if (!proxiedMembers.TryGetValue(property, out var members))
                    proxiedMembers[property] = members = new();

                members.Add(member);
            }
        }

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

        var filename = $"ProxiedModels/{symbol.Name}";

        if (context.DynamicSources.ContainsKey(filename))
            return;

        context.DynamicSources[filename] = SourceText.From(
            $$"""
              {{targetSyntax.GetFormattedUsingDirectives()}}

              namespace {{symbol.ContainingNamespace}};

              {{syntax.NormalizeWhitespace()}}
              """,
            Encoding.UTF8
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

        var hasExtraMembers = TypeUtils.GetBaseTypesAndThis(symbol)
            .SelectMany(x => x
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(JsonModels.IsNotIgnoredJsonProperty)
            ).Any();
        
        if (hasExtraMembers && !JsonModels.AddGetTypeInfoToConverter(ref syntax, symbol))
            return null;

        var read = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override {{typeName}}? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
              {
                  {{(
                      hasExtraMembers
                          ? $$"""
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
                              """
                          : $$"""
                              var result = new {{typeName}}();
                              
                              {{
                                  string.Join(
                                      "\n",
                                      extended.Select(x =>
                                          $$"""
                                            result.{{x.Name}} = JsonSerializer.Deserialize<{{x.Type.ToDisplayString()}}>(ref reader, options)!;
                                            """
                                      )
                                  )
                              }}
                              
                              return result;
                              """
                  )}}
              
                  
              }
              """
        );

        var nodeInit = hasExtraMembers
            ? """
              JsonSerializer.SerializeToNode(value, GetTypeInfoWithoutConverter(options))
                  as System.Text.Json.Nodes.JsonObject;
                  
              if(node is null) return;
              """
            : "new System.Text.Json.Nodes.JsonObject();";
        
        var write = SyntaxFactory.ParseMemberDeclaration(
            $$"""
              public override void Write(Utf8JsonWriter writer, {{typeName}} value, JsonSerializerOptions options)
              {
                  {{(
                      extended.Length > 1 || hasExtraMembers
                      ? $$"""
                        var node = {{nodeInit}}
                        
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
                        """
                      : $$"""
                        JsonSerializer.Serialize(writer, value.{{extended[0].Name}}, options);
                        """
                      )}}
              }
              """
        );

        if (read is null || write is null)
            return null;

        return syntax.AddMembers(read, write);
    }
}