using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageSnapshotModel> MessageSnapshotModel => field ??= Discord.Models.Json.MessageSnapshotModel.CreateTypeInfo(Options);
}

public record MessageSnapshotModel(
    Discord.Models.IMessageModel Message
) : 
    IMessageSnapshotModel,
    IJsonModel,
    IApiModel<IMessageSnapshotModel, MessageSnapshotModel>
{
    public static JsonTypeInfo<MessageSnapshotModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageSnapshotModel>(
        options,
        new JsonObjectInfoValues<MessageSnapshotModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageSnapshotModel(
                Message: (Discord.Models.IMessageModel)args[0]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IMessageModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IMessageModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageSnapshotModel),
                Getter = static instance => ((Discord.Models.Json.MessageSnapshotModel)instance).Message,
                Setter = null,
                PropertyName = "Message",
                JsonPropertyName = "message",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Message",
           ParameterType = typeof(Discord.Models.IMessageModel),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageSnapshotModel From(IMessageSnapshotModel model) => (model as MessageSnapshotModel) ?? new MessageSnapshotModel(
        Message: model.Message
    );

    static MessageSnapshotModel IApiModel<IMessageSnapshotModel, MessageSnapshotModel>.From(IMessageSnapshotModel model) => From(model);
}