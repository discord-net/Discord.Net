using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ChannelModel> ChannelModel => field ??= Discord.Models.Json.ChannelModel.CreateTypeInfo(Options);
}

public record ChannelModel(
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Discord.Models.Snowflake Id
) : 
    IChannelModel,
    IJsonModel,
    IApiModel<IChannelModel, ChannelModel>
{
    public static JsonTypeInfo<ChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ChannelModel>(
        options,
        new JsonObjectInfoValues<ChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ChannelModel(
                Type: (Discord.Models.ChannelType)args[0],
                Flags: (Discord.Models.ChannelFlags)args[1],
                Id: (Discord.Models.Snowflake)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelModel),
                Getter = static instance => ((Discord.Models.Json.ChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ChannelModel),
                Getter = static instance => ((Discord.Models.Json.ChannelModel)instance).Flags,
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
                DeclaringType = typeof(Discord.Models.Json.ChannelModel),
                Getter = static instance => ((Discord.Models.Json.ChannelModel)instance).Id,
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
           Name = "Type",
           ParameterType = typeof(Discord.Models.ChannelType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Models.Snowflake),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ChannelModel From(IChannelModel model) => (model as ChannelModel) ?? new ChannelModel(
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id
    );

    static ChannelModel IApiModel<IChannelModel, ChannelModel>.From(IChannelModel model) => From(model);
}