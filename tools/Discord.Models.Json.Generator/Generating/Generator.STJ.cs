using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Discord.Models.Json.Generator.Specs;

namespace Discord.Models.Json.Generator;

partial class Generator
{
    public static readonly Type[] BuiltIns =
    [
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(double),
        typeof(float),
        typeof(decimal),
        typeof(bool),
        typeof(char),
        typeof(string)
    ];

    private static void GenerateContexts(List<ModelTarget> targets)
    {
        GenerateLookups(targets);
        GenerateMappings(targets);
        GenerateBuiltIns();
    }


    private static void GenerateBuiltIns()
    {
        File.WriteAllText(
            Path.Combine(CONTEXT_OUT_DIR, "DiscordJsonContext.BuiltIns.cs"),
            $$"""
              using System.Diagnostics.CodeAnalysis;
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Models.Json;

              partial class DiscordJsonContext
              {
                  private bool TryGetBuiltIn(Type type, [MaybeNullWhen(false)] out JsonTypeInfo builtIn)
                  {
                      {{
                          string.Join(
                              $"{Environment.NewLine}        ",
                              BuiltIns.Select(x =>
                                  $"if (type == typeof({x.ToCodeString()})) return (builtIn = {x.Name}) is not null;"
                              )
                          )
                      }}
                      
                      builtIn = null;
                      return false;
                  }

                  {{
                      string.Join(
                          $"{Environment.NewLine}    ",
                          BuiltIns.Select(x =>
                              $"""
                                   [field: MaybeNull]
                                   public JsonTypeInfo<{x.ToCodeString()}> {x.Name}
                                       => field ??= JsonMetadataServices.CreateValueInfo<{x.ToCodeString()}>(Options, JsonMetadataServices.{x.Name}Converter);
                                   """.WithNewlinePadding(4)
                          )
                      )
                  }}
              }
              """
        );
    }

    private static void GenerateMappings(List<ModelTarget> targets)
    {
        File.WriteAllText(
            Path.Combine(CONTEXT_OUT_DIR, "DiscordJsonContext.Mapping.cs"),
            $$"""
              using System.Diagnostics.CodeAnalysis;

              namespace Discord.Models.Json;

              partial class DiscordJsonContext
              {
                  public static IJsonModel AsJsonModel(IModel model)
                  {
                      if (model is IJsonModel jsonModel) return jsonModel;
                      
                      return model switch 
                      {
                          {{
                              string.Join(
                                  $",{Environment.NewLine}            ",
                                  targets
                                      .OrderBy(x => targets.Count(y => y.Type.IsAssignableTo(x.Type) && x.Type != y.Type))
                                      .Select(x =>
                                      $"{x.Type.ToCodeString()} narrowed => Discord.Models.Json.{x.TypeSpec.Name}.From(narrowed)"
                                  )
                              )
                          }},
                          _ => throw new InvalidOperationException("The type '{model.GetType()}' is not implemented as a json model.")
                      };
                  }

                  public static bool TryGetJsonModel(Type modelInterface, [MaybeNullWhen(false)] out Type modelType)
                      => _interfaceMapping.TryGetValue(modelInterface, out modelType);

                  private static readonly Dictionary<Type, Type> _interfaceMapping = new Dictionary<Type, Type>()
                  {
                      {{
                          string.Join(
                              $",{Environment.NewLine}        ",
                              targets.Select(x =>
                                  $"{{ typeof(Discord.Models.{x.Type.Name}), typeof(Discord.Models.Json.{x.TypeSpec.Name}) }}"
                              )
                          )
                      }}
                  };
              }
              """
        );
    }

    private static void GenerateLookups(List<ModelTarget> targets)
    {
        File.WriteAllText(
            Path.Combine(CONTEXT_OUT_DIR, "DiscordJsonContext.Lookup.cs"),
            $$"""
              using System.Text.Json.Serialization.Metadata;

              namespace Discord.Models.Json;

              partial class DiscordJsonContext
              {
                  private JsonTypeInfo? LookupGeneratedTypeInfo(Type type)
                  {
                      {{
                          string.Join(
                              $"{Environment.NewLine}        ",
                              targets.Select(x =>
                                  $"""
                                   if (type == typeof(Discord.Models.Json.{x.TypeSpec.Name})) return this.{x.TypeSpec.Name};
                                   """
                              )
                          )
                      }}
                      
                      return null;
                  }
              }
              """
        );
    }

