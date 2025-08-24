using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Discord.Models.Json.Converters;

namespace Discord.Models.Json;

public sealed partial class DiscordJsonContext(JsonSerializerOptions? options = null) : JsonSerializerContext(options)
{
    [field: MaybeNull]
    public JsonTypeInfo<Snowflake> Snowflake
        => field ??= JsonMetadataServices.CreateValueInfo<Snowflake>(Options, SnowflakeConverter.Instance);
    
    [field: MaybeNull]
    public JsonTypeInfo<ImageData> ImageData
        => field ??= JsonMetadataServices.CreateValueInfo<ImageData>(Options, ImageDataConverter.Instance);

    [field: MaybeNull]
    public JsonTypeInfo<IUserModel> BaseUser
        => field ??= JsonMetadataServices.CreateValueInfo<IUserModel>(Options, new UserConverter(UserModel, CurrentUserModel));
    
    public override JsonTypeInfo? GetTypeInfo(Type type)
    {
        if (type == typeof(Snowflake)) return Snowflake;
        if (type == typeof(ImageData)) return ImageData;

        if (type.IsInterface && TryGetCoreJsonTypeInfo(type, out var info))
            return info;

        if (TryGetBuiltIn(type, out var builtIn)) return builtIn;
        if (TryGetImplicitTypeInfo(type, out var implicitInfo)) return implicitInfo;
        
        return LookupGeneratedTypeInfo(type);
    }

    protected override JsonSerializerOptions? GeneratedSerializerOptions => options;
}