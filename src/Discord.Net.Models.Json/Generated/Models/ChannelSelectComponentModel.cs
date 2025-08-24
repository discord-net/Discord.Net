using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ChannelSelectComponentModel> ChannelSelectComponentModel => field ??= Discord.Models.Json.ChannelSelectComponentModel.CreateTypeInfo(Options);
}

public record ChannelSelectComponentModel(
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>> ChannelTypes,
    Discord.Models.Optional<string> Placeholder,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>> DefaultValues,
    Discord.Models.Optional<int> MinValues,
    Discord.Models.Optional<int> MaxValues,
    Discord.Models.Optional<bool> Disabled,
    string CustomId,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IChannelSelectComponentModel,
    IJsonModel,
    IApiModel<IChannelSelectComponentModel, ChannelSelectComponentModel>
{
    public static JsonTypeInfo<ChannelSelectComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ChannelSelectComponentModel>(
        options,
        new JsonObjectInfoValues<ChannelSelectComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ChannelSelectComponentModel(
                ChannelTypes: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>)args[0],
                Placeholder: (Discord.Models.Optional<string>)args[1],
                DefaultValues: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>)args[2],
                MinValues: (Discord.Models.Optional<int>)args[3],
                MaxValues: (Discord.Models.Optional<int>)args[4],
                Disabled: (Discord.Models.Optional<bool>)args[5],
                CustomId: (string)args[6],
                Type: (Discord.Models.ComponentType)args[7],
                Id: (Nullable<int>)args[8]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).ChannelTypes,
                Setter = null,
                PropertyName = "ChannelTypes",
                JsonPropertyName = "channel_types",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).Placeholder,
                Setter = null,
                PropertyName = "Placeholder",
                JsonPropertyName = "placeholder",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).DefaultValues,
                Setter = null,
                PropertyName = "DefaultValues",
                JsonPropertyName = "default_values",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).MinValues,
                Setter = null,
                PropertyName = "MinValues",
                JsonPropertyName = "min_values",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<int>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<int>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).MaxValues,
                Setter = null,
                PropertyName = "MaxValues",
                JsonPropertyName = "max_values",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).Disabled,
                Setter = null,
                PropertyName = "Disabled",
                JsonPropertyName = "disabled",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).CustomId,
                Setter = null,
                PropertyName = "CustomId",
                JsonPropertyName = "custom_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.ChannelSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.ChannelSelectComponentModel)instance).Id,
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
           Name = "ChannelTypes",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ChannelType>>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Placeholder",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultValues",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MinValues",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MaxValues",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Disabled",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "CustomId",
           ParameterType = typeof(string),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static ChannelSelectComponentModel From(IChannelSelectComponentModel model) => (model as ChannelSelectComponentModel) ?? new ChannelSelectComponentModel(
        ChannelTypes: model.ChannelTypes,
        Placeholder: model.Placeholder,
        DefaultValues: model.DefaultValues,
        MinValues: model.MinValues,
        MaxValues: model.MaxValues,
        Disabled: model.Disabled,
        CustomId: model.CustomId,
        Type: model.Type,
        Id: model.Id
    );

    static ChannelSelectComponentModel IApiModel<IChannelSelectComponentModel, ChannelSelectComponentModel>.From(IChannelSelectComponentModel model) => From(model);
}