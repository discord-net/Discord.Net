using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ActionRowComponentModel> ActionRowComponentModel => field ??= Discord.Models.Json.ActionRowComponentModel.CreateTypeInfo(Options);
}

public record ActionRowComponentModel(
    System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel> Components,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IActionRowComponentModel,
    IJsonModel,
    IApiModel<IActionRowComponentModel, ActionRowComponentModel>
{
    public static JsonTypeInfo<ActionRowComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ActionRowComponentModel>(
        options,
        new JsonObjectInfoValues<ActionRowComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ActionRowComponentModel(
                Components: (System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>)args[0],
                Type: (Discord.Models.ComponentType)args[1],
                Id: (Nullable<int>)args[2]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ActionRowComponentModel),
                Getter = static instance => ((Discord.Models.Json.ActionRowComponentModel)instance).Components,
                Setter = null,
                PropertyName = "Components",
                JsonPropertyName = "components",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ActionRowComponentModel),
                Getter = static instance => ((Discord.Models.Json.ActionRowComponentModel)instance).Type,
                Setter = null,
                PropertyName = "Type",
                JsonPropertyName = "type",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Nullable<int>>(
            options,
            new JsonPropertyInfoValues<Nullable<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ActionRowComponentModel),
                Getter = static instance => ((Discord.Models.Json.ActionRowComponentModel)instance).Id,
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
           Name = "Components",
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IMessageComponentModel>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ActionRowComponentModel From(IActionRowComponentModel model) => (model as ActionRowComponentModel) ?? new ActionRowComponentModel(
        Components: model.Components,
        Type: model.Type,
        Id: model.Id
    );

    static ActionRowComponentModel IApiModel<IActionRowComponentModel, ActionRowComponentModel>.From(IActionRowComponentModel model) => From(model);
}