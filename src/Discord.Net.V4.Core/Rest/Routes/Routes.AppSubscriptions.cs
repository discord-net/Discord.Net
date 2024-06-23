using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static IApiOutRoute<Entitlement[]> ListAppSubscriptions(ulong applicationId)
        => new ApiOutRoute<Entitlement[]>(nameof(ListAppSubscriptions),
            RequestMethod.Get,
            $"applications/{applicationId}/entitlements");

    public static IApiInOutRoute<CreateTestEntitlementParams, Entitlement> CreateTestEntitlement(ulong applicationId,
        CreateTestEntitlementParams body)
        => new ApiInOutRoute<CreateTestEntitlementParams, Entitlement>(nameof(CreateTestEntitlement),
            RequestMethod.Post,
            $"applications/{applicationId}/entitlements",
            body);

    public static IApiRoute DeleteTestEntitlement(ulong applicationId, ulong entitlementId)
        => new ApiRoute(nameof(DeleteTestEntitlement),
            RequestMethod.Delete,
            $"applications/{applicationId}/entitlements/{entitlementId}");

    public static IApiOutRoute<SKU[]> ListSKUs(ulong applicationId)
        => new ApiOutRoute<SKU[]>(nameof(ListSKUs),
            RequestMethod.Get,
            $"applications/{applicationId}/skus");
}
