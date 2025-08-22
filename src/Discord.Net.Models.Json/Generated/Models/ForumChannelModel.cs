using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ForumChannelModel> ForumChannelModel => field ??= Discord.Models.Json.ForumChannelModel.CreateTypeInfo(Options);
}

public record ForumChannelModel(
    bool IsNSFW,
    string? Topic,
    int RateLimitPerUser,
    Discord.Models.Optional<Nullable<Discord.Models.EmojiId>> DefaultReactionEmoji,
    System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel> AvailableTags,
    Nullable<Discord.Models.SortOrderType> DefaultSortOrder,
    Discord.Models.ForumLayout DefaultForumLayout,
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
    IForumChannelModel,
    IJsonModel,
    IApiModel<IForumChannelModel, ForumChannelModel>
{
    public static JsonTypeInfo<ForumChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ForumChannelModel>(
        options,
        new JsonObjectInfoValues<ForumChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ForumChannelModel(
                IsNSFW: (bool)args[0],
                Topic: (string?)args[1],
                RateLimitPerUser: (int)args[2],
                DefaultReactionEmoji: (Discord.Models.Optional<Nullable<Discord.Models.EmojiId>>)args[3],
                AvailableTags: (System.Collections.Generic.IReadOnlyList<Discord.Models.ITagModel>)args[4],
                DefaultSortOrder: (Nullable<Discord.Models.SortOrderType>)args[5],
                DefaultForumLayout: (Discord.Models.ForumLayout)args[6],
                DefaultAutoArchiveDuration: (Discord.Models.DefaultAutoArchiveDuration)args[7],
                Name: (string)args[8],
                GuildId: (Snowflake)args[9],
                Position: (int)args[10],
                PermissionOverwrites: (System.Collections.Generic.IReadOnlyList<Discord.Models.IOverwriteModel>)args[11],
                Permissions: (Discord.Models.Optional<Discord.Models.PermissionBitSet>)args[12],
                Type: (Discord.Models.ChannelType)args[13],
                Flags: (Discord.Models.ChannelFlags)args[14],
                Id: (Snowflake)args[15],
                ParentId: (Discord.Models.Optional<Nullable<Snowflake>>)args[16]
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).IsNSFW,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Topic,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).RateLimitPerUser,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).DefaultReactionEmoji,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).AvailableTags,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).DefaultSortOrder,
                Setter = null,
                PropertyName = "DefaultSortOrder",
                JsonPropertyName = "default_sort_order",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ForumLayout>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ForumLayout>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).DefaultForumLayout,
                Setter = null,
                PropertyName = "DefaultForumLayout",
                JsonPropertyName = "default_forum_layout",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.DefaultAutoArchiveDuration>(
            options,
            new JsonPropertyInfoValues<Discord.Models.DefaultAutoArchiveDuration>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).DefaultAutoArchiveDuration,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Name,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).GuildId,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Position,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).PermissionOverwrites,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Permissions,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Flags,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).Id,
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
                DeclaringType = typeof(Discord.Models.Json.ForumChannelModel),
                Getter = static instance => ((Discord.Models.Json.ForumChannelModel)instance).ParentId,
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
           Name = "DefaultForumLayout",
           ParameterType = typeof(Discord.Models.ForumLayout),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultAutoArchiveDuration",
           ParameterType = typeof(Discord.Models.DefaultAutoArchiveDuration),
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
        },
        new()
        {
           Name = "ParentId",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Snowflake>>),
           Position = 16,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ForumChannelModel From(IForumChannelModel model) => (model as ForumChannelModel) ?? new ForumChannelModel(
        IsNSFW: model.IsNSFW,
        Topic: model.Topic,
        RateLimitPerUser: model.RateLimitPerUser,
        DefaultReactionEmoji: model.DefaultReactionEmoji,
        AvailableTags: model.AvailableTags,
        DefaultSortOrder: model.DefaultSortOrder,
        DefaultForumLayout: model.DefaultForumLayout,
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

    static ForumChannelModel IApiModel<IForumChannelModel, ForumChannelModel>.From(IForumChannelModel model) => From(model);
}