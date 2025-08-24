using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageReferenceModel> MessageReferenceModel => field ??= Discord.Models.Json.MessageReferenceModel.CreateTypeInfo(Options);
}

public record MessageReferenceModel(
    Discord.Models.Optional<Discord.Models.MessageReferenceType> Type,
    Discord.Models.Optional<Discord.Snowflake> MessageId,
    Discord.Models.Optional<Discord.Snowflake> ChannelId,
    Discord.Models.Optional<Discord.Snowflake> GuildId,
    Discord.Models.Optional<bool> FailIfNotExists
) : 
    IMessageReferenceModel,
    IJsonModel,
    IApiModel<IMessageReferenceModel, MessageReferenceModel>
{
    public static JsonTypeInfo<MessageReferenceModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageReferenceModel>(
        options,
        new JsonObjectInfoValues<MessageReferenceModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageReferenceModel(
                Type: (Discord.Models.Optional<Discord.Models.MessageReferenceType>)args[0],
                MessageId: (Discord.Models.Optional<Discord.Snowflake>)args[1],
                ChannelId: (Discord.Models.Optional<Discord.Snowflake>)args[2],
                GuildId: (Discord.Models.Optional<Discord.Snowflake>)args[3],
                FailIfNotExists: (Discord.Models.Optional<bool>)args[4]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.MessageReferenceType>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.MessageReferenceType>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageReferenceModel),
                Getter = static instance => ((Discord.Models.Json.MessageReferenceModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageReferenceModel),
                Getter = static instance => ((Discord.Models.Json.MessageReferenceModel)instance).MessageId,
                Setter = null,
                PropertyName = "MessageId",
                JsonPropertyName = "message_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageReferenceModel),
                Getter = static instance => ((Discord.Models.Json.MessageReferenceModel)instance).ChannelId,
                Setter = null,
                PropertyName = "ChannelId",
                JsonPropertyName = "channel_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageReferenceModel),
                Getter = static instance => ((Discord.Models.Json.MessageReferenceModel)instance).GuildId,
                Setter = null,
                PropertyName = "GuildId",
                JsonPropertyName = "guild_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageReferenceModel),
                Getter = static instance => ((Discord.Models.Json.MessageReferenceModel)instance).FailIfNotExists,
                Setter = null,
                PropertyName = "FailIfNotExists",
                JsonPropertyName = "fail_if_not_exists",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.MessageReferenceType>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MessageId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ChannelId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "GuildId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "FailIfNotExists",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageReferenceModel From(IMessageReferenceModel model) => (model as MessageReferenceModel) ?? new MessageReferenceModel(
        Type: model.Type,
        MessageId: model.MessageId,
        ChannelId: model.ChannelId,
        GuildId: model.GuildId,
        FailIfNotExists: model.FailIfNotExists
    );

    static MessageReferenceModel IApiModel<IMessageReferenceModel, MessageReferenceModel>.From(IMessageReferenceModel model) => From(model);
}