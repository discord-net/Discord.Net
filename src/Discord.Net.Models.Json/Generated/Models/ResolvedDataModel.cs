using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ResolvedDataModel> ResolvedDataModel => field ??= Discord.Models.Json.ResolvedDataModel.CreateTypeInfo(Options);
}

public record ResolvedDataModel(
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>> Users,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>> Members,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>> Roles,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>> Channels,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>> Messages,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>> Attachments
) : 
    IResolvedDataModel,
    IJsonModel,
    IApiModel<IResolvedDataModel, ResolvedDataModel>
{
    public static JsonTypeInfo<ResolvedDataModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ResolvedDataModel>(
        options,
        new JsonObjectInfoValues<ResolvedDataModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ResolvedDataModel(
                Users: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>)args[0],
                Members: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>)args[1],
                Roles: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>)args[2],
                Channels: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>)args[3],
                Messages: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>)args[4],
                Attachments: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>)args[5]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Users,
                Setter = null,
                PropertyName = "Users",
                JsonPropertyName = "users",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Members,
                Setter = null,
                PropertyName = "Members",
                JsonPropertyName = "members",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Roles,
                Setter = null,
                PropertyName = "Roles",
                JsonPropertyName = "roles",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Channels,
                Setter = null,
                PropertyName = "Channels",
                JsonPropertyName = "channels",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Messages,
                Setter = null,
                PropertyName = "Messages",
                JsonPropertyName = "messages",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ResolvedDataModel),
                Getter = static instance => ((Discord.Models.Json.ResolvedDataModel)instance).Attachments,
                Setter = null,
                PropertyName = "Attachments",
                JsonPropertyName = "attachments",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Users",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IUserModel>>>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Members",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.Optional<Discord.Models.IMemberModel>>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Roles",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IRoleModel>>>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Channels",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IChannelModel>>>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Messages",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IMessageModel>>>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Attachments",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyDictionary<Discord.Snowflake,Discord.Models.IdOrModel<Discord.Snowflake,Discord.Models.IAttachmentModel>>>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ResolvedDataModel From(IResolvedDataModel model) => (model as ResolvedDataModel) ?? new ResolvedDataModel(
        Users: model.Users,
        Members: model.Members,
        Roles: model.Roles,
        Channels: model.Channels,
        Messages: model.Messages,
        Attachments: model.Attachments
    );

    static ResolvedDataModel IApiModel<IResolvedDataModel, ResolvedDataModel>.From(IResolvedDataModel model) => From(model);
}