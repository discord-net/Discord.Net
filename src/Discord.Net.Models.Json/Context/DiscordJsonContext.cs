using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Discord.Models.Json.Converters;

namespace Discord.Models.Json;

public sealed partial class DiscordJsonContext(JsonSerializerOptions? options) : JsonSerializerContext(options)
{
    [field: MaybeNull]
    public JsonTypeInfo<Snowflake> Snowflake
        => field ??= JsonMetadataServices.CreateValueInfo<Snowflake>(Options, SnowflakeConverter.Instance);
    
    public override JsonTypeInfo? GetTypeInfo(Type type)
    {
        if (type == typeof(Snowflake)) return Snowflake;
        
        if (TryGetJsonModel(type, out var jsonModelType)) 
            type = jsonModelType;

        if (TryGetBuiltIn(type, out var builtIn)) return builtIn;
        if (TryGetImplicitTypeInfo(type, out var implicitInfo)) return implicitInfo;
        
        return LookupGeneratedTypeInfo(type);
    }

    protected override JsonSerializerOptions? GeneratedSerializerOptions => options;
}