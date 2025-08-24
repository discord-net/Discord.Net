using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<AttachmentModel> AttachmentModel => field ??= Discord.Models.Json.AttachmentModel.CreateTypeInfo(Options);
}

public record AttachmentModel(
    string Filename,
    Discord.Models.Optional<string> Title,
    Discord.Models.Optional<string> Description,
    Discord.Models.Optional<string> ContentType,
    int Size,
    string Url,
    string ProxyUrl,
    Discord.Models.Optional<Nullable<int>> Height,
    Discord.Models.Optional<Nullable<int>> Width,
    Discord.Models.Optional<bool> Ephemeral,
    Discord.Models.Optional<float> DurationSecs,
    Discord.Models.Optional<string> Waveform,
    Discord.Models.Optional<Discord.Models.AttachmentFlags> Flags,
    Discord.Snowflake Id
) : 
    IAttachmentModel,
    IJsonModel,
    IApiModel<IAttachmentModel, AttachmentModel>
{
    public static JsonTypeInfo<AttachmentModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<AttachmentModel>(
        options,
        new JsonObjectInfoValues<AttachmentModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new AttachmentModel(
                Filename: (string)args[0],
                Title: (Discord.Models.Optional<string>)args[1],
                Description: (Discord.Models.Optional<string>)args[2],
                ContentType: (Discord.Models.Optional<string>)args[3],
                Size: (int)args[4],
                Url: (string)args[5],
                ProxyUrl: (string)args[6],
                Height: (Discord.Models.Optional<Nullable<int>>)args[7],
                Width: (Discord.Models.Optional<Nullable<int>>)args[8],
                Ephemeral: (Discord.Models.Optional<bool>)args[9],
                DurationSecs: (Discord.Models.Optional<float>)args[10],
                Waveform: (Discord.Models.Optional<string>)args[11],
                Flags: (Discord.Models.Optional<Discord.Models.AttachmentFlags>)args[12],
                Id: (Discord.Snowflake)args[13]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Filename,
                Setter = null,
                PropertyName = "Filename",
                JsonPropertyName = "filename",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Title,
                Setter = null,
                PropertyName = "Title",
                JsonPropertyName = "title",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Description,
                Setter = null,
                PropertyName = "Description",
                JsonPropertyName = "description",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).ContentType,
                Setter = null,
                PropertyName = "ContentType",
                JsonPropertyName = "content_type",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Size,
                Setter = null,
                PropertyName = "Size",
                JsonPropertyName = "size",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).ProxyUrl,
                Setter = null,
                PropertyName = "ProxyUrl",
                JsonPropertyName = "proxy_url",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<int>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<int>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Height,
                Setter = null,
                PropertyName = "Height",
                JsonPropertyName = "height",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<int>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<int>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Width,
                Setter = null,
                PropertyName = "Width",
                JsonPropertyName = "width",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<bool>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<bool>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Ephemeral,
                Setter = null,
                PropertyName = "Ephemeral",
                JsonPropertyName = "ephemeral",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<float>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<float>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).DurationSecs,
                Setter = null,
                PropertyName = "DurationSecs",
                JsonPropertyName = "duration_secs",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Waveform,
                Setter = null,
                PropertyName = "Waveform",
                JsonPropertyName = "waveform",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Models.AttachmentFlags>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Models.AttachmentFlags>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Flags,
                Setter = null,
                PropertyName = "Flags",
                JsonPropertyName = "flags",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.AttachmentModel),
                Getter = static instance => ((Discord.Models.Json.AttachmentModel)instance).Id,
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
           Name = "Filename",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Title",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Description",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ContentType",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Size",
           ParameterType = typeof(int),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Url",
           ParameterType = typeof(string),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ProxyUrl",
           ParameterType = typeof(string),
           Position = 6,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Height",
           ParameterType = typeof(Discord.Models.Optional<Nullable<int>>),
           Position = 7,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Width",
           ParameterType = typeof(Discord.Models.Optional<Nullable<int>>),
           Position = 8,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Ephemeral",
           ParameterType = typeof(Discord.Models.Optional<bool>),
           Position = 9,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "DurationSecs",
           ParameterType = typeof(Discord.Models.Optional<float>),
           Position = 10,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Waveform",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 11,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Flags",
           ParameterType = typeof(Discord.Models.Optional<Discord.Models.AttachmentFlags>),
           Position = 12,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Id",
           ParameterType = typeof(Discord.Snowflake),
           Position = 13,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static AttachmentModel From(IAttachmentModel model) => (model as AttachmentModel) ?? new AttachmentModel(
        Filename: model.Filename,
        Title: model.Title,
        Description: model.Description,
        ContentType: model.ContentType,
        Size: model.Size,
        Url: model.Url,
        ProxyUrl: model.ProxyUrl,
        Height: model.Height,
        Width: model.Width,
        Ephemeral: model.Ephemeral,
        DurationSecs: model.DurationSecs,
        Waveform: model.Waveform,
        Flags: model.Flags,
        Id: model.Id
    );

    static AttachmentModel IApiModel<IAttachmentModel, AttachmentModel>.From(IAttachmentModel model) => From(model);
}