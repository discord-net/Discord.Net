using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ModifyCurrentUserParams> ModifyCurrentUserParams => field ??= Discord.Models.Json.ModifyCurrentUserParams.CreateTypeInfo(Options);
}

public record ModifyCurrentUserParams(
    Discord.Models.Optional<string> Username,
    Discord.Models.Optional<Nullable<Discord.Models.ImageData>> Avatar,
    Discord.Models.Optional<Nullable<Discord.Models.ImageData>> Banner
) : 
    IModifyCurrentUserParams,
    IJsonModel,
    IApiModel<IModifyCurrentUserParams, ModifyCurrentUserParams>
{
    public static JsonTypeInfo<ModifyCurrentUserParams> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ModifyCurrentUserParams>(
        options,
        new JsonObjectInfoValues<ModifyCurrentUserParams>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ModifyCurrentUserParams(
                Username: (Discord.Models.Optional<string>)args[0],
                Avatar: (Discord.Models.Optional<Nullable<Discord.Models.ImageData>>)args[1],
                Banner: (Discord.Models.Optional<Nullable<Discord.Models.ImageData>>)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ModifyCurrentUserParams),
                Getter = static instance => ((Discord.Models.Json.ModifyCurrentUserParams)instance).Username,
                Setter = null,
                PropertyName = "Username",
                JsonPropertyName = "username",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ModifyCurrentUserParams),
                Getter = static instance => ((Discord.Models.Json.ModifyCurrentUserParams)instance).Avatar,
                Setter = null,
                PropertyName = "Avatar",
                JsonPropertyName = "avatar",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Discord.Models.ImageData>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ModifyCurrentUserParams),
                Getter = static instance => ((Discord.Models.Json.ModifyCurrentUserParams)instance).Banner,
                Setter = null,
                PropertyName = "Banner",
                JsonPropertyName = "banner",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Username",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Avatar",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Discord.Models.ImageData>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Banner",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Discord.Models.ImageData>>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ModifyCurrentUserParams From(IModifyCurrentUserParams model) => (model as ModifyCurrentUserParams) ?? new ModifyCurrentUserParams(
        Username: model.Username,
        Avatar: model.Avatar,
        Banner: model.Banner
    );

    static ModifyCurrentUserParams IApiModel<IModifyCurrentUserParams, ModifyCurrentUserParams>.From(IModifyCurrentUserParams model) => From(model);
}