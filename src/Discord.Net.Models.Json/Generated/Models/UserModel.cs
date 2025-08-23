using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<UserModel> UserModel => field ??= Discord.Models.Json.UserModel.CreateTypeInfo(Options);
}

public record UserModel(
    string Username,
    string Discriminator,
    string? GlobalName,
    string? Avatar,
    Discord.Models.Optional<string?> Banner,
    Discord.Models.Optional<Nullable<int>> AccentColor,
    Discord.Models.Optional<bool> Bot,
    Discord.Models.Optional<bool> System,
    Discord.Models.Optional<Discord.Models.UserFlags> Flags,
    Discord.Models.Optional<Discord.Models.UserFlags> PublicFlags,
    Discord.Snowflake Id
) : 
    IUserModel,
    IJsonModel,
    IApiModel<IUserModel, UserModel>
{
    public static JsonTypeInfo<UserModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<UserModel>(
        options,
        new JsonObjectInfoValues<UserModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new UserModel(
                Username: (string)args[0],
                Discriminator: (string)args[1],
                GlobalName: (string?)args[2],
                Avatar: (string?)args[3],
                Banner: (Discord.Models.Optional<string?>)args[4],
                Bot: (Discord.Models.Optional<bool>)args[5],
                System: (Discord.Models.Optional<bool>)args[6],
                Flags: (Discord.Models.Optional<Discord.Models.UserFlags>)args[7],
                PublicFlags: (Discord.Models.Optional<Discord.Models.UserFlags>)args[8],
                Id: (Discord.Snowflake)args[9],
                AccentColor: (Discord.Models.Optional<Nullable<int>>)args[10]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Username,
                Setter = null,
                PropertyName = "Username",
                JsonPropertyName = "username",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Discriminator,
                Setter = null,
                PropertyName = "Discriminator",
                JsonPropertyName = "discriminator",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string?>(
            options,
            new JsonPropertyInfoValues<string?>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).GlobalName,
                Setter = null,
                PropertyName = "GlobalName",
                JsonPropertyName = "global_name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string?>(
            options,
            new JsonPropertyInfoValues<string?>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Avatar,
                Setter = null,
                PropertyName = "Avatar",
                JsonPropertyName = "avatar",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string?>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string?>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Banner,
                Setter = null,
                PropertyName = "Banner",
                JsonPropertyName = "banner",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Bot,
                Setter = null,
                PropertyName = "Bot",
                JsonPropertyName = "bot",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).System,
                Setter = null,
                PropertyName = "System",
                JsonPropertyName = "system",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.UserFlags>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.UserFlags>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.UserFlags>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.UserFlags>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).PublicFlags,
                Setter = null,
                PropertyName = "PublicFlags",
                JsonPropertyName = "public_flags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<int>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<int>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UserModel),
                Getter = static instance => ((Discord.Models.Json.UserModel)instance).AccentColor,
                Setter = null,
                PropertyName = "AccentColor",
                JsonPropertyName = "accent_color",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Username",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Discriminator",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GlobalName",
           ParameterType = typeof(string),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "Avatar",
           ParameterType = typeof(string),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "Banner",
           ParameterType = typeof(Discord.Models.Optional<string?>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Bot",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "System",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.UserFlags>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PublicFlags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.UserFlags>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AccentColor",
           ParameterType = typeof(Discord.Models.Optional<Nullable<int>>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static UserModel From(IUserModel model) => (model as UserModel) ?? new UserModel(
        Username: model.Username,
        Discriminator: model.Discriminator,
        GlobalName: model.GlobalName,
        Avatar: model.Avatar,
        Banner: model.Banner,
        Bot: model.Bot,
        System: model.System,
        Flags: model.Flags,
        PublicFlags: model.PublicFlags,
        Id: model.Id,
        AccentColor: model.AccentColor
    );

    static UserModel IApiModel<IUserModel, UserModel>.From(IUserModel model) => From(model);
}