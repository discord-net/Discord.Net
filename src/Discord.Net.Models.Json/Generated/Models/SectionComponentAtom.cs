using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<SectionComponentAtom> SectionComponentAtom => field ??= Discord.Models.Json.SectionComponentAtom.CreateTypeInfo(Options);
}

public record SectionComponentAtom(
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    ISectionComponentAtom,
    IJsonModel,
    IApiModel<ISectionComponentAtom, SectionComponentAtom>
{
    public static JsonTypeInfo<SectionComponentAtom> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<SectionComponentAtom>(
        options,
        new JsonObjectInfoValues<SectionComponentAtom>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new SectionComponentAtom(
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
                DeclaringType = typeof(Discord.Models.Json.SectionComponentAtom),
                Getter = static instance => ((Discord.Models.Json.SectionComponentAtom)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.SectionComponentAtom),
                Getter = static instance => ((Discord.Models.Json.SectionComponentAtom)instance).Id,
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

    public static SectionComponentAtom From(ISectionComponentAtom model) => (model as SectionComponentAtom) ?? new SectionComponentAtom(
        Type: model.Type,
        Id: model.Id
    );

    static SectionComponentAtom IApiModel<ISectionComponentAtom, SectionComponentAtom>.From(ISectionComponentAtom model) => From(model);
}