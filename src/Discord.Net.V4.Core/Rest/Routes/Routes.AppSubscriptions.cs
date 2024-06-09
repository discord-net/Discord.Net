using Discord.API;
using Discord.Models.Json;

namespace Discord.Rest;

public partial class Routes
{
    public static ApiRoute<Entitlement[]> ListAppSubscriptions(ulong applicationId)
        => new(nameof(ListAppSubscriptions),
            RequestMethod.Get,
            $"applications/{applicationId}/entitlements");

    public static ApiBodyRoute<CreateTestEntitlementParams, Entitlement> CreateTestEntitlement(ulong applicationId, CreateTestEntitlementParams body)
        => new(nameof(CreateTestEntitlement),
            RequestMethod.Post,
            $"applications/{applicationId}/entitlements",
            body,
            ContentType.JsonBody);

    public static BasicApiRoute DeleteTestEntitlement(ulong applicationId, ulong entitlementId)
        => new(nameof(DeleteTestEntitlement),
            RequestMethod.Delete,
            $"applications/{applicationId}/entitlements/{entitlementId}");

    public static ApiRoute<SKU[]> ListSKUs(ulong applicationId)
        => new(nameof(ListSKUs),
            RequestMethod.Get,
            $"applications/{applicationId}/skus");
}
