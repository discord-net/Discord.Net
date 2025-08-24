using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<SeparatorComponentItem> SeparatorComponentItem => field ??= Discord.Models.Json.SeparatorComponentItem.CreateTypeInfo(Options);
}

public record SeparatorComponentItem(
    Discord.Models.Optional<bool> Divider,
    Discord.Models.Optional<Discord.Models.SeparatorSpacing> Spacing,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    ISeparatorComponentItem,
    IJsonModel,
    IApiModel<ISeparatorComponentItem, SeparatorComponentItem>
{
    public static JsonTypeInfo<SeparatorComponentItem> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<SeparatorComponentItem>(
        options,
        new JsonObjectInfoValues<SeparatorComponentItem>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new SeparatorComponentItem(
                Divider: (Discord.Models.Optional<bool>)args[0],
                Spacing: (Discord.Models.Optional<Discord.Models.SeparatorSpacing>)args[1],
                Type: (Discord.Models.ComponentType)args[2],
                Id: (Nullable<int>)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SeparatorComponentItem),
                Getter = static instance => ((Discord.Models.Json.SeparatorComponentItem)instance).Divider,
                Setter = null,
                PropertyName = "Divider",
                JsonPropertyName = "divider",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.SeparatorSpacing>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.SeparatorSpacing>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SeparatorComponentItem),
                Getter = static instance => ((Discord.Models.Json.SeparatorComponentItem)instance).Spacing,
                Setter = null,
                PropertyName = "Spacing",
                JsonPropertyName = "spacing",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.SeparatorComponentItem),
                Getter = static instance => ((Discord.Models.Json.SeparatorComponentItem)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.SeparatorComponentItem),
                Getter = static instance => ((Discord.Models.Json.SeparatorComponentItem)instance).Id,
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
           Name = "Divider",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Spacing",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.SeparatorSpacing>),
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

    public static SeparatorComponentItem From(ISeparatorComponentItem model) => (model as SeparatorComponentItem) ?? new SeparatorComponentItem(
        Divider: model.Divider,
        Spacing: model.Spacing,
        Type: model.Type,
        Id: model.Id
    );

    static SeparatorComponentItem IApiModel<ISeparatorComponentItem, SeparatorComponentItem>.From(ISeparatorComponentItem model) => From(model);
}