using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<StickerItemModel> StickerItemModel => field ??= Discord.Models.Json.StickerItemModel.CreateTypeInfo(Options);
}

public record StickerItemModel(
    string Name,
    Discord.Models.StickerFormatType FormatType,
    Discord.Snowflake Id
) : 
    IStickerItemModel,
    IJsonModel,
    IApiModel<IStickerItemModel, StickerItemModel>
{
    public static JsonTypeInfo<StickerItemModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<StickerItemModel>(
        options,
        new JsonObjectInfoValues<StickerItemModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new StickerItemModel(
                Name: (string)args[0],
                FormatType: (Discord.Models.StickerFormatType)args[1],
                Id: (Discord.Snowflake)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.StickerItemModel),
                Getter = static instance => ((Discord.Models.Json.StickerItemModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.StickerFormatType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.StickerFormatType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.StickerItemModel),
                Getter = static instance => ((Discord.Models.Json.StickerItemModel)instance).FormatType,
                Setter = null,
                PropertyName = "FormatType",
                JsonPropertyName = "format_type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.StickerItemModel),
                Getter = static instance => ((Discord.Models.Json.StickerItemModel)instance).Id,
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
           Name = "Name",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "FormatType",
           ParameterType = typeof(Discord.Models.StickerFormatType),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static StickerItemModel From(IStickerItemModel model) => (model as StickerItemModel) ?? new StickerItemModel(
        Name: model.Name,
        FormatType: model.FormatType,
        Id: model.Id
    );

    static StickerItemModel IApiModel<IStickerItemModel, StickerItemModel>.From(IStickerItemModel model) => From(model);
}