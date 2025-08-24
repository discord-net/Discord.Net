using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<MessageCallModel> MessageCallModel => field ??= Discord.Models.Json.MessageCallModel.CreateTypeInfo(Options);
}

public record MessageCallModel(
    System.Collections.Generic.IReadOnlyList<Discord.Snowflake> Participants,
    Discord.Models.Optional<Nullable<DateTimeOffset>> EndedTimestamp
) : 
    IMessageCallModel,
    IJsonModel,
    IApiModel<IMessageCallModel, MessageCallModel>
{
    public static JsonTypeInfo<MessageCallModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<MessageCallModel>(
        options,
        new JsonObjectInfoValues<MessageCallModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new MessageCallModel(
                Participants: (System.Collections.Generic.IReadOnlyList<Discord.Snowflake>)args[0],
                EndedTimestamp: (Discord.Models.Optional<Nullable<DateTimeOffset>>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageCallModel),
                Getter = static instance => ((Discord.Models.Json.MessageCallModel)instance).Participants,
                Setter = null,
                PropertyName = "Participants",
                JsonPropertyName = "participants",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<DateTimeOffset>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<DateTimeOffset>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.MessageCallModel),
                Getter = static instance => ((Discord.Models.Json.MessageCallModel)instance).EndedTimestamp,
                Setter = null,
                PropertyName = "EndedTimestamp",
                JsonPropertyName = "ended_timestamp",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Participants",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Snowflake>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "EndedTimestamp",
           ParameterType = typeof(Discord.Models.Optional<Nullable<DateTimeOffset>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static MessageCallModel From(IMessageCallModel model) => (model as MessageCallModel) ?? new MessageCallModel(
        Participants: model.Participants,
        EndedTimestamp: model.EndedTimestamp
    );

    static MessageCallModel IApiModel<IMessageCallModel, MessageCallModel>.From(IMessageCallModel model) => From(model);
}