using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<SelectDefaultValueModel> SelectDefaultValueModel => field ??= Discord.Models.Json.SelectDefaultValueModel.CreateTypeInfo(Options);
}

public record SelectDefaultValueModel(
    Discord.Models.SelectDefaultValueType Type,
    Discord.Snowflake Id
) : 
    ISelectDefaultValueModel,
    IJsonModel,
    IApiModel<ISelectDefaultValueModel, SelectDefaultValueModel>
{
    public static JsonTypeInfo<SelectDefaultValueModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<SelectDefaultValueModel>(
        options,
        new JsonObjectInfoValues<SelectDefaultValueModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new SelectDefaultValueModel(
                Type: (Discord.Models.SelectDefaultValueType)args[0],
                Id: (Discord.Snowflake)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.SelectDefaultValueType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.SelectDefaultValueType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectDefaultValueModel),
                Getter = static instance => ((Discord.Models.Json.SelectDefaultValueModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SelectDefaultValueModel),
                Getter = static instance => ((Discord.Models.Json.SelectDefaultValueModel)instance).Id,
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
           Name = "Type",
           ParameterType = typeof(Discord.Models.SelectDefaultValueType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static SelectDefaultValueModel From(ISelectDefaultValueModel model) => (model as SelectDefaultValueModel) ?? new SelectDefaultValueModel(
        Type: model.Type,
        Id: model.Id
    );

    static SelectDefaultValueModel IApiModel<ISelectDefaultValueModel, SelectDefaultValueModel>.From(ISelectDefaultValueModel model) => From(model);
}