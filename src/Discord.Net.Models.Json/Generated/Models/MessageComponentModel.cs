using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageComponentModel> MessageComponentModel => field ??= Discord.Models.Json.MessageComponentModel.CreateTypeInfo(Options);
}

public record MessageComponentModel(
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IMessageComponentModel,
    IJsonModel,
    IApiModel<IMessageComponentModel, MessageComponentModel>
{
    public static JsonTypeInfo<MessageComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageComponentModel>(
        options,
        new JsonObjectInfoValues<MessageComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageComponentModel(
                Type: (Discord.Models.ComponentType)args[0],
                Id: (Nullable<int>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageComponentModel),
                Getter = static instance => ((Discord.Models.Json.MessageComponentModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<int>>(
            options,
            new JsonPropertyInfoValues<Nullable<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageComponentModel),
                Getter = static instance => ((Discord.Models.Json.MessageComponentModel)instance).Id,
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
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static MessageComponentModel From(IMessageComponentModel model) => (model as MessageComponentModel) ?? new MessageComponentModel(
        Type: model.Type,
        Id: model.Id
    );

    static MessageComponentModel IApiModel<IMessageComponentModel, MessageComponentModel>.From(IMessageComponentModel model) => From(model);
}