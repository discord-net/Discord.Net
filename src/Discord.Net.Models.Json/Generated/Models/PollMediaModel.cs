using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<PollMediaModel> PollMediaModel => field ??= Discord.Models.Json.PollMediaModel.CreateTypeInfo(Options);
}

public record PollMediaModel(
    Discord.Models.Optional<string> Text,
    Discord.Models.Optional<Discord.Models.EmojiId> Emoji
) : 
    IPollMediaModel,
    IJsonModel,
    IApiModel<IPollMediaModel, PollMediaModel>
{
    public static JsonTypeInfo<PollMediaModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<PollMediaModel>(
        options,
        new JsonObjectInfoValues<PollMediaModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new PollMediaModel(
                Text: (Discord.Models.Optional<string>)args[0],
                Emoji: (Discord.Models.Optional<Discord.Models.EmojiId>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollMediaModel),
                Getter = static instance => ((Discord.Models.Json.PollMediaModel)instance).Text,
                Setter = null,
                PropertyName = "Text",
                JsonPropertyName = "text",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.EmojiId>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.EmojiId>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollMediaModel),
                Getter = static instance => ((Discord.Models.Json.PollMediaModel)instance).Emoji,
                Setter = null,
                PropertyName = "Emoji",
                JsonPropertyName = "emoji",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Text",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Emoji",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.EmojiId>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static PollMediaModel From(IPollMediaModel model) => (model as PollMediaModel) ?? new PollMediaModel(
        Text: model.Text,
        Emoji: model.Emoji
    );

    static PollMediaModel IApiModel<IPollMediaModel, PollMediaModel>.From(IPollMediaModel model) => From(model);
}