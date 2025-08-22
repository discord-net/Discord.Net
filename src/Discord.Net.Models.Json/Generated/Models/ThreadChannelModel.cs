using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ThreadChannelModel> ThreadChannelModel => field ??= Discord.Models.Json.ThreadChannelModel.CreateTypeInfo(Options);
}

public record ThreadChannelModel(
    Snowflake ParentId,
    int MemberCount,
    Discord.Models.Optional<int> MessageCount,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Snowflake>> AppliedTags,
    Snowflake OwnerId,
    bool IsNSFW,
    string? Topic,
    Discord.Models.Optional<int> RateLimitPerUser,
    string Name,
    Snowflake GuildId,
    int Position,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel> PermissionOverwrites,
    Discord.Models.Optional<Discord.Models.PermissionBitSet> Permissions,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Snowflake Id
) : 
    IThreadChannelModel,
    IJsonModel,
    IApiModel<IThreadChannelModel, ThreadChannelModel>
{
    public static JsonTypeInfo<ThreadChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ThreadChannelModel>(
        options,
        new JsonObjectInfoValues<ThreadChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ThreadChannelModel(
                ParentId: (Snowflake)args[0],
                MemberCount: (int)args[1],
                MessageCount: (Discord.Models.Optional<int>)args[2],
                AppliedTags: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Snowflake>>)args[3],
                OwnerId: (Snowflake)args[4],
                IsNSFW: (bool)args[5],
                Topic: (string?)args[6],
                RateLimitPerUser: (Discord.Models.Optional<int>)args[7],
                Name: (string)args[8],
                GuildId: (Snowflake)args[9],
                Position: (int)args[10],
                PermissionOverwrites: (System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)args[11],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[12],
                Type: (Discord.Models.ChannelType)args[13],
                Flags: (Discord.Models.ChannelFlags)args[14],
                Id: (Snowflake)args[15]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).ParentId,
                Setter = null,
                PropertyName = "ParentId",
                JsonPropertyName = "parent_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).MemberCount,
                Setter = null,
                PropertyName = "MemberCount",
                JsonPropertyName = "member_count",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).MessageCount,
                Setter = null,
                PropertyName = "MessageCount",
                JsonPropertyName = "message_count",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Snowflake>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Snowflake>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).AppliedTags,
                Setter = null,
                PropertyName = "AppliedTags",
                JsonPropertyName = "applied_tags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).OwnerId,
                Setter = null,
                PropertyName = "OwnerId",
                JsonPropertyName = "owner_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).IsNSFW,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Topic,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).RateLimitPerUser,
                Setter = null,
                PropertyName = "RateLimitPerUser",
                JsonPropertyName = "rate_limit_per_user",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).GuildId,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Position,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).PermissionOverwrites,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Permissions,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ThreadChannelModel),
                Getter = static instance => ((Discord.Models.Json.ThreadChannelModel)instance).Id,
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
           Name = "ParentId",
           ParameterType = typeof(Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MemberCount",
           ParameterType = typeof(int),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MessageCount",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AppliedTags",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Snowflake>>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "OwnerId",
           ParameterType = typeof(Snowflake),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "IsNSFW",
           ParameterType = typeof(bool),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Topic",
           ParameterType = typeof(string),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "RateLimitPerUser",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GuildId",
           ParameterType = typeof(Snowflake),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Position",
           ParameterType = typeof(int),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PermissionOverwrites",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Permissions",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.PermissionBitSet>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 14,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Snowflake),
           Position = 15,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ThreadChannelModel From(IThreadChannelModel model) => (model as ThreadChannelModel) ?? new ThreadChannelModel(
        ParentId: model.ParentId,
        MemberCount: model.MemberCount,
        MessageCount: model.MessageCount,
        AppliedTags: model.AppliedTags,
        OwnerId: model.OwnerId,
        IsNSFW: model.IsNSFW,
        Topic: model.Topic,
        RateLimitPerUser: model.RateLimitPerUser,
        Name: model.Name,
        GuildId: model.GuildId,
        Position: model.Position,
        PermissionOverwrites: model.PermissionOverwrites,
        Permissions: model.Permissions,
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id
    );

    static ThreadChannelModel IApiModel<IThreadChannelModel, ThreadChannelModel>.From(IThreadChannelModel model) => From(model);
}