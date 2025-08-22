using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MediaChannelModel> MediaChannelModel => field ??= Discord.Models.Json.MediaChannelModel.CreateTypeInfo(Options);
}

public record MediaChannelModel(
    bool IsNSFW,
    string? Topic,
    int RateLimitPerUser,
    Discord.Models.Optional<Nullable<Discord.Models.EmojiId>> DefaultReactionEmoji,
    System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel> AvailableTags,
    Nullable<Discord.Models.SortOrderType> DefaultSortOrder,
    Discord.Models.DefaultAutoArchiveDuration DefaultAutoArchiveDuration,
    string Name,
    Snowflake GuildId,
    int Position,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel> PermissionOverwrites,
    Discord.Models.Optional<Discord.Models.PermissionBitSet> Permissions,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Snowflake Id,
    Discord.Models.Optional<Nullable<Snowflake>> ParentId
) : 
    IMediaChannelModel,
    IJsonModel,
    IApiModel<IMediaChannelModel, MediaChannelModel>
{
    public static JsonTypeInfo<MediaChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MediaChannelModel>(
        options,
        new JsonObjectInfoValues<MediaChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MediaChannelModel(
                IsNSFW: (bool)args[0],
                Topic: (string?)args[1],
                RateLimitPerUser: (int)args[2],
                DefaultReactionEmoji: (Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>)args[3],
                AvailableTags: (System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>)args[4],
                DefaultSortOrder: (Nullable<Discord.Models.SortOrderType>)args[5],
                DefaultAutoArchiveDuration: (Discord.Models.DefaultAutoArchiveDuration)args[6],
                Name: (string)args[7],
                GuildId: (Snowflake)args[8],
                Position: (int)args[9],
                PermissionOverwrites: (System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)args[10],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[11],
                Type: (Discord.Models.ChannelType)args[12],
                Flags: (Discord.Models.ChannelFlags)args[13],
                Id: (Snowflake)args[14],
                ParentId: (Discord.Models.Optional<Nullable<Snowflake>>)args[15]
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).IsNSFW,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Topic,
                Setter = null,
                PropertyName = "Topic",
                JsonPropertyName = "topic",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).RateLimitPerUser,
                Setter = null,
                PropertyName = "RateLimitPerUser",
                JsonPropertyName = "rate_limit_per_user",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).DefaultReactionEmoji,
                Setter = null,
                PropertyName = "DefaultReactionEmoji",
                JsonPropertyName = "default_reaction_emoji",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).AvailableTags,
                Setter = null,
                PropertyName = "AvailableTags",
                JsonPropertyName = "available_tags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<Discord.Models.SortOrderType>>(
            options,
            new JsonPropertyInfoValues<Nullable<Discord.Models.SortOrderType>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).DefaultSortOrder,
                Setter = null,
                PropertyName = "DefaultSortOrder",
                JsonPropertyName = "default_sort_order",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.DefaultAutoArchiveDuration>(
            options,
            new JsonPropertyInfoValues<Discord.Models.DefaultAutoArchiveDuration>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).DefaultAutoArchiveDuration,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Name,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).GuildId,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Position,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).PermissionOverwrites,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Permissions,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Flags,
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
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Snowflake>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Snowflake>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MediaChannelModel),
                Getter = static instance => ((Discord.Models.Json.MediaChannelModel)instance).ParentId,
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
           ParameterType = typeof(int),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultReactionEmoji",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AvailableTags",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultSortOrder",
           ParameterType = typeof(Nullable<Discord.Models.SortOrderType>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "DefaultAutoArchiveDuration",
           ParameterType = typeof(Discord.Models.DefaultAutoArchiveDuration),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GuildId",
           ParameterType = typeof(Snowflake),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Position",
           ParameterType = typeof(int),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PermissionOverwrites",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>),
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
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Snowflake),
           Position = 14,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ParentId",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Snowflake>>),
           Position = 15,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MediaChannelModel From(IMediaChannelModel model) => (model as MediaChannelModel) ?? new MediaChannelModel(
        IsNSFW: model.IsNSFW,
        Topic: model.Topic,
        RateLimitPerUser: model.RateLimitPerUser,
        DefaultReactionEmoji: model.DefaultReactionEmoji,
        AvailableTags: model.AvailableTags,
        DefaultSortOrder: model.DefaultSortOrder,
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

    static MediaChannelModel IApiModel<IMediaChannelModel, MediaChannelModel>.From(IMediaChannelModel model) => From(model);
}