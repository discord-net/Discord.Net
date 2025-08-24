using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ContainerComponentModel> ContainerComponentModel => field ??= Discord.Models.Json.ContainerComponentModel.CreateTypeInfo(Options);
}

public record ContainerComponentModel(
    System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom> Components,
    Discord.Models.Optional<Nullable<Discord.Color>> AccentColor,
    Discord.Models.Optional<bool> Spoiler,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IContainerComponentModel,
    IJsonModel,
    IApiModel<IContainerComponentModel, ContainerComponentModel>
{
    public static JsonTypeInfo<ContainerComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ContainerComponentModel>(
        options,
        new JsonObjectInfoValues<ContainerComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ContainerComponentModel(
                Components: (System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>)args[0],
                AccentColor: (Discord.Models.Optional<Nullable<Discord.Color>>)args[1],
                Spoiler: (Discord.Models.Optional<bool>)args[2],
                Type: (Discord.Models.ComponentType)args[3],
                Id: (Nullable<int>)args[4]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>>(
            options,
            new JsonPropertyInfoValues<System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ContainerComponentModel),
                Getter = static instance => ((Discord.Models.Json.ContainerComponentModel)instance).Components,
                Setter = null,
                PropertyName = "Components",
                JsonPropertyName = "components",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<Discord.Color>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<Discord.Color>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ContainerComponentModel),
                Getter = static instance => ((Discord.Models.Json.ContainerComponentModel)instance).AccentColor,
                Setter = null,
                PropertyName = "AccentColor",
                JsonPropertyName = "accent_color",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ContainerComponentModel),
                Getter = static instance => ((Discord.Models.Json.ContainerComponentModel)instance).Spoiler,
                Setter = null,
                PropertyName = "Spoiler",
                JsonPropertyName = "spoiler",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ContainerComponentModel),
                Getter = static instance => ((Discord.Models.Json.ContainerComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ContainerComponentModel),
                Getter = static instance => ((Discord.Models.Json.ContainerComponentModel)instance).Id,
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
           ParameterType = typeof(System.Collections.Generic.IReadOnlyList<Discord.Models.IContainerAtom>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AccentColor",
           ParameterType = typeof(Discord.Models.Optional<Nullable<Discord.Color>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Spoiler",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ContainerComponentModel From(IContainerComponentModel model) => (model as ContainerComponentModel) ?? new ContainerComponentModel(
        Components: model.Components,
        AccentColor: model.AccentColor,
        Spoiler: model.Spoiler,
        Type: model.Type,
        Id: model.Id
    );

    static ContainerComponentModel IApiModel<IContainerComponentModel, ContainerComponentModel>.From(IContainerComponentModel model) => From(model);
}