using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageActivityModel> MessageActivityModel => field ??= Discord.Models.Json.MessageActivityModel.CreateTypeInfo(Options);
}

public record MessageActivityModel(
    Discord.Models.MessageActivityType Type,
    Discord.Models.Optional<string> PartyId
) : 
    IMessageActivityModel,
    IJsonModel,
    IApiModel<IMessageActivityModel, MessageActivityModel>
{
    public static JsonTypeInfo<MessageActivityModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageActivityModel>(
        options,
        new JsonObjectInfoValues<MessageActivityModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageActivityModel(
                Type: (Discord.Models.MessageActivityType)args[0],
                PartyId: (Discord.Models.Optional<string>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.MessageActivityType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.MessageActivityType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageActivityModel),
                Getter = static instance => ((Discord.Models.Json.MessageActivityModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageActivityModel),
                Getter = static instance => ((Discord.Models.Json.MessageActivityModel)instance).PartyId,
                Setter = null,
                PropertyName = "PartyId",
                JsonPropertyName = "party_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.MessageActivityType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PartyId",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageActivityModel From(IMessageActivityModel model) => (model as MessageActivityModel) ?? new MessageActivityModel(
        Type: model.Type,
        PartyId: model.PartyId
    );

    static MessageActivityModel IApiModel<IMessageActivityModel, MessageActivityModel>.From(IMessageActivityModel model) => From(model);
}