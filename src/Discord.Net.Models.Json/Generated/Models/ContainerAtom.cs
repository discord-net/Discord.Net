using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ContainerAtom> ContainerAtom => field ??= Discord.Models.Json.ContainerAtom.CreateTypeInfo(Options);
}

public record ContainerAtom(
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IContainerAtom,
    IJsonModel,
    IApiModel<IContainerAtom, ContainerAtom>
{
    public static JsonTypeInfo<ContainerAtom> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ContainerAtom>(
        options,
        new JsonObjectInfoValues<ContainerAtom>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ContainerAtom(
                Type: (Discord.Models.ComponentType)args[0],
                Id: (Nullable<int>)args[1]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ContainerAtom),
                Getter = static instance => ((Discord.Models.Json.ContainerAtom)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ContainerAtom),
                Getter = static instance => ((Discord.Models.Json.ContainerAtom)instance).Id,
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
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ContainerAtom From(IContainerAtom model) => (model as ContainerAtom) ?? new ContainerAtom(
        Type: model.Type,
        Id: model.Id
    );

    static ContainerAtom IApiModel<IContainerAtom, ContainerAtom>.From(IContainerAtom model) => From(model);
}