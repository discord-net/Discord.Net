using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Discord.Converters.Meta;


[RequiresDynamicCode("Requires reflection")]
public class UnionConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
        => typeToConvert
            .GetProperties()
            .Any(x => x.GetCustomAttribute<DiscriminatedUnionAttribute>() is not null);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var unionProps = typeToConvert
            .GetProperties()
            .Select(x => (Prop: x, Attr: x.GetCustomAttribute<DiscriminatedUnionAttribute>()))
            .Where(x => x.Attr is not null)
            .ToArray();

        if (unionProps.Length == 0) 
            throw new InvalidOperationException("Missing union properties");

        var propertyInfos = new UnionPropertyInfo[unionProps.Length];

        for (var i = 0; i < unionProps.Length; i++)
        {
            var (propertyInfo, attribute) = unionProps[i];
            
            var targetProp = typeToConvert
                .GetProperty(attribute!.PropertyName, BindingFlags.Instance | BindingFlags.Instance);

            if (targetProp is null)
                throw new InvalidOperationException($"Cannot find prop '{attribute.PropertyName}' on {typeToConvert}");
            
            var targetJsonName = targetProp.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? targetProp.Name;
            var propJsonName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? propertyInfo.Name;

            var entries = propertyInfo
                .GetCustomAttributes()
                .OfType<IDiscriminatedUnionEntry>()
                .ToArray();
            
            if(entries.Length == 0)
                throw new InvalidOperationException($"Missing discriminated union entries for property '{propertyInfo.Name}'");
            
            propertyInfos[i] = new UnionPropertyInfo(
                propertyInfo,
                targetJsonName,
                propJsonName,
                targetProp.PropertyType,
                entries
                    .SelectMany(x => x
                        .Values
                        .Select(y => 
                            new KeyValuePair<object, Type>(y, x.Type)
                        )
                    )
                    .ToDictionary()
                    
            );
        }

        return (JsonConverter)Activator.CreateInstance(
            typeof(UnionTypeConverter<>)
                .MakeGenericType(
                    typeToConvert
                ),
            [
                propertyInfos
            ]
        )!;
    }
}

file sealed record UnionPropertyInfo(
    PropertyInfo Property,
    string TargetPropJsonName,
    string PropertyJsonName,
    Type TargetPropType,
    Dictionary<object, Type> Entries
);


file sealed class UnionTypeConverter<T> : JsonConverter<T>
    where T : class
{
    private readonly UnionPropertyInfo[] _properties;
    private JsonTypeInfo<T>? _innerInfo;

    public UnionTypeConverter(
        UnionPropertyInfo[] properties,
        JsonTypeInfo<T> innerInfo)
    {
        _properties = properties;
        _innerInfo = innerInfo;
    }
    
    private JsonTypeInfo<T> GetInnerTypeInfo(Type typeToConvert, JsonSerializerOptions options)
    {
        if (_innerInfo is not null)
            return _innerInfo;

        return _innerInfo = (JsonTypeInfo<T>)options.GetTypeInfo(typeToConvert);
    }
    
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var info = GetInnerTypeInfo(typeToConvert, options);
        
        var obj = JsonDocument.ParseValue(ref reader).RootElement;
        var instance = info.CreateObject?.Invoke() ?? (T?)Activator.CreateInstance(typeToConvert);

        if (instance is null) return default;
        
        foreach (var (property, targetName, propName, targetType, entries) in _properties)
        {
            if (!obj.TryGetProperty(propName, out var propJsonValue))
            {
                if (property.PropertyType.Name is "Optional")
                    continue;
                
                throw new JsonException(
                    $"Required property '{targetName}' was not present in the provided json object"
                );
            }
            
            if(!obj.TryGetProperty(targetName, out var targetProp))
                throw new JsonException(
                    $"Required property '{targetName}' was not present in the provided json object");
            
            if(targetProp.Deserialize(targetType, options) is not {} value || !entries.TryGetValue(value, out var type))
                throw new JsonException($"Invalid value for property '{targetName}'");
            
            property.SetValue(instance, propJsonValue.Deserialize(type, options));
        }

        return instance;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}