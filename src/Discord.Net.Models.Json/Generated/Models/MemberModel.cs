using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MemberModel> MemberModel => field ??= Discord.Models.Json.MemberModel.CreateTypeInfo(Options);
}

public record MemberModel(
    Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>> User,
    Discord.Models.Optional<string?> Nick,
    Discord.Models.Optional<string?> Avatar,
    Discord.Models.Optional<string?> Banner,
    System.Collections.Generic.IReadOnlyList<Discord.Snowflake> Roles,
    Nullable<DateTimeOffset> JoinedAt,
    Discord.Models.Optional<Nullable<DateTimeOffset>> PremiumSince,
    bool Deaf,
    bool Mute,
    Discord.Models.MemberFlags Flags,
    Discord.Models.Optional<bool> Pending,
    Discord.Models.Optional<Discord.Models.PermissionBitSet> Permissions,
    Discord.Models.Optional<Nullable<DateTimeOffset>> CommunicationsDisabledUntil,
    Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel?> AvatarDecorationData
) : 
    IMemberModel,
    IJsonModel,
    IApiModel<IMemberModel, MemberModel>
{
    public static JsonTypeInfo<MemberModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MemberModel>(
        options,
        new JsonObjectInfoValues<MemberModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MemberModel(
                User: (Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)args[0],
                Nick: (Discord.Models.Optional<string?>)args[1],
                Avatar: (Discord.Models.Optional<string?>)args[2],
                Banner: (Discord.Models.Optional<string?>)args[3],
                Roles: (System.Collections.Generic.IReadOnlyList<Discord.Snowflake>)args[4],
                JoinedAt: (Nullable<DateTimeOffset>)args[5],
                PremiumSince: (Discord.Models.Optional<Nullable<DateTimeOffset>>)args[6],
                Deaf: (bool)args[7],
                Mute: (bool)args[8],
                Flags: (Discord.Models.MemberFlags)args[9],
                Pending: (Discord.Models.Optional<bool>)args[10],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[11],
                CommunicationsDisabledUntil: (Discord.Models.Optional<Nullable<DateTimeOffset>>)args[12],
                AvatarDecorationData: (Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel?>)args[13]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).User,
                Setter = null,
                PropertyName = "User",
                JsonPropertyName = "user",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string?>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string?>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Nick,
                Setter = null,
                PropertyName = "Nick",
                JsonPropertyName = "nick",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string?>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string?>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Avatar,
                Setter = null,
                PropertyName = "Avatar",
                JsonPropertyName = "avatar",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string?>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string?>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Banner,
                Setter = null,
                PropertyName = "Banner",
                JsonPropertyName = "banner",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Roles,
                Setter = null,
                PropertyName = "Roles",
                JsonPropertyName = "roles",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<DateTimeOffset>>(
            options,
            new JsonPropertyInfoValues<Nullable<DateTimeOffset>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).JoinedAt,
                Setter = null,
                PropertyName = "JoinedAt",
                JsonPropertyName = "joined_at",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<DateTimeOffset>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<DateTimeOffset>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).PremiumSince,
                Setter = null,
                PropertyName = "PremiumSince",
                JsonPropertyName = "premium_since",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Deaf,
                Setter = null,
                PropertyName = "Deaf",
                JsonPropertyName = "deaf",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Mute,
                Setter = null,
                PropertyName = "Mute",
                JsonPropertyName = "mute",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.MemberFlags>(
            options,
            new JsonPropertyInfoValues<Discord.Models.MemberFlags>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Pending,
                Setter = null,
                PropertyName = "Pending",
                JsonPropertyName = "pending",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.PermissionBitSet>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.PermissionBitSet>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).Permissions,
                Setter = null,
                PropertyName = "Permissions",
                JsonPropertyName = "permissions",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<DateTimeOffset>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<DateTimeOffset>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).CommunicationsDisabledUntil,
                Setter = null,
                PropertyName = "CommunicationsDisabledUntil",
                JsonPropertyName = "communications_disabled_until",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel?>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel?>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MemberModel),
                Getter = static instance => ((Discord.Models.Json.MemberModel)instance).AvatarDecorationData,
                Setter = null,
                PropertyName = "AvatarDecorationData",
                JsonPropertyName = "avatar_decoration_data",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "User",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Nick",
           ParameterType = typeof(Discord.Models.Optional<string?>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Avatar",
           ParameterType = typeof(Discord.Models.Optional<string?>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Banner",
           ParameterType = typeof(Discord.Models.Optional<string?>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Roles",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Snowflake>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "JoinedAt",
           ParameterType = typeof(Nullable<DateTimeOffset>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "PremiumSince",
           ParameterType = typeof(Discord.Models.Optional<Nullable<DateTimeOffset>>),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Deaf",
           ParameterType = typeof(bool),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Mute",
           ParameterType = typeof(bool),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.MemberFlags),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Pending",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Permissions",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "CommunicationsDisabledUntil",
           ParameterType = typeof(Discord.Models.Optional<Nullable<DateTimeOffset>>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AvatarDecorationData",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IAvatarDecorationDataModel?>),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MemberModel From(IMemberModel model) => (model as MemberModel) ?? new MemberModel(
        User: model.User,
        Nick: model.Nick,
        Avatar: model.Avatar,
        Banner: model.Banner,
        Roles: model.Roles,
        JoinedAt: model.JoinedAt,
        PremiumSince: model.PremiumSince,
        Deaf: model.Deaf,
        Mute: model.Mute,
        Flags: model.Flags,
        Pending: model.Pending,
        Permissions: model.Permissions,
        CommunicationsDisabledUntil: model.CommunicationsDisabledUntil,
        AvatarDecorationData: model.AvatarDecorationData
    );

    static MemberModel IApiModel<IMemberModel, MemberModel>.From(IMemberModel model) => From(model);
}