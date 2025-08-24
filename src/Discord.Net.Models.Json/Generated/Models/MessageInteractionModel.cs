using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageInteractionModel> MessageInteractionModel => field ??= Discord.Models.Json.MessageInteractionModel.CreateTypeInfo(Options);
}

public record MessageInteractionModel(
    Discord.Models.InteractionType Type,
    string Name,
    Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel> User,
    Discord.Models.Optional<Discord.Models.IMemberModel> Member,
    Discord.Snowflake Id
) : 
    IMessageInteractionModel,
    IJsonModel,
    IApiModel<IMessageInteractionModel, MessageInteractionModel>
{
    public static JsonTypeInfo<MessageInteractionModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageInteractionModel>(
        options,
        new JsonObjectInfoValues<MessageInteractionModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageInteractionModel(
                Type: (Discord.Models.InteractionType)args[0],
                Name: (string)args[1],
                User: (Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>)args[2],
                Member: (Discord.Models.Optional<Discord.Models.IMemberModel>)args[3],
                Id: (Discord.Snowflake)args[4]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.InteractionType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.InteractionType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionModel)instance).User,
                Setter = null,
                PropertyName = "User",
                JsonPropertyName = "user",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IMemberModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IMemberModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionModel)instance).Member,
                Setter = null,
                PropertyName = "Member",
                JsonPropertyName = "member",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionModel)instance).Id,
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
           ParameterType = typeof(Discord.Models.InteractionType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "User",
           ParameterType = typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Member",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IMemberModel>),
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

    public static MessageInteractionModel From(IMessageInteractionModel model) => (model as MessageInteractionModel) ?? new MessageInteractionModel(
        Type: model.Type,
        Name: model.Name,
        User: model.User,
        Member: model.Member,
        Id: model.Id
    );

    static MessageInteractionModel IApiModel<IMessageInteractionModel, MessageInteractionModel>.From(IMessageInteractionModel model) => From(model);
}