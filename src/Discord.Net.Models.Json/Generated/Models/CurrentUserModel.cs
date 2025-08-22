using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<CurrentUserModel> CurrentUserModel => field ??= Discord.Models.Json.CurrentUserModel.CreateTypeInfo(Options);
}

public record CurrentUserModel(
    Discord.Models.Optional<Discord.Models.PremiumType> PremiumType,
    Discord.Models.Optional<string> Email,
    Discord.Models.Optional<bool> IsVerified,
    Discord.Models.Optional<string> Locale,
    Discord.Models.Optional<bool> MFAEnabled,
    string Username,
    string Discriminator,
    string? GlobalName,
    string? Avatar,
    Discord.Models.Optional<string?> Banner,
    Discord.Models.Optional<bool> Bot,
    Discord.Models.Optional<bool> System,
    Discord.Models.Optional<Discord.Models.UserFlags> Flags,
    Discord.Models.Optional<Discord.Models.UserFlags> PublicFlags,
    Discord.Models.Snowflake Id
) : 
    ICurrentUserModel,
    IJsonModel,
    IApiModel<ICurrentUserModel, CurrentUserModel>
{
    public static JsonTypeInfo<CurrentUserModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<CurrentUserModel>(
        options,
        new JsonObjectInfoValues<CurrentUserModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new CurrentUserModel(
                PremiumType: (Discord.Models.Optional<Discord.Models.PremiumType>)args[0],
                Email: (Discord.Models.Optional<string>)args[1],
                IsVerified: (Discord.Models.Optional<bool>)args[2],
                Locale: (Discord.Models.Optional<string>)args[3],
                MFAEnabled: (Discord.Models.Optional<bool>)args[4],
                Username: (string)args[5],
                Discriminator: (string)args[6],
                GlobalName: (string?)args[7],
                Avatar: (string?)args[8],
                Banner: (Discord.Models.Optional<string?>)args[9],
                Bot: (Discord.Models.Optional<bool>)args[10],
                System: (Discord.Models.Optional<bool>)args[11],
                Flags: (Discord.Models.Optional<Discord.Models.UserFlags>)args[12],
                PublicFlags: (Discord.Models.Optional<Discord.Models.UserFlags>)args[13],
                Id: (Discord.Models.Snowflake)args[14]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.PremiumType>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.PremiumType>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).PremiumType,
                Setter = null,
                PropertyName = "PremiumType",
                JsonPropertyName = "premium_type",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Email,
                Setter = null,
                PropertyName = "Email",
                JsonPropertyName = "email",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).IsVerified,
                Setter = null,
                PropertyName = "IsVerified",
                JsonPropertyName = "is_verified",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Locale,
                Setter = null,
                PropertyName = "Locale",
                JsonPropertyName = "locale",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).MFAEnabled,
                Setter = null,
                PropertyName = "MFAEnabled",
                JsonPropertyName = "mfa_enabled",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Username,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Discriminator,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).GlobalName,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Avatar,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Banner,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Bot,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).System,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Flags,
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
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).PublicFlags,
                Setter = null,
                PropertyName = "PublicFlags",
                JsonPropertyName = "public_flags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.CurrentUserModel),
                Getter = static instance => ((Discord.Models.Json.CurrentUserModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "PremiumType",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.PremiumType>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Email",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "IsVerified",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Locale",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MFAEnabled",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Username",
           ParameterType = typeof(string),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Discriminator",
           ParameterType = typeof(string),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GlobalName",
           ParameterType = typeof(string),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "Avatar",
           ParameterType = typeof(string),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "Banner",
           ParameterType = typeof(Discord.Models.Optional<string?>),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Bot",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "System",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.UserFlags>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PublicFlags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.UserFlags>),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Models.Snowflake),
           Position = 14,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static CurrentUserModel From(ICurrentUserModel model) => (model as CurrentUserModel) ?? new CurrentUserModel(
        PremiumType: model.PremiumType,
        Email: model.Email,
        IsVerified: model.IsVerified,
        Locale: model.Locale,
        MFAEnabled: model.MFAEnabled,
        Username: model.Username,
        Discriminator: model.Discriminator,
        GlobalName: model.GlobalName,
        Avatar: model.Avatar,
        Banner: model.Banner,
        Bot: model.Bot,
        System: model.System,
        Flags: model.Flags,
        PublicFlags: model.PublicFlags,
        Id: model.Id
    );

    static CurrentUserModel IApiModel<ICurrentUserModel, CurrentUserModel>.From(ICurrentUserModel model) => From(model);
}