using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<SectionComponentModel> SectionComponentModel => field ??= Discord.Models.Json.SectionComponentModel.CreateTypeInfo(Options);
}

public record SectionComponentModel(
    System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom> Components,
    Discord.Models.ISectionComponentAccessory Accessory,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    ISectionComponentModel,
    IJsonModel,
    IApiModel<ISectionComponentModel, SectionComponentModel>
{
    public static JsonTypeInfo<SectionComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<SectionComponentModel>(
        options,
        new JsonObjectInfoValues<SectionComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new SectionComponentModel(
                Components: (System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>)args[0],
                Accessory: (Discord.Models.ISectionComponentAccessory)args[1],
                Type: (Discord.Models.ComponentType)args[2],
                Id: (Nullable<int>)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SectionComponentModel),
                Getter = static instance => ((Discord.Models.Json.SectionComponentModel)instance).Components,
                Setter = null,
                PropertyName = "Components",
                JsonPropertyName = "components",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ISectionComponentAccessory>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ISectionComponentAccessory>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SectionComponentModel),
                Getter = static instance => ((Discord.Models.Json.SectionComponentModel)instance).Accessory,
                Setter = null,
                PropertyName = "Accessory",
                JsonPropertyName = "accessory",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SectionComponentModel),
                Getter = static instance => ((Discord.Models.Json.SectionComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.SectionComponentModel),
                Getter = static instance => ((Discord.Models.Json.SectionComponentModel)instance).Id,
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
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.ISectionComponentAtom>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Accessory",
           ParameterType = typeof(Discord.Models.ISectionComponentAccessory),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static SectionComponentModel From(ISectionComponentModel model) => (model as SectionComponentModel) ?? new SectionComponentModel(
        Components: model.Components,
        Accessory: model.Accessory,
        Type: model.Type,
        Id: model.Id
    );

    static SectionComponentModel IApiModel<ISectionComponentModel, SectionComponentModel>.From(ISectionComponentModel model) => From(model);
}