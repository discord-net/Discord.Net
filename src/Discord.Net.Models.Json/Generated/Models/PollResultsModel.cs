using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<PollResultsModel> PollResultsModel => field ??= Discord.Models.Json.PollResultsModel.CreateTypeInfo(Options);
}

public record PollResultsModel(
    bool IsFinalized,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel> AnswerCounts
) : 
    IPollResultsModel,
    IJsonModel,
    IApiModel<IPollResultsModel, PollResultsModel>
{
    public static JsonTypeInfo<PollResultsModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<PollResultsModel>(
        options,
        new JsonObjectInfoValues<PollResultsModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new PollResultsModel(
                IsFinalized: (bool)args[0],
                AnswerCounts: (System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollResultsModel),
                Getter = static instance => ((Discord.Models.Json.PollResultsModel)instance).IsFinalized,
                Setter = null,
                PropertyName = "IsFinalized",
                JsonPropertyName = "is_finalized",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollResultsModel),
                Getter = static instance => ((Discord.Models.Json.PollResultsModel)instance).AnswerCounts,
                Setter = null,
                PropertyName = "AnswerCounts",
                JsonPropertyName = "answer_counts",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "IsFinalized",
           ParameterType = typeof(bool),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AnswerCounts",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerCountModel>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static PollResultsModel From(IPollResultsModel model) => (model as PollResultsModel) ?? new PollResultsModel(
        IsFinalized: model.IsFinalized,
        AnswerCounts: model.AnswerCounts
    );

    static PollResultsModel IApiModel<IPollResultsModel, PollResultsModel>.From(IPollResultsModel model) => From(model);
}