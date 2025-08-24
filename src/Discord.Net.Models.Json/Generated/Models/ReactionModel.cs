using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ReactionModel> ReactionModel => field ??= Discord.Models.Json.ReactionModel.CreateTypeInfo(Options);
}

public record ReactionModel(
    int Count,
    Discord.Models.IReactionCountDetailsModel CountDetails,
    bool Me,
    bool MeBurst,
    Discord.Models.EmojiId Emoji,
    System.Collections.Generic.IReadOnlyList<Discord.Color> BurstColors
) : 
    IReactionModel,
    IJsonModel,
    IApiModel<IReactionModel, ReactionModel>
{
    public static JsonTypeInfo<ReactionModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ReactionModel>(
        options,
        new JsonObjectInfoValues<ReactionModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ReactionModel(
                Count: (int)args[0],
                CountDetails: (Discord.Models.IReactionCountDetailsModel)args[1],
                Me: (bool)args[2],
                MeBurst: (bool)args[3],
                Emoji: (Discord.Models.EmojiId)args[4],
                BurstColors: (System.Collections.Generic.IReadOnlyList<Discord.Color>)args[5]
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
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).Count,
                Setter = null,
                PropertyName = "Count",
                JsonPropertyName = "count",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IReactionCountDetailsModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IReactionCountDetailsModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).CountDetails,
                Setter = null,
                PropertyName = "CountDetails",
                JsonPropertyName = "count_details",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).Me,
                Setter = null,
                PropertyName = "Me",
                JsonPropertyName = "me",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).MeBurst,
                Setter = null,
                PropertyName = "MeBurst",
                JsonPropertyName = "me_burst",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.EmojiId>(
            options,
            new JsonPropertyInfoValues<Discord.Models.EmojiId>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).Emoji,
                Setter = null,
                PropertyName = "Emoji",
                JsonPropertyName = "emoji",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Color>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Color>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ReactionModel),
                Getter = static instance => ((Discord.Models.Json.ReactionModel)instance).BurstColors,
                Setter = null,
                PropertyName = "BurstColors",
                JsonPropertyName = "burst_colors",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Count",
           ParameterType = typeof(int),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "CountDetails",
           ParameterType = typeof(Discord.Models.IReactionCountDetailsModel),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Me",
           ParameterType = typeof(bool),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MeBurst",
           ParameterType = typeof(bool),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Emoji",
           ParameterType = typeof(Discord.Models.EmojiId),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "BurstColors",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Color>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static ReactionModel From(IReactionModel model) => (model as ReactionModel) ?? new ReactionModel(
        Count: model.Count,
        CountDetails: model.CountDetails,
        Me: model.Me,
        MeBurst: model.MeBurst,
        Emoji: model.Emoji,
        BurstColors: model.BurstColors
    );

    static ReactionModel IApiModel<IReactionModel, ReactionModel>.From(IReactionModel model) => From(model);
}