using System.Reflection;
using Discord.Models.Json.Generator.Specs;
using Discord.Models.Validation;

namespace Discord.Models.Json.Generator.Hell;

public class TypeVisitor
{
    private readonly TypeVisitCache _cache;

    private readonly List<(Type Type, string Name)> _infoProperties;
    private TypeSpec _spec;

    public TypeVisitor(TypeVisitCache cache)
    {
        _cache = cache;
        _infoProperties = [];

        _spec = new(
            "DiscordJsonContext",
            "class",
            modifiers: ["partial"]
        );
    }

    public static void Run(IEnumerable<Type> types)
    {
        var visitor = new TypeVisitor(new TypeVisitCache(types));

        while (visitor._cache.TryGetNext(out var type))
            visitor.Process(type);

        visitor._spec.Methods.Add(
            new MethodSpec(
                "TryGetImplicitTypeInfo",
                "bool",
                Accessibility.Private,
                parameters: [("Type", "type"), ("[MaybeNullWhen(false)] out JsonTypeInfo", "info")],
                body:
                $"""
                 {
                     string.Join(
                         Environment.NewLine,
                         visitor._infoProperties.Select(x =>
                             $"if (type == typeof({x.Type.ToCodeString()})) return (info = {x.Name}) is not null;"
                         )
                     )
                 }
                 
                 info = null;
                 return false;
                 """
            )
        );

        File.WriteAllText(
            Path.Combine(Generator.CONTEXT_OUT_DIR, "DiscordJsonContext.Implicits.cs"),
            $"""
             using System.Diagnostics.CodeAnalysis;
             using System.Text.Json;
             using System.Text.Json.Serialization;
             using System.Text.Json.Serialization.Metadata;

             namespace Discord.Models.Json;

             {visitor._spec}
             """
        );
    }

    private void Process(Type type)
    {
        /*
         * Cases:
         *   - built-ins: pass
         *   - nullable value type: generate type info and add the generic to the cache
         *   - collections: TODO
         *
         */

        if (type == typeof(Snowflake)) return;
        if (type == typeof(PermissionBitSet)) return;
        if (type == typeof(EmojiId)) return;
        
        // pass for built-ins
        if (Generator.BuiltIns.Contains(type)) return;


        if (type is {Name: "Nullable`1", GenericTypeArguments: [{ } inner]})
        {
            VisitNullable(type, inner);
            return;
        }

        if (type is {Name: "Optional`1", GenericTypeArguments: [{ } innerOptional]})
        {
            VisitOptional(type, innerOptional);
            return;
        }

        if (type is {Name: "IReadOnlyList`1", GenericTypeArguments: [{ } element]})
        {
            VisitList(type, element);
            return;
        }

        if (type.IsEnum)
        {
            VisitEnum(type);
            return;
        }

        if (type is {Name: "IdOrModel`2", GenericTypeArguments: [{ } idType, { } modelType]})
        {
            VisitIdOrModel(type, idType, modelType);
            return;
        }

        // is it a model type
        if (type.GetCustomAttribute<APIModelAttribute>() is not null) return;

        throw new InvalidOperationException($"Unknown type for STJ '{type}'");
    }

    private string GetPropertyName(Type type)
        => type switch
        {
            {Name: "Optional`1"} => $"Optional{GetPropertyName(type.GenericTypeArguments[0])}",
            {Name: "Nullable`1"} => $"Nullable{GetPropertyName(type.GenericTypeArguments[0])}",
            {Name: "IReadOnlyList`1"} => $"ListOf{GetPropertyName(type.GenericTypeArguments[0])}",
            {Name: "IdOrModel`2"} => $"IdOrModelOf{GetPropertyName(type.GenericTypeArguments[1])}",
            _ when Generator.IsAPIModel(type) => type.Name[1..],
            _ => type.Name
        };

    private void VisitIdOrModel(Type type, Type idType, Type modelType)
    {
        _cache.Add(idType);
        _cache.Add(modelType);
        
        var propName = GetPropertyName(type);

        _infoProperties.Add((type, propName));
        
        _spec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{type.ToCodeString()}>",
                propName,
                Accessibility.Public,
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}],
                Expression:
                $"""
                 field ??= JsonMetadataServices.CreateValueInfo<{type.ToCodeString()}>(
                     Options, 
                     new Converters.IdOrModelConverter<{idType.ToCodeString()}, {modelType.ToCodeString()}>(
                        {GetPropertyName(idType)},
                        {GetPropertyName(modelType)}
                     )
                 )
                 """
            )
        );
    }
    
    private void VisitEnum(Type type)
    {
        // generate a converter
        var propName = GetPropertyName(type);

        _infoProperties.Add((type, propName));

        _spec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{type.ToCodeString()}>",
                propName,
                Accessibility.Public,
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}],
                Expression:
                $"""
                 field ??= JsonMetadataServices.CreateValueInfo<{type.ToCodeString()}>(
                     Options, 
                     JsonMetadataServices.GetEnumConverter<{type.ToCodeString()}>(Options)
                 )
                 """
            )
        );
    }

    private void VisitOptional(Type type, Type innerType)
    {
        // prep the inner type
        _cache.Add(innerType);

        // generate a converter
        var propName = GetPropertyName(type);

        _infoProperties.Add((type, propName));

        _spec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{type.ToCodeString()}>",
                propName,
                Accessibility.Public,
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}],
                Expression:
                $"""
                 field ??= JsonMetadataServices.CreateValueInfo<{type.ToCodeString()}>(
                     Options, 
                     Converters.OptionalConverter<{innerType.ToCodeString()}>.Instance
                 )
                 """
            )
        );
    }

    private void VisitList(Type type, Type innerType)
    {
        // prep the inner type
        _cache.Add(innerType);

        // generate a converter
        var propName = GetPropertyName(type);

        _infoProperties.Add((type, propName));

        _spec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{type.ToCodeString()}>",
                propName,
                Accessibility.Public,
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}],
                Expression:
                $"""
                 field ??= JsonMetadataServices.CreateIEnumerableInfo<{type.ToCodeString()}, {innerType.ToCodeString()}>(
                     Options, 
                     new JsonCollectionInfoValues<{type.ToCodeString()}>()
                 )
                 """
            )
        );
    }

    private void VisitNullable(Type type, Type innerType)
    {
        // prep the inner type
        _cache.Add(innerType);

        // generate a converter
        var propName = GetPropertyName(type);

        _infoProperties.Add((type, propName));

        _spec.Properties.Add(
            new PropertySpec(
                $"JsonTypeInfo<{type.ToCodeString()}>",
                propName,
                Accessibility.Public,
                Attributes: [new AttributeSpec("MaybeNull") {Target = "field"}],
                Expression:
                $"""
                 field ??= JsonMetadataServices.CreateValueInfo<{innerType.ToCodeString()}?>(
                     Options, 
                     JsonMetadataServices.GetNullableConverter<{innerType.ToCodeString()}>(Options)
                 )
                 """
            )
        );
    }
}