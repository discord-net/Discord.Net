using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<PollModel> PollModel => field ??= Discord.Models.Json.PollModel.CreateTypeInfo(Options);
}

public record PollModel(
    Discord.Models.IPollMediaModel Question,
    System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel> Answers,
    Nullable<DateTimeOffset> Expiry,
    bool AllowMultiselect,
    Discord.Models.PollLayoutType LayoutType,
    Discord.Models.Optional<Discord.Models.IPollResultsModel> Results
) : 
    IPollModel,
    IJsonModel,
    IApiModel<IPollModel, PollModel>
{
    public static JsonTypeInfo<PollModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<PollModel>(
        options,
        new JsonObjectInfoValues<PollModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new PollModel(
                Question: (Discord.Models.IPollMediaModel)args[0],
                Answers: (System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>)args[1],
                Expiry: (Nullable<DateTimeOffset>)args[2],
                AllowMultiselect: (bool)args[3],
                LayoutType: (Discord.Models.PollLayoutType)args[4],
                Results: (Discord.Models.Optional<Discord.Models.IPollResultsModel>)args[5]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IPollMediaModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IPollMediaModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).Question,
                Setter = null,
                PropertyName = "Question",
                JsonPropertyName = "question",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).Answers,
                Setter = null,
                PropertyName = "Answers",
                JsonPropertyName = "answers",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<DateTimeOffset>>(
            options,
            new JsonPropertyInfoValues<Nullable<DateTimeOffset>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).Expiry,
                Setter = null,
                PropertyName = "Expiry",
                JsonPropertyName = "expiry",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).AllowMultiselect,
                Setter = null,
                PropertyName = "AllowMultiselect",
                JsonPropertyName = "allow_multiselect",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.PollLayoutType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.PollLayoutType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).LayoutType,
                Setter = null,
                PropertyName = "LayoutType",
                JsonPropertyName = "layout_type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.IPollResultsModel>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.IPollResultsModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollModel),
                Getter = static instance => ((Discord.Models.Json.PollModel)instance).Results,
                Setter = null,
                PropertyName = "Results",
                JsonPropertyName = "results",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Question",
           ParameterType = typeof(Discord.Models.IPollMediaModel),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Answers",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IPollAnswerModel>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Expiry",
           ParameterType = typeof(Nullable<DateTimeOffset>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "AllowMultiselect",
           ParameterType = typeof(bool),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "LayoutType",
           ParameterType = typeof(Discord.Models.PollLayoutType),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Results",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.IPollResultsModel>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static PollModel From(IPollModel model) => (model as PollModel) ?? new PollModel(
        Question: model.Question,
        Answers: model.Answers,
        Expiry: model.Expiry,
        AllowMultiselect: model.AllowMultiselect,
        LayoutType: model.LayoutType,
        Results: model.Results
    );

    static PollModel IApiModel<IPollModel, PollModel>.From(IPollModel model) => From(model);
}