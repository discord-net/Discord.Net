using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<UnfurledMediaItemModel> UnfurledMediaItemModel => field ??= Discord.Models.Json.UnfurledMediaItemModel.CreateTypeInfo(Options);
}

public record UnfurledMediaItemModel(
    string Url,
    Discord.Models.Optional<string> ProxyUrl,
    Discord.Models.Optional<Nullable<int>> Height,
    Discord.Models.Optional<Nullable<int>> Width,
    Discord.Models.Optional<string> ContentType,
    Discord.Models.Optional<Discord.Snowflake> AttachmentId
) : 
    IUnfurledMediaItemModel,
    IJsonModel,
    IApiModel<IUnfurledMediaItemModel, UnfurledMediaItemModel>
{
    public static JsonTypeInfo<UnfurledMediaItemModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<UnfurledMediaItemModel>(
        options,
        new JsonObjectInfoValues<UnfurledMediaItemModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new UnfurledMediaItemModel(
                Url: (string)args[0],
                ProxyUrl: (Discord.Models.Optional<string>)args[1],
                Height: (Discord.Models.Optional<Nullable<int>>)args[2],
                Width: (Discord.Models.Optional<Nullable<int>>)args[3],
                ContentType: (Discord.Models.Optional<string>)args[4],
                AttachmentId: (Discord.Models.Optional<Discord.Snowflake>)args[5]
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
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).Url,
                Setter = null,
                PropertyName = "Url",
                JsonPropertyName = "url",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).ProxyUrl,
                Setter = null,
                PropertyName = "ProxyUrl",
                JsonPropertyName = "proxy_url",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Nullable<int>>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Nullable<int>>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).Height,
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
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).Width,
                Setter = null,
                PropertyName = "Width",
                JsonPropertyName = "width",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<string>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<string>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).ContentType,
                Setter = null,
                PropertyName = "ContentType",
                JsonPropertyName = "content_type",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<Discord.Models.Optional<Discord.Snowflake>>(
            options,
            new JsonPropertyInfoValues<Discord.Models.Optional<Discord.Snowflake>>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.UnfurledMediaItemModel),
                Getter = static instance => ((Discord.Models.Json.UnfurledMediaItemModel)instance).AttachmentId,
                Setter = null,
                PropertyName = "AttachmentId",
                JsonPropertyName = "attachment_id",
                IgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "Url",
           ParameterType = typeof(string),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ProxyUrl",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Height",
           ParameterType = typeof(Discord.Models.Optional<Nullable<int>>),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "Width",
           ParameterType = typeof(Discord.Models.Optional<Nullable<int>>),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "ContentType",
           ParameterType = typeof(Discord.Models.Optional<string>),
           Position = 4,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "AttachmentId",
           ParameterType = typeof(Discord.Models.Optional<Discord.Snowflake>),
           Position = 5,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static UnfurledMediaItemModel From(IUnfurledMediaItemModel model) => (model as UnfurledMediaItemModel) ?? new UnfurledMediaItemModel(
        Url: model.Url,
        ProxyUrl: model.ProxyUrl,
        Height: model.Height,
        Width: model.Width,
        ContentType: model.ContentType,
        AttachmentId: model.AttachmentId
    );

    static UnfurledMediaItemModel IApiModel<IUnfurledMediaItemModel, UnfurledMediaItemModel>.From(IUnfurledMediaItemModel model) => From(model);
}