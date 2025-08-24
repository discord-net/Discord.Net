using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<RoleSelectComponentModel> RoleSelectComponentModel => field ??= Discord.Models.Json.RoleSelectComponentModel.CreateTypeInfo(Options);
}

public record RoleSelectComponentModel(
    Discord.Models.Optional<string> Placeholder,
    Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>> DefaultValues,
    Discord.Models.Optional<int> MinValues,
    Discord.Models.Optional<int> MaxValues,
    Discord.Models.Optional<bool> Disabled,
    Discord.Models.ComponentType Type,
    Nullable<int> Id,
    string CustomId
) : 
    IRoleSelectComponentModel,
    IJsonModel,
    IApiModel<IRoleSelectComponentModel, RoleSelectComponentModel>
{
    public static JsonTypeInfo<RoleSelectComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<RoleSelectComponentModel>(
        options,
        new JsonObjectInfoValues<RoleSelectComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new RoleSelectComponentModel(
                Placeholder: (Discord.Models.Optional<string>)args[0],
                DefaultValues: (Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>)args[1],
                MinValues: (Discord.Models.Optional<int>)args[2],
                MaxValues: (Discord.Models.Optional<int>)args[3],
                Disabled: (Discord.Models.Optional<bool>)args[4],
                Type: (Discord.Models.ComponentType)args[5],
                Id: (Nullable<int>)args[6],
                CustomId: (string)args[7]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).Placeholder,
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
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).DefaultValues,
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
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).MinValues,
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
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).MaxValues,
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
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).Disabled,
                Setter = null,
                PropertyName = "Disabled",
                JsonPropertyName = "disabled",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).Id,
                Setter = null,
                PropertyName = "Id",
                JsonPropertyName = "id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSelectComponentModel),
                Getter = static instance => ((Discord.Models.Json.RoleSelectComponentModel)instance).CustomId,
                Setter = null,
                PropertyName = "CustomId",
                JsonPropertyName = "custom_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Placeholder",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DefaultValues",
           ParameterType = typeof(Discord.Models.Optional<System.Collections.Generic.IReadOnlyList<Discord.Models.ISelectDefaultValueModel>>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MinValues",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "MaxValues",
           ParameterType = typeof(Discord.Models.Optional<int>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Disabled",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        },
        new()
        {
           Name = "CustomId",
           ParameterType = typeof(string),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static RoleSelectComponentModel From(IRoleSelectComponentModel model) => (model as RoleSelectComponentModel) ?? new RoleSelectComponentModel(
        Placeholder: model.Placeholder,
        DefaultValues: model.DefaultValues,
        MinValues: model.MinValues,
        MaxValues: model.MaxValues,
        Disabled: model.Disabled,
        Type: model.Type,
        Id: model.Id,
        CustomId: model.CustomId
    );

    static RoleSelectComponentModel IApiModel<IRoleSelectComponentModel, RoleSelectComponentModel>.From(IRoleSelectComponentModel model) => From(model);
}