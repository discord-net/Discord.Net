using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<OverwriteModel> OverwriteModel => field ??= Discord.Models.Json.OverwriteModel.CreateTypeInfo(Options);
}

public record OverwriteModel(
    Discord.Models.OverwriteType Type,
    Discord.Models.PermissionBitSet Allow,
    Discord.Models.PermissionBitSet Deny,
    Discord.Snowflake Id
) : 
    IOverwriteModel,
    IJsonModel,
    IApiModel<IOverwriteModel, OverwriteModel>
{
    public static JsonTypeInfo<OverwriteModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<OverwriteModel>(
        options,
        new JsonObjectInfoValues<OverwriteModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new OverwriteModel(
                Type: (Discord.Models.OverwriteType)args[0],
                Allow: (Discord.Models.PermissionBitSet)args[1],
                Deny: (Discord.Models.PermissionBitSet)args[2],
                Id: (Discord.Snowflake)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.OverwriteType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.OverwriteType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.OverwriteModel),
                Getter = static instance => ((Discord.Models.Json.OverwriteModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.PermissionBitSet>(
            options,
            new JsonPropertyInfoValues<Discord.Models.PermissionBitSet>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.OverwriteModel),
                Getter = static instance => ((Discord.Models.Json.OverwriteModel)instance).Allow,
                Setter = null,
                PropertyName = "Allow",
                JsonPropertyName = "allow",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.PermissionBitSet>(
            options,
            new JsonPropertyInfoValues<Discord.Models.PermissionBitSet>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.OverwriteModel),
                Getter = static instance => ((Discord.Models.Json.OverwriteModel)instance).Deny,
                Setter = null,
                PropertyName = "Deny",
                JsonPropertyName = "deny",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.OverwriteModel),
                Getter = static instance => ((Discord.Models.Json.OverwriteModel)instance).Id,
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
           ParameterType = typeof(Discord.Models.OverwriteType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Allow",
           ParameterType = typeof(Discord.Models.PermissionBitSet),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Deny",
           ParameterType = typeof(Discord.Models.PermissionBitSet),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static OverwriteModel From(IOverwriteModel model) => (model as OverwriteModel) ?? new OverwriteModel(
        Type: model.Type,
        Allow: model.Allow,
        Deny: model.Deny,
        Id: model.Id
    );

    static OverwriteModel IApiModel<IOverwriteModel, OverwriteModel>.From(IOverwriteModel model) => From(model);
}