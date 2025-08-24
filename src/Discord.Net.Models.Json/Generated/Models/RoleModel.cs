using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<RoleModel> RoleModel => field ??= Discord.Models.Json.RoleModel.CreateTypeInfo(Options);
}

public record RoleModel(
    Discord.Snowflake Id
) : 
    IRoleModel,
    IJsonModel,
    IApiModel<IRoleModel, RoleModel>
{
    public static JsonTypeInfo<RoleModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<RoleModel>(
        options,
        new JsonObjectInfoValues<RoleModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new RoleModel(
                Id: (Discord.Snowflake)args[0]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleModel),
                Getter = static instance => ((Discord.Models.Json.RoleModel)instance).Id,
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
           ParameterType = typeof(Discord.Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static RoleModel From(IRoleModel model) => (model as RoleModel) ?? new RoleModel(
        Id: model.Id
    );

    static RoleModel IApiModel<IRoleModel, RoleModel>.From(IRoleModel model) => From(model);
}