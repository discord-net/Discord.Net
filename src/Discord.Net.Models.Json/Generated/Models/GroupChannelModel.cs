using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<GroupChannelModel> GroupChannelModel => field ??= Discord.Models.Json.GroupChannelModel.CreateTypeInfo(Options);
}

public record GroupChannelModel(
    Discord.Snowflake OwnerId,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>> Recipients,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Discord.Snowflake Id
) : 
    IGroupChannelModel,
    IJsonModel,
    IApiModel<IGroupChannelModel, GroupChannelModel>
{
    public static JsonTypeInfo<GroupChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<GroupChannelModel>(
        options,
        new JsonObjectInfoValues<GroupChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new GroupChannelModel(
                OwnerId: (Discord.Snowflake)args[0],
                Recipients: (System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)args[1],
                Type: (Discord.Models.ChannelType)args[2],
                Flags: (Discord.Models.ChannelFlags)args[3],
                Id: (Discord.Snowflake)args[4]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.GroupChannelModel),
                Getter = static instance => ((Discord.Models.Json.GroupChannelModel)instance).OwnerId,
                Setter = null,
                PropertyName = "OwnerId",
                JsonPropertyName = "owner_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.GroupChannelModel),
                Getter = static instance => ((Discord.Models.Json.GroupChannelModel)instance).Recipients,
                Setter = null,
                PropertyName = "Recipients",
                JsonPropertyName = "recipients",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.GroupChannelModel),
                Getter = static instance => ((Discord.Models.Json.GroupChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.GroupChannelModel),
                Getter = static instance => ((Discord.Models.Json.GroupChannelModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.GroupChannelModel),
                Getter = static instance => ((Discord.Models.Json.GroupChannelModel)instance).Id,
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
           Name = "OwnerId",
           ParameterType = typeof(Discord.Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Recipients",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static GroupChannelModel From(IGroupChannelModel model) => (model as GroupChannelModel) ?? new GroupChannelModel(
        OwnerId: model.OwnerId,
        Recipients: model.Recipients,
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id
    );

    static GroupChannelModel IApiModel<IGroupChannelModel, GroupChannelModel>.From(IGroupChannelModel model) => From(model);
}