using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageInteractionMetadataModel> MessageInteractionMetadataModel => field ??= Discord.Models.Json.MessageInteractionMetadataModel.CreateTypeInfo(Options);
}

public record MessageInteractionMetadataModel(
    Discord.Models.InteractionType Type,
    Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel> User,
    System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake> AuthorizingIntegrationOwners,
    Discord.Models.Optional<Discord.Snowflake> OriginalResponseMessageId,
    Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>> TargetUser,
    Discord.Models.Optional<Discord.Snowflake> TargetMessageId,
    Discord.Snowflake Id
) : 
    IMessageInteractionMetadataModel,
    IJsonModel,
    IApiModel<IMessageInteractionMetadataModel, MessageInteractionMetadataModel>
{
    public static JsonTypeInfo<MessageInteractionMetadataModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageInteractionMetadataModel>(
        options,
        new JsonObjectInfoValues<MessageInteractionMetadataModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageInteractionMetadataModel(
                Type: (Discord.Models.InteractionType)args[0],
                User: (Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>)args[1],
                AuthorizingIntegrationOwners: (System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>)args[2],
                OriginalResponseMessageId: (Discord.Models.Optional<Discord.Snowflake>)args[3],
                TargetUser: (Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>)args[4],
                TargetMessageId: (Discord.Models.Optional<Discord.Snowflake>)args[5],
                Id: (Discord.Snowflake)args[6]
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
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).User,
                Setter = null,
                PropertyName = "User",
                JsonPropertyName = "user",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).AuthorizingIntegrationOwners,
                Setter = null,
                PropertyName = "AuthorizingIntegrationOwners",
                JsonPropertyName = "authorizing_integration_owners",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).OriginalResponseMessageId,
                Setter = null,
                PropertyName = "OriginalResponseMessageId",
                JsonPropertyName = "original_response_message_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).TargetUser,
                Setter = null,
                PropertyName = "TargetUser",
                JsonPropertyName = "target_user",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).TargetMessageId,
                Setter = null,
                PropertyName = "TargetMessageId",
                JsonPropertyName = "target_message_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageInteractionMetadataModel),
                Getter = static instance => ((Discord.Models.Json.MessageInteractionMetadataModel)instance).Id,
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
           Name = "User",
           ParameterType = typeof(Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AuthorizingIntegrationOwners",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyDictionary<Discord.Models.ApplicationIntegrationType,Discord.Snowflake>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "OriginalResponseMessageId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "TargetUser",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "TargetMessageId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageInteractionMetadataModel From(IMessageInteractionMetadataModel model) => (model as MessageInteractionMetadataModel) ?? new MessageInteractionMetadataModel(
        Type: model.Type,
        User: model.User,
        AuthorizingIntegrationOwners: model.AuthorizingIntegrationOwners,
        OriginalResponseMessageId: model.OriginalResponseMessageId,
        TargetUser: model.TargetUser,
        TargetMessageId: model.TargetMessageId,
        Id: model.Id
    );

    static MessageInteractionMetadataModel IApiModel<IMessageInteractionMetadataModel, MessageInteractionMetadataModel>.From(IMessageInteractionMetadataModel model) => From(model);
}