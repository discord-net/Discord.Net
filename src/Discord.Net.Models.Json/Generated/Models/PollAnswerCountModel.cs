using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<PollAnswerCountModel> PollAnswerCountModel => field ??= Discord.Models.Json.PollAnswerCountModel.CreateTypeInfo(Options);
}

public record PollAnswerCountModel(
    int Id,
    int Count,
    bool MeVoted
) : 
    IPollAnswerCountModel,
    IJsonModel,
    IApiModel<IPollAnswerCountModel, PollAnswerCountModel>
{
    public static JsonTypeInfo<PollAnswerCountModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<PollAnswerCountModel>(
        options,
        new JsonObjectInfoValues<PollAnswerCountModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new PollAnswerCountModel(
                Id: (int)args[0],
                Count: (int)args[1],
                MeVoted: (bool)args[2]
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
                DeclaringType = typeof(Discord.Models.Json.PollAnswerCountModel),
                Getter = static instance => ((Discord.Models.Json.PollAnswerCountModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollAnswerCountModel),
                Getter = static instance => ((Discord.Models.Json.PollAnswerCountModel)instance).Count,
                Setter = null,
                PropertyName = "Count",
                JsonPropertyName = "count",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.PollAnswerCountModel),
                Getter = static instance => ((Discord.Models.Json.PollAnswerCountModel)instance).MeVoted,
                Setter = null,
                PropertyName = "MeVoted",
                JsonPropertyName = "me_voted",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Id",
           ParameterType = typeof(int),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Count",
           ParameterType = typeof(int),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MeVoted",
           ParameterType = typeof(bool),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static PollAnswerCountModel From(IPollAnswerCountModel model) => (model as PollAnswerCountModel) ?? new PollAnswerCountModel(
        Id: model.Id,
        Count: model.Count,
        MeVoted: model.MeVoted
    );

    static PollAnswerCountModel IApiModel<IPollAnswerCountModel, PollAnswerCountModel>.From(IPollAnswerCountModel model) => From(model);
}