    private static void GenerateSTJMapping(
        ModelTarget target
    )
    {
        target.TypeSpec.Methods.Add(CreateTypeInfoMethod(target));
        target.TypeSpec.Methods.Add(CreatePropertyMapMethod(target));
        target.TypeSpec.Methods.Add(CreateCtorInfoMethod(target));

        // add the type info to the context
        target.JsonContextPartialSpec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{target.TypeSpec.Name}>",
                target.TypeSpec.Name,
                Accessibility.Public,
                Expression: $"field ??= Discord.Models.Json.{target.TypeSpec.Name}.CreateTypeInfo(Options)",
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}]
            )
        );
    }

    private static MethodSpec CreateCtorInfoMethod(ModelTarget target)
        => new MethodSpec(
            "CreateConstructorParameterInfos",
            "JsonParameterInfoValues[]",
            Accessibility.Private,
            ["static"],
            expression:
            $"""
             [
                 {
                     string.Join(
                         $",{Environment.NewLine}    ",
                         target.SpecModel.Properties.Select((x, i) =>
                             $$"""
                                   new()
                                   {
                                      Name = "{{x.Key}}",
                                      ParameterType = typeof({{target.GetProperty(x.Key)!.PropertyTypeToCodeString(true)}}),
                                      Position = {{i}},
                                      HasDefaultValue = false,
                                      DefaultValue = null,
                                      IsNullable = {{(
                                          target.GetProperty(x.Key)?.PropertyType.Name is "Nullable`1" ||
                                          target.GetProperty(x.Key)?.GetCustomAttribute<NullableAttribute>()?.NullableFlags.FirstOrDefault() is 2
                                      ).ToString().ToLower()}}
                                   }
                                   """.WithNewlinePadding(4)
                         )
                     )
                 }
             ]
             """
        );

    private static MethodSpec CreateTypeInfoMethod(ModelTarget target)
    {
        return new MethodSpec(
            "CreateTypeInfo",
            $"JsonTypeInfo<{target.TypeSpec.Name}>",
            Accessibility.Public,
            ["static"],
            [
                ("JsonSerializerOptions", "options")
            ],
            expression:
            $$"""
              JsonMetadataServices.CreateObjectInfo<{{target.TypeSpec.Name}}>(
                  options,
                  new JsonObjectInfoValues<{{target.TypeSpec.Name}}>()
                  {
                      ObjectWithParameterizedConstructorCreator = static args => {{CreateParameterConstructorFunc().WithNewlinePadding(8)}},
                      PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
                      ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
                  }
              )
              """
        );

        string CreateParameterConstructorFunc()
        {
            var sb = new StringBuilder($"new {target.TypeSpec.Name}(")
                .AppendLine();

            var i = 0;
            foreach (var (dotnetName, spec) in target.SpecModel.Properties)
            {
                if (i > 0) sb.AppendLine(",");

                sb.Append("    ");
                sb.Append($"{dotnetName}: ({target.GetProperty(dotnetName)!.PropertyTypeToCodeString()})args[{i++}]");
            }

            return sb.AppendLine().Append(")").ToString();
        }
    }

    private static MethodSpec CreatePropertyMapMethod(ModelTarget target)
    {
        return new MethodSpec(
            "CreatePropertyInfos",
            "JsonPropertyInfo[]",
            Accessibility.Public,
            ["static"],
            [
                ("JsonSerializerOptions", "options")
            ],
            expression:
            $$"""
              [
                  {{string.Join($",{Environment.NewLine}    ", target.SpecModel.Properties.Select(x => MakePropertyInfo(x.Key, x.Value).WithNewlinePadding(4)))}}
              ]
              """
        );


        string MakePropertyInfo(string dotnetName, SpecProperty property)
        {
            var dotnetPropertyInfo = target.GetProperty(dotnetName);

            if (dotnetPropertyInfo is null)
                throw new InvalidOperationException();

            return
                $$"""
                  JsonMetadataServices.CreatePropertyInfo<{{dotnetPropertyInfo.PropertyTypeToCodeString()}}>(
                      options,
                      new JsonPropertyInfoValues<{{dotnetPropertyInfo.PropertyTypeToCodeString()}}>
                      {
                          IsProperty = true,
                          IsPublic = true,
                          DeclaringType = typeof(Discord.Models.Json.{{target.TypeSpec.Name}}),
                          Getter = static instance => ((Discord.Models.Json.{{target.TypeSpec.Name}})instance).{{dotnetName}},
                          Setter = null,
                          PropertyName = "{{dotnetName}}",
                          JsonPropertyName = "{{property.Json}}",
                          IgnoreCondition = JsonIgnoreCondition.{{(
                              dotnetPropertyInfo.PropertyType.Name is "Optional`1" ? "WhenWritingDefault" : "Never"
                          )}}
                      }
                  )
                  """;
        }
    }
}