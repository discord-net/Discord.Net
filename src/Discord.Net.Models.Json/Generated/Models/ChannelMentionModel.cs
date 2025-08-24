using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ChannelMentionModel> ChannelMentionModel => field ??= Discord.Models.Json.ChannelMentionModel.CreateTypeInfo(Options);
}

public record ChannelMentionModel(
    Discord.Snowflake GuildId,
    Discord.Models.ChannelType Type,
    string Name,
    Discord.Snowflake Id
) : 
    IChannelMentionModel,
    IJsonModel,
    IApiModel<IChannelMentionModel, ChannelMentionModel>
{
    public static JsonTypeInfo<ChannelMentionModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ChannelMentionModel>(
        options,
        new JsonObjectInfoValues<ChannelMentionModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ChannelMentionModel(
                GuildId: (Discord.Snowflake)args[0],
                Type: (Discord.Models.ChannelType)args[1],
                Name: (string)args[2],
                Id: (Discord.Snowflake)args[3]
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
                DeclaringType = typeof(Discord.Models.Json.ChannelMentionModel),
                Getter = static instance => ((Discord.Models.Json.ChannelMentionModel)instance).GuildId,
                Setter = null,
                PropertyName = "GuildId",
                JsonPropertyName = "guild_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelMentionModel),
                Getter = static instance => ((Discord.Models.Json.ChannelMentionModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelMentionModel),
                Getter = static instance => ((Discord.Models.Json.ChannelMentionModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelMentionModel),
                Getter = static instance => ((Discord.Models.Json.ChannelMentionModel)instance).Id,
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
           Name = "GuildId",
           ParameterType = typeof(Discord.Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ChannelMentionModel From(IChannelMentionModel model) => (model as ChannelMentionModel) ?? new ChannelMentionModel(
        GuildId: model.GuildId,
        Type: model.Type,
        Name: model.Name,
        Id: model.Id
    );

    static ChannelMentionModel IApiModel<IChannelMentionModel, ChannelMentionModel>.From(IChannelMentionModel model) => From(model);
}