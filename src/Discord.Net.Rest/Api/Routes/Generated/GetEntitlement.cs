using Discord.Models;

namespace Discord.Rest.Api;

partial class Routes
{
   public sealed partial record GetEntitlement(
        RouteParameters.ApplicationId ApplicationId,
        RouteParameters.EntitlementId EntitlementId
    ) : IOperation, Expand<GetEntitlement, GetEntitlement>
    {
        public static IReadOnlyList<Type> RouteParameterTypes
            => [typeof(RouteParameters.ApplicationId), typeof(RouteParameters.EntitlementId)];
            
        public IReadOnlyList<RouteParameters> RouteParameters
            => [ApplicationId, EntitlementId];
    
        public static string Path => @"/applications/{application_id}/entitlements/{entitlement_id}";
        public static string OperationId => "get_entitlement";
        public static RequestMethod Method => RequestMethod.Get;
        public static AuthenticationScheme AuthenticationScheme => AuthenticationScheme.BotToken | AuthenticationScheme.BearerToken;
        
        public string Format() => $"/applications/{ApplicationId}/entitlements/{EntitlementId}";
    }
}