namespace Discord.Models.Json.Generator;

partial class Generator
{
    private static void GenerateVariants(List<ModelTarget> targets)
    {
        // find all models with some variants
        foreach (var target in targets.Where(HasVariants))
        {
            GenerateVariantConverter(target, targets);
        }


        bool HasVariants(ModelTarget target)
            => target
                   .SpecModel
                   .Hierarchy
                   ?.Descendants
                   .Any(x =>
                       targets
                           .FirstOrDefault(y => y.Type.Name == x)
                           ?.SpecModel.Variant is not null
                   )
               ?? false;
    }

    private static void GenerateVariantConverter(ModelTarget target, List<ModelTarget> targets)
    {
        var variantTargetsGroup = target
            .SpecModel
            .Hierarchy
            ?.Descendants
            .Select(x => targets.FirstOrDefault(y => y.Type.Name == x)!)
            .Where(x => x?.SpecModel.Variant is not null)
            .GroupBy(x => x.SpecModel.Variant!.PropertyName)
            .ToArray();

        if (variantTargetsGroup is null or {Length: not 1})
            throw new InvalidOperationException("Invalid variant configuration");

        var variantTargets = variantTargetsGroup
            .First();

        var sortedVariants = variantTargets
            .OrderByDescending(x => x.SpecModel.Hierarchy?.Ancestors.Count)
            .ToArray();

        if (target.GetProperty(variantTargets.Key) is not { } variantPropertyInfo)
            throw new InvalidOperationException("Missing variant property");

        var converterName = $"{target.TypeSpec.Name}VariantConverter";
        var modelName = $"Discord.Models.Json.{target.TypeSpec.Name}";
        var coreModelName = target.Type.ToCodeString();
        
        // get the json name
        if(!target.SpecModel.Properties.TryGetValue(variantTargets.Key, out var variantJsonProperty))
            throw new InvalidOperationException("Missing variant property");

        target.CoreConverter =
            $"new {converterName}({string.Join(", ", variantTargets.Select(x => x.TypeSpec.Name))}, {target.TypeSpec.Name})";
        
        var code =
            $$"""
              using System.Text.Json;
              using System.Text.Json.Nodes;
              using System.Text.Json.Serialization;
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Models.Json.Converters;

              public sealed class {{target.TypeSpec.Name}}VariantConverter : JsonConverter<{{coreModelName}}>
              {
                  {{
                      string.Join(
                          Environment.NewLine.Postfix(4),
                          variantTargets.Select(x =>
                              $"private readonly JsonTypeInfo<Discord.Models.Json.{x.TypeSpec.Name}> _{x.TypeSpec.Name.ToCamelCase()};"
                          )
                      )
                  }}
                  
                  private readonly JsonTypeInfo<{{modelName}}> _default;

                  public {{converterName}}(
                      {{
                          string.Join(
                              $",{Environment.NewLine.Postfix(8)}",
                              variantTargets.Select(x => $"JsonTypeInfo<Discord.Models.Json.{x.TypeSpec.Name}> {x.TypeSpec.Name.ToCamelCase()}")
                          )
                      }},
                      JsonTypeInfo<{{modelName}}> @default
                  )
                  {
                      _default = @default;
                      {{
                          string.Join(
                              $"{Environment.NewLine.Postfix(8)}",
                              variantTargets.Select(x => $"_{x.TypeSpec.Name.ToCamelCase()} = {x.TypeSpec.Name.ToCamelCase()};")
                          )
                      }}
                  }
                  
                  public override {{coreModelName}}? Read(
                      ref Utf8JsonReader reader,
                      Type typeToConvert,
                      JsonSerializerOptions options
                  )
                  {
                      if (JsonNode.Parse(ref reader) is not JsonObject jsonObject)
                          throw new JsonException("Expected object type");
                          
                      if (!jsonObject.TryGetPropertyValue("{{variantJsonProperty.Json}}", out var variant))
                          return JsonSerializer.Deserialize(ref reader, _default);
                          
                      JsonTypeInfo info = variant.Deserialize<{{variantPropertyInfo.PropertyType.ToCodeString()}}>(options) switch 
                      {
                          {{
                              string.Join(
                                  $",{Environment.NewLine.Postfix(12)}",
                                  sortedVariants.Select(x =>
                                      $"""
                                       {
                                           string.Join(
                                               " or ",
                                               x.SpecModel.Variant!.Values.Select(x =>
                                                   FormatVariantValue(variantPropertyInfo.PropertyType, x)
                                               )
                                           )
                                       } => _{x.TypeSpec.Name.ToCamelCase()}
                                       """
                                  )
                              )
                          }},
                          _ => _default
                      };
                      
                      return ({{coreModelName}}?)jsonObject.Deserialize(info);
                  }
                  
                  public override void Write(Utf8JsonWriter writer, {{coreModelName}} value, JsonSerializerOptions options) 
                  {
                      JsonTypeInfo info = value switch 
                      {
                          {{
                              string.Join(
                                  $",{Environment.NewLine.Postfix(12)}",
                                  sortedVariants.Select(x => 
                                      $"{x.Type.ToCodeString()} => _{x.TypeSpec.Name.ToCamelCase()}"
                                  )
                              )
                          }},
                          _ => _default
                      };
                      
                      JsonSerializer.Serialize(writer, value, info);
                  }
              }
              """;

        File.WriteAllText(
            Path.Combine(CONVERTERS_OUT_DIR, $"{modelName}.Variants.cs"),
            code
        );
    }

    private static string FormatVariantValue(Type type, object value)
    {
        if (type.IsEnum)
        {
            return $"{type.ToCodeString()}.{value}";
        }

        throw new InvalidOperationException($"Unknown variant value type '{type}'");
    }
}