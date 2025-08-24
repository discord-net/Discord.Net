using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<PollAnswerModel> PollAnswerModel => field ??= Discord.Models.Json.PollAnswerModel.CreateTypeInfo(Options);
}

public record PollAnswerModel(
    int AnswerId,
    Discord.Models.IPollMediaModel PollMedia
) : 
    IPollAnswerModel,
    IJsonModel,
    IApiModel<IPollAnswerModel, PollAnswerModel>
{
    public static JsonTypeInfo<PollAnswerModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<PollAnswerModel>(
        options,
        new JsonObjectInfoValues<PollAnswerModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new PollAnswerModel(
                AnswerId: (int)args[0],
                PollMedia: (Discord.Models.IPollMediaModel)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollAnswerModel),
                Getter = static instance => ((Discord.Models.Json.PollAnswerModel)instance).AnswerId,
                Setter = null,
                PropertyName = "AnswerId",
                JsonPropertyName = "answer_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IPollMediaModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IPollMediaModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollAnswerModel),
                Getter = static instance => ((Discord.Models.Json.PollAnswerModel)instance).PollMedia,
                Setter = null,
                PropertyName = "PollMedia",
                JsonPropertyName = "poll_media",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "AnswerId",
           ParameterType = typeof(int),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "PollMedia",
           ParameterType = typeof(Discord.Models.IPollMediaModel),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static PollAnswerModel From(IPollAnswerModel model) => (model as PollAnswerModel) ?? new PollAnswerModel(
        AnswerId: model.AnswerId,
        PollMedia: model.PollMedia
    );

    static PollAnswerModel IApiModel<IPollAnswerModel, PollAnswerModel>.From(IPollAnswerModel model) => From(model);
}