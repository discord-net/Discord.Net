using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<ApplicationModel> ApplicationModel => field ??= Discord.Models.Json.ApplicationModel.CreateTypeInfo(Options);
}

public record ApplicationModel : 
    IApplicationModel,
    IJsonModel,
    IApiModel<IApplicationModel, ApplicationModel>
{
    public static JsonTypeInfo<ApplicationModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<ApplicationModel>(
        options,
        new JsonObjectInfoValues<ApplicationModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new ApplicationModel(
            
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        
    ];

    public static ApplicationModel From(IApplicationModel model) => (model as ApplicationModel) ?? new ApplicationModel(
        
    );

    static ApplicationModel IApiModel<IApplicationModel, ApplicationModel>.From(IApplicationModel model) => From(model);
}