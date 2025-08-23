using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<DMChannelModel> DMChannelModel => field ??= Discord.Models.Json.DMChannelModel.CreateTypeInfo(Options);
}

public record DMChannelModel(
    Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel> Recipient,
    Discord.Models.ChannelType Type,
    Discord.Models.ChannelFlags Flags,
    Discord.Snowflake Id
) : 
    IDMChannelModel,
    IJsonModel,
    IApiModel<IDMChannelModel, DMChannelModel>
{
    public static JsonTypeInfo<DMChannelModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<DMChannelModel>(
        options,
        new JsonObjectInfoValues<DMChannelModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new DMChannelModel(
                Recipient: (Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>)args[0],
                Type: (Discord.Models.ChannelType)args[1],
                Flags: (Discord.Models.ChannelFlags)args[2],
                Id: (Discord.Snowflake)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.DMChannelModel),
                Getter = static instance => ((Discord.Models.Json.DMChannelModel)instance).Recipient,
                Setter = null,
                PropertyName = "Recipient",
                JsonPropertyName = "recipient",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ChannelType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ChannelType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.DMChannelModel),
                Getter = static instance => ((Discord.Models.Json.DMChannelModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.DMChannelModel),
                Getter = static instance => ((Discord.Models.Json.DMChannelModel)instance).Flags,
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
                DeclaringType = typeof(Discord.Models.Json.DMChannelModel),
                Getter = static instance => ((Discord.Models.Json.DMChannelModel)instance).Id,
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
           Name = "Recipient",
           ParameterType = typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>),
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
           Name = "Flags",
           ParameterType = typeof(Discord.Models.ChannelFlags),
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

    public static DMChannelModel From(IDMChannelModel model) => (model as DMChannelModel) ?? new DMChannelModel(
        Recipient: model.Recipient,
        Type: model.Type,
        Flags: model.Flags,
        Id: model.Id
    );

    static DMChannelModel IApiModel<IDMChannelModel, DMChannelModel>.From(IDMChannelModel model) => From(model);
}