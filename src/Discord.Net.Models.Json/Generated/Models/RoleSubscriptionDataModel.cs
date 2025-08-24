using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;


namespace Discord.Models.Json;

public partial class DiscordJsonContext
{
    [field: MaybeNull]
    public JsonTypeInfo<RoleSubscriptionDataModel> RoleSubscriptionDataModel => field ??= Discord.Models.Json.RoleSubscriptionDataModel.CreateTypeInfo(Options);
}

public record RoleSubscriptionDataModel(
    Discord.Snowflake RoleSubscriptionListingId,
    string TierName,
    int TotalMonthsSubscribed,
    bool IsRenewal
) : 
    IRoleSubscriptionDataModel,
    IJsonModel,
    IApiModel<IRoleSubscriptionDataModel, RoleSubscriptionDataModel>
{
    public static JsonTypeInfo<RoleSubscriptionDataModel> CreateTypeInfo(JsonSerializerOptions options) => JsonMetadataServices.CreateObjectInfo<RoleSubscriptionDataModel>(
        options,
        new JsonObjectInfoValues<RoleSubscriptionDataModel>()
        {
            ObjectWithParameterizedConstructorCreator = static args => new RoleSubscriptionDataModel(
                RoleSubscriptionListingId: (Discord.Snowflake)args[0],
                TierName: (string)args[1],
                TotalMonthsSubscribed: (int)args[2],
                IsRenewal: (bool)args[3]
            ),
            PropertyMetadataInitializer = _ => CreatePropertyInfos(options),
            ConstructorParameterMetadataInitializer = CreateConstructorParameterInfos
        }
    );

    public static JsonPropertyInfo[] CreatePropertyInfos(JsonSerializerOptions options) => [
        JsonMetadataServices.CreatePropertyInfo<Discord.Snowflake>(
            options,
            new JsonPropertyInfoValues<Discord.Snowflake>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSubscriptionDataModel),
                Getter = static instance => ((Discord.Models.Json.RoleSubscriptionDataModel)instance).RoleSubscriptionListingId,
                Setter = null,
                PropertyName = "RoleSubscriptionListingId",
                JsonPropertyName = "role_subscription_listing_id",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<string>(
            options,
            new JsonPropertyInfoValues<string>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSubscriptionDataModel),
                Getter = static instance => ((Discord.Models.Json.RoleSubscriptionDataModel)instance).TierName,
                Setter = null,
                PropertyName = "TierName",
                JsonPropertyName = "tier_name",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<int>(
            options,
            new JsonPropertyInfoValues<int>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSubscriptionDataModel),
                Getter = static instance => ((Discord.Models.Json.RoleSubscriptionDataModel)instance).TotalMonthsSubscribed,
                Setter = null,
                PropertyName = "TotalMonthsSubscribed",
                JsonPropertyName = "total_months_subscribed",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        ),
        JsonMetadataServices.CreatePropertyInfo<bool>(
            options,
            new JsonPropertyInfoValues<bool>
            {
                IsProperty = true,
                IsPublic = true,
                DeclaringType = typeof(Discord.Models.Json.RoleSubscriptionDataModel),
                Getter = static instance => ((Discord.Models.Json.RoleSubscriptionDataModel)instance).IsRenewal,
                Setter = null,
                PropertyName = "IsRenewal",
                JsonPropertyName = "is_renewal",
                IgnoreCondition = JsonIgnoreCondition.Never
            }
        )
    ];

    private static JsonParameterInfoValues[] CreateConstructorParameterInfos() => [
        new()
        {
           Name = "RoleSubscriptionListingId",
           ParameterType = typeof(Discord.Snowflake),
           Position = 0,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "TierName",
           ParameterType = typeof(string),
           Position = 1,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "TotalMonthsSubscribed",
           ParameterType = typeof(int),
           Position = 2,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        },
        new()
        {
           Name = "IsRenewal",
           ParameterType = typeof(bool),
           Position = 3,
           HasDefaultValue = false,
           DefaultValue = null,
           IsNullable = false
        }
    ];

    public static RoleSubscriptionDataModel From(IRoleSubscriptionDataModel model) => (model as RoleSubscriptionDataModel) ?? new RoleSubscriptionDataModel(
        RoleSubscriptionListingId: model.RoleSubscriptionListingId,
        TierName: model.TierName,
        TotalMonthsSubscribed: model.TotalMonthsSubscribed,
        IsRenewal: model.IsRenewal
    );

    static RoleSubscriptionDataModel IApiModel<IRoleSubscriptionDataModel, RoleSubscriptionDataModel>.From(IRoleSubscriptionDataModel model) => From(model);
}