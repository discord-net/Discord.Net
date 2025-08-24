using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<FileComponentModel> FileComponentModel => field ??= Discord.Models.Json.FileComponentModel.CreateTypeInfo(Options);
}

public record FileComponentModel(
    Discord.Models.IUnfurledMediaItemModel File,
    Discord.Models.Optional<bool> Spoiler,
    string Name,
    int Size,
    Discord.Models.ComponentType Type,
    Nullable<int> Id
) : 
    IFileComponentModel,
    IJsonModel,
    IApiModel<IFileComponentModel, FileComponentModel>
{
    public static JsonTypeInfo<FileComponentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<FileComponentModel>(
        options,
        new JsonObjectInfoValues<FileComponentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new FileComponentModel(
                File: (Discord.Models.IUnfurledMediaItemModel)args[0],
                Spoiler: (Discord.Models.Optional<bool>)args[1],
                Name: (string)args[2],
                Size: (int)args[3],
                Type: (Discord.Models.ComponentType)args[4],
                Id: (Nullable<int>)args[5]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.IUnfurledMediaItemModel>(
            options,
            new JsonPropertyInfoValues<Discord.Models.IUnfurledMediaItemModel>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).File,
                Setter = null,
                PropertyName = "File",
                JsonPropertyName = "file",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).Spoiler,
                Setter = null,
                PropertyName = "Spoiler",
                JsonPropertyName = "spoiler",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).Name,
                Setter = null,
                PropertyName = "Name",
                JsonPropertyName = "name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).Size,
                Setter = null,
                PropertyName = "Size",
                JsonPropertyName = "size",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.ComponentType>(
            options,
            new JsonPropertyInfoValues<Discord.Models.ComponentType>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).Type,
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
                DeclaringType = typeof(Discord.Models.Json.FileComponentModel),
                Getter = static instance => ((Discord.Models.Json.FileComponentModel)instance).Id,
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
           Name = "File",
           ParameterType = typeof(Discord.Models.IUnfurledMediaItemModel),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Spoiler",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Name",
           ParameterType = typeof(string),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Size",
           ParameterType = typeof(int),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Type",
           ParameterType = typeof(Discord.Models.ComponentType),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Nullable<int>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = true
        }
    ];

    public static FileComponentModel From(IFileComponentModel model) => (model as FileComponentModel) ?? new FileComponentModel(
        File: model.File,
        Spoiler: model.Spoiler,
        Name: model.Name,
        Size: model.Size,
        Type: model.Type,
        Id: model.Id
    );

    static FileComponentModel IApiModel<IFileComponentModel, FileComponentModel>.From(IFileComponentModel model) => From(model);
}