using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<AvatarDecorationDataModel> AvatarDecorationDataModel => field ??= Discord.Models.Json.AvatarDecorationDataModel.CreateTypeInfo(Options);
}

public record AvatarDecorationDataModel : 
    IAvatarDecorationDataModel,
    IJsonModel,
    IApiModel<IAvatarDecorationDataModel, AvatarDecorationDataModel>
{
    public static JsonTypeInfo<AvatarDecorationDataModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<AvatarDecorationDataModel>(
        options,
        new JsonObjectInfoValues<AvatarDecorationDataModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new AvatarDecorationDataModel(
            
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        
    ];

    public static AvatarDecorationDataModel From(IAvatarDecorationDataModel model) => (model as AvatarDecorationDataModel) ?? new AvatarDecorationDataModel(
        
    );

    static AvatarDecorationDataModel IApiModel<IAvatarDecorationDataModel, AvatarDecorationDataModel>.From(IAvatarDecorationDataModel model) => From(model);
}