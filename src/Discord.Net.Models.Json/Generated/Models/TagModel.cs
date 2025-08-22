using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<TagModel> TagModel => field ??= Discord.Models.Json.TagModel.CreateTypeInfo(Options);
}

public record TagModel(
    Snowflake Id
) : 
    ITagModel,
    IJsonModel,
    IApiModel<ITagModel, TagModel>
{
    public static JsonTypeInfo<TagModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<TagModel>(
        options,
        new JsonObjectInfoValues<TagModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new TagModel(
                Id: (Snowflake)args[0]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Snowflake>(
            options,
            new JsonPropertyInfoValues<Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.TagModel),
                Getter = static instance => ((Discord.Models.Json.TagModel)instance).Id,
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
           Name = "Id",
           ParameterType = typeof(Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static TagModel From(ITagModel model) => (model as TagModel) ?? new TagModel(
        Id: model.Id
    );

    static TagModel IApiModel<ITagModel, TagModel>.From(ITagModel model) => From(model);
}