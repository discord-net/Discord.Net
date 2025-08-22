using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<TextChannelModel> TextChannelModel => field ??= Discord.Models.Json.TextChannelModel.CreateTypeInfo(Options);
}

public record TextChannelModel(
    bool IsNSFW,
    string? Topic,
    Discord.Models.Optional<int> RateLimitPerUser,
    Discord.Models.DefaultAutoArchiveDuration DefaultAutoArchiveDuration,
    string Name,
    Discord.Models.Snowflake GuildId,
    int Position,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel> PermissionOverwrites,
    Discord.Models.Optional<Discord.Models.PermissionBitSet> Permissions,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Discord.Models.Snowflake Id,
    Discord.Models.Optional<Nullable<Discord.Models.Snowflake>> ParentId
) : 
    ITextChannelModel,
    IJsonModel,
    IApiModel<ITextChannelModel, TextChannelModel>
{
    public static JsonTypeInfo<TextChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<TextChannelModel>(
        options,
        new JsonObjectInfoValues<TextChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new TextChannelModel(
                IsNSFW: (bool)args[0],
                Topic: (string?)args[1],
                RateLimitPerUser: (Discord.Models.Optional<int>)args[2],
                DefaultAutoArchiveDuration: (Discord.Models.DefaultAutoArchiveDuration)args[3],
                Name: (string)args[4],
                GuildId: (Discord.Models.Snowflake)args[5],
                Position: (int)args[6],
                PermissionOverwrites: (System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)args[7],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[8],
                Type: (Discord.Models.ChannelType)args[9],
                Flags: (Discord.Models.ChannelFlags)args[10],
                Id: (Discord.Models.Snowflake)args[11],
                ParentId: (Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>)args[12]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).IsNSFW,
                Setter = null,
                PropertyName = "IsNSFW",
                JsonPropertyName = "is_n_s_f_w",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string?>(
            options,
            new JsonPropertyInfoValues<string?>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Topic,
                Setter = null,
                PropertyName = "Topic",
                JsonPropertyName = "topic",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).RateLimitPerUser,
                Setter = null,
                PropertyName = "RateLimitPerUser",
                JsonPropertyName = "rate_limit_per_user",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.DefaultAutoArchiveDuration>(
            options,
            new JsonPropertyInfoValues<Discord.Models.DefaultAutoArchiveDuration>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).DefaultAutoArchiveDuration,
                Setter = null,
                PropertyName = "DefaultAutoArchiveDuration",
                JsonPropertyName = "default_auto_archive_duration",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).GuildId,
                Setter = null,
                PropertyName = "GuildId",
                JsonPropertyName = "guild_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Position,
                Setter = null,
                PropertyName = "Position",
                JsonPropertyName = "position",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).PermissionOverwrites,
                Setter = null,
                PropertyName = "PermissionOverwrites",
                JsonPropertyName = "permission_overwrites",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.PermissionBitSet>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.PermissionBitSet>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Permissions,
                Setter = null,
                PropertyName = "Permissions",
                JsonPropertyName = "permissions",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelFlags>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelFlags>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TextChannelModel),
                Getter = static instance => ((Discord.Models.Json.TextChannelModel)instance).ParentId,
                Setter = null,
                PropertyName = "ParentId",
                JsonPropertyName = "parent_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "IsNSFW",
           ParameterType = typeof(bool),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Topic",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "RateLimitPerUser",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultAutoArchiveDuration",
           ParameterType = typeof(Discord.Models.DefaultAutoArchiveDuration),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GuildId",
           ParameterType = typeof(Discord.Models.Snowflake),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Position",
           ParameterType = typeof(int),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PermissionOverwrites",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Permissions",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Models.Snowflake),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ParentId",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Discord.Models.Snowflake>>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static TextChannelModel From(ITextChannelModel model) => (model as TextChannelModel) ?? new TextChannelModel(
        IsNSFW: model.IsNSFW,
        Topic: model.Topic,
        RateLimitPerUser: model.RateLimitPerUser,
        DefaultAutoArchiveDuration: model.DefaultAutoArchiveDuration,
        Name: model.Name,
        GuildId: model.GuildId,
        Position: model.Position,
        PermissionOverwrites: model.PermissionOverwrites,
        Permissions: model.Permissions,
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id,
        ParentId: model.ParentId
    );

    static TextChannelModel IApiModel<ITextChannelModel, TextChannelModel>.From(ITextChannelModel model) => From(model);
}