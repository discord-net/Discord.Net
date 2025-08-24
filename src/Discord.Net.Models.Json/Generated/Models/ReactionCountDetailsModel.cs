using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ReactionCountDetailsModel> ReactionCountDetailsModel => field ??= Discord.Models.Json.ReactionCountDetailsModel.CreateTypeInfo(Options);
}

public record ReactionCountDetailsModel(
    int Burst,
    int Normal
) : 
    IReactionCountDetailsModel,
    IJsonModel,
    IApiModel<IReactionCountDetailsModel, ReactionCountDetailsModel>
{
    public static JsonTypeInfo<ReactionCountDetailsModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ReactionCountDetailsModel>(
        options,
        new JsonObjectInfoValues<ReactionCountDetailsModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ReactionCountDetailsModel(
                Burst: (int)args[0],
                Normal: (int)args[1]
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
                DeclaringType = typeof(Discord.Models.Json.ReactionCountDetailsModel),
                Getter = static instance => ((Discord.Models.Json.ReactionCountDetailsModel)instance).Burst,
                Setter = null,
                PropertyName = "Burst",
                JsonPropertyName = "burst",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionCountDetailsModel),
                Getter = static instance => ((Discord.Models.Json.ReactionCountDetailsModel)instance).Normal,
                Setter = null,
                PropertyName = "Normal",
                JsonPropertyName = "normal",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Burst",
           ParameterType = typeof(int),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Normal",
           ParameterType = typeof(int),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ReactionCountDetailsModel From(IReactionCountDetailsModel model) => (model as ReactionCountDetailsModel) ?? new ReactionCountDetailsModel(
        Burst: model.Burst,
        Normal: model.Normal
    );

    static ReactionCountDetailsModel IApiModel<IReactionCountDetailsModel, ReactionCountDetailsModel>.From(IReactionCountDetailsModel model) => From(model);
}