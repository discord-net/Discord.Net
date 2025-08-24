using System.Reflection;
using System.Runtime.CompilerServices;
using Discord.Models.Json.Generator.Hell;
using Discord.Models.Json.Generator.Specs;
using Discord.Models.Validation;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Discord.Models.Json.Generator;

public static partial class Generator
{
    public const string SPEC_PATH = "../../../../../src/Discord.Net.Models.Json/spec";
    public const string CONTEXT_OUT_DIR = "../../../../../src/Discord.Net.Models.Json/Generated";
    public const string MODELS_OUT_DIR = "../../../../../src/Discord.Net.Models.Json/Generated/Models";
    public const string CONVERTERS_OUT_DIR = "../../../../../src/Discord.Net.Models.Json/Generated/Converters";

    private static IDeserializer _deserializer = new DeserializerBuilder()
        .WithNodeDeserializer(SpecProperty.Serializer.Instance)
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    private static ISerializer _serializer = new SerializerBuilder()
        .WithTypeConverter(SpecProperty.Serializer.Instance)
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitDefaults)
        .Build();

    private static HashSet<Type> _modelTypes;

    public static void Run()
    {
        if (!Directory.Exists(MODELS_OUT_DIR)) Directory.CreateDirectory(MODELS_OUT_DIR);
        if (!Directory.Exists(CONTEXT_OUT_DIR)) Directory.CreateDirectory(CONTEXT_OUT_DIR);
        if (!Directory.Exists(CONVERTERS_OUT_DIR)) Directory.CreateDirectory(CONVERTERS_OUT_DIR);

        // find all models to generate

        _modelTypes = typeof(IModel)
            .Assembly
            .GetTypes()
            .Where(IsAPIModel)
            .ToHashSet();

        var targets =
            _modelTypes
                .Select(ProcessType)
                .ToList();

        GenerateVariants(targets);
        GenerateContexts(targets);

        TypeVisitor.Run(targets.SelectMany(x => x
                .SpecModel
                .Properties
                .Keys
                .Select(y => x.GetProperty(y)?.PropertyType)
                .Where(x => x is not null)!
            )!
        );
    }

    private static ModelTarget ProcessType(Type model)
    {
        var spec = GenerateModelRecord(model);

        var specName = GetSpecName(model);

        // find or load the spec file
        var specFilePath = Path.Combine(SPEC_PATH, $"{specName}.yaml");

        SpecModel mappingSpec;

        var hierarchy = _modelTypes
            .Where(x =>
                x != model &&
                (
                    x.IsAssignableFrom(model)
                    ||
                    x.IsAssignableTo(model)
                )
            )
            .ToArray();

        var specHierarchy = hierarchy.Length is 0
            ? null
            : new SpecHierarchy()
            {
                Ancestors = [..hierarchy.Where(model.IsAssignableTo).Select(x => x.Name)],
                Descendants = [..hierarchy.Where(model.IsAssignableFrom).Select(x => x.Name)],
            };

        SpecVariant? variant = null;
        if (model.GetCustomAttribute<VariantAttribute>() is { } variantAttribute)
        {
            variant = new()
            {
                PropertyName = variantAttribute.PropertyName,
                Values = variantAttribute.Values
            };
        }
        
        if (Path.Exists(specFilePath))
        {
            mappingSpec = _deserializer.Deserialize<SpecModel>(
                File.ReadAllText(specFilePath)
            );

            mappingSpec.Hierarchy = specHierarchy;
            mappingSpec.Variant = variant;

            // add missing properties
            foreach (var modelProp in GetAllProperties(model))
            {
                if (!mappingSpec.Properties.ContainsKey(modelProp.Name))
                {
                    mappingSpec.Properties[modelProp.Name] = new SpecProperty()
                    {
                        Json = UnderscoredNamingConvention.Instance.Apply(modelProp.Name)
                    };
                }
            }
        }
        else
        {
            mappingSpec = new SpecModel()
            {
                Base = model.Name,
                Properties = GetAllProperties(model)
                    .ToDictionary(x => x.Name, x => new SpecProperty()
                    {
                        Json = UnderscoredNamingConvention.Instance.Apply(x.Name)
                    }),
                Hierarchy = specHierarchy,
                Variant = variant
            };
        }

        var target = new ModelTarget(model, spec, mappingSpec);
        GenerateSTJMapping(target);

        ImplementApiModelInterface(target);

        // write the spec
        var outFile = Path.Combine(MODELS_OUT_DIR, $"{spec.Name}.cs");

        File.WriteAllText(
            outFile,
            $"""
             using System.Diagnostics.CodeAnalysis;
             using System.Text.Json;
             using System.Text.Json.Serialization;
             using System.Text.Json.Serialization.Metadata;


             namespace Discord.Models.Json;

             {target.JsonContextPartialSpec}

             {spec}
             """
        );

        File.WriteAllText(specFilePath, _serializer.Serialize(mappingSpec));

        return target;
    }

    public static string GetSpecName(Type type)
        => type
            .Name[1..]
            .Replace("Model", string.Empty)
            .ToSnakeCase();

    private static void ImplementApiModelInterface(ModelTarget target)
    {
        var baseType = $"IApiModel<{target.Type.Name}, {target.TypeSpec.Name}>";
        target.TypeSpec.Bases.Add(baseType);

        target.TypeSpec.Methods.AddRange(
            new MethodSpec(
                "From",
                target.TypeSpec.Name,
                Accessibility.Public,
                ["static"],
                [(target.Type.Name, "model")],
                expression:
                $$"""
                  (model as {{target.TypeSpec.Name}}) ?? new {{target.TypeSpec.Name}}(
                      {{
                          string.Join($",{Environment.NewLine}    ", target.SpecModel.Properties.Select(x => $"{x.Key}: model.{x.Key}"))
                      }}
                  )
                  """
            ),
            new MethodSpec(
                "From",
                target.TypeSpec.Name,
                modifiers: ["static"],
                parameters: [(target.Type.Name, "model")],
                explicitInterfaceImplementation: baseType,
                expression: "From(model)"
            )
        );
    }

    private static TypeSpec GenerateModelRecord(Type modelType)
        => new TypeSpec(
            modelType.Name[1..],
            "record",
            bases: [modelType.Name, "IJsonModel"],
            record: true,
            parameters: GetAllProperties(modelType)
                .Select(x =>
                {
                    var type = x.PropertyType.ToCodeString(x.GetCustomAttribute<NullableAttribute>()?.NullableFlags);
                    return new ParameterSpec(
                        type,
                        x.Name
                    );
                })
        );


    private static IEnumerable<PropertyInfo> GetAllProperties(Type type)
    {
        var seenNames = new HashSet<string>();
        var seen = new HashSet<Type>();
        var queue = new Queue<Type>([type]);

        while (queue.TryDequeue(out var current))
        {
            if (!seen.Add(current)) continue;

            foreach (var propertyInfo in current.GetProperties())
            {
                if (seenNames.Add(propertyInfo.Name))
                    yield return propertyInfo;
            }

            foreach (var iface in current.GetInterfaces())
            {
                queue.Enqueue(iface);
            }
        }
    }

    public static bool IsAPIModel(Type type)
        => type.GetCustomAttribute<APIModelAttribute>() is not null;
}